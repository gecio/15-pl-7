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

            // log4net configuration - log on console
            BasicConfigurator.Configure();

            // topshelf
            HostFactory.Run(x =>
            {
                x.UseLog4Net();

                x.Service<CommunicationServerService>(s =>
                {
                    s.ConstructUsing(name =>
                    {
                        var service = container.Resolve<CommunicationServerService>();
                        service.ApplyArguments(args);
                        return service;
                    });
                    s.WhenStarted(cs => cs.Start());
                    s.WhenStopped(cs => cs.Stop());
                });

                x.RunAsLocalService();

                x.SetDescription("Group 7.");
                x.SetDisplayName("ComputationalCluster Communication Server");
                x.SetServiceName("CommunicationServer");
            });
        }
    }
}
