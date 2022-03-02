// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

using System.Dynamic;

public class ExpandoObjectConverterTests : TestFixtureBase
{
    public class ExpandoContainer
    {
        public string Before { get; set; }
        public ExpandoObject Expando { get; set; }
        public string After { get; set; }
    }

    [Fact]
    public void SerializeExpandoObject()
    {
        var d = new ExpandoContainer
        {
            Before = "Before!",
            Expando = new ExpandoObject(),
            After = "After!"
        };

        dynamic o = d.Expando;

        o.String = "String!";
        o.Integer = 234;
        o.Float = 1.23d;
        o.List = new List<string> { "First", "Second", "Third" };
        o.Object = new Dictionary<string, object>
        {
            { "First", 1 }
        };

        var json = JsonConvert.SerializeObject(d, Formatting.Indented);

        XUnitAssert.AreEqualNormalized(@"{
  ""Before"": ""Before!"",
  ""Expando"": {
    ""String"": ""String!"",
    ""Integer"": 234,
    ""Float"": 1.23,
    ""List"": [
      ""First"",
      ""Second"",
      ""Third""
    ],
    ""Object"": {
      ""First"": 1
    }
  },
  ""After"": ""After!""
}", json);
    }

    [Fact]
    public void SerializeNullExpandoObject()
    {
        var d = new ExpandoContainer();

        var json = JsonConvert.SerializeObject(d, Formatting.Indented);

        XUnitAssert.AreEqualNormalized(@"{
  ""Before"": null,
  ""Expando"": null,
  ""After"": null
}", json);
    }

    [Fact]
    public void DeserializeExpandoObject()
    {
        var json = @"{
  ""Before"": ""Before!"",
  ""Expando"": {
    ""String"": ""String!"",
    ""Integer"": 234,
    ""Float"": 1.23,
    ""List"": [
      ""First"",
      ""Second"",
      ""Third""
    ],
    ""Object"": {
      ""First"": 1
    }
  },
  ""After"": ""After!""
}";

    }
}