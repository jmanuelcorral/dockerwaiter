using System.Linq;
using System.Text.RegularExpressions;
using Docker.DotNet.Models;

namespace dockerwaiter
{
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
