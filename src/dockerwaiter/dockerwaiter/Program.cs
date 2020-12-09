using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using ConsoleTables;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace dockerwaiter
{
    class Program
    {
        static int Main(string[] args)
        {

            return Parser.Default.ParseArguments<WaiterArguments>(args).MapResult(
                (opts) => RunOptionsAndReturnExitCode(opts),
                 errs => HandleParseError(errs));
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;
            Console.WriteLine("errors {0}", errs.Count());
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;
            Console.WriteLine("Exit code {0}", result);
            return result;
        }

        static int RunOptionsAndReturnExitCode(WaiterArguments o)
        {
            var waiter = new Waiter();
            int timeout = (o.Timeout > 0) ? o.Timeout: 3600;
            var task = Task.Run(() => waiter.Execute(o));
            if (task.Wait(TimeSpan.FromSeconds(timeout)))
                return task.Result;
            else
                return -1; 
        }
    }
}
