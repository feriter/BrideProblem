using BrideProblem;
using BrideProblem.Services;
using Moq;

namespace BrideTest; 

[TestFixture]
public class HallTests {
    private Mock<ContenderGenerator>? _mock;
    private Hall? _hall;

    [SetUp]
    public void HallSetup() {
        _mock = new Mock<ContenderGenerator>();
        var candies = new Candidate?[100];
        for (var i = 0; i < 100; ++i) {
            candies[i] = new Candidate(i + 1, (i + 1).ToString());
        }
        _mock.SetupGet(x => x.Candidates).Returns(candies);
    }

    [Test]
    public void NextCallingTest() {
        if (_mock != null) _hall = new Hall(_mock.Object);
        for (var i = 0; i < 100; ++i) {
            if (_hall == null) continue;
            var candy = _hall.Next();
            Assert.NotNull(candy);
            Assert.True(candy!.Score > 0);
            Assert.True(candy.Score <= 100);
        }
    }

    [Test]
    public void NoContendersTest() {
        if (_mock != null) _hall = new Hall(_mock.Object);
        for (var i = 0; i < 100; ++i) {
            _hall?.Next();
        }

        if (_hall != null) Assert.Null(_hall.Next());
    }
}