using BrideProblem;
using BrideProblem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Assert = NUnit.Framework.Assert;

namespace BrideTest; 

[TestFixture]
public class PrincessTests {
    private Hall? _hall;
    private Princess? _princess;
    private Friend? _friend;
    private Candidate[]? _candies;

    [SetUp]
    public void PrincessSetup() {
        _hall = new Hall();
        _candies = new Candidate[100];
        for (var i = 0; i < 100; ++i) {
            _candies[i] = new Candidate(i + 1, (i + 1).ToString());
        }
        _friend = new Friend();
        _hall.SetCandidates(_candies);
        _princess = new Princess(_hall, _friend);
    }

    [Test]
    public void KnownContendersTest() {
        var cont1 = _candies![0];
        var cont2 = _candies![1];
        Assert.False(_friend?.IsLeftBetter(cont1, cont2));
        Assert.False(_friend?.IsLeftBetter(cont2, cont1));
        
        _friend?.Reject(cont1);

        Assert.True(_friend?.IsLeftBetter(cont2, cont1));
    }

    [Test]
    public void StrategyTest() {
        var chosen = _princess.HighestChanceOfChoosingTheBest2();
        // After 15 skips the 16s is the best
        Assert.That(chosen?.Score == 16);
    }
}