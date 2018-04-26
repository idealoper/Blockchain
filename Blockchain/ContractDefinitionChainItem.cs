namespace Blockchain
{
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
}
