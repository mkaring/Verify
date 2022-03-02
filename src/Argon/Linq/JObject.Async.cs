// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

public partial class JObject
{
    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/> asynchronously.
    /// </summary>
    public override Task WriteToAsync(JsonWriter writer, CancellationToken cancellation, params JsonConverter[] converters)
    {
        var t = writer.WriteStartObjectAsync(cancellation);
        if (!t.IsCompletedSucessfully())
        {
            return AwaitProperties(t, 0, writer, cancellation, converters);
        }

        for (var i = 0; i < properties.Count; i++)
        {
            t = properties[i].WriteToAsync(writer, cancellation, converters);
            if (!t.IsCompletedSucessfully())
            {
                return AwaitProperties(t, i + 1, writer, cancellation, converters);
            }
        }

        return writer.WriteEndObjectAsync(cancellation);

        // Local functions, params renamed (capitalized) so as not to capture and allocate when calling async
        async Task AwaitProperties(Task task, int i, JsonWriter Writer, CancellationToken CancellationToken, JsonConverter[] Converters)
        {
            await task.ConfigureAwait(false);
            for (; i < properties.Count; i++)
            {
                await properties[i].WriteToAsync(Writer, CancellationToken, Converters).ConfigureAwait(false);
            }

            await Writer.WriteEndObjectAsync(CancellationToken).ConfigureAwait(false);
        }
    }

}