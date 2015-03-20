using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext() : base() // todo: set conection string name
        {

        }

        public DbSet<Task> Tasks { get; set; }
    }
}