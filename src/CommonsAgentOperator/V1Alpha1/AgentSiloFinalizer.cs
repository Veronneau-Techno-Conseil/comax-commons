using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using KubeOps.KubernetesClient;

using k8s.Models;
using KubeOps.Operator.Finalizer;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1
{
   public class AgentSiloFinalizer : IResourceFinalizer<AgentSilo>
   {
       private readonly ILogger<AgentSiloFinalizer> _logger;
       private readonly IKubernetesClient _client;

       public AgentSiloFinalizer(ILogger<AgentSiloFinalizer> logger, IKubernetesClient client)
       {
           _logger = logger;
           _client = client;
       }

       public async Task FinalizeAsync(AgentSilo entity)
       {
           _logger.LogInformation(
           "Starting finalization of resource {Name} in namespace {Namespace}",
           entity.Name(),
           entity.Namespace()
           );

           await _client.DeleteObject<V1Service>(_logger, entity.Namespace(), $"{entity.GetDeploymentName()}-ep");

           await _client.DeleteObject<V1Deployment>(_logger, entity.Namespace(), entity.GetDeploymentName());

       }
   }
}
