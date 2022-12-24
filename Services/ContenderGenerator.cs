using System;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace BrideProblem.Services {
    public sealed class ContenderGenerator {
        private const int MaxContenders = 100;
        public Candidate?[] Candidates { get; }

        public ContenderGenerator() {
            Candidates = new Candidate?[MaxContenders];
            for (var i = 0; i < MaxContenders; ++i) {
                Candidates[i] = new Candidate(i + 1, (i + 1).ToString());
            }
        }

        public Candidate[]? GetBatch() {
            return Candidates.Clone() as Candidate[];
        }
    }
}