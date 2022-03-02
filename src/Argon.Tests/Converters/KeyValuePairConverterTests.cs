public class KeyValuePairConverterTests : TestFixtureBase
{
    [Fact]
    public void SerializeUsingInternalConverter()
    {
        var contractResolver = new DefaultContractResolver();
        var contract = (JsonObjectContract)contractResolver.ResolveContract(typeof(KeyValuePair<string, int>));

        Assert.Equal(typeof(KeyValuePairConverter), contract.InternalConverter.GetType());

        var values = new List<KeyValuePair<string, int>>
        {
            new("123", 123),
            new("456", 456)
        };

        var json = JsonConvert.SerializeObject(values, Formatting.Indented);

        XUnitAssert.AreEqualNormalized(@"[
  {
    ""Key"": ""123"",
    ""Value"": 123
  },
  {
    ""Key"": ""456"",
    ""Value"": 456
  }
]", json);
    }
}