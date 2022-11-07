namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IFieldValidatorLookup
    {
        IFieldValidator? Get(string tag);

        bool TryGet(string tag, out IFieldValidator validator);
    }

}

