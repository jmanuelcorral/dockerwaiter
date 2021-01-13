using dockerwaiter.unittests.Fakes;
using System;
using Xunit;

namespace dockerwaiter.unittests
{
    public class WaiterTests
    {
        [Fact]
        public async Task TestMonitorContainers()
        {
            var arguments = new WaiterArguments() { ContainersToFilter = ["container1", "container2"], DockerCompose = "docker-compose.yml", LogPath = "", Timeout = 3600 };

            var containerHelper = FakeContainerHelper.Setup(//Pasarle la coleccion de contenedores, 
                                                            //accion a ejecutar dentro del fakeContainer 
                                                            // tal vez un exitContainer("nombre", 0, millisegundos cuando pasará )
                                                            );
                )
            var waiter = new Waiter(arguments, containerHelper);
            var result = await waiter.Execute();
            Assert.True(result == 0);

            
        }
    }
}
