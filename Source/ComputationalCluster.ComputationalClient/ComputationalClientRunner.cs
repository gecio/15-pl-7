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
        private Encoding _encoding;

        public ComputationalClientRunner()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalClientModule>();
            var container = builder.Build();

            _client = container.Resolve<INetClient>();
            _encoding = container.Resolve<Encoding>();
        }

        //todo: to refactor

       /// <summary>
       /// Metoda opakowuje proces przygotowania i wysłania danych do Serwera
       /// </summary>
       /// <param name="data">dane wejściowe</param>
       /// <param name="problemType">typ problemu z TaskSolvera</param>
       /// <param name="duration">maksymalny czas trawania obliczeń</param>
       /// <returns>identyfikator zadania przypisany przez serwer</returns>
        public ulong SendSolveRequest(string data, string problemType, ulong? duration = null)
        {
            var solverRequest = new SolveRequest();
            solverRequest.Data = Convert.ToBase64String(_encoding.GetBytes(data));
            solverRequest.ProblemType = problemType;
            if (duration != null)
            {
                solverRequest.SolvingTimeout = duration.Value;
                solverRequest.SolvingTimeoutSpecified = true;
            }
            var responses = _client.Send_ManyResponses(solverRequest);
            ulong id=0;

            foreach (var response in responses)
            {
                if (response.GetType() == typeof(SolveRequestResponse))
                    id = (response as SolveRequestResponse).Id;
                if (response.GetType() == typeof(NoOperation))
                {
                    //TODO: zapisanie informacji o backup'ie
                }
            }
            return id;
        }

        public IMessage SendSolutionRequest(int problemId)
        {
            var solutionRequest = new SolutionRequest
            {
                Id = (ulong)problemId
            };
            return _client.Send(solutionRequest);
        }
    }
}
