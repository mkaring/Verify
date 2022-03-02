// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

public class GenericJsonConverterTests : TestFixtureBase
{
    public class TestGenericConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

    [Fact]
    public void WriteJsonObject()
    {
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        var converter = new TestGenericConverter();
        converter.WriteJson(jsonWriter, (object)"String!", null);

        Assert.Equal(@"""String!""", stringWriter.ToString());
    }

    [Fact]
    public void WriteJsonGeneric()
    {
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        var converter = new TestGenericConverter();
        converter.WriteJson(jsonWriter, "String!", null);

        Assert.Equal(@"""String!""", stringWriter.ToString());
    }

    [Fact]
    public void WriteJsonBadType()
    {
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        var converter = new TestGenericConverter();

        XUnitAssert.Throws<JsonSerializationException>(
            () => converter.WriteJson(jsonWriter, 123, null),
            "Converter cannot write specified value to JSON. System.String is required.");
    }
}