using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DockerIntegration
{
    public class Client
    {
        public async Task<IList<ContainerListResponse>> InstallContainer(string containerName, string image, List<(int,int)> portMappings)
        {
            DockerClient client = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine"))
                 .CreateClient();

            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        All = true
                    });

            containers = containers.Where(x=>x.Names.Any(x=>x.Contains(containerName))).ToList();

            return containers;
        }
    }
}
