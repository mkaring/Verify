// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

using System.Data.SqlTypes;

namespace Argon;

/// <summary>
/// Converts a binary value to and from a base 64 string value.
/// </summary>
public class BinaryConverter : JsonConverter
{
    const string binaryTypeName = "System.Data.Linq.Binary";
    const string binaryToArrayName = "ToArray";
    static ReflectionObject? reflectionObject;

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var data = GetByteArray(value);

        writer.WriteValue(data);
    }

    static byte[] GetByteArray(object value)
    {
        if (value.GetType().FullName == binaryTypeName)
        {
            EnsureReflectionObject(value.GetType());
            MiscellaneousUtils.Assert(reflectionObject != null);

            return (byte[])reflectionObject.GetValue(value, binaryToArrayName)!;
        }
        if (value is SqlBinary binary)
        {
            return binary.Value;
        }

        throw new JsonSerializationException($"Unexpected value type when writing binary: {value.GetType()}");
    }

    static void EnsureReflectionObject(Type type)
    {
        reflectionObject ??= ReflectionObject.Create(type, type.GetConstructor(new[] {typeof(byte[])}), binaryToArrayName);
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type type)
    {
        return type.FullName == binaryTypeName ||
               type == typeof(SqlBinary) ||
               type == typeof(SqlBinary?);
    }
}