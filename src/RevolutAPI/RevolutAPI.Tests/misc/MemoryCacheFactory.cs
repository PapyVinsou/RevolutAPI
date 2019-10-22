using Microsoft.Extensions.Caching.Memory;
using RevolutAPI.Models.Authorization;

namespace RevolutAPI.Tests.misc
{
    internal class MemoryCacheFactory
    {
        public MemoryCache CreateInstance(AuthorizationCodeResp token)
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            memoryCache.Set(token.AccessToken, token);
            return memoryCache;
        }
    }
}