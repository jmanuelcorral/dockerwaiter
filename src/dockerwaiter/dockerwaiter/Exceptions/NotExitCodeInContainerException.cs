using System;
using System.Collections.Generic;
using System.Text;

namespace dockerwaiter.Exceptions
{
    public class NotExitCodeInContainerException : Exception
    {
        public NotExitCodeInContainerException(string containerName) : base($"The container {containerName} has exited without exit code.")
        {
        }
    }
}
