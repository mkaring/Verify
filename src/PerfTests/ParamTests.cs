[TestFixture]
public class ParamTests
{
    static IEnumerable<int> testCases = Enumerable.Range(0, 100);

    [TestCaseSource(nameof(testCases))]
    public Task LargeNumber(int input)
    {
        var result = input * 2;
        return Verify(result)
            .UseParameters(input);
    }

    record TestResult(int input, int result);

    [Test]
    public Task Alternative()
    {
        var list = from input in Enumerable.Range(0, 100)
            let result = input * 2
            select new TestResult(input, result);

        return Verify(list);
    }
}