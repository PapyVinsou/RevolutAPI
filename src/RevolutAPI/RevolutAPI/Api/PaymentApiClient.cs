using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevolutAPI.Helpers;
using RevolutAPI.Interfaces;
using RevolutAPI.Models.Payment;

namespace RevolutAPI.Api
{
    public class PaymentApiClient
    {
        private readonly IRevolutApiClient _apiClient;

        public PaymentApiClient(IRevolutApiClient client)
        {
            _apiClient = client;
        }

        public async Task<Result<TransferResp>> CreateTransfer(TransferReq req)
        {
            var endpoint = "/transfer";
            return await _apiClient.PostAsync<TransferResp>(endpoint, req);
        }

        public async Task<Result<CreatePaymentResp>> CreatePayment(CreatePaymentReq req)
        {
            var endpoint = "/pay";
            return await _apiClient.PostAsync<CreatePaymentResp>(endpoint, req);
        }

        /// <summary>
        ///     SchedulePayment can only accept dates whitout times
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Result<CreatePaymentResp>> SchedulePayment(SchedulePaymentReq req)
        {
            var endpoint = "/pay";
            return await _apiClient.PostAsync<CreatePaymentResp>(endpoint, req);
        }

        public async Task<CheckPaymentStatusResp> CheckPaymentStatusByTransactionId(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException();

            var endpoint = "/transaction/" + transactionId;
            return await _apiClient.GetAsync<CheckPaymentStatusResp>(endpoint);
        }

        public async Task<CheckPaymentStatusResp> CheckPaymentStatusByRequestId(string requestId)
        {
            if (string.IsNullOrEmpty(requestId)) throw new ArgumentException();

            var endpoint = $"/transaction/{requestId}?id_type=request_id";
            return await _apiClient.GetAsync<CheckPaymentStatusResp>(endpoint);
        }

        public async Task<IReadOnlyCollection<TransactionResp>> GetTransactions(DateTime from, DateTime to, string type,
            string counterparty = null)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));

            if (string.IsNullOrEmpty(type)) throw new ArgumentException("type parameter cannot be null or empty");
            var fromParam = from.ToString("yyyy-MM-dd");
            var endpoint = string.Format("/transactions?from={0}&type={1}", fromParam, type);

            if (to != null || to != DateTime.MinValue)
            {
                var toParam = to.ToString("yyyy-MM-dd");
                endpoint += "&to=" + toParam;
            }

            if (!string.IsNullOrEmpty(counterparty)) endpoint += "&counterparty=" + counterparty;

            return await _apiClient.GetAsync<List<TransactionResp>>(endpoint);
        }

        public async Task<List<TransactionResp>> GetTransactions(string from = null, string to = null,
            string counterparty = null, int count = 0, string type = null)
        {
            var endpoint = $"/transactions?";

            if (!string.IsNullOrEmpty(from)) endpoint += $"from={from}&";

            if (!string.IsNullOrEmpty(to)) endpoint += $"to={to}&";

            if (!string.IsNullOrEmpty(counterparty)) endpoint += $"counterparty={counterparty}&";

            if (count > 0) endpoint += $"count={count}";

            if (!string.IsNullOrEmpty(type)) endpoint += $"type={type}&";

            if (endpoint[endpoint.Length - 1] == '?' || endpoint[endpoint.Length - 1] == '&')
                endpoint = endpoint.Remove(endpoint.Length - 1);


            return await _apiClient.GetAsync<List<TransactionResp>>(endpoint);
        }

        public async Task<bool> CancelPayment(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException();

            var endpoint = $"/transaction/{transactionId}";
            return await _apiClient.DeleteAsync(endpoint);
        }
    }
}