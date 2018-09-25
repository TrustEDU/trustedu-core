using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrustEDU.Core.Base.Caching
{
    public class ReflectionCache<T> : Dictionary<T, Type>
    {
        public static ReflectionCache<T> CreateFromEnum<EnumType>() where EnumType : struct, IConvertible
        {
            Type enumType = typeof(EnumType);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("Enumerated type is required");
                
            ReflectionCache<T> r = new ReflectionCache<T>();

            foreach (object t in Enum.GetValues(enumType))
            {
                // Get enumn member
                MemberInfo[] memInfo = enumType.GetMember(t.ToString());
                if (memInfo == null || memInfo.Length != 1)
                    throw (new FormatException());

                // Get attribute
                ReflectionCacheAttribute attribute = memInfo[0].GetCustomAttributes(typeof(ReflectionCacheAttribute), false)
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
            if (TryGetValue(key, out Type tp)) return Activator.CreateInstance(tp);

            return def; // null
        }
        /// <summary>
        /// Creates a cache instance 
        /// </summary>
        /// <typeparam name="K">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        public K CreateInstance<K>(T key, K def = default(K))
        {
            Type tp;

            // Get Type from cache
            if (TryGetValue(key, out tp)) return (K)Activator.CreateInstance(tp);

            // return null
            return def;
        }
    }
}