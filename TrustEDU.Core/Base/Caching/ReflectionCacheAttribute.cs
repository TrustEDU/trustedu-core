using System;

namespace TrustEDU.Core.Base.Caching
{
    public class ReflectionCacheAttribute : Attribute
    {
        public Type Type { get; private set; }

        public ReflectionCacheAttribute(Type type)
        {
            Type = type;
        }
    }
}