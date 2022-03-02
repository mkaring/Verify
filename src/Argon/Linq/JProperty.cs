// Copyright (c) 2007 James Newton-King. All rights reserved.
// Use of this source code is governed by The MIT License,
// as found in the license.md file.

namespace Argon;

/// <summary>
/// Represents a JSON property.
/// </summary>
public partial class JProperty : JContainer
{
    #region JPropertyList
    class JPropertyList : IList<JToken>
    {
        internal JToken? token;

        public IEnumerator<JToken> GetEnumerator()
        {
            if (token != null)
            {
                yield return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(JToken item)
        {
            token = item;
        }

        public void Clear()
        {
            token = null;
        }

        public bool Contains(JToken item)
        {
            return token == item;
        }

        public void CopyTo(JToken[] array, int arrayIndex)
        {
            if (token != null)
            {
                array[arrayIndex] = token;
            }
        }

        public bool Remove(JToken item)
        {
            if (token == item)
            {
                token = null;
                return true;
            }
            return false;
        }

        public int Count => token != null ? 1 : 0;

        public bool IsReadOnly => false;

        public int IndexOf(JToken item)
        {
            return token == item ? 0 : -1;
        }

        public void Insert(int index, JToken item)
        {
            if (index == 0)
            {
                token = item;
            }
        }

        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                token = null;
            }
        }

        public JToken this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }

                MiscellaneousUtils.Assert(token != null);
                return token;
            }
            set
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }

                token = value;
            }
        }
    }
    #endregion

    readonly JPropertyList content = new();

    /// <summary>
    /// Gets the container's children tokens.
    /// </summary>
    protected override IList<JToken> ChildrenTokens => content;

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name { [DebuggerStepThrough] get; }

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    public JToken Value
    {
        [DebuggerStepThrough]
        get => content.token!;
        set
        {
            var newValue = value ?? JValue.CreateNull();

            if (content.token == null)
            {
                InsertItem(0, newValue, false);
            }
            else
            {
                SetItem(0, newValue);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JProperty"/> class from another <see cref="JProperty"/> object.
    /// </summary>
    /// <param name="other">A <see cref="JProperty"/> object to copy from.</param>
    public JProperty(JProperty other)
        : base(other)
    {
        Name = other.Name;
    }

    internal override JToken GetItem(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        return Value;
    }

    internal override void SetItem(int index, JToken? item)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (IsTokenUnchanged(Value, item))
        {
            return;
        }

        base.SetItem(0, item);
    }

    internal override bool RemoveItem(JToken? item)
    {
        throw new JsonException($"Cannot add or remove items from {typeof(JProperty)}.");
    }

    internal override void RemoveItemAt(int index)
    {
        throw new JsonException($"Cannot add or remove items from {typeof(JProperty)}.");
    }

    internal override int IndexOfItem(JToken? item)
    {
        if (item == null)
        {
            return -1;
        }

        return content.IndexOf(item);
    }

    internal override bool InsertItem(int index, JToken? item, bool skipParentCheck)
    {
        // don't add comments to JProperty
        if (item is {Type: JTokenType.Comment})
        {
            return false;
        }

        if (Value != null)
        {
            throw new JsonException($"{typeof(JProperty)} cannot have multiple values.");
        }

        return base.InsertItem(0, item, false);
    }

    internal override bool ContainsItem(JToken? item)
    {
        return Value == item;
    }

    internal override void MergeItem(object content, JsonMergeSettings? settings)
    {
        var value = (content as JProperty)?.Value;

        if (value != null && value.Type != JTokenType.Null)
        {
            Value = value;
        }
    }

    internal override void ClearItems()
    {
        throw new JsonException($"Cannot add or remove items from {typeof(JProperty)}.");
    }

    internal override bool DeepEquals(JToken node)
    {
        return node is JProperty t && Name == t.Name && ContentsEqual(t);
    }

    internal override JToken CloneToken()
    {
        return new JProperty(this);
    }

    /// <summary>
    /// Gets the node type for this <see cref="JToken"/>.
    /// </summary>
    public override JTokenType Type
    {
        [DebuggerStepThrough]
        get => JTokenType.Property;
    }

    internal JProperty(string name)
    {
        // called from JTokenWriter
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JProperty"/> class.
    /// </summary>
    public JProperty(string name, params object[] content)
        : this(name, (object)content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JProperty"/> class.
    /// </summary>
    public JProperty(string name, object? content)
    {
        Name = name;

        Value = IsMultiContent(content)
            ? new JArray(content)
            : CreateFromContent(content);
    }

    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/>.
    /// </summary>
    public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
    {
        writer.WritePropertyName(Name);

        var value = content.token;
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            value.WriteTo(writer, converters);
        }
    }

    internal override int GetDeepHashCode()
    {
        return Name.GetHashCode() ^ (Value?.GetDeepHashCode() ?? 0);
    }
}