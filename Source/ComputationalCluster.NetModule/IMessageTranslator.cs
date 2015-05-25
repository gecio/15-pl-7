using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Interfejs tłumacza wiadomości, który na podstawie tekstu buduje obiekt i na odwrót.
    /// </summary>
    public interface IMessageTranslator
    {
        IMessage CreateObject(string message);
        string Stringify(IMessage message);
    }
}
