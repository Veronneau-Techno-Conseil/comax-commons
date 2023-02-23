using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;

using IdentityModel;
using KubeOps.KubernetesClient;

using k8s.Models;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Rbac;
using k8s;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1
{
    [EntityRbac(typeof(V1Namespace), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(ComaxAgent), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(AgentReferee), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(V1PersistentVolumeClaim), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(V1Deployment), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(V1Service), Verbs = RbacVerb.All)]
    [EntityRbac(typeof(V1Ingress), Verbs = RbacVerb.All)]
    public class ComaxAgentController : BaseController<ComaxAgent, ComaxAgentSpec, ComaxAgentState>, IResourceController<ComaxAgent>
    {

        public ComaxAgentController(ILogger<ComaxAgentController> logger, IFinalizerManager<ComaxAgent> finalizeManager, IKubernetesClient client) :
            base(logger, finalizeManager, client)
        {
            
        }

        public async Task<ResourceControllerResult?> ReconcileAsync(ComaxAgent entity)
        {
            try
            {
                switch (Enum.Parse<Status>(entity.Status.CurrentState))
                {
                    case Status.Stable:
                        entity.Status.CurrentState = Status.Updating.ToString();
                        entity = await UpdateStatus(entity);
                        entity = await Update(entity);
                        entity.Status.CurrentState = Status.Stable.ToString();
                        entity = await UpdateStatus(entity);
                        break;
                    case Status.Unknown:
                        entity.Status.CurrentState = Status.Creating.ToString();
                        entity = await UpdateStatus(entity);
                        entity = await Create(entity);
                        entity.Status.CurrentState = Status.Stable.ToString();
                        entity = await UpdateStatus(entity);
                        await _finalizeManager.RegisterFinalizerAsync<ComaxAgentFinalizer>(entity);
                        break;
                    case Status.Creating:
                    case Status.Starting:
                    case Status.Updating:
                       _logger.LogInformation(
                            "Resource {Id} is currently in a non-editable state",
                            entity.Name()
                        );

                        var timeDiff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - entity.Status.StateTs;
                        if (timeDiff > 60 * 5)
                        {
                            _logger.LogInformation(
                                "Resource {Name} have been in a non-editable state for {Seconds} seconds, setting state to broken",
                                entity.Name(),
                                timeDiff
                            );

                            entity.Status.CurrentState = Status.Broken.ToString();
                            entity = await UpdateStatus(entity);
                            return ResourceControllerResult.RequeueEvent(TimeSpan.FromSeconds(10));
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Resource {Name} have been in a non-editable state for {Seconds} seconds, waiting for {Time} more seconds",
                                    entity.Name(),
                                timeDiff,
                                ((60 * 5) - timeDiff)
                            );

                            return ResourceControllerResult.RequeueEvent(TimeSpan.FromSeconds(Math.Max(10, timeDiff)));
                        }
                    case Status.Broken:
                        _logger.LogInformation("Broken resource {Name} encountered", entity.Name());
                        _logger.LogInformation("Will remove any active deployment, but leave PVCs");
                        await DeleteBrokenAsync(entity);
                        break;
                    case Status.Stopped:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch(k8s.Autorest.HttpOperationException exc)
            {
                _logger.LogError("Unable to perform operation", exc);

            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to perform operation", ex);
            }
            return await Task.FromResult<ResourceControllerResult?>(null);

        }

        protected override IEnumerable<IKubernetesObject<V1ObjectMeta>> GetWorkload(ComaxAgent entity)
        {
            return Builder.DeploymentBuilder.Build(entity);
        }

        //
        // Summary:
        //     Called for KubeOps.Operator.Kubernetes.ResourceEventType.Deleted events for a
        //     given entity.
        //
        // Parameters:
        //   entity:
        //     The entity that fired the deleted event.
        //
        // Returns:
        //     A task that completes, when the reconciliation is done.
        public override async Task DeletedAsync(ComaxAgent entity)
        {
            if(!string.IsNullOrWhiteSpace(entity.GetAgentRefereeName()))
                await _client.DeleteObject<AgentReferee>(_logger, entity.Namespace(), entity.GetAgentRefereeName());

            _logger.LogInformation(
                "{Name} in namespace {Namespace} deleted",
                entity.Name(),
                entity.Namespace()
            );
        }
    }
}
