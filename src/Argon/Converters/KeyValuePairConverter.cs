// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts a <see cref="KeyValuePair{TKey,TValue}"/> to and from JSON.
/// </summary>
public class KeyValuePairConverter : JsonConverter
{
    const string keyName = "Key";
    const string valueName = "Value";

    static readonly ThreadSafeStore<Type, ReflectionObject> reflectionObjectPerType = new(InitializeReflectionObject);

    static ReflectionObject InitializeReflectionObject(Type type)
    {
        var genericArguments = type.GetGenericArguments();
        var keyType = genericArguments[0];
        var valueType = genericArguments[1];

        return ReflectionObject.Create(type, type.GetConstructor(new[] { keyType, valueType }), keyName, valueName);
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var reflectionObject = reflectionObjectPerType.Get(value.GetType());

        var resolver = serializer.ContractResolver as DefaultContractResolver;

        writer.WriteStartObject();
        writer.WritePropertyName(resolver != null ? resolver.GetResolvedPropertyName(keyName) : keyName);
        serializer.Serialize(writer, reflectionObject.GetValue(value, keyName), reflectionObject.GetType(keyName));
        writer.WritePropertyName(resolver != null ? resolver.GetResolvedPropertyName(valueName) : valueName);
        serializer.Serialize(writer, reflectionObject.GetValue(value, valueName), reflectionObject.GetType(valueName));
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
        var t = type.IsNullableType()
            ? Nullable.GetUnderlyingType(type)!
            : type;

        if (t.IsValueType && t.IsGenericType)
        {
            return t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        return false;
    }
}