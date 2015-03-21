using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Autofac;
using ComputationalCluster.CommunicationServer.Consumers;
using ComputationalCluster.CommunicationServer.Database;
using ComputationalCluster.CommunicationServer.Database.Entities;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerRunner
    {
        private INetServer _server;

        public void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationServerModule>();
            var container = builder.Build();

            _server = container.Resolve<INetServer>();
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}
