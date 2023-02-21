using k8s.Models;
using KubeOps.Operator.Entities;
using System.Text.Json.Serialization;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities
{

    [KubernetesEntity(
        ApiVersion = "v1alpha1",
        Group = "communaxiom.org",
        Kind = "ComaxCommonsClient",
        PluralName = "comaxcomclients")]
    public class ComaxCommonsClient : CustomKubernetesEntity<ComaxCommonsClientSpec, ComaxCommonsClientState>
    {

    }

    public static class ComaxCommonsClientExtensions
    {
        public static string GetDeploymentName(this ComaxCommonsClient agentReferee)
        {
            return $"{agentReferee.Name()}-depl";
        }
    }

    public class ComaxCommonsClientState
    {

        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = Status.Unknown.ToString();

        [JsonPropertyName("stateTsMs")]
        public long StateTsMs { get; set; } = 0;

        [JsonPropertyName("stateTs")]
        public long StateTs { get; set; } = 0;

        [JsonPropertyName("clusterState")]
        public IDictionary<string, string> ClusterState { get; set; }

    }

    public class ComaxCommonsClientSpec
    {

        [JsonPropertyName("ingressHost")]
        public string IngressHost { get; set; }

        [JsonPropertyName("ingressCertManager")]
        public string IngressCertManager { get; set; }

        [JsonPropertyName("ingressCertSecret")]
        public string IngressCertSecret { get; set; }

        [JsonPropertyName("useHttps")]
        public bool UseHttps { get; set; }


        [JsonPropertyName("oidcAuthority")]
        public string OidcAuthority { get; set; }

        [JsonPropertyName("oidcClientId")]
        public string OidcClientId { get; set; }

        [JsonPropertyName("oidcSecretName")]
        public string OidcSecretName { get; set; }

        [JsonPropertyName("oidcSecretKey")]
        public string OidcSecretKey { get; set; }


        [JsonPropertyName("commonsClientImage")]
        public string CommonsClientImage { get; set; }

        [JsonPropertyName("commonsMembershipHost")]
        public string CommonsMembershipHost { get; set; }


        [JsonPropertyName("commonsMembershipCacheDuration")]
        public string CommonsMembershipCacheDuration { get; set; }



        [JsonPropertyName("clusterId")]
        public string ClusterId { get; set; }

        [JsonPropertyName("serviceId")]
        public string ServiceId { get; set; }


        /// <summary>
        /// Optional resource limits and requests.
        /// Defaults to limit.cpu: 500m, limit.memory: 512M, requests.cpu: 200m, requests.memory: 256M
        /// </summary>
        [JsonPropertyName("resources")]
        public V1ResourceRequirements? Resources { get; set; } = new();

        /// <summary>
        /// Optional extra labels to add to deployment.
        /// </summary>
        [JsonPropertyName("labels")]
        public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
    }

}
