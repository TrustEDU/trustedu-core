using TrustEDU.Core.Models.SmartContract.Enumerators;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.SmartContract
{
    internal interface IIterator : IEnumerator
    {
        StackItem Key();
    }
}
