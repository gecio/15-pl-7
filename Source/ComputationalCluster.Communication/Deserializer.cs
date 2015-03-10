using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ComputationalCluster.Communication
{
    /// <summary>
    /// Klasa służąca do zamiany wiadomości w formacie XML na obiekty.
    /// </summary>
    public class Deserializer
    {
        public IMessage DeserializeMessage(string data)
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
                        }
                        break;
                    }
                }

                XmlSerializer serializer = new XmlSerializer(type);
                IMessage message = (IMessage)serializer.Deserialize(reader);

                return message;
            }
        }
    }
}
