using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Referee.Contracts;

namespace Referee.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly Orleans.IMembershipTable _membershipTable;
        public MembershipController(Orleans.IMembershipTable membershipTable)
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
            var retVal = await _membershipTable.InsertRow(request.Entry.ToOrleans(), request.TableVersion.ToOrleans());

            var res = new InsertRowResponse
            {
                Success = retVal
            };
            return res;
        }

        [Authorize("Reader")]
        [HttpGet("ReadAll")]
        public async Task<MembershipTableData> ReadAll()
        {
            var res = await _membershipTable.ReadAll();
            return Contracts.MembershipTableData.Parse(res);
        }

        [Authorize("Reader")]
        [HttpPost("ReadRow")]
        public async Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            var res = await (_membershipTable.ReadRow(key.ToOrleans()));
            return MembershipTableData.Parse(res);
        }

        [Authorize("Actor")]
        [HttpPost("UpdateIAmAlive")]
        public async Task UpdateIAmAlive(MembershipEntry entry)
        {
            await _membershipTable.UpdateIAmAlive(entry.ToOrleans());
        }

        [Authorize("Actor")]
        [HttpPost("UpdateRow")]
        public async Task<UpdateRowResponse> UpdateRow(UpdateRowRequest request)
        {
            var res = new UpdateRowResponse { Success = await _membershipTable.UpdateRow(request.Entry.ToOrleans(), request.Etag, request.TableVersion.ToOrleans()) };
            return res;
        }
    }
}
