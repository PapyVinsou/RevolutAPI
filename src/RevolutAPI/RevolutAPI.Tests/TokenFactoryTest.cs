using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RevolutAPI.Api;
using RevolutAPI.Factories;
using RevolutAPI.Helpers;
using RevolutAPI.Models.Authorization;
using RevolutAPI.Models.JWT;
using RevolutAPI.Tests.misc;
using Xunit;

namespace RevolutAPI.Tests
{
    public class TokenFactoryTest : IDisposable
    {
        public TokenFactoryTest()
        {
            tokenManager = new TokenManager($"{Environment.CurrentDirectory}\\Certificats\\token.json");
            token = tokenManager.LoadToken();
            memoryCache = new MemoryCacheFactory().CreateInstance(token);
            var config = new ConfigTest();
            var httpClient = new HttpClient();
            var api = new RevolutApiClient(config, httpClient);

            tokenFactory = new TokenFactory(new AuthorizationApiClient(api, config), memoryCache);
        }

        public void Dispose()
        {
            tokenManager.SaveToken(token);
        }

        private readonly TokenManager tokenManager;
        private readonly AuthorizationCodeResp token;
        private readonly TokenFactory tokenFactory;
        private readonly IMemoryCache memoryCache;


        [Fact]
        public async Task TestGetToken_Success()
        {
            var newToken = await tokenFactory.GetTokenAsync(token.AccessToken);
            Assert.NotNull(newToken);

            var result = memoryCache.TryGetValue(newToken, out AuthorizationCodeResp myToken);
            Assert.True(result);
            Assert.NotNull(myToken.AccessToken);
            Assert.NotEmpty(myToken.AccessToken);
            Assert.NotEmpty(myToken.RefreshToken);
        }

        [Fact]
        public void TestJwtSignData_Success()
        {
            var config = new ConfigTest();
            var data = config.Certificate.Certificate;
            var result = JwtSignerHelper.SignData(new JwtPayload
            {
                iss = config.Issuer,
                sub = config.AccountId
            }, data);

            Assert.NotNull(result);
        }


        [Fact]
        public async void TestRefreshToken_Success()
        {
            var oldToken = token.AccessToken;
            token.ExpirationDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            var newToken = await tokenFactory.GetTokenAsync(token.AccessToken);
            Assert.NotNull(newToken);

            var result = memoryCache.TryGetValue(newToken, out AuthorizationCodeResp myToken);
            Assert.True(result);
            Assert.NotEqual(oldToken, newToken);
            Assert.True(myToken.ExpirationDate > DateTime.Now);
        }
    }
}