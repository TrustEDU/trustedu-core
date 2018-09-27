using System;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.SmartContract.Enumerators
{
    internal interface IEnumerator : IDisposable, IInteropContract
    {
        bool Next();
        StackItem Value();
    }
}
