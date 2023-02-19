using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using KubeOps.KubernetesClient;

using k8s.Models;
using KubeOps.Operator.Finalizer;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1
{
   public class ComaxAgentFinalizer : IResourceFinalizer<ComaxAgent>
   {
       private readonly ILogger<ComaxAgent> _logger;
       private readonly IKubernetesClient _client;

       public ComaxAgentFinalizer(ILogger<ComaxAgent> logger, IKubernetesClient client)
       {
           _logger = logger;
           _client = client;
       }

       public async Task FinalizeAsync(ComaxAgent entity)
       {
           _logger.LogInformation(
           "Starting finalization of resource {Name} in namespace {Namespace}",
           entity.Name(),
           entity.Namespace()
           );
            
           await _client.DeleteObject<ComaxAgent>(_logger, entity.Namespace(), entity.Status.AgentRefereeName);

       }
   }
}
