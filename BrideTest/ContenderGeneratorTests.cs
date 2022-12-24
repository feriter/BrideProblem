using BrideProblem.Services;

namespace BrideTest; 

[TestFixture]
public class ContenderGeneratorTests {
    private ContenderGenerator? _contenderGenerator;
    
    [SetUp]
    public void ContenderGeneratorSetup() {
        _contenderGenerator = new ContenderGenerator();
    }
    
    [Test]
    public void UniquenessTest() {
        var expectedSize = 100;

        if (_contenderGenerator == null) return;
        var contenderSet = _contenderGenerator.Candidates.ToHashSet();
        var actualSize = contenderSet.Count;
        
        Assert.AreEqual(expectedSize, actualSize, 0);
    }
}