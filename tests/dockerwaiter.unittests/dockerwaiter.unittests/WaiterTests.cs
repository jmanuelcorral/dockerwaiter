using dockerwaiter.unittests.Fakes;
using System;
using Xunit;

namespace dockerwaiter.unittests
{
    public class WaiterTests
    {
        [Fact]
        public void TestMonitorContainers()
        {
            var arguments = new WaiterArguments() { ContainersToFilter = ["container1", "container2"], DockerCompose = "docker-compose.yml", LogPath = "", Timeout = 3600 };

            var waiter = new Waiter(arguments, FakeContainerHelper.Setup(x=> {
                this.
            }));
            Task. waiter.Execute();

        }
    }
}
