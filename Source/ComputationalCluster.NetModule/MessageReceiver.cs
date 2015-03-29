using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using log4net;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Interfejs obsługi wiadomości odebranych przez inne moduły.
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// Metoda która obsługuje wiadomości przychodzące, może wygenerować odpowiedź.
        /// </summary>
        /// <param name="message">Wiadomość przychodząca.</param>
        /// <returns>Opcjonalna odpowiedź dla wysyłającego.</returns>
        string Dispatch(string message);
    }

    /// <summary>
    /// Domyślna implementacja obsługująca wiadomości przychodzące, obsługująca wiadomości
    /// za pomocą wczesniej zbudowanego zbioru konsumentów.
    /// </summary>
    public class MessageReceiver : IMessageReceiver
    {
        private readonly IMessageTranslator _messageTranslator;
        private readonly IComponentContext _componentContext;
        private readonly ILog _log;

        public MessageReceiver(IMessageTranslator messageTranslator, IComponentContext componentContext,
            ILog log)
        {
            _messageTranslator = messageTranslator;
            _componentContext  = componentContext;
            _log               = log;

            //var builder = new ContainerBuilder();
            //builder.RegisterModule(messageConsumersModule);
            //_messageConsumersResolver = builder.Build();
        }

        
        public string Dispatch(string message)
        {
            var splitMessages = message.Split(NetServer.ETB);
            var results = new List<string>();

            foreach (var msg in splitMessages)
            {
                var messageObject = _messageTranslator.CreateObject(msg);
                if (messageObject == null)
                    throw new Exception("Created message cannot be null.");

                var consumerType = typeof(IMessageConsumer<>).MakeGenericType(new[] { messageObject.GetType() });
                var consumer = (IMessageConsumer)_componentContext.Resolve(consumerType);
                var responses = consumer.Consume(messageObject);

                foreach (var response in responses)
                {
                    _log.InfoFormat("Response received. Type={0} Contents=[{1}]",
                        response.GetType().Name, response.ToString());

                    results.Add(_messageTranslator.Stringify(response));
                }
            }

            return String.Join(NetServer.ETB.ToString(), results.Where(msg => msg != null));  
        }

    }
}
