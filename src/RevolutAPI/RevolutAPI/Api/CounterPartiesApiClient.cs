using System.Collections.Generic;
using System.Threading.Tasks;
using RevolutAPI.Helpers;
using RevolutAPI.Interfaces;
using RevolutAPI.Models.Counterparties;

namespace RevolutAPI.Api
{
    public class CounterPartiesApiClient
    {
        private readonly IRevolutApiClient _apiClient;

        public CounterPartiesApiClient(IRevolutApiClient client)
        {
            _apiClient = client;
        }

        public async Task<IReadOnlyCollection<GetCounterpartyResp>> GetCounterparties()
        {
            var endpoint = "/counterparties";
            return await _apiClient.GetAsync<List<GetCounterpartyResp>>(endpoint);
        }

        public async Task<GetCounterpartyResp> GetCounterparty(string id)
        {
            var endpoint = "/counterparty/" + id;
            return await _apiClient.GetAsync<GetCounterpartyResp>(endpoint);
        }

        public async Task<Result<AddCounterpartyResp>> CreateCounterparty(AddCounterpartyReq req)
        {
            var endpoint = "/counterparty";
            var result = await _apiClient.PostAsync<AddCounterpartyResp>(endpoint, req);
            return result;
        }

        public async Task<Result<AddNonRevolutCounterpartyResp>> CreateNonRevolutCounterparty(
            AddNonRevolutCounterpartyReq req)
        {
            var endpoint = "/counterparty";
            return await _apiClient.PostAsync<AddNonRevolutCounterpartyResp>(endpoint, req);
        }

        public async Task<bool> DeleteCounterparty(string id)
        {
            var endpoit = "/counterparty/" + id;
            return await _apiClient.DeleteAsync(endpoit);
        }
    }
}