// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

public partial class JProperty
{
    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/> asynchronously.
    /// </summary>
    public override Task WriteToAsync(JsonWriter writer, CancellationToken cancellation, params JsonConverter[] converters)
    {
        var task = writer.WritePropertyNameAsync(Name, cancellation);
        if (task.IsCompletedSucessfully())
        {
            return WriteValueAsync(writer, cancellation, converters);
        }

        return WriteToAsync(task, writer, cancellation, converters);
    }

    async Task WriteToAsync(Task task, JsonWriter writer, CancellationToken cancellation, params JsonConverter[] converters)
    {
        await task.ConfigureAwait(false);

        await WriteValueAsync(writer, cancellation, converters).ConfigureAwait(false);
    }

    Task WriteValueAsync(JsonWriter writer, CancellationToken cancellation, JsonConverter[] converters)
    {
        var value = content.token;
        if (value == null)
        {
            return writer.WriteNullAsync(cancellation);
        }

        return value.WriteToAsync(writer, cancellation, converters);

    }
}