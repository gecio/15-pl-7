using System.Collections.Generic;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Ogólny konsument wiadomości.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Konsumuje wiadomość.
        /// </summary>
        /// <param name="message">Wiadomość typu ogólnego IMessage</param>
        /// <param name="connectionInfo">Informacje o przysyłającym wiadomość</param>
        /// <returns></returns>
        ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null);
    }

    /// <summary>
    /// Typowany konsument wiadomości.
    /// </summary>
    /// <typeparam name="TMessage">Typ przyjmowanej wiadomości.</typeparam>
    public interface IMessageConsumer<TMessage> : IMessageConsumer
        where TMessage : IMessage
    {
        /// <summary>
        /// Konsumuje wiadomość danego typu.
        /// </summary>
        /// <param name="message">Wiadomość typu TMessage</param>
        /// <param name="connectionInfo">Informacje o przysyłającym wiadomość</param>
        /// <returns></returns>
        ICollection<IMessage> Consume(TMessage message, ConnectionInfo connectionInfo = null);
    }
}