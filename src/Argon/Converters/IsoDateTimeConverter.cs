// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts a <see cref="DateTime"/> to and from the ISO 8601 date format (e.g. <c>"2008-04-12T12:53Z"</c>).
/// </summary>
public class IsoDateTimeConverter : DateTimeConverterBase
{
    const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

    string? dateTimeFormat;
    CultureInfo? culture;

    /// <summary>
    /// Gets or sets the date time styles used when converting a date to and from JSON.
    /// </summary>
    public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.RoundtripKind;

    /// <summary>
    /// Gets or sets the date time format used when converting a date to and from JSON.
    /// </summary>
    public string? DateTimeFormat
    {
        get => dateTimeFormat ?? string.Empty;
        set => dateTimeFormat = StringUtils.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>
    /// Gets or sets the culture used when converting a date to and from JSON.
    /// </summary>
    public CultureInfo Culture
    {
        get => culture ?? CultureInfo.CurrentCulture;
        set => culture = value;
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        string text;

        if (value is DateTime dateTime)
        {
            if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            text = dateTime.ToString(dateTimeFormat ?? DefaultDateTimeFormat, Culture);
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                dateTimeOffset = dateTimeOffset.ToUniversalTime();
            }

            text = dateTimeOffset.ToString(dateTimeFormat ?? DefaultDateTimeFormat, Culture);
        }
        else
        {
            throw new JsonSerializationException($"Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {ReflectionUtils.GetObjectType(value)!}.");
        }

        writer.WriteValue(text);
    }
}