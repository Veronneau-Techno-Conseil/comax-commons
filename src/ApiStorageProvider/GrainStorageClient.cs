using Comax.Commons.ApiStorageProvider;
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
    public class GrainStorageClientFactory
    {
        private readonly TokenManager _tokenManager;
        private readonly ApiStorageConfiguration _apiStorageConfiguration;
        public GrainStorageClientFactory(TokenManager tokenManager, ApiStorageConfiguration configs)
        {
            _tokenManager = tokenManager;
            _apiStorageConfiguration = configs;
        }

        public GrainStorageClient Create()
        {
            return new GrainStorageClient(_tokenManager, _apiStorageConfiguration);
        }
    }

    public class GrainStorageClient
    {
        private readonly ApiStorageConfiguration _apiStorageConfiguration;
        private readonly TokenManager _tokenManager;
        private HttpClient _httpClient;

        private DateTime _tokenExpiration;

        public GrainStorageClient(TokenManager tokenManager, ApiStorageConfiguration configs)
        {
            _tokenManager = tokenManager;
            _apiStorageConfiguration = configs;
        }

        private async Task EnsureHttpClient()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(_apiStorageConfiguration.ApiStorageUri);
                var td = await _tokenManager.Fetch();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", td.access_token);
            }
            else if (DateTime.UtcNow > _tokenManager.TokenExpires)
            {
                var td = await _tokenManager.Fetch();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", td.access_token);
            }
        }


        public async Task UpsertValue(string dataType, string key, JObject value)
        {
            await EnsureHttpClient();

            using (var stringContent = new StringContent(value.ToString(), Encoding.UTF8, "application/json"))
            {
                bool success = false;
                int trial = 0;
                while (!success && trial < 2)
                {
                    trial++;
                    var res = await _httpClient.PostAsync($"Api/State/{dataType}/{key}", stringContent);
                    if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await EnsureHttpClient();
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
                msg = await _httpClient.GetAsync($"Api/State/{dataType}/{key}");
                if (msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await EnsureHttpClient();
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

            var msg = await _httpClient.GetAsync($"Api/State/Any/{dataType}/{key}");
            if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                return false;
            else
                return true;
        }

        public async Task<bool> Delete(string dataType, string key)
        {
            await EnsureHttpClient();

            int trial = 0;

            var msg = await _httpClient.DeleteAsync($"Api/State/{dataType}/{key}");
            if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                return false;
            else
                return true;
        }

    }
}
