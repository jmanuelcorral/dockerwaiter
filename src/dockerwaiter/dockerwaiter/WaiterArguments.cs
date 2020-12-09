using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace dockerwaiter
{
    public class WaiterArguments
    {
        [Option('c', "containers", Separator =',' , Required = true, HelpText = "the container names that will be waited")]
        public IEnumerable<string> ContainersToFilter { get; set; }

        [Option('d', "dockercompose", Required = true, HelpText = "the docker-compose filename that launch the containers")]
        public string DockerCompose { get; set; }

        [Option('t', "timeout", Required = false, HelpText = "a timeout value in seconds for cleaning all the stuff if it will not work, by default it will be 3600")]
        public int Timeout { get; set; }
    }
}
