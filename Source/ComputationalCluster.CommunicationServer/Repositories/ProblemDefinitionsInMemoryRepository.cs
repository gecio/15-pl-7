using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ProblemDefinitionsInMemoryRepository : IProblemDefinitionsRepository
    {
        private int _nextValidId = 1;
        private List<ProblemDefinition> _problemDefinitions;

        public ProblemDefinitionsInMemoryRepository()
        {
            _problemDefinitions = new List<ProblemDefinition>();
        }

        public int Add(ProblemDefinition problemDefinition)
        {
            problemDefinition.Id = _nextValidId;

            _problemDefinitions.Add(problemDefinition);

            return problemDefinition.Id;
        }

        public ProblemDefinition FindByName(string name)
        {
            return _problemDefinitions.FirstOrDefault(item => item.Name == name);
        }
    }
}
