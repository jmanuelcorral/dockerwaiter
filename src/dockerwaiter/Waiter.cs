using ConsoleTables;
using dockerwaiter.Containers;
using dockerwaiter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dockerwaiter
{
    public class Waiter
    {
        private ContainerHelper _containerHelper;
        public Waiter()
        {
        }

        public async Task<int> Execute(WaiterArguments arguments)
        {
            _containerHelper = new ContainerHelper(arguments.LogPath);
            List<string> containersBeingWatched = arguments.ContainersToFilter.ToList();
            bool allContainersExited = false;
            DisplayArguments(arguments);
            var containersBeforeLoop = await _containerHelper.GetContainersInDockerComposeFilteredByNames(arguments.DockerCompose, arguments.ContainersToFilter.ToArray());
            DisplayInConsole("This Containers Will be awaited", containersBeforeLoop);
            try
            {
                while (!allContainersExited)
                {
                    var containers = await _containerHelper.GetContainersInDockerComposeFilteredByNames(arguments.DockerCompose, containersBeingWatched.ToArray());
                    if (containers.Count() > 0)
                    {
                        foreach (var container in containers)
                        {
                            var exitcode = RemoveContainerFromBeingWatchedIfAreExited(container, containersBeingWatched);
                            if (exitcode != 0)
                            {
                                await _containerHelper.StopAllContainersInDockerCompose(arguments.DockerCompose);
                                return exitcode;
                            }
                        }
                    }
                    else
                    {
                        var containersWillBeRemoved = await _containerHelper.GetContainersInDockerCompose(arguments.DockerCompose);
                        await _containerHelper.StopAllContainersInDockerCompose(arguments.DockerCompose);
                        allContainersExited = true;
                    }

                }
                var containersAfterLoop = await _containerHelper.GetContainersInDockerComposeFilteredByNames(arguments.DockerCompose, arguments.ContainersToFilter.ToArray());
                
                DisplayInConsole("This Containers Will be awaited", containersBeforeLoop);
                
                return 0;
            } catch(Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
                await _containerHelper.StopAllContainersInDockerCompose(arguments.DockerCompose);
                return -1;
            }
        }



        private int RemoveContainerFromBeingWatchedIfAreExited(ContainerProperties container, List<string> containersBeingWatched)
        {
                if (!container.IsExecuting && container.ExitCode.HasValue)
                {
                        containersBeingWatched.Remove(container.ContainerName);
                    if (!container.ExitCode.HasValue)
                        throw new NotExitCodeInContainerException(container.ContainerName);
                    return container.ExitCode.Value;
                }
            return 0;
        }

        private void DisplayArguments(WaiterArguments arguments)
        {
            Console.WriteLine($"Monitoring the containers: { string.Join(',', arguments.ContainersToFilter.ToArray())}");
            Console.WriteLine($"docker-compose file: {arguments.DockerCompose}");
        }

        private void DisplayInConsole(string title, IEnumerable<ContainerProperties> containers)
        {
            Console.WriteLine(title);
            ConsoleTable
            .From<ContainerProperties>(containers)
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.Alternative);
            Thread.Sleep(1000);
        }
    }
}
