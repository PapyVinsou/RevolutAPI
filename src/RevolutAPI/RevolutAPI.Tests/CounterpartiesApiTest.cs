using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RevolutAPI.Api;
using RevolutAPI.Models.Authorization;
using RevolutAPI.Models.Counterparties;
using RevolutAPI.Tests.misc;
using Xunit;

namespace RevolutAPI.Tests
{
    public class CounterpartiesApiTest : IDisposable
    {
        //TODO: rajouter des tests d'unicité sur les numeros de téléphones
        public CounterpartiesApiTest()
        {
            tokenManager = new TokenManager($"{Environment.CurrentDirectory}\\Certificats\\token.json");
            memoryCache = new MemoryCacheFactory().CreateInstance(token = tokenManager.LoadToken());
            var config = new ConfigTest();
            var httpClient = new HttpClient();
            var api = new RevolutApiClient(config, token.AccessToken, httpClient, memoryCache);
            _counterpartiesApiClient = new CounterPartiesApiClient(api);
        }

        public void Dispose()
        {
            tokenManager.SaveToken(token);
        }

        private readonly MemoryCache memoryCache;
        private readonly TokenManager tokenManager;
        private readonly AuthorizationCodeResp token;

        private readonly CounterPartiesApiClient _counterpartiesApiClient;

        [Fact]
        public async void TestAddNonRevolutCounterparty_Success()
        {
            var req = new AddNonRevolutCounterpartyReq
            {
                CompanyName = "John Smith Co.",
                BankCountry = "GB",
                Currency = "GBP",
                AccountNo = "12345678",
                SortCode = "223344",
                Email = "john@smith.co",
                Phone = "+447771234455",
                Address = new AddressData
                {
                    StreetLine1 = "1 Canada Square",
                    StreetLine2 = "Canary Wharf",
                    Region = "East End",
                    Postcode = "E115AB",
                    City = "London",
                    Country = "GB"
                }
            };

            var resp = await _counterpartiesApiClient.CreateNonRevolutCounterparty(req);
            Assert.NotNull(resp);
        }

        [Fact]
        public async Task TestDeleteCounterparty_Success()
        {
            var phone = "+4412345678908";
            string counterpartyId;

            var counterparties = await _counterpartiesApiClient.GetCounterparties();
            Assert.NotNull(counterparties);

            await Task.Delay(2000);

            counterpartyId = counterparties?.FirstOrDefault(x => x.Phone == phone)?.Id;
            if (counterpartyId == null)
            {
                var req = new AddCounterpartyReq
                {
                    ProfileType = ProfileType.Personal,
                    Name = "John Smith",
                    Phone = phone
                };

                var counterparty = await _counterpartiesApiClient.CreateCounterparty(req);
                Assert.NotNull(counterparty);
                counterpartyId = counterparty.Value.Id;
            }

            await Task.Delay(2000);
            var resp = await _counterpartiesApiClient.DeleteCounterparty(counterpartyId);
            Assert.True(resp);
        }

        [Fact]
        public async void TestGetCounterparties_Success()
        {
            var resp = await _counterpartiesApiClient.GetCounterparties();
            Assert.NotNull(resp);
        }

        [Fact]
        public async void TestGetCounterparty_Success()
        {
            var counterparties = await _counterpartiesApiClient.GetCounterparties();
            Assert.NotEmpty(counterparties);

            var resp = await _counterpartiesApiClient.GetCounterparty(counterparties.FirstOrDefault()?.Id);
            Assert.NotNull(resp);
        }
    }
}