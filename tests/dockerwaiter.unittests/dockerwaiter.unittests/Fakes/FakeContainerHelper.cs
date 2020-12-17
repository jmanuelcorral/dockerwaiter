using dockerwaiter.Containers;
using dockerwaiter.unittests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dockerwaiter.unittests.Fakes
{
    public class FakeContainerHelper : IContainerHelper
    {

        private List<ContainerProperties> _containerData;
        

        private FakeContainerHelper()
        {
            _containerData =  new List<ContainerProperties>();
            _containerData.Add(new ContainerProperties(){ ContainerName = "container1", ImageName = "image1", ExitCode = 0, Id = "myContainerId1", IsExecuting = false, ComposeFile = "docker-compose.yml" });
            _containerData.Add(new ContainerProperties() { ContainerName = "container2", ImageName = "image2", ExitCode = 0, Id = "myContainerId2", IsExecuting = false, ComposeFile = "docker-compose.yml" });
            _containerData.Add(new ContainerProperties() { ContainerName = "container3", ImageName = "image3", ExitCode = 0, Id = "myContainerId3", IsExecuting = false, ComposeFile = "docker-compose.yml" });
            _containerData.Add(new ContainerProperties() { ContainerName = "container4", ImageName = "image4", ExitCode = 0, Id = "myContainerId4", IsExecuting = false, ComposeFile = "docker-compose.yml" });
            _containerData.Add(new ContainerProperties() { ContainerName = "container5", ImageName = "image5", ExitCode = 0, Id = "myContainerId5", IsExecuting = false, ComposeFile = "docker-compose.yml" });
        
        }

        

        public static FakeContainerHelper Setup(/*colleccion, accionaejecutar, milisegundos*/)
        {
            return new FakeContainerHelper();
        }

        public Task<IEnumerable<ContainerProperties>> GetContainersInDockerCompose(string dockerComposeFilename)
        {
            return _containerData.Where(x => x.ComposeFile == dockerComposeFilename).BoxInsideTask();
        }

        public Task<IEnumerable<ContainerProperties>> GetContainersInDockerComposeFilteredByNames(string dockerComposeFilename, params string[] containerlist)
        {
            return _containerData.Where(x => x.ComposeFile == dockerComposeFilename && containerlist.Contains(x.ContainerName)).BoxInsideTask();
        }

        public Task StopAllContainersInDockerCompose(string dockerComposeFilename)
        {
            return Task.CompletedTask;
        }
    }
}
