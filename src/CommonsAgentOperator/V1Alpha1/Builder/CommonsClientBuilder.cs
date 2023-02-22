using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s.Models;
using k8s;
using System.Collections.ObjectModel;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder
{
    public partial class DeploymentBuilder
    {
        public static IEnumerable<IKubernetesObject<V1ObjectMeta>> Build(ComaxCommonsClient client)
        {
            var labels = new Dictionary<string, string>()
            {
                { Constants.ControlledBy, Constants.OperatorName },
                { Constants.App, "referee" },
                { Constants.Kind, "service" },
                { Constants.Name, client.Name() }
            };

            List<IKubernetesObject<V1ObjectMeta>> objects = new List<IKubernetesObject<V1ObjectMeta>>();
            objects.Add(new V1Deployment
            {
                Metadata = new V1ObjectMeta
                {
                    Name = client.GetDeploymentName(),
                    NamespaceProperty = client.Namespace(),
                    Labels = labels
                },
                Spec = CreatCommonsClientDeploymentSpec(client, labels)
            });

            objects.Add(CreateCommonsClientService(client, labels));

            return objects;
        }

        private static V1Service CreateCommonsClientService(ComaxCommonsClient client, IDictionary<string, string> labels)
        {
            return new V1Service()
            {
                Metadata = new V1ObjectMeta
                {
                    Labels = labels,
                    Name = $"{client.GetDeploymentName()}-ep",
                    NamespaceProperty = client.Namespace()
                },
                Spec = new V1ServiceSpec
                {
                    Ports = new V1ServicePort[]
                    {
                        new V1ServicePort
                        {
                            Protocol= "TCP",
                            Port=80,
                            TargetPort=new IntstrIntOrString{ Value = "5001" },
                        }
                    },
                    Selector = labels,
                    Type = "ClusterIP"
                }
            };
        }

        private static V1DeploymentSpec CreatCommonsClientDeploymentSpec(ComaxCommonsClient client, IDictionary<string, string> labels)
        {
            foreach (var kvp in client.Spec.Labels)
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
                        //Annotations = client.Spec.Annotations
                    },
                    Spec = new V1PodSpec
                    {
                        Containers = new Collection<V1Container>
                        {
                            GetCommonsClientContainerSpec(client)
                        }
                    }
                }
            };

            return spec;
        }

        private static V1Container GetCommonsClientContainerSpec(ComaxCommonsClient client)
        {
            var spec = client.Spec;

            var envVariables = new List<V1EnvVar>();
            
            var container = new V1Container
            {
                Name = "commonsclient",
                Image = spec.CommonsClientImage,
                Ports = new Collection<V1ContainerPort>
                {
                    new V1ContainerPort(5001, name: "svc")
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
