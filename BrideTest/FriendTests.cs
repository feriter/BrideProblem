using BrideProblem;
using BrideProblem.Services;

namespace BrideTest; 

[TestFixture]
public class FriendTests {
    private Friend? _friend;
    [SetUp]
    public void FriendSetup() {
        _friend = new Friend();
    }

    [Test]
    public void CompareTest() {
        var candies = new Candidate[30];
        for (var i = 0; i < 30; ++i) {
            candies[i] = new Candidate(i, i.ToString());
        }
        for (var i = 0; i < 30; ++i) {
            _friend?.Reject(candies[i]);
        }
        var newCandidate = new Candidate(50, "50");
        var foreignCandidate = new Candidate(101, "101");
        
        Assert.True(_friend!.IsLeftBetter(newCandidate, candies[0]));
        Assert.True(_friend!.IsLeftBetter(newCandidate, candies[29]));
        Assert.False(_friend!.IsLeftBetter(newCandidate, foreignCandidate));
    }
}