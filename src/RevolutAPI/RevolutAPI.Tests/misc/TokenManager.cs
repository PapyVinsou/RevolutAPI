using System.IO;
using Newtonsoft.Json;
using RevolutAPI.Models.Authorization;

namespace RevolutAPI.Tests.misc
{
    internal class TokenManager
    {
        private readonly AuthorizationCodeResp tokenbyDefault = new AuthorizationCodeResp
        {
            AccessToken = "oa_sand_jw_rX6pHawRxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            ExpiresIn = 2399,
            TokenType = "bearer",
            RefreshToken = "oa_sand_ksH-jxFAYQxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
        };

        private readonly string tokenFilePath;

        public TokenManager(string tokenFilePath)
        {
            this.tokenFilePath = tokenFilePath;
        }

        public AuthorizationCodeResp LoadToken()
        {
            try
            {
                using (var reader = new StreamReader(tokenFilePath))
                {
                    return JsonConvert.DeserializeObject<AuthorizationCodeResp>(reader.ReadToEnd());
                }
            }
            catch
            {
                // nothing
            }

            return tokenbyDefault;
        }

        public void SaveToken(AuthorizationCodeResp token)
        {
            using (var writer = new StreamWriter(tokenFilePath, false))
            {
                var json = JsonConvert.SerializeObject(token);
                writer.Write(json);
            }
        }
    }
}