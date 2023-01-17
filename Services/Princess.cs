using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace BrideProblem.Services {
    public class Princess : IHostedService{
        private Hall _hall;
        private Friend _friend;
        private readonly Candidate?[] _rejectedCandidates;
        private int _rejectedCount;
        private readonly IHostApplicationLifetime? _appLifetime;
        public static int Result { get; private set; }

        public Princess(Hall hall, Friend friend) {
            _hall = hall;
            _friend = friend;
            _rejectedCandidates = new Candidate?[100];
        }
        
        public Princess(IHostApplicationLifetime appLifetime, Hall hall, Friend friend) {
            _appLifetime = appLifetime;
            _hall = hall;
            _friend = friend;
            _rejectedCandidates = new Candidate?[100];
        }

        // Returns chosen candidate. Null if none is chosen.
        public Candidate? HighestChanceOfChoosingTheBest2() {
            _rejectedCount = 0;
            Candidate? candidate;

            const int candidatesToReject = 15;
            // Skip first N candidates
            for (var i = 0; i < candidatesToReject; ++i) {
                candidate = _hall.Next();
                Reject(candidate);
            }
            
            // Choose the first one that is better than all previous
            for (var i = candidatesToReject; i < 100; ++i) {
                candidate = _hall.Next();
                var isBest = true;
                for (var j = 0; j < _rejectedCount; ++j) {
                    if (candidate == null || _rejectedCandidates[j] == null ||
                        _friend.IsLeftBetter(candidate, _rejectedCandidates[j]!)) continue;
                    isBest = false;
                    Reject(candidate);
                    break;
                }

                if (!isBest) {
                    continue;
                }
                return candidate;
            }
            return null;
        }

        private void Reject(Candidate? candidate) {
            _friend.Reject(candidate);
            _rejectedCandidates[_rejectedCount++] = candidate;
        }

        private void Start() {
            var chosen = HighestChanceOfChoosingTheBest2();
            if (chosen != null) {
                Result = chosen.Score;
            }
            Console.Write($"{(chosen == null ? "Nobody was chosen" : chosen.Name)}, ");
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            try {
                Start();
            } catch (Exception e) {
                Console.Out.WriteLineAsync($"Unhandled exception: \n{e.Message}");
            }
            _appLifetime?.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _friend.Clean();
            return Task.CompletedTask;
        }
    }
}