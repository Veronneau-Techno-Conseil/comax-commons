using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using k8s.Models;
using k8s;
using System.Collections.ObjectModel;

using System.Reflection.Emit;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Builder
{
   public partial class DeploymentBuilder
   {

       public static IEnumerable<IKubernetesObject<V1ObjectMeta>> Build(AgentSilo silo)
       {
           var labels = new Dictionary<string, string>()
           {
               { Constants.ControlledBy, Constants.OperatorName },
               { Constants.Kind, "service" },
               { Constants.Name, silo.Name() }
           };

           foreach (var o in BuildAgentSilo(silo, labels)) yield return o;

           foreach (var o in BuildMariaDb(silo, labels)) yield return o;

           foreach (var o in BuildStoreApi(silo, labels)) yield return o;
       }

       #region AgentSilo

       private static IEnumerable<IKubernetesObject<V1ObjectMeta>> BuildAgentSilo(AgentSilo silo, IDictionary<string, string> labels)
       {
           var dico = new Dictionary<string, string>(labels)
           {
               { Constants.App, "agentsilo" }
           };

           yield return new V1Deployment
           {
               Metadata = new V1ObjectMeta
               {
                   Name = silo.GetDeploymentName(),
                   NamespaceProperty = silo.Namespace(),
                   Labels = dico
               },
               Spec = CreateAgentSiloDeploymentSpec(silo, dico)
           };

           yield return CreateService(silo, dico);
       }

       private static V1Service CreateService(AgentSilo silo, IDictionary<string, string> labels)
       {
           return new V1Service()
           {
               Metadata = new V1ObjectMeta
               {
                   Labels = labels,
                   Name = $"{silo.GetDeploymentName()}-ep",
                   NamespaceProperty = silo.Namespace()
               },
               Spec = new V1ServiceSpec
               {
                   Ports = new V1ServicePort[]
                   {
                       new V1ServicePort
                       {
                           Protocol= "TCP",
                           Port= int.Parse(silo.Spec.SiloPort),
                           TargetPort=new IntstrIntOrString{ Value = silo.Spec.SiloPort },
                       },
                       new V1ServicePort
                       {
                           Protocol= "TCP",
                           Port= int.Parse(silo.Spec.GatewayPort),
                           TargetPort=new IntstrIntOrString{ Value = silo.Spec.GatewayPort },
                       }
                   },
                   Selector = labels,
                   Type = "ClusterIP"
               }
           };
       }


       private static V1DeploymentSpec CreateAgentSiloDeploymentSpec(AgentSilo silo, IDictionary<string, string> labels)
       {
           Dictionary<string, string> lbls = new Dictionary<string, string>(labels);
           foreach (var kvp in silo.Spec.Labels)
               lbls.TryAdd(kvp.Key, kvp.Value);

           var spec = new V1DeploymentSpec
           {
               Replicas = 1,
               Selector = new V1LabelSelector(null, labels),
               Template = new V1PodTemplateSpec
               {
                   Metadata = new V1ObjectMeta
                   {
                       Labels = labels,
                       Annotations = silo.Spec.Annotations
                   },
                   Spec = new V1PodSpec
                   {
                       Containers = new Collection<V1Container>
                       {
                           GetSiloContainerSpec(silo)
                       }
                   }
               }
           };

           return spec;
       }

       private static V1Container GetSiloContainerSpec(AgentSilo silo)
       {
           var spec = silo.Spec;

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

           envVariables.Add(new("client_mode", "prod"));

           if (!string.IsNullOrWhiteSpace(spec.MembershipAddress))
               envVariables.Add(new("membership__host", spec.MembershipAddress));

           if (!string.IsNullOrWhiteSpace(spec.SiloPort))
               envVariables.Add(new("siloPort", spec.SiloPort));

           if (!string.IsNullOrWhiteSpace(spec.GatewayPort))
               envVariables.Add(new("gatewayPort", spec.GatewayPort));

           envVariables.Add(new V1EnvVar("advertisedIp", valueFrom: new V1EnvVarSource(fieldRef: new V1ObjectFieldSelector { ApiVersion = "v1", FieldPath = "status.podIP" })));

           if (!string.IsNullOrWhiteSpace(spec.CommonsMembership))
               envVariables.Add(new("CommonsMembership__Host", spec.CommonsMembership));

           envVariables.Add(new("AgentConfig__LiveTickerPeriod", "30"));
           envVariables.Add(new("AgentConfig__ConnectionCheckPeriod", "30"));
           envVariables.Add(new("AgentConfig__SaveStatePeriod", "60"));

           envVariables.AddRange(spec.EnvironmentVariables);

           if (int.TryParse(spec.SiloPort, out int sport))
               sport = 7717;

           if (int.TryParse(spec.GatewayPort, out int gport))
               gport = 30000;

           var container = new V1Container
           {
               Name = "agentsilo",
               Image = spec.Image,
               Ports = new Collection<V1ContainerPort>
               {
                   new( sport, name: "siloport"),
                   new( gport, name: "gatewayport"),
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
       #endregion


       #region MariaDbStore

       private static IEnumerable<IKubernetesObject<V1ObjectMeta>> BuildMariaDb(AgentSilo silo, IDictionary<string, string> labels)
       {
           var dico = new Dictionary<string, string>(labels)
           {
               { Constants.App, "grainstoredb" }
           };

           yield return CreateMariaDbDeploymentSpec(silo, dico);

           yield return CreateMariaDbService(silo, dico);
       }

       private static IKubernetesObject<V1ObjectMeta> CreateMariaDbService(AgentSilo silo, Dictionary<string, string> labels)
       {

           return new V1Service()
           {
               Metadata = new V1ObjectMeta
               {
                   Labels = labels,
                   Name = $"grainstoredb",
                   NamespaceProperty = silo.Namespace()
               },
               Spec = new V1ServiceSpec
               {
                   Ports = new V1ServicePort[]
                   {
                       new V1ServicePort
                       {
                           Protocol= "TCP",
                           Port= 3306,
                           TargetPort=3306
                       }
                   },
                   Selector = labels,
                   Type = "ClusterIP"
               }
           };
       }

       private static IKubernetesObject<V1ObjectMeta> CreateMariaDbDeploymentSpec(AgentSilo silo, Dictionary<string, string> dico)
       {
           var depl = new V1Deployment
           {
               Metadata = new V1ObjectMeta
               {
                   Name = $"{silo.GetDeploymentName()}-store",
                   NamespaceProperty = silo.Namespace(),
                   Labels = dico
               },
               Spec = new V1DeploymentSpec
               {
                   Replicas = 1,
                   Selector = new V1LabelSelector(null, dico),
                   Template = new V1PodTemplateSpec
                   {
                       Metadata = new V1ObjectMeta
                       {
                           Labels = dico,
                           Annotations = silo.Spec.Annotations
                       },
                       Spec = new V1PodSpec
                       {
                           Containers = new Collection<V1Container>
                           {
                               GetMariaDbContainerSpec(silo)
                           }
                       }
                   }
               }
           };

           return depl;
       }

       private static V1Container GetMariaDbContainerSpec(AgentSilo silo)
       {
           var spec = silo.Spec;

           var envVariables = new List<V1EnvVar>()
           {
               new V1EnvVar("MYSQL_ROOT_PASSWORD", valueFrom: new V1EnvVarSource(
                   secretKeyRef: new V1SecretKeySelector(spec.DbCredRootPasswordKey, spec.DbCredSecretName))),
               new V1EnvVar("MYSQL_USER", valueFrom: new V1EnvVarSource(
                   secretKeyRef: new V1SecretKeySelector(spec.DbCredRootPasswordKey, spec.DbCredUsernameKey))),
               new V1EnvVar("MYSQL_PASSWORD", valueFrom: new V1EnvVarSource(
                   secretKeyRef: new V1SecretKeySelector(spec.DbCredRootPasswordKey, spec.DbCredPasswordKey))),
               new V1EnvVar("MYSQL_PASSWORD", "grainstates")
           };

           var container = new V1Container
           {
               Name = "mariadb",
               Image = string.IsNullOrWhiteSpace(spec.MariadbImage) ? "mariadb:10.10" : spec.MariadbImage,
               Ports = new Collection<V1ContainerPort>
               {
                   new( 3306, name: "svcport", protocol: "tcp")
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


       #endregion

       #region StorageApi

       private static IEnumerable<IKubernetesObject<V1ObjectMeta>> BuildStoreApi(AgentSilo silo, IDictionary<string, string> labels)
       {
           var dico = new Dictionary<string, string>(labels)
           {
               { Constants.App, "storeapi" }
           };

           yield return CreateStorageApiDeploymentSpec(silo, dico);

           yield return CreateStoreApiService(silo, dico);
       }

       private static V1Service CreateStoreApiService(AgentSilo silo, IDictionary<string, string> labels)
       {
           return new V1Service()
           {
               Metadata = new V1ObjectMeta
               {
                   Labels = labels,
                   Name = $"{silo.GetDeploymentName()}-store-ep",
                   NamespaceProperty = silo.Namespace()
               },
               Spec = new V1ServiceSpec
               {
                   Ports = new V1ServicePort[]
                   {
                       new V1ServicePort
                       {
                           Protocol= "TCP",
                           Port= 80,
                           TargetPort=80
                       }
                   },
                   Selector = labels,
                   Type = "ClusterIP"
               }
           };
       }

       private static V1Deployment CreateStorageApiDeploymentSpec(AgentSilo silo, IDictionary<string, string> dico)
       {

           var depl = new V1Deployment
           {
               Metadata = new V1ObjectMeta
               {
                   Name = $"{silo.GetDeploymentName()}-store",
                   NamespaceProperty = silo.Namespace(),
                   Labels = dico
               },
               Spec = new V1DeploymentSpec
               {
                   Replicas = 1,
                   Selector = new V1LabelSelector(null, dico),
                   Template = new V1PodTemplateSpec
                   {
                       Metadata = new V1ObjectMeta
                       {
                           Labels = dico,
                           Annotations = silo.Spec.Annotations
                       },
                       Spec = new V1PodSpec
                       {
                           Containers = new Collection<V1Container>
                           {
                               GetStoreApiContainerSpec(silo)
                           }
                       }
                   }
               }
           };

           return depl;

       }

       private static V1Container GetStoreApiContainerSpec(AgentSilo silo)
       {
           var spec = silo.Spec;

           var envVariables = new List<V1EnvVar>()
           {
               new V1EnvVar("DbConfig__MemoryDb","false"),
               new V1EnvVar("DbConfig__Server",$"grainstoredb.{silo.Namespace()}.svc.cluster.local"),
               new V1EnvVar("DbConfig__Username", valueFrom: new V1EnvVarSource(
                   secretKeyRef: new V1SecretKeySelector(spec.DbCredUsernameKey, spec.DbCredSecretName)
               )),
               new V1EnvVar("DbConfig__Password",valueFrom: new V1EnvVarSource(
                   secretKeyRef: new V1SecretKeySelector(spec.DbCredPasswordKey, spec.DbCredSecretName)
               )),
               new V1EnvVar("DbConfig__Database","grainstates"),
               new V1EnvVar("DbConfig__ShouldDrop","false"),
               new V1EnvVar("DbConfig__ShouldMigrate","true"),
               new V1EnvVar("DbConfig__Port","3306"),
           };

           var container = new V1Container
           {
               Name = "storeapi",
               Image = spec.StoreApiImage,
               Ports = new Collection<V1ContainerPort>
               {
                   new( 80, name: "svcport")
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

       #endregion
   }
}
