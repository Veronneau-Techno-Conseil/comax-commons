using k8s.Models;
using KubeOps.Operator.Entities;
using System.Text.Json.Serialization;

namespace CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities
{
    [KubernetesEntity(
    ApiVersion = "v1alpha1",
    Group = "communaxiom.org",
    Kind = "AgentReferee",
    PluralName = "agentrefs")
]
    public class AgentReferee : CustomKubernetesEntity<AgentRefereeSpec, AgentRefereeState>
    {

    }

    public static class AgentRefereeExtensions
    {
        public static string GetDeploymentName(this AgentReferee agentReferee)
        {
            return $"{agentReferee.Name()}-deployment";
        }
    }

    public class AgentRefereeState
    {

        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = Status.Unknown.ToString();

        [JsonPropertyName("stateTsMs")]
        public long StateTsMs { get; set; } = 0;

        [JsonPropertyName("stateTs")]
        public long StateTs { get; set; } = 0;
    }

    public class AgentRefereeSpec
    {
        [JsonPropertyName("image")]
        public string Image { get; set; } = "vertechcon/comax-agentreferee:latest";

        [JsonPropertyName("listenPort")]
        public int ListenPort { get; set; } = 5004;

        [JsonPropertyName("oidcAuthority")]
        public string OidcAuthority { get; set; }

        [JsonPropertyName("oidcClientId")]
        public string OidcClientId { get; set; }

        [JsonPropertyName("oidcSecretName")]
        public string OidcSecretName { get; set; }

        [JsonPropertyName("oidcSecretKey")]
        public string OidcSecretKey { get; set; }

        [JsonPropertyName("useHttps")]
        public bool UseHttps { get; set; }

        [JsonPropertyName("certPath")]
        public string CertPath { get; set; }
        [JsonPropertyName("keyPath")]
        public string KeyPath { get; set; }
        [JsonPropertyName("dbProvider")]
        public string DbProvider { get; set; }

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
