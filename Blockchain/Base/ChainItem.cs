using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
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
}
