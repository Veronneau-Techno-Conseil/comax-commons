using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{
    public class ApiMembershipProvider : IMembershipTable
    {
        private readonly ISvcClientFactory _svcClientFactory;
        private readonly ILogger<ApiMembershipProvider> _logger;

        private async Task<ApiRef.RefereeSvc> GetClient()
        {
            var client = await _svcClientFactory.GetRefereeSvc();
            return client;
        }

        public ApiMembershipProvider(ISvcClientFactory svcClientFactory, ILogger<ApiMembershipProvider> logger)
        {
            _svcClientFactory = svcClientFactory;
            _logger = logger;
        }
        public async Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
        {
            var cl = await this.GetClient();

            await WithLog(async () =>
            {
                await cl.CleanupDefunctSiloEntriesAsync(new ApiRef.CleanupRequest { BeforeDate = beforeDate });
            });
        }

        public async Task DeleteMembershipTableEntries(string clusterId)
        {
            var cl = await this.GetClient();
            await WithLog(async () =>
            {
                await cl.DeleteMembershipTableEntriesAsync(new ApiRef.DeleteMembershipEntriesRequest { ClusterId = clusterId });
            });
        }

        public async Task InitializeMembershipTable(bool tryInitTableVersion)
        {
            var cl = await this.GetClient();
            await WithLog(async () =>
            {
                await cl.InitializeMembershipTableAsync(new ApiRef.InitializeMembershipTableRequest { TryInitTableVersion = tryInitTableVersion });
            });
        }

        public async Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        {
            var cl = await this.GetClient();
            return await WithLog(async () =>
            {
                var e = ApiRef.MembershipEntry.Parse(entry);
                if (e == null)
                    return false;
                var parameter = new ApiRef.InsertRowRequest { Entry = e, TableVersion = ApiRef.TableVersion.Parse(tableVersion) };
                var res = await cl.InsertRowAsync(parameter);
                return res != null && res.Success;
            });
        }

        public async Task<MembershipTableData> ReadAll()
        {
            var cl = await this.GetClient();
            return await WithLog(async () =>
            {
                var res = await cl.ReadAllAsync();
                return res.ToOrleans();
            });
        }

        public async Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            var cl = await this.GetClient();
            return await WithLog(async () =>
            {
                var res = await cl.ReadRowAsync(ApiRef.SiloAddress.Parse(key));
                return res.ToOrleans();
            });
        }

        public async Task UpdateIAmAlive(MembershipEntry entry)
        {
            var cl = await this.GetClient();
            await WithLog(async () =>
            {
                await cl.UpdateIAmAliveAsync(ApiRef.MembershipEntry.Parse(entry));
            });
        }

        public async Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            var cl = await this.GetClient();
            return await WithLog(async () =>
            {
                var res = await cl.UpdateRowAsync(new ApiRef.UpdateRowRequest
                {
                    Entry = ApiRef.MembershipEntry.Parse(entry),
                    Etag = etag,
                    TableVersion = ApiRef.TableVersion.Parse(tableVersion)
                });

                return res != null && res.Success;
            });
        }

        public Task<TRes> WithLog<TRes>(Func<Task<TRes>> func)
        {
            try
            {
                return func();
            }
            catch(Exception e)
            {
                _logger.LogError(e, null);
                throw;
            }
        }

        public Task WithLog(Func<Task> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                throw;
            }
        }
    }
}