using BrideProblem.Entities;
using BrideProblem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using DbContext = System.Data.Entity.DbContext;

namespace BrideProblem; 

internal static class Program {
    private class EnvironmentContext : Microsoft.EntityFrameworkCore.DbContext {
        private const string ConnectionString = "Server=localhost;Port=5432;UserId=postgres;" +
                                                "Password=bride;Database=postgres;";
        public Microsoft.EntityFrameworkCore.DbSet<Case> Cases { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }
    
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

        using var context = new EnvironmentContext();
        context.Database.EnsureCreated();
        if (num != 0) {
            SingleRun(args, num, context);
        } else {
            MultipleRun(args, context);
        }
    }

    private static void SingleRun(String[] args, int num, EnvironmentContext context) {
        _host = CreateHostBuilder(args).Build();
        var hall = _host.Services.GetService(typeof(Hall));
        if (hall == null) return;
        
        var result = context.Cases
            .First(c => c.Number.Equals(num))
            .Seed;

        var seed = int.Parse(result.ToString());

        if (seed != 0) {
            ((Hall)hall).SetSeed(seed);
        }

        _host.Run();
    }
    
    private static int CheckMode(string input) {
        var num = int.Parse(input);
        return num is < 1 or > 100 ? 0 : num;
    }

    private static void MultipleRun(String[] args, EnvironmentContext context) {
        var sum = 0;
        
        for (var i = 1; i <= 100; ++i) {
            _host = CreateHostBuilder(args).Build();
            var hallObj = _host.Services.GetService(typeof(Hall));
            if (hallObj == null) break;
            var hall = (Hall)hallObj;
            

            var result = context.Cases
                .First(c => c.Number.Equals(i))
                .Seed;
                
            var seed = int.Parse(result.ToString());
            hall.SetSeed(seed);

            _host.Start();
            
            // Chosen candidate's score
            var score = Princess.Result;
            sum += JudgeScore(score);
            Console.WriteLine($"Total: {sum}");

            context.Cases
                .First(c => c.Number.Equals(i))
                .Result = score;
        }

        context.SaveChanges();
        
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

        using (var context = new EnvironmentContext()) {
            context.Cases.RemoveRange(context.Cases);
            context.SaveChanges();

            for (var i = 1; i <= 100; ++i) {
                context.Cases.Add(new Case { Number = i, Seed = rnd.Next()});
            }

            context.SaveChanges();
        }
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
