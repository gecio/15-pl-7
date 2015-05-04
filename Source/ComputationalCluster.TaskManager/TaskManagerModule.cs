using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Communication;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.Dependencies;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskManager
{
    public class TaskManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonModule>();

            builder.RegisterType<NetServer>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<NetClient>().AsImplementedInterfaces().AsSelf();

            builder.RegisterType<UTF8Encoding>().As<Encoding>();
            builder.RegisterType<MessageReceiver>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageTranslator>().AsImplementedInterfaces().AsSelf();

            builder.RegisterType<ConfigProviderThreads>().As<IConfigProvider>().AsSelf().SingleInstance();

            builder.RegisterType<TaskSolversRepository>().As<ITaskSolversRepository>().SingleInstance();
        }
    }
}
