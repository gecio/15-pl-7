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
            SolveRequestResponse response = _client.Send(solverRequest) as SolveRequestResponse;
            return response.Id;
        }

        public string SendSolutionRequest(int problemId)
        {
            var solutionRequest = new SolutionRequest
            {
                Id = (ulong)problemId
            };
            Solutions response = _client.Send(solutionRequest) as Solutions;
            string result =null;
            if(response.Solutions1.Length > 0 && response.Solutions1[0].Type == SolutionsSolutionType.Final)
            {
                result = _encoding.GetString( Convert.FromBase64String(response.Solutions1[0].Data));
            }
            return result;
        }
    }
}
