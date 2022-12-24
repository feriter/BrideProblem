namespace BrideProblem {
    public class Candidate {
            public int Score { get; }
            public string Name { get; }
    
            public Candidate(int score, string name) {
                Score = score;
                Name = name;
            }
        }
}