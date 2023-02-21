﻿using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s;
using k8s.Models;
using System.Collections.ObjectModel;
using System.Resources;
using System.Runtime.CompilerServices;

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
            agent.Status.AgentSiloName = $"{agent.Name()}-agt";

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

            yield return new AgentSilo
            {
                Metadata = new V1ObjectMeta
                {
                    Name = agent.Status.AgentSiloName,
                    NamespaceProperty = agent.Namespace(),
                    Labels = labels
                },
                Spec = CreateAgentSiloSpec(agent, labels)
            };
        }

        private static AgentSiloSpec CreateAgentSiloSpec(ComaxAgent agent, IDictionary<string, string> labels)
        {
            var s = agent.Spec;
            var spec = new AgentSiloSpec
            {
                ClusterId = s.ClusterId,
                CommonsMembership = $"http://{agent.Status.AgentRefereeName}.{agent.Namespace()}.svc.cluster.local",
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
