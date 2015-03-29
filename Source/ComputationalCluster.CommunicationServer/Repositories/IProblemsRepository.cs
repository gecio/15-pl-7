﻿using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface IProblemsRepository
    {
        ulong Add(Problem problem);
        Problem FindById(int id);
    }
}
