using ComputationalCluster.Common;
using ComputationalCluster.PluginManager;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCTaskSolver;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface ITaskSolversRepository
    {
        Type GetSolverType(string name);
        TaskSolver GetSolverInstance(string name, byte[] problemData = null);
    }

    public class TaskSolversRepository : ITaskSolversRepository
    {
        private readonly IConfigProvider _configProvider;
        private readonly ILog _log;

        private Dictionary<string, Type> _taskSolvers;

        public TaskSolversRepository(IConfigProvider configProvider, IPluginManager<TaskSolver> pluginManager,
            ILog log)
        {
            _configProvider = configProvider;
            _log            = log;

            _taskSolvers = new Dictionary<string, Type>();

            pluginManager.AddDirectory("plugins/");
            foreach (var plugin in pluginManager.GetPlugins())
            {
                var pluginInstance = (TaskSolver)Activator.CreateInstance(plugin, new object[] { null });
                _log.InfoFormat("Plugin found. Name=[{0}]", pluginInstance.Name);
                _taskSolvers.Add(pluginInstance.Name, plugin);
            }
        }

        public Type GetSolverType(string name)
        {
            return _taskSolvers[name];
        }

        public TaskSolver GetSolverInstance(string name, byte[] problemData = null)
        {
            return (TaskSolver)Activator.CreateInstance(GetSolverType(name), new object[] { problemData });
        }
    }
}
