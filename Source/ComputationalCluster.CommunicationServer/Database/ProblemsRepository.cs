using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.CommunicationServer.Database.Entities;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class ProblemsRepository : RepositoryBase<Problem>
    {
        public ProblemsRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
