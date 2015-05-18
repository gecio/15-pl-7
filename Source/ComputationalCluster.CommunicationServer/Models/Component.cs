using ComputationalCluster.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComputationalCluster.CommunicationServer.Queueing;

namespace ComputationalCluster.CommunicationServer.Models
{
    public class Component
    {
        public Component()
        {
            _runningTasks = new List<QueueableTask>();
        }

        public ulong Id { get; set; }
        public DateTime LastStatusTimestamp { get; set; }
        public RegisterType Type { get; set; }
        public int MaxThreads { get; set; }
        public ICollection<ProblemDefinition> SolvableProblems { get; set; }
        //---------------------------------------------------------------------
        private List<QueueableTask> _runningTasks;

        public virtual void AddTask(QueueableTask task)
        {
            _runningTasks.Add(task);
            if (task.AssignedTo == null)
            {
                task.AssignedTo = this;
            }
        }

        public virtual void RemoveTask(QueueableTask task)
        {
            _runningTasks.Remove(task);
            if (task.AssignedTo != null)
            {
                task.AssignedTo = null;
            }
        }

        public void ClearAndRepeatTasks()
        {
            QueueableTask[] tmp =new QueueableTask[_runningTasks.Count];
            _runningTasks.CopyTo(tmp);
            foreach (var queueableTask in tmp)
            {
                queueableTask.IsAwaiting = true;
                queueableTask.AssignedTo = null;
            }
        }

    }
}
