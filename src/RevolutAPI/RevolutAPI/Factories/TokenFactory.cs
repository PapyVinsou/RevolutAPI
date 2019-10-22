using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RevolutAPI.Api;
using RevolutAPI.Models.Authorization;

namespace RevolutAPI.Factories
{
    internal class TokenFactory
    {
        private readonly AuthorizationApiClient authorizationApiClient;
        private readonly IMemoryCache memoryCache;

        public TokenFactory(AuthorizationApiClient authorizationApiClient, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.authorizationApiClient = authorizationApiClient;
        }

        /// <summary>
        ///     Create an instance of Token
        /// </summary>
        public async Task<string> GetTokenAsync(string token = null)
        {
            if (token == null)
                return await CreateTokenAsync();

            if (memoryCache.TryGetValue(token, out AuthorizationCodeResp existingToken) &&
                existingToken?.ExpirationDate > DateTime.Now)
                return existingToken.AccessToken;

            if (existingToken == null)
                return await CreateTokenAsync();
            return await RefreshTokenAsync(existingToken);
        }

        private static TimeSpan ComputeNextExpireIn(long expireIn)
        {
            return TimeSpan.FromSeconds(expireIn - 10);
        }

        private async Task<string> RefreshTokenAsync(AuthorizationCodeResp accessToken)
        {
            var response = await authorizationApiClient.RefreshAccessToken(accessToken);

            if (!response.Success)
                throw new ApplicationException("Token Cannot be refreshed");

            var newToken = response.Value;

            newToken.RefreshToken = accessToken.RefreshToken;
            newToken.ExpirationDate = DateTime.Now.AddSeconds(newToken.ExpiresIn);
            memoryCache.Set(newToken.AccessToken, newToken, ComputeNextExpireIn(newToken.ExpiresIn));
            memoryCache.Remove(accessToken.AccessToken);

            accessToken.AccessToken = newToken.AccessToken;
            accessToken.ExpirationDate = newToken.ExpirationDate;
            return newToken.AccessToken;
        }

        private async Task<string> CreateTokenAsync()
        {
            var response = await authorizationApiClient.AuthorizeAsync();
            if (!response.Success)
                throw new ApplicationException($"Token Cannot be created {response.Error}");

            var newToken = response.Value;

            newToken.ExpirationDate = DateTime.Now.AddSeconds(newToken.ExpiresIn);
            memoryCache.Set(newToken.AccessToken, newToken, ComputeNextExpireIn(newToken.ExpiresIn));

            return newToken.AccessToken;
        }
    }
}