// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace TestObjects;

public class EncodingReadConverter : JsonConverter
{
    public override bool CanConvert(Type type)
    {
        return typeof(Encoding).IsAssignableFrom(type);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}