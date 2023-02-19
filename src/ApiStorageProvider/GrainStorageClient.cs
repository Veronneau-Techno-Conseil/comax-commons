using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.StorageProvider
{
    public class GrainStorageClient
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ApiStorageConfiguration _apiStorageConfiguration;
        private HttpClient _httpClient;
        private TokenData _tokenData;
        public GrainStorageClient(ISettingsProvider settingsProvider, IOptionsMonitor<ApiStorageConfiguration> configs)
        {
            _settingsProvider = settingsProvider;
            _apiStorageConfiguration = configs.CurrentValue;
        }

        private async Task EnsureHttpClient()
        {
            _httpClient = new HttpClient();
            var settings = await _settingsProvider.GetOIDCSettings();
            TokenClient tokenClient = new TokenClient(settings);
            var (res, data) = await tokenClient.AuthenticateClient(settings.ClientId, settings.Secret, settings.Scopes);
            if (!res)
                throw new UnauthorizedAccessException("Authentication failed");

            _tokenData = data;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenData.access_token);
            _httpClient.BaseAddress = new Uri(_apiStorageConfiguration.ApiStorageUri);
        }

        private async Task RefreshToken()
        {
            var settings = await _settingsProvider.GetOIDCSettings();
            TokenClient tokenClient = new TokenClient(settings);
            var (res, data) = await tokenClient.RefreshToken(_tokenData.refresh_token);
            if (!res)
                throw new UnauthorizedAccessException("Authentication failed");

            _tokenData = data;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenData.access_token);
        }

        public async Task UpsetValue(string dataType, string key, JObject value)
        {
            await EnsureHttpClient();

            using (var stringContent = new StringContent(value.ToString(), Encoding.UTF8, "application/json"))
            {
                bool success = false;
                int trial = 0;
                while (!success && trial < 2)
                {
                    trial++;
                    var res = await _httpClient.PostAsync($"State/{dataType}/{key}", stringContent);
                    if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await RefreshToken();
                    }
                    else if (res.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception(res.ReasonPhrase);
                    else
                        success = true;
                }

                if (!success)
                {
                    throw new Exception("Could not complete saving");
                }
            }
        }

        public async Task<JObject> GetValue(string dataType, string key)
        {
            await EnsureHttpClient();

            bool success = false;
            int trial = 0;
            HttpResponseMessage msg = null;
            while (!success && trial < 2)
            {
                trial++;
                msg = await _httpClient.GetAsync($"State/{dataType}/{key}");
                if (msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await RefreshToken();
                }
                else if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception(msg.ReasonPhrase);
                else
                    success = true;
            }

            if (!success)
            {
                throw new Exception("Could not complete saving");
            }

            var str = await msg.Content.ReadAsStringAsync();
            return JObject.Parse(str);
        }

        public async Task<bool> Any(string dataType, string key)
        {
            await EnsureHttpClient();

            int trial = 0;
            
            var msg = await _httpClient.GetAsync($"State/Any/{dataType}/{key}");
            if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                return false;
            else
                return true;
        }

        public async Task<bool> Delete(string dataType, string key)
        {
            await EnsureHttpClient();

            int trial = 0;

            var msg = await _httpClient.DeleteAsync($"State/{dataType}/{key}");
            if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                return false;
            else
                return true;
        }

    }
}
