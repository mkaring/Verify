// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts a <see cref="Regex"/> to and from JSON.
/// </summary>
public class RegexConverter : JsonConverter
{
    const string patternName = "Pattern";
    const string optionsName = "Options";

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var regex = (Regex) value;

        WriteJson(writer, regex, serializer);
    }

    static void WriteJson(JsonWriter writer, Regex regex, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        var value = regex.ToString();
        if (serializer.ContractResolver is DefaultContractResolver resolver)
        {
            writer.WritePropertyName(resolver.GetResolvedPropertyName(patternName));
            writer.WriteValue(value);
            writer.WritePropertyName(resolver.GetResolvedPropertyName(optionsName));
        }
        else
        {
            writer.WritePropertyName(patternName);
            writer.WriteValue(value);
            writer.WritePropertyName(optionsName);
        }

        serializer.Serialize(writer, regex.Options);
        writer.WriteEndObject();
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type type)
    {
        return type.Name == nameof(Regex) && IsRegex(type);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static bool IsRegex(Type type)
    {
        return type == typeof(Regex);
    }
}