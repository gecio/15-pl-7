using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP.DataReader
{
    public class Key
    {
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public DataType DataType { get; private set; }
        public object DefaultValue { get; private set; }
        public DataFormat DataFormat { get; private set; }
        public dynamic Value { get; set; }
        public bool Found { get; set; }

        public Key(string name, bool required, DataType dataType, DataFormat dataFormat, object defaultValue = null)
        {
            Name = name;
            Required = required;
            DataType = dataType;
            DefaultValue = defaultValue;
            DataFormat = dataFormat;
            SpecialAction = () => { throw new NotImplementedException("Special action was not implemented"); };
        }

        public Action SpecialAction { get; set; }
        public Type EnumType { get; set; }
        public Type MultilineType { get; set; }

        public void DoSpecialThing()
        {
            SpecialAction.Invoke();
        }
    }
}
