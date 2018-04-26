namespace Blockchain
{
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
}
