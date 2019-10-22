using RevolutAPI.Models.Authorization;

namespace RevolutAPI.Interfaces
{
    public interface IRevolutApiSettings
    {
        string Endpoint { get; }

        string AuthCode { get; }

        string ClientId { get; }

        string AccountId { get; }

        string Issuer { get; }

        PFXCertificate Certificate { get; }
    }
}