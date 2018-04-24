using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            var chain = new List<ChainItem>();
            var root = new ChainItem("ROOT");
            chain.Add(root);

            var currentItem = root;
            for (int i = 0; i < 100; i++)
            {
                currentItem = new ChainItem(currentItem, "Item " + i.ToString());
                chain.Add(currentItem);
            }

            var branchItem = new ChainItem(chain[40], "Branch");

            var r1 = ChainItem.IsTheSameChainItems(chain[23], chain[45]);
            var r2 = ChainItem.IsTheSameChainItems(chain[23], chain[23]);
            var r3 = ChainItem.IsTheSameChainItems(chain[56], chain[13]);
            var r4 = ChainItem.IsTheSameChainItems(branchItem, chain[13]);
            var r5 = ChainItem.IsTheSameChainItems(branchItem, chain[56]);
        }
    }

    class ChainItem
    {
        public int Index { get; private set; }

        public ChainItem PreviousItem { get; private set; }

        public string Payload { get; private set; }

        public byte[] Hash { get; private set; }

        public ChainItem(string payload)
        {
            Index = 0;

            Payload = payload ?? string.Empty;

            Hash = ComputeHash();
        }

        public ChainItem(ChainItem previousItem, string payload)
        {
            if (previousItem == null)
                throw new ArgumentNullException("previousItem");
            if(!ValidateChain(previousItem))
                throw new ArgumentException("previousItem");

            Index = previousItem.Index + 1;
            PreviousItem = previousItem;
            Payload = payload ?? string.Empty;

            Hash = ComputeHash();

            if (!PreviousItem.IsApplicable(this))
                throw new InvalidOperationException("Текущий элемент не может быть добавлен к этой цепочке");
        }

        protected virtual bool IsApplicable(ChainItem nextChainItem)
        {
            return true;
        }

        private byte[] ComputeHash()
        {
            var dataToHashing = _getChainItemDataAsBytes(); 
            if (PreviousItem != null)
            {
                dataToHashing = dataToHashing.Concat(PreviousItem.Hash).ToArray();
            }

            return _getHashForData(dataToHashing);
        }

        private byte[] _getChainItemDataAsBytes()
        {
            return Encoding.UTF8.GetBytes(Payload);
        }

        private static byte[] _getHashForData(byte[] dataToHashing)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(dataToHashing);
            }
        }

        public static bool ValidateChain(ChainItem chainItem)
        {
            if (chainItem.PreviousItem != null)
            {
                if (!ValidateChain(chainItem.PreviousItem) || !chainItem.PreviousItem.IsApplicable(chainItem))
                    return false;
            }

            var hash = chainItem.ComputeHash();
            return Enumerable.SequenceEqual(chainItem.Hash, hash);
        }

        public static bool IsTheSameChainItems(ChainItem chainItem1, ChainItem chainItem2)
        {
            if (chainItem1 == null)
                throw new ArgumentNullException("chainItem1");
            if (chainItem2 == null)
                throw new ArgumentNullException("chainItem2");

            int offset = 0;
            ChainItem parentItem = null;
            ChainItem childItem = null;
            if (chainItem1.Index == chainItem2.Index)
            {
                return Enumerable.SequenceEqual(chainItem1.Hash, chainItem2.Hash);
            }
            else if (chainItem1.Index < chainItem2.Index)
            {
                offset = chainItem2.Index - chainItem1.Index;
                childItem = chainItem1;
                parentItem = chainItem2;
            }
            else if (chainItem1.Index > chainItem2.Index)
            {
                offset = chainItem1.Index - chainItem2.Index;
                parentItem = chainItem1;
                childItem = chainItem2;
            }

            while((offset > 0) && parentItem != null)
            {
                parentItem = parentItem.PreviousItem;
                offset--;
            }

            return parentItem != null && IsTheSameChainItems(parentItem, childItem);
        }

        public override string ToString()
        {
            return Payload;
        }
    }

    class ChainItem<TPayload> : ChainItem
    {
        private static readonly IPayloadSerializer<TPayload> Serializer = PayloadSerializerFactory.Create<TPayload>();

        public new TPayload Payload { get { return Serializer.Deserialize(base.Payload); } }

        public ChainItem(TPayload payload) 
            : base(Serializer.Serialize(payload))
        {
        }

        public ChainItem(ChainItem previousItem, TPayload payload) 
            : base(previousItem, Serializer.Serialize(payload))
        {
        }
    }

    class PayloadSerializerFactory
    {
        public static IPayloadSerializer<TPayload> Create<TPayload>()
        {
            throw new NotImplementedException();
        }
    }

    interface IPayloadSerializer<TPayload>
    {
        string Serialize(TPayload payload);

        TPayload Deserialize(string payload);
    }

    class ContractDefinitionChainItem : ChainItem<ContractDefinition>
    {
        public ContractDefinitionChainItem(ContractDefinition payload) : base(payload)
        {
        }

        protected override bool IsApplicable(ChainItem nextChainItem)
        {
            return nextChainItem is ContractSubjectDefinitionChainItem
                || nextChainItem is ContractObjectDefinitionChainItem;
        }
    }

    class ContractObjectDefinitionChainItem : ChainItem<ContractObjectDefinition>
    {
        public ContractObjectDefinitionChainItem(ChainItem previousItem, ContractObjectDefinition payload)
            : base(previousItem, payload)
        {
        }

        protected override bool IsApplicable(ChainItem nextChainItem)
        {
            return nextChainItem is ContractSigningChainItem
                || nextChainItem is ContractSubjectDefinitionChainItem
                || nextChainItem is ContractObjectDefinitionChainItem;
        }
    }

    class ContractSubjectDefinitionChainItem : ChainItem<ContractSubjectDefinition>
    {
        public ContractSubjectDefinitionChainItem(ChainItem previousItem, ContractSubjectDefinition payload) 
            : base(previousItem, payload)
        {
        }

        protected override bool IsApplicable(ChainItem nextChainItem)
        {
            return nextChainItem is ContractSigningChainItem
                || nextChainItem is ContractSubjectDefinitionChainItem
                || nextChainItem is ContractObjectDefinitionChainItem;
        }
    }

    class ContractSigningChainItem : ChainItem<ContractSigning>
    {
        public ContractSigningChainItem(ChainItem previousItem, ContractSigning payload)
            : base(previousItem, payload)
        {
        }

        protected override bool IsApplicable(ChainItem nextChainItem)
        {
            return false;
        }
    }

    class ContractDefinition
    {

    }

    class ContractObjectDefinition
    {

    }

    class ContractSubjectDefinition
    {

    }

    class ContractSigning
    {

    }
}
