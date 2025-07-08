using System.Reflection;

namespace Dispatcher
{
    public class DispatcherConfiguration
    {
        public List<Assembly> AssembliesToScan { get; } = [];
    }
}
