using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.PluginManager
{
    /// <summary>
    /// Interfejs menadżera pluginów
    /// </summary>
    /// <typeparam name="T">Typ klasy bazowej wyszukiwanych pluginów</typeparam>
    public interface IPluginManager<T>
    {
        void AddDirectory(string directoryPath);
        ICollection<Type> GetPlugins();
    }

    /// <summary>
    /// Implementacja menadżera pluginów.
    /// </summary>
    /// <typeparam name="T">Typ klasy bazowej wyszukiwanych pluginów</typeparam>
    public class PluginManager<T> : IPluginManager<T>
    {
        private List<Type> _plugins;

        public PluginManager()
        {
            _plugins = new List<Type>();
        }

        /// <summary>
        /// Przeszukuje nierekursywnie dany katalog w celu odnalezienia klas rozszerzających klasę T.
        /// </summary>
        /// <param name="directoryPath">Ścieżka do katalogu</param>
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

        /// <summary>
        /// Zwraca odnalezione wcześniej pluginy.
        /// </summary>
        /// <returns>Kolekcja (lista) odnalezionych pluginów</returns>
        public ICollection<Type> GetPlugins()
        {
            return _plugins;
        }
    }
}
