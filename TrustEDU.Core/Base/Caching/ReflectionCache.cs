using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrustEDU.Core.Base.Caching
{
    public class ReflectionCache<T> : Dictionary<T, Type>
    {
        public static ReflectionCache<T> CreateFromEnum<TEnumType>() where TEnumType : struct, IConvertible
        {
            var enumType = typeof(TEnumType);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("Enumerated type is required");
                
            var r = new ReflectionCache<T>();

            foreach (var t in Enum.GetValues(enumType))
            {
                // Get enumn member
                var memInfo = enumType.GetMember(t.ToString());
                if (memInfo.Length != 1)
                    throw (new FormatException());

                // Get attribute
                var attribute = memInfo[0].GetCustomAttributes(typeof(ReflectionCacheAttribute), false)
                    .Cast<ReflectionCacheAttribute>()
                    .FirstOrDefault();

                if (attribute == null)
                    throw (new FormatException());

                // Append to cache
                r.Add((T)t, attribute.Type);
            }
            return r;
        }
        /// <summary>
        /// Creates a cache instance using key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        public object CreateInstance(T key, object def = null)
        {
            // Get Type from cache
            if (TryGetValue(key, out var tp)) return Activator.CreateInstance(tp);

            return def; // null
        }
        /// <summary>
        /// Creates a cache instance 
        /// </summary>
        /// <typeparam name="TK">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        public TK CreateInstance<TK>(T key, TK def = default(TK))
        {
            // Get Type from cache
            if (TryGetValue(key, out var tp)) return (TK)Activator.CreateInstance(tp);

            // return null
            return def;
        }
    }
}