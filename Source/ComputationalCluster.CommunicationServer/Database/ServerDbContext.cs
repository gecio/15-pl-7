using ComputationalCluster.CommunicationServer.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ComputationalCluster.CommunicationServer.Database
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext()
            : base("Name=CServer") // todo: set conection string name
        {
           
            System.Data.Entity.Database.SetInitializer<ServerDbContext>(new CreateDatabaseIfNotExists<ServerDbContext>());
        }

        public DbSet<Problem> Problems { get; set; }
    }
}