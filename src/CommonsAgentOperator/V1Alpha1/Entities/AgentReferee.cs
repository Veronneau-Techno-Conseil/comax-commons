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
        public string DeploymentName
        {
            get
            {
                return $"{this.Name()}-deployment";
            }
        }
    }

    public class AgentRefereeState
    {
        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = Status.Unknown.ToString();

        [JsonPropertyName("dbAccess")]
        public bool DbAccess { get; set; } = false;

        [JsonPropertyName("stateTsMs")]
        public long StateTsMs { get; set; } = 0;

        [JsonPropertyName("stateTs")]
        public long StateTs { get; set; } = 0;
    }

    public class AgentRefereeSpec
    {
        public string Image { get; set; } = "vertechcon/referee:0.0.1";

        public int Port { get; set; } = 5002;

        /// <summary>
        /// Optional resource limits and requests.
        /// Defaults to limit.cpu: 500m, limit.memory: 512M, requests.cpu: 200m, requests.memory: 256M
        /// </summary>
        [JsonPropertyName("resources")]
        public V1ResourceRequirements? Resources { get; set; } = new();

        [JsonPropertyName("dbServerName")]
        public string DbServerName { get; set; }

        [JsonPropertyName("dbUser")]
        public string DbUser { get; set; }

        /// <summary>
        /// Name of secret where password for dbUser is stored.
        /// </summary>
        [JsonPropertyName("dbPasswordSecretName")]
        public string DbPasswordSecretName { get; set; } = null!;

        /// <summary>
        /// Key in DbPasswordSecretName which the password is stored in.
        /// </summary>
        [JsonPropertyName("dbPasswordSecretKey")]
        public string DbPasswordSecretKey { get; set; } = "password";

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
