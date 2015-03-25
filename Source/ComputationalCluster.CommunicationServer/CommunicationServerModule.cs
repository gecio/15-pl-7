using Autofac;
using ComputationalCluster.Communication;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Consumers;
using ComputationalCluster.CommunicationServer.Database;
using ComputationalCluster.CommunicationServer.Database.Entities;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.Dependencies;
using ComputationalCluster.NetModule;
using System.Data.Entity;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommunicationServerService>().AsSelf();

            builder.RegisterModule<CommonModule>();

            builder.RegisterType<RegisterConsumer>().As<IMessageConsumer<Register>>();
            builder.RegisterType<StatusConsumer>().As<IMessageConsumer<Status>>();
            builder.RegisterType<SolveRequestConsumer>().As<IMessageConsumer<SolveRequest>>();

            builder.RegisterType<ServerDbContext>().As<DbContext>().AsSelf().InstancePerDependency();
            builder.RegisterType<ProblemsRepository>().As<RepositoryBase<Problem>>().As<IRepository<Problem>>().AsSelf();

            builder.RegisterType<ComponentsInMemoryRepository>().As<IComponentsRepository>().SingleInstance();
            builder.RegisterType<ProblemDefinitionsInMemoryRepository>().As<IProblemDefinitionsRepository>().SingleInstance();

            builder.RegisterType<MessageReceiver>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageTranslator>().AsImplementedInterfaces().AsSelf();
        }
    }
}