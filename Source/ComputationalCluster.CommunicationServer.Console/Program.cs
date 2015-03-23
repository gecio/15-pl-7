using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.CommunicationServer;

namespace ComputationalCluster.CommunicationServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var cs = new CommunicationServerRunner();
            cs.Start();
            while (true) ;
        }
    }
}
