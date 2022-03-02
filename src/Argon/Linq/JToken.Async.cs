// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

public abstract partial class JToken
{
    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/> asynchronously.
    /// </summary>
    public virtual Task WriteToAsync(JsonWriter writer, CancellationToken cancellation, params JsonConverter[] converters)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/> asynchronously.
    /// </summary>
    public Task WriteToAsync(JsonWriter writer, params JsonConverter[] converters)
    {
        return WriteToAsync(writer, default, converters);
    }
}