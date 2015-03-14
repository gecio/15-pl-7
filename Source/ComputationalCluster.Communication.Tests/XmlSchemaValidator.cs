using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ComputationalCluster.Communication.Tests
{
    public class XmlSchemaValidator
    {
        private XmlSchema _schema;
        private bool _isValid;
        private XmlReaderSettings _settings;

        public XmlSchemaValidator(string schemaFileLocation)
        {
            using (var fs = File.OpenRead(schemaFileLocation))
            {
                _schema = XmlSchema.Read(fs, ValidationEventHandler);
            }
            _settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            _settings.ValidationEventHandler += ValidationEventHandler;
            _settings.Schemas.Add(_schema);
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs arg)
        {
            _isValid = false;
        }

        public bool IsValid(string xml)
        {
            _isValid = true;

            using (var xmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
            {
                var xmlFile = XmlReader.Create(xmlStream, _settings);
                try
                {
                    while (xmlFile.Read()) { }
                }
                catch (XmlException xex)
                {
                    return false;
                }
            }
            return _isValid;
        }
    }
}
