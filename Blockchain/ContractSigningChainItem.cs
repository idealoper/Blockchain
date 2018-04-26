namespace Blockchain
{
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
}
