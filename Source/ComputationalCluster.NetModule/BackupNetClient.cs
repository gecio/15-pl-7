using ComputationalCluster.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Wersja NetClienta dla serwera backupowego. Różni się od zwykłego klienta tym, że możemy
    /// wyslać wiadomości z określonego portu (wynika z dokumentacji i sposobu informowania
    /// na jakim porcie backup nasłuchuje).
    /// </summary>
    public class BackupNetClient
    {
        private readonly IMessageTranslator _messageTranslator;
        private readonly Encoding _encoding;
        private readonly IConfigProviderBackup _configProvider;

        public BackupNetClient(IMessageTranslator translator, Encoding encoding, IConfigProviderBackup configProvider)
        {
            _messageTranslator = translator;
            _encoding = encoding;
            _configProvider = configProvider;
        }

        public IEnumerable<IMessage> Send(IMessage message, int port = 0)
        {
            var serverEndPoint = new IPEndPoint(_configProvider.MasterIP, _configProvider.MasterPort);
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            var client = new TcpClient(localEndPoint);
            client.Connect(serverEndPoint);
            var stream = client.GetStream();
            var request = _messageTranslator.Stringify(message);
            byte[] encodedRequest = _encoding.GetBytes(request);
            stream.WriteBuffered(encodedRequest, 0, encodedRequest.Length);
            client.Client.Shutdown(SocketShutdown.Send);
            byte[] encodedResponse = stream.ReadBuffered(0);
            var response = _encoding.GetString(encodedResponse, 0, encodedResponse.Length);
            client.Close();
            return response.Split(NetServer.ETB).Select(msg => _messageTranslator.CreateObject(msg));
        }

    }
}
