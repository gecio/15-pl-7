using ComputationalCluster.Common;
using ComputationalCluster.PluginManager;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCTaskSolver;

namespace ComputationalCluster.PluginManager
{
    /// <summary>
    /// Interfejs repozytorium implementacji task solverów.
    /// </summary>
    public interface ITaskSolversRepository
    {
        ICollection<string> GetSolversNames();
        Type GetSolverType(string name);
        UCCTaskSolver.TaskSolver GetSolverInstance(string name, byte[] problemData = null);
    }

    /// <summary>
    /// Implementacja repozytorium task solverów. 
    /// </summary>
    public class TaskSolversRepository : ITaskSolversRepository
    {
        private readonly IConfigProvider _configProvider;
        private readonly ILog _log;

        private Dictionary<string, Type> _taskSolvers;

        public TaskSolversRepository(IConfigProvider configProvider, IPluginManager<UCCTaskSolver.TaskSolver> pluginManager,
            ILog log)
        {
            _configProvider = configProvider;
            _log            = log;

            _taskSolvers = new Dictionary<string, Type>();

            _log.Info("Looking for plugins...");

            pluginManager.AddDirectory("plugins/");
            foreach (var plugin in pluginManager.GetPlugins())
            {
                var pluginInstance = (UCCTaskSolver.TaskSolver)Activator.CreateInstance(plugin, new object[] { null });
                _log.InfoFormat("Plugin found. Name=[{0}]", pluginInstance.Name);
                _taskSolvers.Add(pluginInstance.Name, plugin);
            }
        }

        public ICollection<string> GetSolversNames()
        {
            return _taskSolvers.Keys.ToList();
        }

        public Type GetSolverType(string name)
        {
            return _taskSolvers[name];
        }

        public UCCTaskSolver.TaskSolver GetSolverInstance(string name, byte[] problemData = null)
        {
            return (UCCTaskSolver.TaskSolver)Activator.CreateInstance(GetSolverType(name), new object[] { problemData });
        }
    }
}
