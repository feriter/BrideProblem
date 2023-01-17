﻿namespace BrideProblem.Services {
    public class Hall {
        private readonly ContenderGenerator? _contenderGenerator;
        private Candidate?[] _candidates;
        private Random? _rnd;
        private int _current;

        public Hall() {
            _candidates = new Candidate[100];
        }
        
        public Hall(ContenderGenerator generator) {
            _contenderGenerator = generator;
            _candidates = generator.GetBatch()!;
        }

        public virtual Candidate? Next() {
            _rnd ??= new Random();
            return _current < 100 ? _candidates[_current++] : null;
        }

        public void SetSeed(int seed) {
            _rnd = new Random(seed);
            _candidates = _contenderGenerator.GetBatch()!;
            _candidates = _candidates.OrderBy(_ => _rnd.Next()).ToArray();
        }

        public void SetCandidates(Candidate?[] candies) {
            _candidates = candies;
        }
    }
}