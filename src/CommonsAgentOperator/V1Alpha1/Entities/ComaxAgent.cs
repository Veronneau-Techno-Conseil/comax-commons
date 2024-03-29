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
    public class ComaxAgent : CustomKubernetesEntity<ComaxAgentSpec, ComaxAgentState>, IAssignableSpec<ComaxAgentSpec>
    {
        public ComaxAgent() : base()
        {
            this.ApiVersion = "communaxiom.org/v1alpha1";
            this.Kind = "ComaxAgent";
        }

        public void Assign(IAssignableSpec<ComaxAgentSpec> other)
        {
            this.Spec.Assign(other.Spec);
        }

        public void Assign(IAssignableSpec other)
        {
            var ca = (ComaxAgent)other;
            this.Spec.Assign(ca.Spec);
        }
    }

    public static class ComaxAgentExtensions
    {
        public static string GetDeploymentName(this ComaxAgent agentReferee)
        {
            return $"{agentReferee.Name()}-depl";
        }

        public static string GetAgentRefereeName(this ComaxAgent agent)
        {
            return $"{agent.Name()}-ref";
        }

        public static string GetAgentSiloName(this ComaxAgent agent)
        {
            return $"{agent.Name()}-agt";
        }
            
    }

    public class ComaxAgentState: IComaxState
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

    public class ComaxAgentSpec : IAssignable<ComaxAgentSpec>
    {
        public void Assign(ComaxAgentSpec other)
        {
            this.AgentSiloImage = other.AgentSiloImage;
            this.ClusterId = other.ClusterId;
            this.CommonsClientImage = other.CommonsClientImage;
            this.DbCredPasswordKey = other.DbCredPasswordKey;
            this.DbCredRootPasswordKey = other.DbCredRootPasswordKey;
            this.DbCredSecretName = other.DbCredSecretName;
            this.DbCredUsernameKey = other.DbCredUsernameKey;
            this.GatewayPort = other.GatewayPort;
            this.IngressCertManager = other.IngressCertManager;
            this.IngressCertSecret = other.IngressCertSecret;
            this.IngressHost = other.IngressHost;
            this.Labels = other.Labels;
            this.MariadbImage = other.MariadbImage;
            this.MembershipAddress = other.MembershipAddress;
            this.OidcAuthority = other.OidcAuthority;
            this.OidcClientId = other.OidcClientId;
            this.OidcSecretKey = other.OidcSecretKey;
            this.OidcSecretName = other.OidcSecretName;
            this.RefereeImage = other.RefereeImage;
            this.ServiceId = other.ServiceId;
            this.SiloPort = other.SiloPort;
            this.StoreApiImage = other.StoreApiImage;
            this.UseHttps = other.UseHttps;
        }

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
