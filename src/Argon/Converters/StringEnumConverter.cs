// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts an <see cref="Enum"/> to and from its name string value.
/// </summary>
public class StringEnumConverter : JsonConverter
{
    /// <summary>
    /// Gets or sets the naming strategy used to resolve how enum text is written.
    /// </summary>
    public NamingStrategy? NamingStrategy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether integer values are allowed when serializing and deserializing.
    /// The default value is <c>true</c>.
    /// </summary>
    public bool AllowIntegerValues { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
    /// </summary>
    public StringEnumConverter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
    /// </summary>
    public StringEnumConverter(NamingStrategy namingStrategy, bool allowIntegerValues = true)
    {
        NamingStrategy = namingStrategy;
        AllowIntegerValues = allowIntegerValues;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
    /// </summary>
    /// <param name="namingStrategyType">The <see cref="System.Type"/> of the <see cref="Argon.NamingStrategy"/> used to write enum text.</param>
    public StringEnumConverter(Type namingStrategyType)
    {
        NamingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(namingStrategyType, null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
    /// </summary>
    /// <param name="namingStrategyType">The <see cref="System.Type"/> of the <see cref="Argon.NamingStrategy"/> used to write enum text.</param>
    /// <param name="namingStrategyParameters">
    /// The parameter list to use when constructing the <see cref="Argon.NamingStrategy"/> described by <paramref name="namingStrategyType"/>.
    /// If <c>null</c>, the default constructor is used.
    /// When non-<c>null</c>, there must be a constructor defined in the <see cref="Argon.NamingStrategy"/> that exactly matches the number,
    /// order, and type of these parameters.
    /// </param>
    public StringEnumConverter(Type namingStrategyType, object[] namingStrategyParameters)
    {
        NamingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(namingStrategyType, namingStrategyParameters);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
    /// </summary>
    /// <param name="namingStrategyType">The <see cref="System.Type"/> of the <see cref="Argon.NamingStrategy"/> used to write enum text.</param>
    /// <param name="namingStrategyParameters">
    /// The parameter list to use when constructing the <see cref="Argon.NamingStrategy"/> described by <paramref name="namingStrategyType"/>.
    /// If <c>null</c>, the default constructor is used.
    /// When non-<c>null</c>, there must be a constructor defined in the <see cref="Argon.NamingStrategy"/> that exactly matches the number,
    /// order, and type of these parameters.
    /// </param>
    /// <param name="allowIntegerValues"><c>true</c> if integers are allowed when serializing and deserializing; otherwise, <c>false</c>.</param>
    public StringEnumConverter(Type namingStrategyType, object[] namingStrategyParameters, bool allowIntegerValues)
    {
        NamingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(namingStrategyType, namingStrategyParameters);
        AllowIntegerValues = allowIntegerValues;
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var e = (Enum)value;

        if (EnumUtils.TryToString(e.GetType(), value, NamingStrategy, out var enumName))
        {
            writer.WriteValue(enumName);
            return;
        }

        if (!AllowIntegerValues)
        {
            throw JsonSerializationException.Create(null, writer.ContainerPath, $"Integer value {e.ToString("D")} is not allowed.", null);
        }

        // enum value has no name so write number
        writer.WriteValue(value);
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type type)
    {
        if (type.IsNullableType())
        {
            return Nullable.GetUnderlyingType(type)!.IsEnum;
        }

        return type.IsEnum;
    }
}