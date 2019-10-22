using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevolutAPI.Helpers;

namespace RevolutAPI.Interfaces
{
    public interface IRevolutApiClient
    {
        Task<bool> DeleteAsync(string url);
        Task<bool> DeleteAsync(Uri url);
        Task<T> DeleteAsync<T>(string url);
        Task<T> DeleteAsync<T>(Uri url);
        void Dispose();
        Task<T> GetAsync<T>(string url);
        Task<T> GetAsync<T>(Uri url);
        Task<Result<T>> PostAsync<T>(string url, object obj);
        Task<Result<T>> PostAsync<T>(Uri url, object obj);
        Task<Result<T>> PostFormDataAsync<T>(Uri url, List<KeyValuePair<string, string>> data);
        Task<Result<T>> PostFormDataAsync<T>(string url, List<KeyValuePair<string, string>> data);
        Task<T> PutAsync<T>(string url, object obj);
        Task<T> PutAsync<T>(Uri url, object obj);
    }
}