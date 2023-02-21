using k8s.Models;
using KubeOps.Operator.Entities;
using System.Text.Json.Serialization;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities
{
    [KubernetesEntity(
    ApiVersion = "v1alpha1",
    Group = "communaxiom.org",
    Kind = "ComaxAgent",
    PluralName = "comaxagents")]
    public class ComaxAgent : CustomKubernetesEntity<ComaxAgentSpec, ComaxAgentState>
    {

    }

    public static class ComaxAgentExtensions
    {
        public static string GetDeploymentName(this ComaxAgent agentReferee)
        {
            return $"{agentReferee.Name()}-depl";
        }
    }

    public class ComaxAgentState
    {

        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = Status.Unknown.ToString();

        [JsonPropertyName("stateTsMs")]
        public long StateTsMs { get; set; } = 0;

        [JsonPropertyName("stateTs")]
        public long StateTs { get; set; } = 0;

        [JsonPropertyName("clusterState")]
        public IDictionary<string, string> ClusterState { get; set; }

        [JsonPropertyName("agentRefereeName")]
        public string AgentRefereeName { get; set; }

        [JsonPropertyName("agentSiloName")]
        public string AgentSiloName { get; set; }
    }

    public class ComaxAgentSpec
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

        [JsonPropertyName("refereeImage")]
        public string RefereeImage { get; set; }

        [JsonPropertyName("agentSiloImage")]
        public string AgentSiloImage { get; set; }

        [JsonPropertyName("storeApiImage")]
        public string StoreApiImage { get; set; }

        [JsonPropertyName("mariadbImage")]
        public string MariadbImage { get; set; }


        

        [JsonPropertyName("membershipAddress")]
        public string MembershipAddress { get; set; }

        [JsonPropertyName("clusterId")]
        public string ClusterId { get; set; }

        [JsonPropertyName("serviceId")]
        public string ServiceId { get; set; }

        [JsonPropertyName("siloPort")]
        public string SiloPort { get; set; } = "7717";

        [JsonPropertyName("gatewayPort")]
        public string GatewayPort { get; set; } = "30000";

        [JsonPropertyName("dbCredSecretName")]
        public string DbCredSecretName { get; set; }

        [JsonPropertyName("dbCredUsernameKey")]
        public string DbCredUsernameKey { get; set; }

        [JsonPropertyName("dbCredPasswordKey")]
        public string DbCredPasswordKey { get; set; }

        [JsonPropertyName("dbCredRootPasswordKey")]
        public string DbCredRootPasswordKey { get; set; }

        /// <summary>
        /// Optional extra labels to add to deployment.
        /// </summary>
        [JsonPropertyName("labels")]
        public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
    }
}
