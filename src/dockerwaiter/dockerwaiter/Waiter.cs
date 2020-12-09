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
        private readonly ContainerHelper _containerHelper;
        public Waiter()
        {
            _containerHelper = new ContainerHelper();
        }

        public async Task<int> Execute(WaiterArguments arguments)
        {
            List<string> containersBeingWatched = arguments.ContainersToFilter.ToList();
            bool allContainersExited = false;
            try
            {
                while (!allContainersExited)
                {
                    var containers = await _containerHelper.GetContainersInDockerComposeFilteredByNames(arguments.DockerCompose, containersBeingWatched.ToArray());
                    if (containers.Count() > 0)
                    {
                        DisplayInConsole("These containers will be awaited", containers);
                        foreach (var container in containers)
                        {
                            RemoveContainerFromBeingWatchedIfAreExited(container, containersBeingWatched);
                        }
                    }
                    else
                    {
                        var containersWillBeRemoved = await _containerHelper.GetContainersInDockerCompose(arguments.DockerCompose);
                        DisplayInConsole("These containers will be stopped", containersWillBeRemoved);
                        await _containerHelper.StopAllContainersInDockerCompose(arguments.DockerCompose);
                        allContainersExited = true;
                    }

                }
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


        private void DisplayInConsole(string title, IEnumerable<ContainerProperties> containers)
        {
            Console.Clear();
            Console.WriteLine(title);
            ConsoleTable
            .From<ContainerProperties>(containers)
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.Alternative);
            Thread.Sleep(1000);
        }
    }
}
