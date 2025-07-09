using System.Reflection;

namespace Dispatcher
{
    public class DispatcherConfiguration
    {
        /// <summary>
        /// Add assemblies to scan for commands, events and handlers
        /// </summary>
        public List<Assembly> AssembliesToScan { get; } = [];

        /// <summary>
        /// Add types to scan for commands, events and handlers
        /// </summary>
        public List<Type> Types { get; } = [];
    }
}
