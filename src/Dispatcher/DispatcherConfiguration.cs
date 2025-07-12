using System.Reflection;

namespace Dispatcher
{
    public class DispatcherConfiguration
    {
        /// <summary>
        /// Add assemblies to scan for requests, events and handlers
        /// </summary>
        public List<Assembly> AssembliesToScan { get; } = [];

        /// <summary>
        /// Add explicit types for requests, events and handlers
        /// </summary>
        public List<Type> ExplicitTypes { get; } = [];
        public List<Type> OpenBehaviors { get; } = [];
      
    }
}
