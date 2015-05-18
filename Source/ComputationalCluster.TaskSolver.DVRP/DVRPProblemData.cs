using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP
{
    internal class Node
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        // NON-SERIALIZABLE ELEMENTS! (for dfs)
        public Stack<Node> NextOnPath { get; set; }
        public bool Visited { get; set; }

        public Node()
        {
            NextOnPath = new Stack<Node>();
        }
    }

    internal class Depot : Node
    {
        public float Starts { get; set; }
        public float Ends { get; set; }

        public Depot() : base()
        {
        }
    }

    internal class Pickup : Node
    {
        public float AvailableAfter { get; set; }
        public float UnloadTime { get; set; }
        public float Size { get; set; }

        public Pickup() : base()
        {
        }
    }

    [Serializable]
    public class DVRPRange
    {
        public int[] Start { get; set; }
        public int[] End { get; set; }

        public DVRPRange()
        {
        }
    }

    internal class DVRPCommonData
    {
        public IList<Depot> Depots { get; set; }
        public IList<Pickup> Pickups { get; set; }
        public float VehicleSpeed { get; set; }
        public float VehicleCapacity { get; set; }
        public int NumVehicles { get; set; }

        public DVRPCommonData()
        {
        }
    }
}
