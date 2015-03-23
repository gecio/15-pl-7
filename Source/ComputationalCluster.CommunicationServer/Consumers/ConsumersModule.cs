using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.NetModule;
using ComputationalCluster.Communication.Messages;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class ConsumersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegisterConsumer>().As<IMessageConsumer<Register>>();
            builder.RegisterType<StatusConsumer>().As<IMessageConsumer<Status>>();
            builder.RegisterType<SolveRequestConsumer>().As<IMessageConsumer<SolveRequest>>();
        }
    }
}
