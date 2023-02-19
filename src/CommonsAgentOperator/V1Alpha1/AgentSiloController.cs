
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;


using IdentityModel;
using k8s.Models;
using KubeOps.KubernetesClient;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Rbac;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1
{
   [EntityRbac(typeof(AgentSilo), Verbs = RbacVerb.All)]
   [EntityRbac(typeof(V1PersistentVolumeClaim), Verbs = RbacVerb.All)]
   [EntityRbac(typeof(V1Deployment), Verbs = RbacVerb.All)]
   [EntityRbac(typeof(V1Service), Verbs = RbacVerb.All)]
   [EntityRbac(typeof(V1Ingress), Verbs = RbacVerb.All)]
   public class AgentSiloController : IResourceController<AgentSilo>
   {
       private readonly IKubernetesClient _client;
       private readonly ILogger<AgentSiloController> _logger;
       private readonly IFinalizerManager<AgentSilo> _finalizeManager;

       public AgentSiloController(ILogger<AgentSiloController> logger, IFinalizerManager<AgentSilo> finalizeManager, IKubernetesClient client)
       {
           _client = client;
           _finalizeManager = finalizeManager;
           _logger = logger;
       }

       public async Task<ResourceControllerResult?> ReconcileAsync(AgentSilo entity)
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
                       await UpdateStatus(entity);
                       break;
                   case Status.Unknown:
                       entity.Status.CurrentState = Status.Creating.ToString();
                       entity = await UpdateStatus(entity);
                       entity = await Create(entity);
                       entity.Status.CurrentState = Status.Stable.ToString();
                       entity = await UpdateStatus(entity);
                       await _finalizeManager.RegisterFinalizerAsync<AgentSiloFinalizer>(entity);
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
                           await UpdateStatus(entity);
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
           catch (Exception ex)
           {
               throw;
           }
           return await Task.FromResult<ResourceControllerResult?>(null);

       }

       private async Task<AgentSilo> Create(AgentSilo entity)
       {
           var deployment = DeploymentBuilder.Build(entity);

           foreach (var item in deployment)
           {
               try
               {
                   await _client.CreateObject(item);
               }
               catch (Exception ex)
               {
                   throw;
               }
           }

           return entity;
       }

       private async Task<AgentSilo> Update(AgentSilo entity)
       {
           var deployments = DeploymentBuilder.Build(entity);
           foreach (var item in deployments)
               await _client.UpdateObject(item);
           return entity;
       }

       private async Task<AgentSilo> UpdateStatus(AgentSilo entity)
       {
           entity.Status.StateTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
           entity.Status.StateTsMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
           await _client.UpdateStatus(entity);
           return entity;
       }


       // Summary:
       //     Called for KubeOps.Operator.Kubernetes.ResourceEventType.StatusUpdated events
       //     for a given entity.
       //
       // Parameters:
       //   entity:
       //     The entity that fired the status-modified event.
       //
       // Returns:
       //     A task that completes, when the reconciliation is done.
       public Task StatusModifiedAsync(AgentSilo entity)
       {
           return Task.CompletedTask;
       }

       private async Task DeleteBrokenAsync(AgentSilo entity)
       {
           _logger.LogInformation(
               "Removing deployment for {Name} in namespace {Namespace}",
               entity.Name(),
               entity.Namespace()
           );

           await DeletedAsync(entity);

           _logger.LogInformation(
               "Removed deployment for {Name} in namespace {Namespace}",
               entity.Name(),
               entity.Namespace()
           );
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
       public async Task DeletedAsync(AgentSilo entity)
       {

           await _client.DeleteObject<V1Service>(_logger, entity.Namespace(), $"{entity.GetDeploymentName()}-ep");

           await _client.DeleteObject<V1Deployment>(_logger, entity.Namespace(), entity.GetDeploymentName());

           _logger.LogInformation(
               "{Name} in namespace {Namespace} deleted",
               entity.Name(),
               entity.Namespace()
           );
       }
   }
}
