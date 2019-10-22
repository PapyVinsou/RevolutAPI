using System.Security.Cryptography.X509Certificates;
using RevolutAPI.Interfaces;
using RevolutAPI.Models.Authorization;

namespace RevolutAPI.Tests
{
    public class ConfigTest : IRevolutApiSettings
    {
        // used in payments tests
        public static readonly string ACCOUNT_ID = "";
        public static readonly string COUNTERPARTY_ID = "";
        public static readonly string COUNTERPARTY_ACCOUNT_ID = "";

        public ConfigTest()
        {
            var cert = new X509Certificate2(
                @"./Certificats/revolut_pfx.pfx",
                "01234");
            Certificate = new PFXCertificate(cert, GetKey());
        }

        public string Endpoint => "https://sandbox-b2b.revolut.com/api/1.0";
        public string AuthCode => "oa_sand_p4mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        public string ClientId => "q39mQ66DJm-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; /* OK */
        public string AccountId => "nu-bx5Win4xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        public string Issuer => "revolut-jwt-sandbox.glitch.me";

        public PFXCertificate Certificate { get; }

        private string GetKey()
        {
            return "01234";
        }
    }
}