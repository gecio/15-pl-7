using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Queueing
{
    /// <summary>
    /// Interfejs kolejkowalnego zadania.
    /// </summary>
    public interface IQueueableTask
    {
        /// <summary>
        /// Data zlecenia zadania.
        /// </summary>
        DateTime RequestDate { get; }

        /// <summary>
        /// Rodzaj problemu, którego zadanie dotyczy.
        /// </summary>
        ProblemDefinition ProblemDefinition { get; }

        /// <summary>
        /// Czy zadanie oczekuje na przydzielenie zasobu.
        /// </summary>
        bool IsAwaiting { get; }

        /// <summary>
        /// komponent, który wykonuje zadanie
        /// </summary>
        Component AssignedTo { get; set; }
    }
}
