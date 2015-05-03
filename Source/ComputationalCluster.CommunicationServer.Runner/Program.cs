using Autofac;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ComputationalCluster.CommunicationServer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationServerModule>();
            var container = builder.Build();

            BasicConfigurator.Configure();

            var service = container.Resolve<CommunicationServerService>();
            service.ApplyArguments(args);
            service.Start();
        }
    }
}
