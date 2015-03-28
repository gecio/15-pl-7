using Autofac;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.ComputationalNode
{
    public class ComputationalNodeRunner
    {
        private INetClient _client;

        public ComputationalNodeRunner()
        {
        }

        public void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalNodeModule>();
            var container = builder.Build();

            _client = container.Resolve<INetClient>();
            var response = _client.Send(new Register()
            {
                Type = RegisterType.ComputationalNode,
            }) as RegisterResponse;

            Console.WriteLine("Response {0}", response.Id);

            while(true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, (int)(response.Timeout/2)));
                _client.Send(new Status()
                {
                    Id = response.Id,
                });
            }
        }

        public void Stop()
        {
        }
    }
}
