using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup
{
    public class BackupClient
    {
        private readonly INetClient _client;
        private readonly ILog _log;
        private readonly IConfigProviderBackup _configProvider;
        private readonly IComponentContext _componentContext;
        private ulong _id;


        public ulong Id
        {
            get { return _id; }
        }

        public BackupClient(INetClient client,IConfigProviderBackup configProviderBackup,  ILog log, IComponentContext componentContext)
        {
            _client = client;
            _log = log;
            _configProvider = configProviderBackup;
            _componentContext = componentContext;
        }

        public void Start()
        {
            SendRegisterMessage();

            while (true)
            {
                var response = _client.Send_ManyResponses(new Status(),_configProvider.MasterIP, _configProvider.MasterPort);
                foreach (var message in response)
                {
                    var consumerType = typeof(IMessageConsumer<>).MakeGenericType(new[] { message.GetType() });
                    var consumer = (IMessageConsumer)_componentContext.Resolve(consumerType);
                    consumer.Consume(message); 
                }
            }
        }


        

        /// <summary>
        /// Rejestracja jako BackupServer. 
        /// Konfiguracja zostaje zmieniona na konfiguracje serwera, z którego będzimy odczytywać dane.
        /// </summary>
        private void SendRegisterMessage()
        {
            var registerMessage = new Register
            {
                Deregister = false,
                IdSpecified = false,
                ParallelThreads = 1,
                Type = RegisterType.CommunicationServer
            };

            bool isRegistered = false;
            IPAddress address = _configProvider.MasterIP;
            int port = _configProvider.MasterPort;

            while (!isRegistered)
            {
                var response = _client.Send_ManyResponses(registerMessage, address, port);
                var registerResponse = response.ElementAt(0) as RegisterResponse;
                if (registerResponse.BackupCommunicationServers != null &&
                    registerResponse.BackupCommunicationServers.BackupCommunicationServer != null)
                {
                    var backupServer = registerResponse.BackupCommunicationServers.BackupCommunicationServer;
                    address = IPAddress.Parse(backupServer.address);
                    port = backupServer.port;
                }
                else
                {
                    _id = registerResponse.Id;
                    _configProvider.MasterIP = address;
                    _configProvider.MasterPort = port;
                    isRegistered = true;
                }
            }
            _log.InfoFormat("BackupServer registered. ID={0}, MasterIP={1}, MasterPort={2}",_id,_configProvider.MasterIP, _configProvider.MasterIP);
        }
    }
}
