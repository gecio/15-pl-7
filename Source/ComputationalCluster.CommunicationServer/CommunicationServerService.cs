using Autofac;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerService
    {
        private readonly INetServer _server;
        private readonly ILog _log;

        public CommunicationServerService(INetServer server, ILog log)
        {
            _server = server;
            _log    = log;
        }

        public void Start()
        {
            _log.Info("Starting...");

            _server.Start();

            _log.Info("Started.");
        }

        public void Stop()
        {
            _log.Info("Stopping...");

            _server.Stop();

            _log.Info("Stopped.");
        }
    }
}
