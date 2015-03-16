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
            _encoding          = encoding;
            _configProvider = configProvider;
        }

        public IMessage Send(IMessage message)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(_configProvider.IP, _configProvider.Port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            //todo: buforowane wejście i wyjście
            var stream = client.GetStream();
            var request = _messageTranslator.Stringify(message);
            byte[] encodedRequest = _encoding.GetBytes(request);
            stream.WriteBuffered(encodedRequest, 0, encodedRequest.Length);

            byte[] encodedResponse = stream.ReadBuffered(0);
            var response = _encoding.GetString(encodedResponse, 0, encodedResponse.Length);
            client.Close();

            var responseMessage = _messageTranslator.CreateObject(response);
            return responseMessage;
        }

    }
}
