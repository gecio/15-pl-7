using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.CommunicationServer.Queueing;

namespace ComputationalCluster.CommunicationServer.Models
{
    public abstract class QueueableTask : IQueueableTask
    {
        protected Component _assignedTo;

        public virtual DateTime RequestDate { get; set; }

        public virtual ProblemDefinition ProblemDefinition { get; set; }

        public virtual bool IsAwaiting { get; set; }


        public virtual Component AssignedTo
        {
            get { return _assignedTo; }
            set
            {
                if (_assignedTo != null)
                {
                    var tmp = _assignedTo;
                    _assignedTo = null;
                    tmp.RemoveTask(this);
                }
                _assignedTo = value;
                if (value != null)
                {
                    value.AddTask(this);
                }
            }
        }
    }
}
