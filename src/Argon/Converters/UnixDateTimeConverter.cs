// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts a <see cref="DateTime"/> to and from Unix epoch time
/// </summary>
public class UnixDateTimeConverter : DateTimeConverterBase
{
    internal static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        long seconds;

        if (value is DateTime dateTime)
        {
            seconds = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            seconds = (long)(dateTimeOffset.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }
        else
        {
            throw new JsonSerializationException("Expected date object value.");
        }

        if (seconds < 0)
        {
            throw new JsonSerializationException("Cannot convert date value that is before Unix epoch of 00:00:00 UTC on 1 January 1970.");
        }

        writer.WriteValue(seconds);
    }
}