using BrideProblem.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BrideProblem; 

internal static class Program {
    // Local db settings
    private const string ConnectionString = "Server=localhost;Port=5432;UserId=postgres;" +
                                            "Password=bride;Database=postgres;";
    private static readonly NpgsqlConnection Con = new(ConnectionString);
    private static readonly Random Rand = new();
    private static IHost? _host;

    public static void Main(string[] args) {
        var input = Console.ReadLine();

        // If input is '.' then the table is filled with new cases
        if (input == ".") {
            GenerateCases(Rand.Next());
            input = Console.ReadLine();
        }

        if (input == null) return;
            
        var num = CheckMode(input);

        Con.Open();
        
        if (num != 0) {
            SingleRun(args, num);
        } else {
            MultipleRun(args);
        }

        Con.Close();
    }

    private static void SingleRun(String[] args, int num) {
        _host = CreateHostBuilder(args).Build();
        var hall = _host.Services.GetService(typeof(Hall));
        if (hall == null) return;
        
        var sql = $"SELECT seed FROM cases WHERE number = {num}";
        var com = new NpgsqlCommand(sql, Con);
        var result = com.ExecuteScalar();

        var seed = 0;
        if (result != null) {
            seed = int.Parse(result.ToString() ?? string.Empty);
        }

        if (seed != 0) {
            ((Hall)hall).SetSeed(seed);
        }

        _host.Run();
    }
    
    private static int CheckMode(string input) {
        var num = int.Parse(input);
        return num is < 1 or > 100 ? 0 : num;
    }

    private static void MultipleRun(String[] args) {
        var sum = 0;
        
        for (var i = 1; i <= 100; ++i) {
            _host = CreateHostBuilder(args).Build();
            var hallObj = _host.Services.GetService(typeof(Hall));
            if (hallObj == null) break;
            var hall = (Hall)hallObj;
            
            // Get case seed from db
            var sql = $"SELECT seed FROM cases WHERE number = {i};";
            var com = new NpgsqlCommand(sql, Con);
            var result = com.ExecuteScalar();
                
            var seed = int.Parse(result?.ToString() ?? string.Empty);
            hall.SetSeed(seed);

            _host.Start();
            
            // Chosen candidate's score
            var score = Princess.Result;
            sum += JudgeScore(score);
            Console.WriteLine($"Total: {sum}");

            // Write case result in db
            sql = $"UPDATE cases SET result = {score} WHERE number = {i};";
            com = new NpgsqlCommand(sql, Con);
            com.ExecuteNonQuery();
        }
        
        Console.WriteLine($"Average chosen candidate score: {(double)sum / 100}.");
    }

    private static int JudgeScore(int score) {
        return score switch {
            100 => 20,
            98 => 50,
            96 => 100,
            0 => 10,
            _ => 0
        };
    }

    private static void GenerateCases(int seed) {
        var rnd = new Random(seed);
        // Clears the table with cases
        var sql = "TRUNCATE cases;";

        Con.Open();
        var com = new NpgsqlCommand(sql, Con);
        com.ExecuteNonQuery();

        for (var i = 1; i <= 100; ++i) {
            sql = "insert into cases (id, number, seed)" +
                  $" values (DEFAULT, {i.ToString()}, {rnd.Next()});";
            com.CommandText = sql;
            com.ExecuteNonQuery();
        }
        Con.Close();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => {
                services.AddScoped<ContenderGenerator>();
                services.AddScoped<Hall>();
                services.AddScoped<Friend>();
                services.AddHostedService<Princess>();
            })
            .ConfigureLogging((_, logging) => {
                logging.ClearProviders();
            });
    }
}
