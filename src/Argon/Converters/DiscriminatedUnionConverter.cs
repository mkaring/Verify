// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Converts a F# discriminated union type to and from JSON.
/// </summary>
public class DiscriminatedUnionConverter : JsonConverter
{
    #region UnionDefinition
    internal class Union
    {
        public readonly FSharpFunction TagReader;
        public readonly List<UnionCase> Cases;

        public Union(FSharpFunction tagReader, List<UnionCase> cases)
        {
            TagReader = tagReader;
            Cases = cases;
        }
    }

    internal class UnionCase
    {
        public readonly int Tag;
        public readonly string Name;
        public readonly PropertyInfo[] Fields;
        public readonly FSharpFunction FieldReader;
        public readonly FSharpFunction Constructor;

        public UnionCase(int tag, string name, PropertyInfo[] fields, FSharpFunction fieldReader, FSharpFunction constructor)
        {
            Tag = tag;
            Name = name;
            Fields = fields;
            FieldReader = fieldReader;
            Constructor = constructor;
        }
    }
    #endregion

    const string casePropertyName = "Case";
    const string fieldsPropertyName = "Fields";

    static readonly ThreadSafeStore<Type, Union> unionCache = new(CreateUnion);
    static readonly ThreadSafeStore<Type, Type> unionTypeLookupCache = new(CreateUnionTypeLookup);

    static Type CreateUnionTypeLookup(Type type)
    {
        // this lookup is because cases with fields are derived from union type
        // need to get declaring type to avoid duplicate Unions in cache

        // hacky but I can't find an API to get the declaring type without GetUnionCases
        var cases = (object[])FSharpUtils.Instance.GetUnionCases(null, type, null);

        var caseInfo = cases.First();

        return (Type)FSharpUtils.Instance.GetUnionCaseInfoDeclaringType(caseInfo);
    }

    static Union CreateUnion(Type type)
    {
        var u = new Union((FSharpFunction)FSharpUtils.Instance.PreComputeUnionTagReader(null, type, null), new List<UnionCase>());

        var cases = (object[])FSharpUtils.Instance.GetUnionCases(null, type, null);

        foreach (var unionCaseInfo in cases)
        {
            var unionCase = new UnionCase(
                (int)FSharpUtils.Instance.GetUnionCaseInfoTag(unionCaseInfo),
                (string)FSharpUtils.Instance.GetUnionCaseInfoName(unionCaseInfo),
                (PropertyInfo[])FSharpUtils.Instance.GetUnionCaseInfoFields(unionCaseInfo)!,
                (FSharpFunction)FSharpUtils.Instance.PreComputeUnionReader(null, unionCaseInfo, null),
                (FSharpFunction)FSharpUtils.Instance.PreComputeUnionConstructor(null, unionCaseInfo, null));

            u.Cases.Add(unionCase);
        }

        return u;
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var resolver = serializer.ContractResolver as DefaultContractResolver;

        var unionType = unionTypeLookupCache.Get(value.GetType());
        var union = unionCache.Get(unionType);

        var tag = (int)union.TagReader.Invoke(value);
        var caseInfo = union.Cases.Single(c => c.Tag == tag);

        writer.WriteStartObject();
        writer.WritePropertyName(resolver != null ? resolver.GetResolvedPropertyName(casePropertyName) : casePropertyName);
        writer.WriteValue(caseInfo.Name);
        if (caseInfo.Fields is {Length: > 0})
        {
            var fields = (object[])caseInfo.FieldReader.Invoke(value);

            writer.WritePropertyName(resolver != null ? resolver.GetResolvedPropertyName(fieldsPropertyName) : fieldsPropertyName);
            writer.WriteStartArray();
            foreach (var field in fields)
            {
                serializer.Serialize(writer, field);
            }
            writer.WriteEndArray();
        }
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
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return false;
        }

        // all fsharp objects have CompilationMappingAttribute
        // get the fsharp assembly from the attribute and initialize latebound methods
        var attributes = type.GetCustomAttributes(true);

        var isFSharpType = false;
        foreach (var attribute in attributes)
        {
            var attributeType = attribute.GetType();
            if (attributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute")
            {
                FSharpUtils.EnsureInitialized(attributeType.Assembly);

                isFSharpType = true;
                break;
            }
        }

        if (isFSharpType)
        {
            return (bool) FSharpUtils.Instance.IsUnion(null, type, null);
        }

        return false;
    }
}