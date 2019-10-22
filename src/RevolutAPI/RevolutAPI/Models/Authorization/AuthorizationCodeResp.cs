using System;
using Newtonsoft.Json;

namespace RevolutAPI.Models.Authorization
{
    /// <summary>
    ///     https://revolutdev.github.io/business-api/#exchange-authorisation-code-to-access-your-own-account
    /// </summary>
    public class AuthorizationCodeResp
    {
        private readonly DateTime creationDate;

        public AuthorizationCodeResp()
            : this(DateTime.Now)
        {
        }

        public AuthorizationCodeResp(DateTime creationDate)
        {
            this.creationDate = creationDate;
        }

        /// <summary>
        ///     the access token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        ///     "bearer" means that this token is valid to access the API
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        ///     token expiration time in seconds
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        ///     A token to be used to request a new access token
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }


        public DateTime ExpirationDate { get; set; }
    }
}