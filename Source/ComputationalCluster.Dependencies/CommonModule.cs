using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Dependencies;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCTaskSolver;

namespace ComputationalCluster.Dependencies
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<LoggingModule>();

            builder.RegisterType<NetServer>().AsImplementedInterfaces();
            builder.RegisterType<NetClient>().AsImplementedInterfaces();

            builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
            builder.RegisterType<TimeProviderUtcNow>().As<ITimeProvider>();

            builder.RegisterType<PluginManager<TaskSolver>>().As<IPluginManager<TaskSolver>>();
            builder.RegisterType<TaskSolversRepository>().As<ITaskSolversRepository>().SingleInstance();

            builder.RegisterType<UTF8Encoding>().As<Encoding>();
        }
    }
}
