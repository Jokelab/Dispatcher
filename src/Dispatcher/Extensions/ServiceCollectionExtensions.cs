using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dispatcher.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDispatcher(this IServiceCollection services, Assembly scanAssembly)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IDispatcher, Dispatcher>();

            // commands should have exactly one handler
            RegisterHandlers(services, scanAssembly, typeof(ICommand), typeof(ICommandHandler<,>), mustHaveOneHandler: true);

            // events can have zero or more handlers
            RegisterHandlers(services, scanAssembly, typeof(IEvent), typeof(IEventHandler<>), mustHaveOneHandler: false);

            return services;
        }

        private static void RegisterHandlers(IServiceCollection services, Assembly scanAssembly, Type messageInterfaceType, Type messageHandlerInterfaceType, bool mustHaveOneHandler)
        {
            // find all closed implementations of ICommandHandler<,> or IEventHandler<> in the assembly
            var handlerTypes = scanAssembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                    .Select(i => t))
                .Distinct()
                .ToList();

            // find all message implementations in the assembly
            var messageTypes = scanAssembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && messageInterfaceType.IsAssignableFrom(t))
                .ToList();

            foreach (var msgType in messageTypes)
            {
                // Find all handler types that handle this command
                var matchingHandlers = handlerTypes
                    .SelectMany(handlerType =>
                        handlerType.GetInterfaces()
                            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                            .Where(i => i.GetGenericArguments()[0] == msgType)
                            .Select(i => new { HandlerType = handlerType, HandlerInterface = i })
                    )
                    .ToList();

                // Keep only the most derived handler types if there are multiple handlers for the same message type
                var mostDerivedHandlers = matchingHandlers
                    .GroupBy(h => h.HandlerInterface)
                    .SelectMany(g =>
                    {
                        var handlers = g.Select(x => x.HandlerType).ToList();
                        // Remove base types if a derived type exists
                        var mostDerived = handlers
                            .Where(ht => !handlers.Any(other => other != ht && ht.IsAssignableFrom(other)))
                            .ToList();
                        return g.Where(x => mostDerived.Contains(x.HandlerType));
                    })
                    .ToList();

                if (mustHaveOneHandler && mostDerivedHandlers.Count != 1)
                {
                    throw new InvalidOperationException($"Expected exactly one handler for {msgType.FullName}, but found {mostDerivedHandlers.Count}.");
                }

                foreach (var match in mostDerivedHandlers)
                {
                    services.AddTransient(match.HandlerInterface, match.HandlerType);
                }
            }
        }
    }
}
