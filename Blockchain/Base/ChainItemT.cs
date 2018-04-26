namespace Blockchain
{
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
}
