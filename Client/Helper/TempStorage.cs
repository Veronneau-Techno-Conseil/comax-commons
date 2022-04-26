using CommunAxiom.Commons.ClientUI.ApiContracts;
using Microsoft.Extensions.Caching.Memory;

namespace CommunAxiom.Commons.ClientUI.Helper
{
    public interface ITempData
    {
        void SetOperationResult(string opId, string value);
        string GetOperationResult(string opId);
        bool IsOperationResultSet(string opId);

        void SetTokenData(TokenData tokenData);
        TokenData GetTokenData();
        void ClearTokenData();
        bool IsTokenSet();
    }
    public class TempStorage : ITempData
    {
        public IMemoryCache MemoryCache { get; }

        public TempStorage(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
        }

        public string GetOperationResult(string appId)
        {
            var res = MemoryCache.Get(appId) as string;
            MemoryCache.Remove(appId);
            return res;
        }

        public void SetOperationResult(string appId, string appSecret)
        {
            MemoryCache.Set(appId, appSecret, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromSeconds(30) });
        }

        public bool IsOperationResultSet(string opId)
        {
            return MemoryCache.TryGetValue(opId, out var outVal);
        }

        public void SetTokenData(TokenData tokenData)
        {
            MemoryCache.Set<TokenData>("TOKEN", tokenData);
        }

        public TokenData GetTokenData()
        {
            return MemoryCache.Get<TokenData>("TOKEN");
        }

        public void ClearTokenData()
        {
            MemoryCache.Remove("TOKEN");
        }

        public bool IsTokenSet()
        {
            return MemoryCache.TryGetValue("TOKEN", out _);
        }
    }
}
