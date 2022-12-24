namespace BrideProblem.Services {
    public class Friend {
        private const int MaxContenders = 100;
        private Candidate?[] _rejectedCandidates;
        private int _rejectedCount;
        
        public Friend() {
            _rejectedCount = 0;
            _rejectedCandidates = new Candidate?[MaxContenders];
        }

        public void Reject(Candidate? guy) {
            _rejectedCandidates[_rejectedCount++] = guy;
        }

        public bool IsLeftBetter(Candidate left, Candidate right) {
            return _rejectedCandidates.Contains(right) && left.Score > right.Score;
        }

        public void Clean() {
            _rejectedCandidates = new Candidate?[MaxContenders];
            _rejectedCount = 0;
        }
    }
}