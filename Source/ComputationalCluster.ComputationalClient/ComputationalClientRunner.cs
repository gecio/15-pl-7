using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.ComputationalClient
{
    public class ComputationalClientRunner
    {
        private INetClient _client;
        public ComputationalClientRunner()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalClientModule>();
            var container = builder.Build();

            _client = container.Resolve<INetClient>();
        }

        //todo: to refactor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">argumenty wejściwe dla Task Solvera jako string</param>
        /// <param name="problemType">Nazwa typu problemu (z Task Solvera)</param>
        /// <param name="duration">timeout dla rozwiązywania problemu</param>
        public void SendSolveRequest(string data, string problemType, ulong? duration = null)
        {
            var solverRequest = new SolveRequest();
            solverRequest.Data = Convert.FromBase64String(data);
            solverRequest.ProblemType = problemType;
            if (duration != null)
            {
                solverRequest.SolvingTimeout = duration.Value;
                solverRequest.SolvingTimeoutSpecified = true;
            }
            _client.Send(solverRequest);
        }
    }
}
