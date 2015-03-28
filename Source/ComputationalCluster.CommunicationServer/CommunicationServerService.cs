using Autofac;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;
using System;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerService
    {
        private readonly INetServer _server;
        private readonly IComponentsRepository _components;
        private readonly ILog _log;

        public CommunicationServerService(INetServer server, IComponentsRepository components, ILog log,
            ITaskSolversRepository repository)
        {
            _server     = server;
            _components = components;
            _log        = log;
        }

        public void Start()
        {
            _log.Info("Starting...");

            _server.Start();

            _log.Info("Started.");

            while (true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                _components.RemoveInactive();
            }
        }

        public void Stop()
        {
            _log.Info("Stopping...");

            _server.Stop();

            _log.Info("Stopped.");
        }
    }
}
