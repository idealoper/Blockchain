namespace Blockchain
{
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
}
