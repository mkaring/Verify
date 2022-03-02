// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

using TestObjects;

public class CustomCreationConverterTests : TestFixtureBase
{
    public class MyClass
    {
        public string Value { get; set; }

        [JsonConverter(typeof(MyThingConverter))]
        public IThing Thing { get; set; }
    }

    public interface IThing
    {
        int Number { get; }
    }

    public class MyThing : IThing
    {
        public int Number { get; set; }
    }

    public class MyThingConverter : CustomCreationConverter<IThing>
    {
        public override IThing Create(Type type)
        {
            return new MyThing();
        }
    }

    [Fact]
    public void AssertShouldSerializeTest()
    {
        var myClass = new MyClass
        {
            Value = "Foo",
            Thing = new MyThing { Number = 456, }
        };
        var json = JsonConvert.SerializeObject(myClass); // <-- Exception here

        const string expected = @"{""Value"":""Foo"",""Thing"":{""Number"":456}}";
        Assert.Equal(expected, json);
    }

    internal interface IRange<T>
    {
        T First { get; }
        T Last { get; }
    }

    internal class Range<T> : IRange<T>
    {
        public T First { get; set; }
        public T Last { get; set; }
    }

    internal class NullInterfaceTestClass
    {
        public virtual Guid Id { get; set; }
        public virtual int? Year { get; set; }
        public virtual string Company { get; set; }
        public virtual IRange<decimal> DecimalRange { get; set; }
        public virtual IRange<int> IntRange { get; set; }
        public virtual IRange<decimal> NullDecimalRange { get; set; }
    }

    internal class DecimalRangeConverter : CustomCreationConverter<IRange<decimal>>
    {
        public override IRange<decimal> Create(Type type)
        {
            return new Range<decimal>();
        }
    }

    internal class IntRangeConverter : CustomCreationConverter<IRange<int>>
    {
        public override IRange<int> Create(Type type)
        {
            return new Range<int>();
        }
    }

    [Fact]
    public void DeserializeAndConvertNullValue()
    {
        var initial = new NullInterfaceTestClass
        {
            Company = "Company!",
            DecimalRange = new Range<decimal> { First = 0, Last = 1 },
            Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11),
            IntRange = new Range<int> { First = int.MinValue, Last = int.MaxValue },
            Year = 2010,
            NullDecimalRange = null
        };

        var json = JsonConvert.SerializeObject(initial, Formatting.Indented);

        XUnitAssert.AreEqualNormalized(@"{
  ""Id"": ""00000001-0002-0003-0405-060708090a0b"",
  ""Year"": 2010,
  ""Company"": ""Company!"",
  ""DecimalRange"": {
    ""First"": 0.0,
    ""Last"": 1.0
  },
  ""IntRange"": {
    ""First"": -2147483648,
    ""Last"": 2147483647
  },
  ""NullDecimalRange"": null
}", json);
    }
}