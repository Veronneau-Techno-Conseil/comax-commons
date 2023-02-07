using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s;
using k8s.Models;
using System.Collections.ObjectModel;
using System.Resources;

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
                Spec = CreateAgentRefereeDeploymentSpec(agentReferee, labels)
            };

            yield return CreateService(agentReferee, labels);
        }

        private static V1Service CreateService(AgentReferee agentReferee, IDictionary<string, string> labels)
        {
            return new V1Service()
            {
                Metadata = new V1ObjectMeta
                {
                    Labels = labels,
                    Name = $"{agentReferee.DeploymentName}-ep",
                    NamespaceProperty = agentReferee.Namespace()
                },
                Spec = new V1ServiceSpec
                {
                    Ports = new V1ServicePort[]
                    {
                        new V1ServicePort
                        {
                            Protocol= "TCP",
                            Port=agentReferee.Spec.ListenPort,
                            TargetPort=new IntstrIntOrString{ Value = agentReferee.Spec.ListenPort.ToString() },
                        }
                    },
                    Selector = labels,
                    Type = "ClusterIP"
                }
            };
        }

        private static V1DeploymentSpec CreateAgentRefereeDeploymentSpec(AgentReferee agentReferee, IDictionary<string, string> labels)
        {
            foreach (var kvp in agentReferee.Spec.Labels)
                labels.TryAdd(kvp.Key, kvp.Value);

            var spec = new V1DeploymentSpec
            {
                Replicas = 1,
                Selector = new V1LabelSelector(null, labels),
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = labels,
                        Annotations = agentReferee.Spec.Annotations
                    },
                    Spec = new V1PodSpec
                    {
                        Containers = new Collection<V1Container>
                        {
                            GetRefereeContainerSpec(agentReferee)
                        }
                    }
                }
            };

            return spec;
        }

        private static V1Container GetRefereeContainerSpec(AgentReferee resource)
        {
            var spec = resource.Spec;

            var envVariables = new List<V1EnvVar>();
            if (!string.IsNullOrEmpty(spec.OidcAuthority))
                envVariables.Add(new("OIDC__Authority", spec.OidcAuthority));

            envVariables.Add(new("OIDC__Scopes", "offline_access roles profile email"));

            if (!string.IsNullOrEmpty(spec.OidcClientId))
                envVariables.Add(new("OIDC__ClientId", spec.OidcClientId));

            if (!string.IsNullOrEmpty(spec.OidcSecretName) && !string.IsNullOrEmpty(spec.OidcSecretKey))
                envVariables.Add(
                new("OIDC__Secret", valueFrom: new V1EnvVarSource
                {
                    SecretKeyRef = new V1SecretKeySelector
                    {
                        Key = spec.OidcSecretKey,
                        Name = spec.OidcSecretName,
                    }
                }));

            if (spec.ListenPort > 0)
                envVariables.Add(new("ListenPort", spec.ListenPort.ToString()));

            envVariables.Add(new("UseHttps", spec.UseHttps.ToString()));

            if (!string.IsNullOrEmpty(spec.DbProvider))
                envVariables.Add(new("db", spec.DbProvider));

            envVariables.AddRange(spec.EnvironmentVariables);

            var container = new V1Container
            {
                Name = "agentreferee",
                Image = spec.Image,
                Ports = new Collection<V1ContainerPort>
                {
                    new(spec.ListenPort > 0 ? spec.ListenPort : 5004, name: "svc"),
                },
                Env = envVariables,
            };

            if (spec.Resources != null)
            {
                var reqs = new V1ResourceRequirements();
                reqs.Limits = spec.Resources?.Limits;
                reqs.Requests = spec.Resources?.Requests;
                container.Resources = reqs;
            }

            return container;
        }
    }
}
