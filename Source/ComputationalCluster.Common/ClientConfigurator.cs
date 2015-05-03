using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    public class ClientConfigurator
    {
        private readonly IConfigProvider _configProvider;
        
        public ClientConfigurator(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public void Apply(string[] arguments)
        {
            var parser = new CommandLineParser(new List<CommandLineOption>()
            {
                new CommandLineOption { ShortNotation = 'a', LongNotation = "address", ParameterRequired = true },
                new CommandLineOption { ShortNotation = 'p', LongNotation = "port", ParameterRequired = true },
            });
            parser.Parse(arguments);

            string arg = null;
            if (parser.TryGet("address", out arg))
                _configProvider.IP = IPAddress.Parse(arg);
            if (parser.TryGet("port", out arg))
                _configProvider.Port = Int32.Parse(arg);
        }
    }
}
