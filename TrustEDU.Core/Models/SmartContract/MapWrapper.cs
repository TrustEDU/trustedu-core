using System.Collections.Generic;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.SmartContract
{
    internal class MapWrapper : IIterator
    {
        private readonly IEnumerator<KeyValuePair<StackItem, StackItem>> enumerator;

        public MapWrapper(IEnumerable<KeyValuePair<StackItem, StackItem>> map)
        {
            this.enumerator = map.GetEnumerator();
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public StackItem Key()
        {
            return enumerator.Current.Key;
        }

        public bool Next()
        {
            return enumerator.MoveNext();
        }

        public StackItem Value()
        {
            return enumerator.Current.Value;
        }
    }
}