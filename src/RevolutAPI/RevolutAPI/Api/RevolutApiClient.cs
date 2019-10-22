using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using RevolutAPI.Factories;
using RevolutAPI.Helpers;
using RevolutAPI.Interfaces;
using RevolutAPI.Models;

namespace RevolutAPI.Api
{
    public class RevolutApiClient : IDisposable, IRevolutApiClient
    {
        private readonly string accessToken;
        private readonly HttpClient httpClient;
        private readonly bool isHttpClientDisposable;
        private readonly bool isMemoryCacheDisposable;
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly ILogger logger;
        private readonly IMemoryCache memoryCache;
        private readonly IRevolutApiSettings settings;
        private readonly TokenFactory tokenFactory;

        /// <summary>
        ///     Create an instance of RevolutApiClient
        /// </summary>
        /// <param name="settings">Settings of the revolut account</param>
        /// <param name="httpClient">
        ///     Provides a base class for sending HTTP requests and receiving HTTP responses from a resource
        ///     identified by a URI.
        /// </param>
        /// <param name="memoryCache">
        ///     Represents the type that implements an in-memory cache. Mainly used to access to the token
        ///     database
        /// </param>
        /// <param name="logger">logger</param>
        public RevolutApiClient(
            IRevolutApiSettings settings,
            HttpClient httpClient = null,
            IMemoryCache memoryCache = null,
            ILogger logger = null)
            : this(settings, null, httpClient, memoryCache, logger)
        {
        }

        /// <summary>
        ///     Create an instance of RevolutApiClient
        /// </summary>
        /// <param name="settings">Settings of the revolut account</param>
        /// <param name="accessToken">token of the current connection</param>
        /// <param name="httpClient">
        ///     Provides a base class for sending HTTP requests and receiving HTTP responses from a resource
        ///     identified by a URI.
        /// </param>
        /// <param name="memoryCache">
        ///     Represents the type that implements an in-memory cache. Mainly used to access to the token
        ///     database
        /// </param>
        /// <param name="logger">logger</param>
        public RevolutApiClient(IRevolutApiSettings settings,
            string accessToken,
            HttpClient httpClient = null,
            IMemoryCache memoryCache = null,
            ILogger logger = null)
        {
            this.memoryCache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());
            tokenFactory = new TokenFactory(new AuthorizationApiClient(this, settings), this.memoryCache);
            this.httpClient = httpClient ?? new HttpClient();
            this.logger = logger ?? LogManager.GetCurrentClassLogger();
            this.accessToken = accessToken;
            this.settings = settings;
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateFormatString = "yyyy-MM-dd"
            };
            isHttpClientDisposable = httpClient == null;
            isMemoryCacheDisposable = memoryCache == null;
        }

        public void Dispose()
        {
            if (isHttpClientDisposable) httpClient.Dispose();
            if (isMemoryCacheDisposable) memoryCache.Dispose();
        }

        public async Task<Result<T>> PostFormDataAsync<T>(string url, List<KeyValuePair<string, string>> data)
        {
            return await PostFormDataAsync<T>(new Uri(settings.Endpoint + url), data);
        }

        public async Task<T> GetAsync<T>(Uri url)
        {
            string responseContent = null;
            try
            {
                var token = await tokenFactory.GetTokenAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync(url);
                if (response?.Content == null)
                    return default(T);

                responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    logger.Error(response.StatusCode + "Error: " + responseContent);
                    return default(T);
                }

                logger.Info(responseContent);

                return JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                logger.Error(ex, responseContent);
            }

            return default(T);
        }

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(new Uri(settings.Endpoint + url));
        }

        public Task<Result<T>> PostAsync<T>(string url, object obj)
        {
            return PostAsync<T>(new Uri(settings.Endpoint + url), obj);
        }

        public async Task<Result<T>> PostAsync<T>(Uri url, object obj)
        {
            var responseContent = string.Empty;
            try
            {
                var token = await tokenFactory.GetTokenAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var postData = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
                var response =
                    await httpClient.PostAsync(url, new StringContent(postData, Encoding.UTF8, "application/json"));
                if (response.Content != null) responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return Result.Ok(JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings));
                if (!string.IsNullOrEmpty(responseContent))
                    return Result.Fail<T>(JsonConvert
                        .DeserializeObject<ErrorModel>(responseContent, jsonSerializerSettings).Message);
                logger.Error($"Error posting data. Status code: {response.StatusCode}, Response: null");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return Result.Fail<T>();
        }

        public async Task<Result<T>> PostFormDataAsync<T>(Uri url, List<KeyValuePair<string, string>> data)
        {
            try
            {
                var formContent = new FormUrlEncodedContent(data.ToArray());
                var response = await httpClient.PostAsync(url, formContent);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(
                        $"{nameof(PostAsync)} cannot be executed ({response.StatusCode} - {await response.Content.ReadAsStringAsync()})");

                if (response.Content == null)
                    throw new Exception($"{nameof(PostAsync)} cannot be executed ({response.StatusCode})");

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return Result.Ok(JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings));

                if (!string.IsNullOrEmpty(responseContent))
                    return Result.Fail<T>(JsonConvert
                        .DeserializeObject<ErrorModel>(responseContent, jsonSerializerSettings).Message);
                logger.Error($"Error posting data. Status code: {response.StatusCode}, Response: null");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return Result.Fail<T>();
        }

        public async Task<T> PutAsync<T>(string url, object obj)
        {
            return await PutAsync<T>(new Uri(settings.Endpoint + url), obj);
        }

        public async Task<T> PutAsync<T>(Uri url, object obj)
        {
            try
            {
                var token = await tokenFactory.GetTokenAsync(accessToken);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var postData = JsonConvert.SerializeObject(obj);
                var response =
                    await httpClient.PutAsync(url, new StringContent(postData, Encoding.UTF8, "application/json"));

                if (response.Content == null)
                    throw new Exception($"{nameof(PutAsync)} cannot be executed ({response.StatusCode})");

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings);
                logger.Error(response.StatusCode + "Error: " + responseContent);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return default(T);
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            return await DeleteAsync<T>(new Uri(settings.Endpoint + url));
        }

        public async Task<T> DeleteAsync<T>(Uri url)
        {
            try
            {
                var token = await tokenFactory.GetTokenAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync(url);
                if (response.Content == null)
                    throw new Exception($"{nameof(PutAsync)} cannot be executed ({response.StatusCode})");

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings);

                logger.Error(response.StatusCode + "Error: " + responseContent);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return default(T);
        }

        public async Task<bool> DeleteAsync(string url)
        {
            return await DeleteAsync(new Uri(settings.Endpoint + url));
        }

        public async Task<bool> DeleteAsync(Uri url)
        {
            var responseContent = string.Empty;
            try
            {
                var token = await tokenFactory.GetTokenAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                    return true;

                logger.Error(response.StatusCode + "Error: " + responseContent);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }
    }
}