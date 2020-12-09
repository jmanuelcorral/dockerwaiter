using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTables;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace dockerwaiter
{
    class Program
    {
        private readonly static bool _isWindowsOS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private readonly static int _executionTimeOut = 60 * 60 * 1000;

        async static Task<int> Main(string[] args)
        {
            List<string> containersToFilter = new List<string>() { "platformintegration_tests", "unittests" };
            string dockerComposeFilter = "docker-compose.platformintegration.test.yml";

            DockerClient client;
            if (_isWindowsOS)
            {
                client = new DockerClientConfiguration(
               new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();
            }
            else
            { 
            client = new DockerClientConfiguration(
                new Uri("unix:///var/run/docker.sock"))
                 .CreateClient();
            }

            bool allContainersExited = false;
            while (!allContainersExited)
            {
                var containerList = await client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = 10,
                });
                
                var containersWithoutFilter = containerList.Select(
                    x => new ContainerProperties(x) );
                var containers = containersWithoutFilter.Where(x => containersToFilter.Contains(x.ContainerName) && x.ComposeFile.Contains(dockerComposeFilter));
                if (containers.Count() > 0)
                {
                    //Display Containers
                    ConsoleTable
                    .From<ContainerProperties>(containers)
                    .Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write(Format.Alternative);

                    foreach (var container in containers)
                    {
                        if (!container.IsExecuting && container.ExitCode.HasValue)
                        {
                            if (container.ExitCode.Value == 0)
                            {
                                containersToFilter.Remove(container.ContainerName);
                            }
                            else
                            {
                                return container.ExitCode.Value;
                            }
                        }
                    }
                    // see if is ExecutingOrIsFinished
                    // if Executing, see nextContainer
                    // if Finished see exitCode, if exitcode!= 0 break and exit
                    // if exitcode == 0 release from containerlisttobewatched
                    Thread.Sleep(1000);
                    Console.Clear();
                }
                else
                {
                    foreach(var containertoStop in containersWithoutFilter)
                    {
                        if (containertoStop.ComposeFile.Contains(dockerComposeFilter))
                        {
                            var stopped = await client.Containers.StopContainerAsync(containertoStop.Id,
                                new ContainerStopParameters
                                {
                                    WaitBeforeKillSeconds = 30
                                },
                                CancellationToken.None);
                        }
                    }
                    allContainersExited = true;
                }

            }
            
            return 0;
        }
    }

    public class ContainerProperties
    {
        public ContainerProperties(ContainerListResponse response)
        {
            ContainerName = response.Names.FirstOrDefault().Replace("/", "");
            Id = response.ID;
            ImageName = response.Image;
            string outvalue;
            _ = response.Labels.TryGetValue("com.docker.compose.project.config_files", out outvalue);
            ComposeFile = outvalue;
            if (response.State == "exited")
            {
                IsExecuting = false;
                var resultString = Regex.Match(response.Status, @"\d+").Value;
                ExitCode = int.Parse(resultString);
            }
            else
            {
                IsExecuting = true;
            }
            
        }

        public string ContainerName { get; set; }
        public string ImageName { get; set; }
        public string Id { get; set; }
        public int? ExitCode { get; set; }
        public bool IsExecuting { get; set; }
        public string ComposeFile { get; set; }
    }
}
