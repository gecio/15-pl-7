using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Queueing
{
    /// <summary>
    /// Interfejs dla repozytorium kolejkowalnych zadań.
    /// </summary>
    public interface IQueuableTasksRepository<T> where T : class, IQueueableTask
    {
        ICollection<IQueueableTask> GetQueuableTasks();
        void DequeueTask(IQueueableTask task);
    }
}
