using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Interfejs obsługi wiadomości odebranych przez inne moduły.
    /// </summary>
    public interface IMessagesReceiver
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
    public class MessagesReceiver : IMessagesReceiver
    {
        private readonly IMessageTranslator _messageTranslator;
        private IContainer _messageConsumersResolver;

        public MessagesReceiver(IMessageTranslator messageTranslator, Module messageConsumersModule)
        {
            _messageTranslator = messageTranslator;

            var builder = new ContainerBuilder();
            builder.RegisterModule(messageConsumersModule);
            _messageConsumersResolver = builder.Build();
        }

        public string Dispatch(string message)
        {
            var messageObject = _messageTranslator.CreateObject(message);
            if (messageObject == null)
                throw new Exception("Created message cannot be null.");

            var consumerType = typeof(IMessageConsumer<>).MakeGenericType(new[] { messageObject.GetType() });
            
            using (var scope = _messageConsumersResolver.BeginLifetimeScope())
            {
                var consumer = (IMessageConsumer)scope.Resolve(consumerType);
                var response = consumer.Consume(messageObject);
                var responseString = _messageTranslator.Stringify(response);

                return responseString;
            }
        }

    }
}
