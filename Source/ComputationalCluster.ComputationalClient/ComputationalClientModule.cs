using System.ComponentModel;
using System.Text;
using Autofac;
using ComputationalCluster.Communication;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.ComputationalClient
{
    public class ComputationalClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NetClient>().AsImplementedInterfaces().AsSelf();

            builder.RegisterType<UTF8Encoding>().As<Encoding>();
            builder.RegisterType<MessageReceiver>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageTranslator>().AsImplementedInterfaces().AsSelf();
        }
    }
}