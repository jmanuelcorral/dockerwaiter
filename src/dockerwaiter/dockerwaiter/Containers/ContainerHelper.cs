using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dockerwaiter.Containers
{
    public class ContainerHelper
    {
        private readonly bool _isWindowsOS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private readonly DockerClient _client;
        public ContainerHelper()
        {
            if (_isWindowsOS)
            {
                _client = new DockerClientConfiguration(
               new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();
            }
            else
            {
                _client = new DockerClientConfiguration(
                    new Uri("unix:///var/run/docker.sock"))
                     .CreateClient();
            }
        }

        public async Task<IEnumerable<ContainerProperties>> GetContainersInDockerCompose(string dockerComposeFilename)
        {
            var containerList = await _client.Containers.ListContainersAsync(
               new ContainersListParameters()
               {
                   Limit = 100,
               });

           var list = containerList.Select(
                x => new ContainerProperties(x));
            return list.Where(x => x.ComposeFile.Contains(dockerComposeFilename));
        }

        public async Task<IEnumerable<ContainerProperties>> GetContainersInDockerComposeFilteredByNames(string dockerComposeFilename, params string[] containerlist)
        {
            return (await GetContainersInDockerCompose(dockerComposeFilename)).Where(x => containerlist.Contains(x.ContainerName));
        }

        public async Task StopAllContainersInDockerCompose(string dockerComposeFilename)
        {
            var containersToStop = await GetContainersInDockerCompose(dockerComposeFilename);

            foreach (var containertoStop in containersToStop)
            {
                _ = await _client.Containers.StopContainerAsync(containertoStop.Id,
                    new ContainerStopParameters
                    {
                        WaitBeforeKillSeconds = 30
                    },
                    CancellationToken.None);

            }
        }
    }
}
