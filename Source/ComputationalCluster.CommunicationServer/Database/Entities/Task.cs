﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Database.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UniqueId { get; set; }
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string ProblemType { get; set; }
        public ulong Timeout { get; set; }
    }
}
