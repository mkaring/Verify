// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

using Microsoft.FSharp.Reflection;
using TestObjects;

public class DiscriminatedUnionConverterTests : TestFixtureBase
{
    public class DoubleDoubleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var d = (double)value;

            writer.WriteValue(d * 2);
        }

        public override bool CanConvert(Type type)
        {
            return type == typeof(double);
        }
    }

    [Fact]
    public void SerializeUnionWithConverter()
    {
        var json = JsonConvert.SerializeObject(Shape.NewRectangle(10.0, 5.0), new DoubleDoubleConverter());

        Assert.Equal(@"{""Case"":""Rectangle"",""Fields"":[20.0,10.0]}", json);
    }

    [Fact]
    public void SerializeBasicUnion()
    {
        var json = JsonConvert.SerializeObject(Currency.AUD);

        Assert.Equal(@"{""Case"":""AUD""}", json);
    }

    [Fact]
    public void SerializePerformance()
    {
        var values = new List<Shape>
        {
            Shape.NewRectangle(10.0, 5.0),
            Shape.NewCircle(7.5)
        };

        var json = JsonConvert.SerializeObject(values, Formatting.Indented);

        var ts = new Stopwatch();
        ts.Start();

        for (var i = 0; i < 100; i++)
        {
            JsonConvert.SerializeObject(values);
        }

        ts.Stop();

        Console.WriteLine(ts.Elapsed.TotalSeconds);
    }

    [Fact]
    public void SerializeUnionWithFields()
    {
        var json = JsonConvert.SerializeObject(Shape.NewRectangle(10.0, 5.0));

        Assert.Equal(@"{""Case"":""Rectangle"",""Fields"":[10.0,5.0]}", json);
    }

    public class Union
    {
        public List<UnionCase> Cases;
        public Converter<object, int> TagReader { get; set; }
    }

    public class UnionCase
    {
        public int Tag;
        public string Name;
        public PropertyInfo[] Fields;
        public Converter<object, object[]> FieldReader;
        public Converter<object[], object> Constructor;
    }

    static Union CreateUnion(Type type)
    {
        var u = new Union
        {
            TagReader = s => FSharpValue.PreComputeUnionTagReader(type, null).Invoke(s),
            Cases = new List<UnionCase>()
        };

        var cases = FSharpType.GetUnionCases(type, null);

        foreach (var unionCaseInfo in cases)
        {
            var unionCase = new UnionCase
            {
                Tag = unionCaseInfo.Tag,
                Name = unionCaseInfo.Name,
                Fields = unionCaseInfo.GetFields(),
                FieldReader = s => FSharpValue.PreComputeUnionReader(unionCaseInfo, null).Invoke(s),
                Constructor = s => FSharpValue.PreComputeUnionConstructor(unionCaseInfo, null).Invoke(s)
            };

            u.Cases.Add(unionCase);
        }

        return u;
    }

    [Fact]
    public void Serialize()
    {
        var value = Shape.NewRectangle(10.0, 5.0);

        var union = CreateUnion(value.GetType());

        var tag = union.TagReader.Invoke(value);

        var caseInfo = union.Cases.Single(c => c.Tag == tag);

        var fields = caseInfo.FieldReader.Invoke(value);

        Assert.Equal(10d, fields[0]);
        Assert.Equal(5d, fields[1]);
    }

    [Fact]
    public void Deserialize()
    {
        var union = CreateUnion(typeof(Shape.Rectangle));

        var caseInfo = union.Cases.Single(c => c.Name == "Rectangle");

        var value = (Shape.Rectangle)caseInfo.Constructor.Invoke(new object[]
        {
            10.0, 5.0
        });

        Assert.Equal("TestObjects.Shape+Rectangle", value.ToString());
        Assert.Equal(10, value.width);
        Assert.Equal(5, value.length);
    }

    [Fact]
    public void SerializeUnionWithTypeNameHandlingAndReferenceTracking()
    {
        var json = JsonConvert.SerializeObject(Shape.NewRectangle(10.0, 5.0), new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.All
        });

        Assert.Equal(@"{""Case"":""Rectangle"",""Fields"":[10.0,5.0]}", json);
    }
}