using k8s.Models;
using KubeOps.Operator.Entities;
using System.Globalization;
using System.Text.Json.Serialization;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities
{
    [KubernetesEntity(
        ApiVersion = "v1alpha1",
        Group = "communaxiom.org",
        Kind = "AgentSilo",
        PluralName = "agentsilos")]
    public class AgentSilo : CustomKubernetesEntity<AgentSiloSpec, AgentSiloState>, IAssignableSpec<AgentSiloSpec>
    {
        public AgentSilo()
        {
            this.ApiVersion = "communaxiom.org/v1alpha1";
            this.Kind = "AgentSilo";
        }

        public void Assign(IAssignableSpec<AgentSiloSpec> other)
        {
            this.Spec.Assign(other.Spec);
        }

        public void Assign(IAssignableSpec other)
        {
            var so = (AgentSilo)other;
            this.Spec.Assign(so.Spec);
        }
    }

    public static class AgentSiloExtensions
    {
        public static string GetDeploymentName(this AgentSilo agentReferee)
        {
            return $"{agentReferee.Name()}-depl";
        }
    }

    public class AgentSiloState: IComaxState
    {
        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = Status.Unknown.ToString();

        [JsonPropertyName("stateTsMs")]
        public long StateTsMs { get; set; } = 0;

        [JsonPropertyName("stateTs")]
        public long StateTs { get; set; } = 0;
    }

    public class AgentSiloSpec : IAssignable<AgentSiloSpec>
    {
        public void Assign(AgentSiloSpec other)
        {
            this.Annotations = other.Annotations;
            this.ClusterId = other.ClusterId;
            this.CommonsMembership = other.CommonsMembership;
            this.DbCredPasswordKey = other.DbCredPasswordKey;
            this.DbCredRootPasswordKey = other.DbCredRootPasswordKey;
            this.DbCredSecretName = other.DbCredSecretName;
            this.DbCredUsernameKey = other.DbCredUsernameKey;
            this.EnvironmentVariables = other.EnvironmentVariables;
            this.GatewayPort = other.GatewayPort;
            this.Image = other.Image;
            this.Labels = other.Labels;
            this.MariadbImage = other.MariadbImage;
            this.MembershipAddress = other.MembershipAddress;
            this.OidcAuthority = other.OidcAuthority;
            this.OidcClientId = other.OidcClientId;
            this.OidcSecretKey = other.OidcSecretKey;
            this.OidcSecretName = other.OidcSecretName;
            this.Resources = other.Resources;
            this.ServiceId = other.ServiceId;
            this.SiloPort = other.SiloPort;
            this.StoreApiImage = other.StoreApiImage;            
        }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("storeApiImage")]
        public string StoreApiImage { get; set; }

        [JsonPropertyName("mariadbImage")]
        public string MariadbImage { get; set; }

        [JsonPropertyName("oidcAuthority")]
        public string OidcAuthority { get; set; }

        [JsonPropertyName("oidcClientId")]
        public string OidcClientId { get; set; }

        [JsonPropertyName("oidcSecretName")]
        public string OidcSecretName { get; set; }

        [JsonPropertyName("oidcSecretKey")]
        public string OidcSecretKey { get; set; }

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

        [JsonPropertyName("commonsMembership")]
        public string CommonsMembership { get; set; }


        [JsonPropertyName("dbCredSecretName")]
        public string DbCredSecretName { get; set; }

        [JsonPropertyName("dbCredUsernameKey")]
        public string DbCredUsernameKey { get; set; }

        [JsonPropertyName("dbCredPasswordKey")]
        public string DbCredPasswordKey { get; set; }

        [JsonPropertyName("dbCredRootPasswordKey")]
        public string DbCredRootPasswordKey { get; set; }

        /// <summary>
        /// Optional resource limits and requests.
        /// Defaults to limit.cpu: 500m, limit.memory: 512M, requests.cpu: 200m, requests.memory: 256M
        /// </summary>
        [JsonPropertyName("resources")]
        public V1ResourceRequirements? Resources { get; set; } = new();

        /// <summary>
        /// Additional environment variables to add to the primary container.
        /// </summary>
        [JsonPropertyName("environmentVariables")]
        public IList<V1EnvVar> EnvironmentVariables { get; set; } = new List<V1EnvVar>();

        /// <summary>
        /// Optional extra annotations to add to deployment.
        /// </summary>
        [JsonPropertyName("annotations")]
        public IDictionary<string, string> Annotations { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Optional extra labels to add to deployment.
        /// </summary>
        [JsonPropertyName("labels")]
        public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
    }
}
