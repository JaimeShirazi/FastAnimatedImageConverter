using FAIC.Types;
using System;
using System.Globalization;
using System.Text.Json;

namespace FAIC.Types
{
    public abstract class BaseStreamData
    {
        public bool ReadData { get; private set; }
        private readonly string key;
        public BaseStreamData(string key) { this.key = key; }
        public void TryRead(JsonElement element)
        {
            if (element.TryGetProperty(key, out JsonElement property))
            {
                if (TryReadProperty(property)) ReadData = true;
            }
        }
        /// <summary>
        /// Called from the superclass when the associated <see cref="JsonElement"/> is successfully retrieved.
        /// </summary>
        /// <param name="property">The <see cref="JsonElement"/> associated with the StreamData</param>
        /// <returns>If the read was successful</returns>
        protected abstract bool TryReadProperty(JsonElement property);
    }
    public class StringStreamData : BaseStreamData, IEquatable<StringStreamData>
    {
        public string Value;
        public StringStreamData(string key) : base(key) { }

        protected override bool TryReadProperty(JsonElement property)
        {
            Value = property.ValueKind switch
            {
                JsonValueKind.String => property.GetString(),
                JsonValueKind.Undefined or JsonValueKind.Null => null,
                JsonValueKind.False => "false", //because boolean isn't IParsable, this string struct should be used instead
                JsonValueKind.True => "true",
                _ => property.ToString()
            };
            return true;
        }
        public static implicit operator string(StringStreamData streamData) => streamData.Value;
        public override bool Equals(object obj)
        {
            return Equals(obj as StringStreamData);
        }
        public bool Equals(StringStreamData other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Value == other.Value;
        }
        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(StringStreamData a, StringStreamData b) => a.Value == b.Value;
        public static bool operator !=(StringStreamData a, StringStreamData b) => a.Value != b.Value;
    }
    public class ParsedStreamData<T> : BaseStreamData, IEquatable<ParsedStreamData<T>>
        where T : struct, IParsable<T>, IEquatable<T>
    {
        public T Value;
        public ParsedStreamData(string key) : base(key) { }
        protected override bool TryReadProperty(JsonElement property)
        {
            string propertyStr = property.ValueKind switch
            {
                JsonValueKind.String => property.GetString(),
                JsonValueKind.Undefined or JsonValueKind.Null => null,
                _ => property.ToString() //this is jank, but to keep the logic generic, we're gonna turn the value back into a string
            };
            return T.TryParse(propertyStr, CultureInfo.InvariantCulture, out Value);
        }
        public static implicit operator T(ParsedStreamData<T> streamData) => streamData.Value;
        public override bool Equals(object obj)
        {
            return Equals(obj as ParsedStreamData<T>);
        }
        public bool Equals(ParsedStreamData<T> other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Value.Equals(other.Value);
        }
        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(ParsedStreamData<T> a, ParsedStreamData<T> b) => a.Value.Equals(b.Value);
        public static bool operator !=(ParsedStreamData<T> a, ParsedStreamData<T> b) => !a.Value.Equals(b.Value);
    }
}
