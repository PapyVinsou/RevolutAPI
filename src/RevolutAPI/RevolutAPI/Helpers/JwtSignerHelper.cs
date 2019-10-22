using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using RevolutAPI.Models.JWT;

namespace RevolutAPI.Helpers
{
    internal class JwtSignerHelper
    {
        public static string SignData(JwtPayload payload, X509Certificate2 x509Certificate2)
        {
            var header = new JwtHeader();
            var jsonHeader = JsonConvert.SerializeObject(header);
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var bytesHeader = Encoding.UTF8.GetBytes(jsonHeader);
            var bytesPayload = Encoding.UTF8.GetBytes(jsonPayload);
            var segments = new List<string>();
            segments.Add(Base64UrlEncode(bytesHeader));
            segments.Add(Base64UrlEncode(bytesPayload));
            var bytesToSign = Encoding.UTF8.GetBytes(string.Join(".", segments.ToArray()));

            using (var rsa = x509Certificate2.GetRSAPrivateKey())
            {
                var signedData = rsa.SignData(bytesToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                segments.Add(Base64UrlEncode(signedData));
                return string.Join(".", segments.ToArray());
            }
        }

        public static string SignData(JwtPayload payload, byte[] privateKey, string certificatePassword)
        {
            var x509Certificate2 = new X509Certificate2(privateKey, certificatePassword);

            var header = new JwtHeader();
            var jsonHeader = JsonConvert.SerializeObject(header);
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var bytesHeader = Encoding.UTF8.GetBytes(jsonHeader);
            var bytesPayload = Encoding.UTF8.GetBytes(jsonPayload);
            var segments = new List<string>();
            segments.Add(Base64UrlEncode(bytesHeader));
            segments.Add(Base64UrlEncode(bytesPayload));
            var bytesToSign = Encoding.UTF8.GetBytes(string.Join(".", segments.ToArray()));

            using (var rsa = x509Certificate2.GetRSAPrivateKey())
            {
                var signedData = rsa.SignData(bytesToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                segments.Add(Base64UrlEncode(signedData));
                return string.Join(".", segments.ToArray());
            }
        }

        // from JWT spec
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
    }
}