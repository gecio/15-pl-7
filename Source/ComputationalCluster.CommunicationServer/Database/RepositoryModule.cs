using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Autofac;
using ComputationalCluster.CommunicationServer.Database.Entities;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServerDbContext>().As<DbContext>().AsSelf().InstancePerDependency();

            builder.RegisterType<ProblemsRepository>().As<RepositoryBase<Problem>>().As<IRepository<Problem>>().AsSelf();
        }
    }
}
