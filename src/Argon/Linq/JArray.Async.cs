// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

public partial class JArray
{
    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/> asynchronously.
    /// </summary>
    public override async Task WriteToAsync(JsonWriter writer, CancellationToken cancellation, params JsonConverter[] converters)
    {
        await writer.WriteStartArrayAsync(cancellation).ConfigureAwait(false);

        for (var i = 0; i < values.Count; i++)
        {
            await values[i].WriteToAsync(writer, cancellation, converters).ConfigureAwait(false);
        }

        await writer.WriteEndArrayAsync(cancellation).ConfigureAwait(false);
    }
}