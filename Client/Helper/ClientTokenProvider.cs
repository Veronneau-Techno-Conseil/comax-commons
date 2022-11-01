using Comax.Commons.Shared.OIDC;

namespace CommunAxiom.Commons.ClientUI.Server.Helper
{
    public class ClientTokenProvider : ITokenProvider
    {
        ITempData _tempData;
        IServiceProvider _serviceProvider;
        public ClientTokenProvider(ITempData tempData, IServiceProvider serviceProvider)
        {
            _tempData = tempData;
            _serviceProvider = serviceProvider;
        }
        public Task<string> FetchToken()
        {
            try
            {
                IHttpContextAccessor _httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
                var token = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "comax:token");

                if (token != null)
                {
                    _tempData.SetTokenData(new Shared.Models.TokenData { AccessToken = token.Value });
                    return Task.FromResult(token.Value);
                }
                else
                {
                    var data = _tempData.GetTokenData();
                    if (data == null)
                        return Task.FromResult<string>(null);
                    else
                        return Task.FromResult(_tempData.GetTokenData().AccessToken);
                }

            }
            catch (Exception ex)
            {
                var data = _tempData.GetTokenData();
                if (data == null)
                    return Task.FromResult<string>(null);
                else
                    return Task.FromResult(_tempData.GetTokenData().AccessToken);
            }
        }
    }
}
