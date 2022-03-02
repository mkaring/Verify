// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.


public class JTokenAsyncTests : TestFixtureBase
{
    [Fact]
    public async Task CreateWriterAsync()
    {
        var a =
            new JArray(
                5,
                new JArray(1),
                new JArray(1, 2),
                new JArray(1, 2, 3)
            );

        var writer = a.CreateWriter();
        Assert.NotNull(writer);
        Assert.Equal(4, a.Count);

        await writer.WriteValueAsync("String");
        Assert.Equal(5, a.Count);
        Assert.Equal("String", (string)a[4]);

        await writer.WriteStartObjectAsync();
        await writer.WritePropertyNameAsync("Property");
        await writer.WriteValueAsync("PropertyValue");
        await writer.WriteEndAsync();

        Assert.Equal(6, a.Count);
        Assert.True(JToken.DeepEquals(new JObject(new JProperty("Property", "PropertyValue")), a[5]));
    }
}