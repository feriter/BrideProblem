using BrideProblem.Services;
using Moq;
using NUnit.Framework;

namespace BrideTest; 

[TestFixture]
public class PrincessTests {
    private Mock<Hall>? _hallMock;
    private Mock<Friend>? _friendMock;

    [SetUp]
    public void PrincessSetup() {
        _hallMock = new Mock<Hall>();
        _friendMock = new Mock<Friend>();
    }

    [Test]
    public void StrategyTest() {
        
    }
}