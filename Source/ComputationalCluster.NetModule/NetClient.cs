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
    public interface INetClient
    {
        IEnumerable<IMessage> Send_ManyResponses(IMessage message, IPAddress IP = null, int? port = null);
        IMessage Send(IMessage message);
    }

    public class NetClient : INetClient
    {
        private readonly IMessageTranslator _messageTranslator;
        private readonly Encoding _encoding;
        private readonly IConfigProvider _configProvider;

        public NetClient(IMessageTranslator translator, Encoding encoding, IConfigProvider configProvider)
        {
            _messageTranslator = translator;
            _encoding = encoding;
            _configProvider = configProvider;
        }

        public IEnumerable<IMessage> Send_ManyResponses(IMessage message, IPAddress IP = null, int? port = null)
        {
            IPEndPoint serverEndPoint;
            if (IP != null && port != null)
            {
                serverEndPoint = new IPEndPoint(IP, port.Value);
            }
            else
            {
                serverEndPoint = new IPEndPoint(_configProvider.IP, _configProvider.Port);
            }

            var client = new TcpClient();
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

        public IMessage Send(IMessage message)
        {
            return Send_ManyResponses(message).FirstOrDefault();
        }
    }
}
