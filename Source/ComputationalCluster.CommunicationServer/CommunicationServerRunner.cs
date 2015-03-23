using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.CommunicationServer.Consumers;
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
