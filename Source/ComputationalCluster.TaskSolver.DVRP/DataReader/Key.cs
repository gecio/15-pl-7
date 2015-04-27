using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP.DataReader
{
    public class Key
    {
        public string Name;
        public bool Required;
        public DataType DataType;
        public object DefaultValue;
        public DataFormat DataFormat;

        public Key(string name, bool required, DataType dataType, DataFormat dataFormat, object defaultValue = null)
        {
            Name = name;
            Required = required;
            DataType = dataType;
            DefaultValue = defaultValue;
            DataFormat = dataFormat;
        }
    }
}
