using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.ComputationalNode.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new ComputationalNodeRunner();
            runner.Start();
            runner.Stop();
        }
    }
}
