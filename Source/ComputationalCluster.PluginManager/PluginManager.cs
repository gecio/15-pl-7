using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.PluginManager
{
    public interface IPluginManager<T>
    {
        void AddDirectory(string directoryPath);
        ICollection<Type> GetPlugins();
    }

    public class PluginManager<T> : IPluginManager<T>
    {
        private List<Type> _plugins;

        public PluginManager()
        {
            _plugins = new List<Type>();
        }

        public void AddDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var directory = new DirectoryInfo(directoryPath);
            
            foreach (var file in directory.GetFiles()
                .Where(f => String.Compare(".dll", f.Extension, true) == 0))
            {
                var assembly = Assembly.LoadFrom(file.FullName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (type.BaseType == typeof(T))
                    {
                        _plugins.Add(type);
                    }
                }
            }
        }

        public ICollection<Type> GetPlugins()
        {
            return _plugins;
        }
    }
}
