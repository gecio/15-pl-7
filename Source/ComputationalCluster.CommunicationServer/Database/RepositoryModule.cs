using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServerDbContext>().As<DbContext>().AsSelf().InstancePerDependency();

            builder.RegisterType<RepositoryBase<Task>>().AsSelf().InstancePerDependency();
           // builder.RegisterType<TasksRepository>().As<RepositoryBase<Task>>().AsSelf().InstancePerDependency();
           // builder.RegisterGeneric(typeof (RepositoryBase<>)).As(typeof (IRepository<>));
        }
    }
}
