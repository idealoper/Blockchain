using System.Collections.Generic;

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
}
