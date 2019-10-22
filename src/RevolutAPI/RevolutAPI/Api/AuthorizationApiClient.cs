using System.Collections.Generic;
using System.Threading.Tasks;
using RevolutAPI.Helpers;
using RevolutAPI.Interfaces;
using RevolutAPI.Models.Authorization;
using RevolutAPI.Models.JWT;

namespace RevolutAPI.Api
{
    public class AuthorizationApiClient
    {
        private readonly IRevolutApiClient apiClient;
        private readonly IRevolutApiSettings revolutApiSettings;

        public AuthorizationApiClient(
            IRevolutApiClient client,
            IRevolutApiSettings revolutApiSettings)
        {
            apiClient = client;
            this.revolutApiSettings = revolutApiSettings;
        }

        /// <summary>
        ///     Methode use to create a new token
        /// </summary>
        public async Task<Result<AuthorizationCodeResp>> AuthorizeAsync()
        {
            var clientAssertion = GetClientAssertion();
            var auth = await apiClient.PostFormDataAsync<AuthorizationCodeResp>("/auth/token",
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", revolutApiSettings.AuthCode),
                    new KeyValuePair<string, string>("client_id", revolutApiSettings.ClientId),
                    new KeyValuePair<string, string>("client_assertion_type",
                        "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                    new KeyValuePair<string, string>("client_assertion", clientAssertion)
                });
            return auth;
        }

        /// <summary>
        ///     Methode use to refresh a token
        /// </summary>
        /// <param name="refreshToken">token which was returned as refresh token</param>
        public async Task<Result<AuthorizationCodeResp>> RefreshAccessToken(AuthorizationCodeResp refreshToken)
        {
            var clientAssertion = GetClientAssertion();
            var auth = await apiClient.PostFormDataAsync<AuthorizationCodeResp>("/auth/token",
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken.RefreshToken),
                    new KeyValuePair<string, string>("client_id", revolutApiSettings.AccountId),
                    new KeyValuePair<string, string>("client_assertion_type",
                        "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                    new KeyValuePair<string, string>("client_assertion", clientAssertion)
                });
            return auth;
        }

        private string GetClientAssertion()
        {
            var data = revolutApiSettings.Certificate.Certificate;
            return JwtSignerHelper.SignData(new JwtPayload
            {
                iss = revolutApiSettings.Issuer,
                sub = revolutApiSettings.AccountId
            }, data);
        }
    }
}