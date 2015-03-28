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
        ICollection<UCCTaskSolver.TaskSolver> GetSolvers();
        UCCTaskSolver.TaskSolver GetSolverByName(string name);
        Type GetSolverType(string name);
    }

    public class TaskSolversRepository : ITaskSolversRepository
    {
        private readonly IConfigProvider _configProvider;
        private readonly ILog _log;

        private Dictionary<string, UCCTaskSolver.TaskSolver> _taskSolvers;

        public TaskSolversRepository(IConfigProvider configProvider, IPluginManager<UCCTaskSolver.TaskSolver> pluginManager,
            ILog log)
        {
            _configProvider = configProvider;
            _log            = log;

            _taskSolvers = new Dictionary<string, UCCTaskSolver.TaskSolver>();

            pluginManager.AddDirectory("plugins/");
            foreach (var plugin in pluginManager.GetPlugins())
            {
                _log.InfoFormat("Plugin found. Name=[{0}]", plugin.Name);
                _taskSolvers.Add(plugin.Name, plugin);
            }
        }

        public ICollection<UCCTaskSolver.TaskSolver> GetSolvers()
        {
            return _taskSolvers.Select(t => t.Value).ToList();
        }

        public UCCTaskSolver.TaskSolver GetSolverByName(string name)
        {
            return _taskSolvers[name];
        }

        public Type GetSolverType(string name)
        {
            return _taskSolvers[name].GetType();
        }
    }
}
