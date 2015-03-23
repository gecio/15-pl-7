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
                Id = 666,
                IdSpecified = true,
            }) as RegisterResponse;

            Console.WriteLine("Response {0}", response.Id);

            while(true)
            {
                System.Threading.Thread.Sleep(2000);
                _client.Send(new Status()
                {
                    Id = 666,
                });
            }
        }

        public void Stop()
        {
        }
    }
}
