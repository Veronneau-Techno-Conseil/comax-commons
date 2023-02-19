using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s;
using k8s.Models;
using System.Collections.ObjectModel;
using System.Resources;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder
{
   public partial class DeploymentBuilder
   {
       public static IEnumerable<IKubernetesObject<V1ObjectMeta>> Build(ComaxAgent agent)
       {
           var labels = new Dictionary<string, string>()
           {
               { Constants.ControlledBy, Constants.OperatorName },
               { Constants.App, "comaxagent" },
               { Constants.Kind, "service" },
               { Constants.Name, agent.Name() }
           };

           agent.Status.AgentRefereeName = $"{agent.Name()}-ref";

           yield return new AgentReferee
           {
               Metadata = new V1ObjectMeta
               {
                   Name = agent.Status.AgentRefereeName,
                   NamespaceProperty = agent.Namespace(),
                   Labels = labels
               },
               Spec = CreateAgentRefereeSpec(agent, labels)
           };
       }

       private static AgentRefereeSpec CreateAgentRefereeSpec(ComaxAgent agent, IDictionary<string, string> labels)
       {
           foreach (var kvp in agent.Spec.Labels)
               labels.TryAdd(kvp.Key, kvp.Value);

           var spec = new AgentRefereeSpec
           {
               ListenPort = 5004,
               OidcAuthority = agent.Spec.OidcAuthority,
               OidcClientId= agent.Spec.OidcClientId,
               OidcSecretKey=agent.Spec.OidcSecretKey,
               UseHttps = false
           };

           return spec;
       }       
   }
}
