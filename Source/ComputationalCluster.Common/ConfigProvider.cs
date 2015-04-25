using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    public class ConfigProvider : IConfigProvider
    {
        private static readonly int DefaultPort = 16987;
        private static readonly string DefaultIP = "127.0.0.1";

        private int? _portCache = null;
        public int Port
        {
            get
            {
                if (_portCache == null)
                {
                    var value = ConfigurationManager.AppSettings["Port"];
                    if (value != null)
                    {
                        _portCache = Int32.Parse(value);
                    }
                }
                return _portCache ?? DefaultPort;
            }
            set
            {
                _portCache = value;
            }
        }

        private IPAddress _ipCache = null;
        public IPAddress IP
        {
            get
            {
                if (_ipCache == null)
                {
                    _ipCache = IPAddress.Parse(ConfigurationManager.AppSettings["IP"] ?? DefaultIP);
                }
                return _ipCache;
            }
            set
            {
                _ipCache = value;
            }
        }

        public bool BackupMode { get; set; }
        public int Timeout { get; set; }
    }
}
