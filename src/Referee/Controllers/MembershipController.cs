using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Runtime;
using Referee.Contracts;

namespace Referee.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipTable _membershipTable;
        public MembershipController(IMembershipTable membershipTable)
        {
            _membershipTable = membershipTable;
        }
        
        [Authorize("Actor")]
        [HttpPost("CleanupDefunctSiloEntries")]
        public async Task CleanupDefunctSiloEntries(CleanupRequest request)
        {
            await _membershipTable.CleanupDefunctSiloEntries(request.BeforeDate);
        }

        [Authorize("Actor")]
        [HttpPost("DeleteMembershipTableEntries")]
        public async Task DeleteMembershipTableEntries(DeleteMembershipEntriesRequest request)
        {
            await _membershipTable.DeleteMembershipTableEntries(request.ClusterId);
        }

        [Authorize("Actor")]
        [HttpPost("InitializeMembershipTable")]
        public async Task InitializeMembershipTable(InitializeMembershipTableRequest request)
        {
            await _membershipTable.InitializeMembershipTable(request.TryInitTableVersion);
        }

        [Authorize("Actor")]
        [HttpPost("InsertRow")]
        public async Task<InsertRowResponse> InsertRow(InsertRowRequest request)
        {
            var res = new InsertRowResponse
            {
                Success = await _membershipTable.InsertRow(request.Entry, request.TableVersion)
            };
            return res;
        }

        [Authorize("Reader")]
        [HttpGet("ReadAll")]
        public async Task<MembershipTableData> ReadAll()
        {
            return await _membershipTable.ReadAll();
        }

        [Authorize("Reader")]
        [HttpPost("ReadRow")]
        public async Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            return await (_membershipTable.ReadRow(key));
        }

        [Authorize("Actor")]
        [HttpPost("UpdateIAmAlive")]
        public Task UpdateIAmAlive(MembershipEntry entry)
        {
            return _membershipTable.UpdateIAmAlive(entry);
        }

        [Authorize("Actor")]
        [HttpPost("UpdateRow")]
        public async Task<UpdateRowResponse> UpdateRow(UpdateRowRequest request)
        {
            var res = new UpdateRowResponse { Success = await _membershipTable.UpdateRow(request.Entry, request.Etag, request.TableVersion) };
            return res;
        }
    }
}
