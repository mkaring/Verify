// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
/// </summary>
public class JsonPrimitiveContract : JsonContract
{
    internal PrimitiveTypeCode TypeCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPrimitiveContract"/> class.
    /// </summary>
    public JsonPrimitiveContract(Type underlyingType)
        : base(underlyingType)
    {
        ContractType = JsonContractType.Primitive;

        TypeCode = ConvertUtils.GetTypeCode(underlyingType);
        IsReadOnlyOrFixedSize = true;
    }
}