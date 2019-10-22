using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using RevolutAPI.Api;
using RevolutAPI.Models.Authorization;
using RevolutAPI.Tests.misc;
using Xunit;

namespace RevolutAPI.Tests
{
    public class AccountApiTest : IDisposable
    {
        public AccountApiTest()
        {
            tokenManager = new TokenManager($"{Environment.CurrentDirectory}\\Certificats\\token.json");
            memoryCache = new MemoryCacheFactory().CreateInstance(token = tokenManager.LoadToken());
            var config = new ConfigTest();
            var httpClient = new HttpClient();
            var api = new RevolutApiClient(config, token.AccessToken, httpClient, memoryCache);
            _accountClient = new AccountApiClient(api);
        }

        public void Dispose()
        {
            tokenManager.SaveToken(token);
        }

        private readonly MemoryCache memoryCache;
        private readonly TokenManager tokenManager;
        private readonly AuthorizationCodeResp token;

        private readonly AccountApiClient _accountClient;

        [Fact]
        public async void TestGetAccount_EmptyArg()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _accountClient.GetAccount(string.Empty));
        }

        [Fact]
        public async void TestGetAccount_InalidId()
        {
            var resp = await _accountClient.GetAccount("000");
            Assert.Null(resp);
        }

        [Fact]
        public async void TestGetAccount_NullArg()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _accountClient.GetAccount(null));
        }

        [Fact]
        public async void TestGetAccount_ValidId_Success()
        {
            var accounts = await _accountClient.GetAccounts();
            var resp = await _accountClient.GetAccount(accounts.First().Id);
            Assert.NotNull(resp);
        }

        [Fact]
        public async void TestGetAccountDetails_EmptyArg()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _accountClient.GetAccountDetails(string.Empty));
        }

        [Fact]
        public async void TestGetAccountDetails_NullArg()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _accountClient.GetAccountDetails(null));
        }

        [Fact]
        public async void TestGetAccountDetails_Success()
        {
            var accountsresp = await _accountClient.GetAccounts();
            var resp = await _accountClient.GetAccountDetails(accountsresp.First().Id);
            Assert.NotNull(resp);
        }

        [Fact]
        public async void TestGetAccounts_Success()
        {
            var resp = await _accountClient.GetAccounts();
            Assert.NotNull(resp);
        }
    }
}