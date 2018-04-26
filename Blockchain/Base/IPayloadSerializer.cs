namespace Blockchain
{
    interface IPayloadSerializer<TPayload>
    {
        string Serialize(TPayload payload);

        TPayload Deserialize(string payload);
    }
}
