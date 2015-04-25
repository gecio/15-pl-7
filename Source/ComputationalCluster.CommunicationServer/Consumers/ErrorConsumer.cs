using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class ErrorConsumer : IMessageConsumer<Error>
    {
        private readonly ILog _log;

        public ErrorConsumer(ILog log)
        {
            _log = log;
        }

        public ICollection<IMessage> Consume(Error message)
        {
            _log.Error("Error: type="+message.ErrorType+", message="+message.ErrorMessage);
            return new IMessage[] { new NoOperation() };
        }

        public ICollection<IMessage> Consume(IMessage message)
        {
            var status = message as Error;
            if (status == null)
            {
                throw new NotSupportedException("ErrorConsumer consumes Error messages only.\n");
            }

            return Consume(status);
        }
    }
}
