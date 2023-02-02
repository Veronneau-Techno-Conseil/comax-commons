using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using DotnetKubernetesClient;
using k8s.Models;
using KubeOps.Operator.Finalizer;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1
{
    public class RefereeFinalizer : IResourceFinalizer<AgentReferee>
    {
        private readonly ILogger<RefereeFinalizer> _logger;
        private readonly IKubernetesClient _client;

        public RefereeFinalizer(ILogger<RefereeFinalizer> logger, IKubernetesClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task FinalizeAsync(AgentReferee entity)
        {
            _logger.LogInformation(
            "Starting finalization of resource {Name} in namespace {Namespace}",
            entity.Name(),
            entity.Namespace()
            );

            var dep = await _client.Get<V1Deployment>(
                entity.DeploymentName,
                entity.Namespace()
            );

            if (dep == null)
            {
                _logger.LogError(
                    "Failed to find deployment for resource {Name} in namespace {Namespace}",
                    entity.Name(),
                    entity.Namespace()
                );
                throw new Exception("Failed to find deployment.");
            }

            await _client.Delete(dep);
            _logger.LogInformation(
                "Removed deployment for {Name} in namespace {Namespace}",
                entity.Name(),
                entity.Namespace()
            );

        }
    }
}
