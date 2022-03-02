// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

using TestObjects;

public class IsoDateTimeConverterTests : TestFixtureBase
{
    [Fact]
    public void PropertiesShouldBeSet()
    {
        var converter = new IsoDateTimeConverter();
        Assert.Equal(CultureInfo.CurrentCulture, converter.Culture);
        Assert.Equal(string.Empty, converter.DateTimeFormat);
        Assert.Equal(DateTimeStyles.RoundtripKind, converter.DateTimeStyles);

        converter = new IsoDateTimeConverter
        {
            DateTimeFormat = "F",
            Culture = CultureInfo.InvariantCulture,
            DateTimeStyles = DateTimeStyles.None
        };

        Assert.Equal(CultureInfo.InvariantCulture, converter.Culture);
        Assert.Equal("F", converter.DateTimeFormat);
        Assert.Equal(DateTimeStyles.None, converter.DateTimeStyles);
    }

    public static string GetUtcOffsetText(DateTime d)
    {
        var utcOffset = d.GetUtcOffset();

        return $"{utcOffset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture)}:{utcOffset.Minutes.ToString("00;00", CultureInfo.InvariantCulture)}";
    }

    [Fact]
    public void SerializeDateTime()
    {
        var converter = new IsoDateTimeConverter();

        var d = new DateTime(2000, 12, 15, 22, 11, 3, 55, DateTimeKind.Utc);

        var result = JsonConvert.SerializeObject(d, converter);
        Assert.Equal(@"""2000-12-15T22:11:03.055Z""", result);
    }

    [Fact]
    public void SerializeFormattedDateTimeInvariantCulture()
    {
        var converter = new IsoDateTimeConverter { DateTimeFormat = "F", Culture = CultureInfo.InvariantCulture };

        var d = new DateTime(2000, 12, 15, 22, 11, 3, 0, DateTimeKind.Utc);

        var result = JsonConvert.SerializeObject(d, converter);
        Assert.Equal(@"""Friday, 15 December 2000 22:11:03""", result);
    }

    [Fact]
    public void SerializeCustomFormattedDateTime()
    {
        var converter = new IsoDateTimeConverter
        {
            DateTimeFormat = "dd/MM/yyyy",
            Culture = CultureInfo.InvariantCulture
        };

        var json = @"""09/12/2006""";
    }

    [Fact]
    public void SerializeFormattedDateTimeNewZealandCulture()
    {
        var culture = new CultureInfo("en-NZ")
        {
            DateTimeFormat =
            {
                AMDesignator = "a.m.",
                PMDesignator = "p.m."
            }
        };

        var converter = new IsoDateTimeConverter { DateTimeFormat = "F", Culture = culture };

        var d = new DateTime(2000, 12, 15, 22, 11, 3, 0, DateTimeKind.Utc);

        var result = JsonConvert.SerializeObject(d, converter);
        Assert.Equal(@"""Friday, 15 December 2000 10:11:03 p.m.""", result);
    }

    [Fact]
    public void SerializeDateTimeOffset()
    {
        var converter = new IsoDateTimeConverter();

        var d = new DateTimeOffset(2000, 12, 15, 22, 11, 3, 55, TimeSpan.Zero);

        var result = JsonConvert.SerializeObject(d, converter);
        Assert.Equal(@"""2000-12-15T22:11:03.055+00:00""", result);
    }

    [Fact]
    public void SerializeUTC()
    {
        var c = new DateTimeTestClass
        {
            DateTimeField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime(),
            DateTimeOffsetField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime(),
            PreField = "Pre",
            PostField = "Post"
        };
        var json = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
        Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2008-12-12T12:12:12Z"",""DateTimeOffsetField"":""2008-12-12T12:12:12+00:00"",""PostField"":""Post""}", json);

        //test the other edge case too
        c.DateTimeField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc).ToLocalTime();
        c.DateTimeOffsetField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc).ToLocalTime();
        c.PreField = "Pre";
        c.PostField = "Post";
        json = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
        Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2008-01-01T01:01:01Z"",""DateTimeOffsetField"":""2008-01-01T01:01:01+00:00"",""PostField"":""Post""}", json);
    }

    [Fact]
    public void NullableSerializeUTC()
    {
        var c = new NullableDateTimeTestClass
        {
            DateTimeField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime(),
            DateTimeOffsetField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime(),
            PreField = "Pre",
            PostField = "Post"
        };
        var json = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
        Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2008-12-12T12:12:12Z"",""DateTimeOffsetField"":""2008-12-12T12:12:12+00:00"",""PostField"":""Post""}", json);

        //test the other edge case too
        c.DateTimeField = null;
        c.DateTimeOffsetField = null;
        c.PreField = "Pre";
        c.PostField = "Post";
        json = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
        Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":null,""DateTimeOffsetField"":null,""PostField"":""Post""}", json);
    }

    [Fact]
    public void SerializeShouldChangeNonUTCDates()
    {
        var localDateTime = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Local);

        var c = new DateTimeTestClass
        {
            DateTimeField = localDateTime,
            PreField = "Pre",
            PostField = "Post"
        };
        var json = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }); //note that this fails without the Utc converter...
        c.DateTimeField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc);
        var json2 = JsonConvert.SerializeObject(c, new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });

        var offset = localDateTime.GetUtcOffset();

        // if the current timezone is utc then local already equals utc
        if (offset == TimeSpan.Zero)
        {
            Assert.Equal(json, json2);
        }
        else
        {
            Assert.NotEqual(json, json2);
        }
    }

    [Fact]
    public void BlogCodeSample()
    {
        var p = new Person
        {
            Name = "Keith",
            BirthDate = new DateTime(1980, 3, 8),
            LastModified = new DateTime(2009, 4, 12, 20, 44, 55),
        };

        var jsonText = JsonConvert.SerializeObject(p, new IsoDateTimeConverter());
        // {
        //   "Name": "Keith",
        //   "BirthDate": "1980-03-08T00:00:00",
        //   "LastModified": "2009-04-12T20:44:55"
        // }

        Assert.Equal(@"{""Name"":""Keith"",""BirthDate"":""1980-03-08T00:00:00"",""LastModified"":""2009-04-12T20:44:55""}", jsonText);
    }
}