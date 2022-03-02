// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace TestObjects;

public class NameContainerConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var nameContainer = value as NameContainer;

        if (nameContainer != null)
        {
            writer.WriteValue(nameContainer.Value);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(NameContainer);
    }
}