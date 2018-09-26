﻿using System.IO;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Models.Ledger
{
    public class ValidatorsCountState : StateBase, ICloneable<ValidatorsCountState>
    {
        public Fixed8[] Votes;

        public override int Size => base.Size + Votes.GetVarSize();

        public ValidatorsCountState()
        {
            this.Votes = new Fixed8[Blockchain.MaxValidators];
        }

        ValidatorsCountState ICloneable<ValidatorsCountState>.Clone()
        {
            return new ValidatorsCountState
            {
                Votes = (Fixed8[])Votes.Clone()
            };
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Votes = reader.ReadSerializableArray<Fixed8>();
        }

        void ICloneable<ValidatorsCountState>.FromReplica(ValidatorsCountState replica)
        {
            Votes = replica.Votes;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Votes);
        }
    }
}