using System.Security.Cryptography.X509Certificates;

namespace RevolutAPI.Models.Authorization
{
    public class PFXCertificate
    {
        /// <summary>
        ///     PFXCertificate
        /// </summary>
        /// <param name="certificate">base64 encoded pfx</param>
        /// <param name="secretKey">password for pfx</param>
        public PFXCertificate(X509Certificate2 certificate, string secretKey)
        {
            Certificate = certificate;
            SecretKey = secretKey;
        }

        /// <summary>
        ///     Certificate in base64 (.pfx)
        /// </summary>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        ///     Password for pfx
        /// </summary>
        public string SecretKey { get; }
    }
}