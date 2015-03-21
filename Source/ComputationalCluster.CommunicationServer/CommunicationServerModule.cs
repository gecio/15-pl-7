using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.CommunicationServer.Consumers;
using ComputationalCluster.Communication;
using ComputationalCluster.CommunicationServer.Database;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConsumersModule>().As<Module>().AsSelf();
            builder.RegisterType<RepositoryModule>().As<Module>().AsSelf();

            builder.RegisterType<NetServer>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<NetClient>().AsImplementedInterfaces().AsSelf();

            builder.RegisterType<UTF8Encoding>().As<Encoding>();
            builder.RegisterType<MessageReceiver>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageTranslator>().AsImplementedInterfaces().AsSelf();
        }
    }
}