using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s;
using k8s.Models;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder
{
    public partial class DeploymentBuilder
    {
        public static IEnumerable<IKubernetesObject<V1ObjectMeta>> Build(AgentReferee agentReferee)
        {
            var labels = new Dictionary<string, string>()
            {
                { Constants.ControlledBy, Constants.OperatorName },
                { Constants.App, "referee" },
                { Constants.Kind, "service" },
                { Constants.Name, agentReferee.Name() }
            };

            yield return new V1Deployment
            {
                Metadata = new V1ObjectMeta
                {
                    Name = agentReferee.DeploymentName,
                    NamespaceProperty = agentReferee.Namespace(),
                    Labels = labels
                },
                Spec = DeploymentBuilder.CreateAgentRefereeDeploymentSpec(agentReferee)
            };
        }

        private static V1DeploymentSpec CreateAgentRefereeDeploymentSpec(AgentReferee agentReferee)
        {
            throw new NotImplementedException();
        }
    }
}
