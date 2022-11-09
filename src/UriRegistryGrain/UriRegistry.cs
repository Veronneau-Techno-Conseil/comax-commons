using Orleans;
using System.Threading.Tasks;
using Orleans.Runtime;
using System;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;

namespace CommunAxiom.Commons.CommonsShared.UriRegistryGrain
{
    [AuthorizeClaim]
    public class UriRegistry: Grain, IUriRegistry
    {
        private IPersistentState<IdContainer> _storageState;
        public UriRegistry([PersistentState("uriregistry")] IPersistentState<IdContainer> storageState)
        {
            _storageState = storageState;
        }

        public async Task<UserTuple> GetCurrentUser()
        {
            var user = this.GetUser();
            var id = user.FindFirst(x => x.Type == "sub")?.Value;
            var name = user.FindFirst("name").Value;
            Guid? internalId = null;
            if (!string.IsNullOrWhiteSpace(id))
            {
                var gr = this.GrainFactory.GetGrain<IUriRegistry>(id);
                internalId = await gr.GetOrCreate();
            }

            return new UserTuple { Id = id, InternalId = internalId.GetValueOrDefault(), UserName = name };
        }

        public async Task<Guid> GetOrCreate()
        {
            if (this.GetPrimaryKeyString()==Constants.BLANK_ID)
            {
                var user = this.GetUser();
                var id = user.FindFirst(x => x.Type == "sub")?.Value;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var gr = this.GrainFactory.GetGrain<IUriRegistry>(id);
                    return await gr.GetOrCreate();
                }
            }

            await _storageState.ReadStateAsync();

            if(_storageState.State?.Guid == null)
            {
                _storageState.State = new IdContainer
                {
                    Guid = Guid.NewGuid()
                };
                await _storageState.WriteStateAsync();
            }
            return _storageState.State.Guid.Value;
        }

    }
}
