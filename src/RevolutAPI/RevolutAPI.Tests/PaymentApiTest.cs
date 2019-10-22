using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RevolutAPI.Api;
using RevolutAPI.Models.Account;
using RevolutAPI.Models.Authorization;
using RevolutAPI.Models.Payment;
using RevolutAPI.Tests.misc;
using Xunit;

namespace RevolutAPI.Tests
{
    public class PaymentApiTest
    {
        public PaymentApiTest()
        {
            tokenManager = new TokenManager($"{Environment.CurrentDirectory}\\Certificats\\token.json");
            var memoryCache = new MemoryCacheFactory().CreateInstance(token = tokenManager.LoadToken());
            var config = new ConfigTest();
            var httpClient = new HttpClient();
            var api = new RevolutApiClient(config, token.AccessToken, httpClient, memoryCache);
            _paymentClient = new PaymentApiClient(api);

            var httpClient2 = new HttpClient();
            var api2 = new RevolutApiClient(config, token.AccessToken, httpClient2, memoryCache);
            _counterpartyApiClient = new CounterPartiesApiClient(api2);

            var api3 = new RevolutApiClient(config, token.AccessToken, memoryCache: memoryCache);
            _accountApiClient = new AccountApiClient(api3);
        }

        private readonly TokenManager tokenManager;
        private readonly AuthorizationCodeResp token;
        private readonly PaymentApiClient _paymentClient;
        private readonly CounterPartiesApiClient _counterpartyApiClient;
        private readonly AccountApiClient _accountApiClient;


        [Fact(Skip = "Revolut Api throws an error message: message=Required 'profile id' is missing")]
        public async Task Test_CancelPayment()
        {
            var currency = "GBP";
            var accounts = await _accountApiClient.GetAccounts();
            var accountId = accounts.First(i => i.Currency == currency);

            await Task.Delay(200);
            var contrerparties = await _counterpartyApiClient.GetCounterparties();
            var counterParty = contrerparties.FirstOrDefault(i => i.Accounts.Any(a => a.Currency == currency));
            if (counterParty == null)
                throw new NullReferenceException($"{nameof(counterParty)} cannot be null.");

            await Task.Delay(200);
            var req = new SchedulePaymentReq
            {
                RequestId = Guid.NewGuid().ToString(),
                AccountId = accountId.Id,
                Amount = 100,
                Currency = currency,
                Reference = "Invoice payment #123",
                ScheduleFor = DateTime.Now.AddDays(2),
                Receiver = new ReceiverData
                {
                    CounterpartyId = counterParty.Id,
                    AccountId = counterParty.Accounts.First(i => i.Currency == currency).Id
                }
            };

            var transaction = await _paymentClient.SchedulePayment(req);
            Assert.NotNull(transaction);

            await Task.Delay(200);

            var resp = await _paymentClient.CancelPayment(transaction.Value.Id);
            Assert.True(resp);
        }

        [Fact]
        public async void Test_CheckPaymentStatusByRequestId()
        {
            var transactions = await _paymentClient.GetTransactions();
            Assert.NotEmpty(transactions);

            var transactionWithRequestId = transactions.FirstOrDefault(i => !string.IsNullOrEmpty(i.RequestId));
            if (transactionWithRequestId == null)
                return;
            await Task.Delay(200);
            var resp = await _paymentClient.CheckPaymentStatusByRequestId(transactionWithRequestId.RequestId);
            Assert.NotNull(resp);
        }

        [Fact]
        public async void Test_CheckPaymentStatusByTransactionId()
        {
            var transactions = await _paymentClient.GetTransactions();
            Assert.NotEmpty(transactions);

            var resp = await _paymentClient.CheckPaymentStatusByTransactionId(transactions.First().Id);
            Assert.NotNull(resp);
        }

        [Fact(Skip = "Need to work on it")]
        public async void Test_CreatePayment_Valid()
        {
            var req = new CreatePaymentReq
            {
                RequestId = Guid.NewGuid().ToString(),
                AccountId = ConfigTest.ACCOUNT_ID,
                Amount = 100,
                Currency = "EUR",
                Reference = "Invoice payment #123",
                Receiver = new ReceiverData
                {
                    CounterpartyId = ConfigTest.COUNTERPARTY_ID,
                    AccountId = ConfigTest.COUNTERPARTY_ACCOUNT_ID
                }
            };

            var resp = await _paymentClient.CreatePayment(req);
            Assert.NotNull(resp);
        }

        [Fact]
        public async void Test_GetTransactions()
        {
            var to = DateTime.Parse("04.07.2018");
            var types = new[]
            {
                TransactionType.Atm,
                TransactionType.CardPayment,
                TransactionType.CardRefund,
                TransactionType.CardChargeback,
                TransactionType.CardCredit,
                TransactionType.Exchange,
                TransactionType.Transfer,
                TransactionType.Loan,
                TransactionType.Fee,
                TransactionType.Refund,
                TransactionType.Topup,
                TransactionType.TopupReturn,
                TransactionType.Tax,
                TransactionType.TaxRefund
            };
            foreach (var type in types)
            {
                var resp = await _paymentClient.GetTransactions(DateTime.MinValue, to, type);

                if (resp.Any()) Console.WriteLine("Found tranaction for type {0}", type);
            }
        }

        [Fact]
        public async void Test_GetTransactions_Valid()
        {
            var from = DateTime.Parse("01.06.2018");
            var to = DateTime.Parse("10.06.2018");

            var resp = await _paymentClient.GetTransactions(from, to, TransactionType.CardCredit);
            Assert.NotNull(resp);
        }

        [Fact(Skip = "Need to work on it")]
        public async void Test_SchedulePayment()
        {
            var req = new SchedulePaymentReq
            {
                RequestId = Guid.NewGuid().ToString(),
                AccountId = ConfigTest.ACCOUNT_ID,
                Amount = 100,
                Currency = "EUR",
                Reference = "Invoice payment #123",
                ScheduleFor = DateTime.Now.AddDays(2),
                Receiver = new ReceiverData
                {
                    CounterpartyId = ConfigTest.COUNTERPARTY_ID,
                    AccountId = ConfigTest.COUNTERPARTY_ACCOUNT_ID
                }
            };

            var resp = await _paymentClient.SchedulePayment(req);
            Assert.NotNull(resp);
        }

        [Fact(Skip = "No account with the same currency")]
        public async void Test_Transfer_InSameCurrencys()
        {
            var currency = "EUR";
            var accounts = (await _accountApiClient.GetAccounts()).ToList();

            GetAccountResp accountInCurrencyMain = null;
            GetAccountResp accounInCurrencySecondary = null;

            try
            {
                accountInCurrencyMain = accounts.FirstOrDefault(x => x.Currency == currency);
                if (accountInCurrencyMain == null)
                    throw new Exception($"Account in currency is missing, you need to create a new one");

                accounInCurrencySecondary =
                    accounts.FirstOrDefault(x => x.Currency == currency && x.Id != accountInCurrencyMain.Id);
                if (accounInCurrencySecondary == null)
                    throw new Exception($"account Not InCurrency is missing, you need to create a new one");

                await Task.Delay(200);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"Missing account with {currency} currency - {ex.Message}");
            }

            var req = new TransferReq
            {
                RequestId = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                SourceAccountId = accountInCurrencyMain.Id,
                TargetAccountId = accounInCurrencySecondary.Id,
                Amount = 100,
                Currency = currency
            };

            var resp = await _paymentClient.CreateTransfer(req);
            Assert.NotNull(resp);
        }
    }
}