using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dockerwaiter.Containers
{

    public interface IContainerHelper
    {
        Task<IEnumerable<ContainerProperties>> GetContainersInDockerCompose(string dockerComposeFilename);
        Task<IEnumerable<ContainerProperties>> GetContainersInDockerComposeFilteredByNames(string dockerComposeFilename, params string[] containerlist);
        Task StopAllContainersInDockerCompose(string dockerComposeFilename);
    }
    public class ContainerHelper: IContainerHelper
    {
        private readonly bool _isWindowsOS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private readonly DockerClient _client;
        private readonly string _logsPath = Directory.GetCurrentDirectory();

        public ContainerHelper(string logPath="")
        {
            if (!string.IsNullOrEmpty(logPath))
                _logsPath = logPath;
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

                await WriteLogToFile(containertoStop);
                _ = await _client.Containers.StopContainerAsync(containertoStop.Id,
                    new ContainerStopParameters
                    {
                        WaitBeforeKillSeconds = 30
                    },
                    CancellationToken.None);

            }
        }

        private async Task WriteLogToFile(ContainerProperties container)
        {
            string logFileToWrite = Path.Join(_logsPath, $"{container.ContainerName}.log");
            var logContent = await ExportLogsFromContainer(container.Id);
            await System.IO.File.WriteAllTextAsync(logFileToWrite, logContent);
        }

        public async Task<string> ExportLogsFromContainer(string containerId)
        {
            string content = "";
            try
            {
                
                    var parameters = new ContainerLogsParameters
                    {
                        ShowStdout = true,
                        ShowStderr = true,
                        Timestamps = true,
                    };
                    var logStream = await _client?.Containers?.GetContainerLogsAsync(containerId, parameters, default);
                    if (logStream != null)
                    {
                        using (var reader = new StreamReader(logStream))
                        {
                            content =await reader.ReadToEndAsync();
                        }
                    }
                }
            catch (Exception ex)
            {
                throw ex;
            }
            return content;
        }
    }
}
