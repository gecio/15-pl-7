using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskManager.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new TaskManagerRunner(args);
            runner.Start();
        }
    }
}
