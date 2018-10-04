using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TrustEDU.Core.Base.Json
{
    public class JObject
    {
        public static readonly JObject Null = null;
        private readonly Dictionary<string, JObject> _properties = new Dictionary<string, JObject>();

        public JObject this[string name]
        {
            get
            {
                _properties.TryGetValue(name, out JObject value);
                return value;
            }
            set
            {
                _properties[name] = value;
            }
        }

        public IReadOnlyDictionary<string, JObject> Properties => _properties;

        public virtual bool AsBoolean()
        {
            throw new InvalidCastException();
        }

        public bool AsBooleanOrDefault(bool value = false)
        {
            return !CanConvertTo(typeof(bool)) ? value : AsBoolean();
        }

        public virtual T AsEnum<T>(bool ignoreCase = false)
        {
            throw new InvalidCastException();
        }

        public T AsEnumOrDefault<T>(T value = default(T), bool ignoreCase = false)
        {
            return !CanConvertTo(typeof(T)) ? value : AsEnum<T>(ignoreCase);
        }

        public virtual decimal AsNumber()
        {
            throw new InvalidCastException();
        }

        public decimal AsNumberOrDefault(decimal value = 0)
        {
            return !CanConvertTo(typeof(decimal)) ? value : AsNumber();
        }

        public virtual string AsString()
        {
            throw new InvalidCastException();
        }

        public string AsStringOrDefault(string value = null)
        {
            return !CanConvertTo(typeof(string)) ? value : AsString();
        }

        public virtual bool CanConvertTo(Type type)
        {
            return false;
        }

        public bool ContainsProperty(string key)
        {
            return _properties.ContainsKey(key);
        }

        public static JObject Parse(TextReader reader, int maxNest = 100)
        {
            if (maxNest < 0) throw new FormatException();
            SkipSpace(reader);
            char firstChar = (char)reader.Peek();
            if (firstChar == '\"' || firstChar == '\'')
            {
                return JString.Parse(reader);
            }
            if (firstChar == '[')
            {
                return JArray.Parse(reader, maxNest);
            }
            if ((firstChar >= '0' && firstChar <= '9') || firstChar == '-')
            {
                return JNumber.Parse(reader);
            }
            if (firstChar == 't' || firstChar == 'f')
            {
                return JBoolean.Parse(reader);
            }
            if (firstChar == 'n')
            {
                return ParseNull(reader);
            }
            if (reader.Read() != '{') throw new FormatException();
            SkipSpace(reader);
            JObject obj = new JObject();
            while (reader.Peek() != '}')
            {
                if (reader.Peek() == ',') reader.Read();
                SkipSpace(reader);
                string name = JString.Parse(reader).Value;
                SkipSpace(reader);
                if (reader.Read() != ':') throw new FormatException();
                JObject value = Parse(reader, maxNest - 1);
                obj._properties.Add(name, value);
                SkipSpace(reader);
            }
            reader.Read();
            return obj;
        }

        public static JObject Parse(string value, int maxNest = 100)
        {
            using (StringReader reader = new StringReader(value))
            {
                return Parse(reader, maxNest);
            }
        }

        private static JObject ParseNull(TextReader reader)
        {
            char firstChar = (char)reader.Read();
            if (firstChar == 'n')
            {
                int c2 = reader.Read();
                int c3 = reader.Read();
                int c4 = reader.Read();
                if (c2 == 'u' && c3 == 'l' && c4 == 'l')
                {
                    return null;
                }
            }
            throw new FormatException();
        }

        protected static void SkipSpace(TextReader reader)
        {
            while (reader.Peek() == ' ' || reader.Peek() == '\t' || reader.Peek() == '\r' || reader.Peek() == '\n')
            {
                reader.Read();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            foreach (KeyValuePair<string, JObject> pair in _properties)
            {
                sb.Append('"');
                sb.Append(pair.Key);
                sb.Append('"');
                sb.Append(':');
                if (pair.Value == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append(pair.Value);
                }
                sb.Append(',');
            }
            if (_properties.Count == 0)
            {
                sb.Append('}');
            }
            else
            {
                sb[sb.Length - 1] = '}';
            }
            return sb.ToString();
        }

        public static implicit operator JObject(Enum value)
        {
            return new JString(value.ToString());
        }

        public static implicit operator JObject(JObject[] value)
        {
            return new JArray(value);
        }

        public static implicit operator JObject(bool value)
        {
            return new JBoolean(value);
        }

        public static implicit operator JObject(decimal value)
        {
            return new JNumber(value);
        }

        public static implicit operator JObject(string value)
        {
            return value == null ? null : new JString(value);
        }
    }
}