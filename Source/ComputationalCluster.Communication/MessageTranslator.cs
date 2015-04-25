using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ComputationalCluster.NetModule;
using ComputationalCluster.Communication.Messages;

namespace ComputationalCluster.Communication
{
    /// <summary>
    /// Klasa służąca do zamiany wiadomości w formacie XML na obiekty.
    /// </summary>
    public class MessageTranslator : IMessageTranslator
    {
        public IMessage CreateObject(string message)
        {
            return DeserializeMessage(message);
        }

        public string Stringify(IMessage message)
        {
            return SerializeMessage(message);
        }

        private IMessage DeserializeMessage(string data)
        {
            StringReader strReader = new StringReader(data);
            Type type = null;
            using (XmlReader reader = XmlReader.Create(strReader))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "DivideProblem":
                                type = typeof(DivideProblem);
                                break;
                            case "NoOperation":
                                type = typeof(NoOperation);
                                break;
                            case "SolvePartialProblems":
                                type = typeof(SolvePartialProblems);
                                break;
                            case "Register":
                                type = typeof(Register);
                                break;
                            case "RegisterResponse":
                                type = typeof(RegisterResponse);
                                break;
                            case "Solutions":
                                type = typeof(Solutions);
                                break;
                            case "SolutionRequest":
                                type = typeof(SolutionRequest);
                                break;
                            case "SolveRequest":
                                type = typeof(SolveRequest);
                                break;
                            case "SolveRequestResponse":
                                type = typeof(SolveRequestResponse);
                                break;
                            case "Status":
                                type = typeof(Status);
                                break;
                            case "Error":
                                type = typeof(Error);
                                break;
                        }
                        break;
                    }
                }

                XmlSerializer serializer = new XmlSerializer(type);
                IMessage message = (IMessage)serializer.Deserialize(reader);

                return message;
            }
        }

        private string SerializeMessage(IMessage message)
        {
            string result = null;
            using(StringWriter sw = new Utf8StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(message.GetType());
                serializer.Serialize(sw,message);
                result = sw.ToString();
            }
            return result;
        }

        /// <summary>
        /// stringWriter w UTF-8
        /// </summary>
        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

    }
}
