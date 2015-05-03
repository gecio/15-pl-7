using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using log4net;
using System;
using System.Collections.Generic;

namespace ComputationalCluster.CommunicationServer
{
    public class CommunicationServerService
    {
        private readonly INetServer _server;
        private readonly IComponentsRepository _components;
        private readonly ILog _log;
        private readonly IConfigProvider _configProvider;

        public CommunicationServerService(INetServer server, IComponentsRepository components, ILog log,
            ITaskSolversRepository repository, IConfigProvider configProvider)
        {
            _server         = server;
            _components     = components;
            _log            = log;
            _configProvider = configProvider;
        }

        public void ApplyArguments(string[] arguments)
        {
            CommandLineParser parser = new CommandLineParser(new List<CommandLineOption>()
            {
                new CommandLineOption { ShortNotation = 'p', LongNotation = "port", ParameterRequired = true, },
                new CommandLineOption { ShortNotation = 'b', LongNotation = "backup", ParameterRequired = false, },
                new CommandLineOption { ShortNotation = 't', LongNotation = "t", ParameterRequired = true, },
            });

            parser.Parse(arguments);

            string value = null;
            if (parser.TryGet("port", out value))
                _configProvider.Port = Int32.Parse(value);

            _configProvider.BackupMode = parser.TryGet("backup", out value);

            if (parser.TryGet("t", out value))
                _configProvider.Timeout = Int32.Parse(value);
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
