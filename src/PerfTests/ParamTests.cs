[TestFixture]
public class ParamTests
{
    private static IEnumerable<int> testCases = Enumerable.Range(0, 100);

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
        var list = new List<TestResult>();
        foreach (var input in Enumerable.Range(0, 100))
        {
            var result = input * 2;
            list.Add(new(input, result));
        }

        return Verify(list).UseDirectory("snaps");
    }
}