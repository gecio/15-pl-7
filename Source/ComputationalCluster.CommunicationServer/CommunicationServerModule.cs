using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.CommunicationServer.Consumers;
using ComputationalCluster.Communication;
using ComputationalCluster.NetModule;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.Communication.Messages;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommunicationServerModule>().As<Module>().AsSelf();

            builder.RegisterType<RegisterConsumer>().As<IMessageConsumer<Register>>();
            builder.RegisterType<StatusConsumer>().As<IMessageConsumer<Status>>();

            builder.RegisterType<NetServer>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<NetClient>().AsImplementedInterfaces().AsSelf();

            builder.RegisterType<ComponentsInMemoryRepository>().As<IComponentsRepository>()
                .SingleInstance();

            builder.RegisterType<UTF8Encoding>().As<Encoding>();
            builder.RegisterType<MessageReceiver>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageTranslator>().AsImplementedInterfaces().AsSelf();
        }
    }
}