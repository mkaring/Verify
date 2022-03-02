// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace TestObjects;

public class ListOfIds<T> : JsonConverter where T : Bar, new()
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var list = (IList<T>)value;

        writer.WriteStartArray();
        foreach (var item in list)
        {
            writer.WriteValue(item.Id);
        }
        writer.WriteEndArray();
    }

    public override bool CanConvert(Type type)
    {
        return typeof(IList<T>).IsAssignableFrom(type);
    }
}