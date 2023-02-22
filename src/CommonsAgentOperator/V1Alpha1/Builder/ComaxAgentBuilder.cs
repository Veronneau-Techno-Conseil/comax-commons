using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s;
using k8s.Models;
using System;
using System.Collections.ObjectModel;
using System.Resources;
using System.Runtime.CompilerServices;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder
{
    public partial class DeploymentBuilder
    {
        public static IList<IKubernetesObject<V1ObjectMeta>> Build(ComaxAgent agent)
        {
            var labels = new Dictionary<string, string>()
           {
               { Constants.ControlledBy, Constants.OperatorName },
               { Constants.App, "comaxagent" },
               { Constants.Kind, "service" },
               { Constants.Name, agent.Name() }
           };

            List<IKubernetesObject<V1ObjectMeta>> objects = new List<IKubernetesObject<V1ObjectMeta>>();

            objects.Add(new AgentReferee
            {
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = agent.GetAgentRefereeName(),
                    NamespaceProperty = agent.Namespace(),
                    Labels = labels
                },
                Spec = CreateAgentRefereeSpec(agent, labels)
            });

            objects.Add(new AgentSilo
            {
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = agent.GetAgentSiloName(),
                    NamespaceProperty = agent.Namespace(),
                    Labels = labels
                },
                Spec = CreateAgentSiloSpec(agent, labels)
            });
            return objects;
        }

        private static AgentSiloSpec CreateAgentSiloSpec(ComaxAgent agent, IDictionary<string, string> labels)
        {
            var s = agent.Spec;
            var spec = new AgentSiloSpec
            {
                ClusterId = s.ClusterId,
                CommonsMembership = $"http://{agent.GetAgentRefereeName()}.{agent.Namespace()}.svc.cluster.local",
                DbCredPasswordKey= s.DbCredPasswordKey,
                DbCredRootPasswordKey= s.DbCredRootPasswordKey,
                DbCredSecretName= s.DbCredSecretName,
                DbCredUsernameKey= s.DbCredUsernameKey,
                GatewayPort=s.GatewayPort,
                MariadbImage=s.MariadbImage,
                MembershipAddress=s.MembershipAddress,
                OidcSecretName= s.OidcSecretName,
                OidcAuthority = s.OidcAuthority,
                OidcClientId = s.OidcClientId,
                OidcSecretKey = s.OidcSecretKey,
                Image = s.AgentSiloImage,
                ServiceId=s.ServiceId,
                SiloPort=s.SiloPort,
                StoreApiImage=s.StoreApiImage                
            };

            return spec;
        }

        private static AgentRefereeSpec CreateAgentRefereeSpec(ComaxAgent agent, IDictionary<string, string> labels)
        {
            foreach (var kvp in agent.Spec.Labels)
                labels.TryAdd(kvp.Key, kvp.Value);

            var spec = new AgentRefereeSpec
            {
                ListenPort = 5004,
                OidcAuthority = agent.Spec.OidcAuthority,
                OidcClientId = agent.Spec.OidcClientId,
                OidcSecretKey = agent.Spec.OidcSecretKey,
                OidcSecretName= agent.Spec.OidcSecretName,
                UseHttps = false
            };

            return spec;
        }
    }
}
