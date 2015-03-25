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
        ICollection<T> GetPlugins();
    }

    public class PluginManager<T> : IPluginManager<T>
    {
        private List<T> _plugins;

        public PluginManager()
        {
            _plugins = new List<T>();
        }

        public void AddDirectory(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);
            
            foreach (var file in directory.GetFiles()
                .Where(f => String.Compare(".dll", f.Extension, true) == 0))
            {
                var assembly = Assembly.LoadFrom(file.FullName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (type.BaseType == typeof(T))
                    {
                        var pluginInstance = (T)Activator.CreateInstance(type);
                        _plugins.Add(pluginInstance);
                    }
                }
            }
        }

        public ICollection<T> GetPlugins()
        {
            return _plugins;
        }
    }
}
