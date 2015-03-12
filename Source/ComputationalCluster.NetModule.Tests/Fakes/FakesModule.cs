using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ComputationalCluster.NetModule.Tests.Fakes
{
    public class FakesModule : Module
    {
        private readonly IMessageConsumer _testTextMessageConsumer;

        public FakesModule(IMessageConsumer<TestTextMessage> testTextMessageConsumer)
        {
            _testTextMessageConsumer = testTextMessageConsumer;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IMessageConsumer>(_testTextMessageConsumer)
                .As<IMessageConsumer<TestTextMessage>>();
        }
    }
}
