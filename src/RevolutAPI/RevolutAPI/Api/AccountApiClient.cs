using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevolutAPI.Interfaces;
using RevolutAPI.Models.Account;

namespace RevolutAPI.Api
{
    public class AccountApiClient
    {
        private readonly IRevolutApiClient _apiClient;

        public AccountApiClient(IRevolutApiClient client)
        {
            _apiClient = client;
        }

        public async Task<IReadOnlyCollection<GetAccountResp>> GetAccounts()
        {
            var endpoint = "/accounts";
            return await _apiClient.GetAsync<List<GetAccountResp>>(endpoint);
        }

        public async Task<GetAccountResp> GetAccount(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException();

            var endpoint = "/accounts/" + id;
            return await _apiClient.GetAsync<GetAccountResp>(endpoint);
        }

        public async Task<IReadOnlyCollection<GetAccountDetailsResp>> GetAccountDetails(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException();

            var endpoint = "/accounts/" + id + "/bank-details";
            return await _apiClient.GetAsync<List<GetAccountDetailsResp>>(endpoint);
        }
    }
}