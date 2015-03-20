using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = ComputationalCluster.CommunicationServer.Database.Entities.Task;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class TasksRepository : RepositoryBase<Task>
    {
        public TasksRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
