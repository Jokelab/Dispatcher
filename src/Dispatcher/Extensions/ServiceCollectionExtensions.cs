using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services, Action<DispatcherConfiguration> configuration)
    {
        var config = new DispatcherConfiguration();
        configuration.Invoke(config);
        return services.AddDispatcher(config);
    }

    /// <summary>
    /// Add dispatcher services for the calling assembly
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        var config = new DispatcherConfiguration();
        config.AssembliesToScan.Add(Assembly.GetCallingAssembly());
        return services.AddDispatcher(config);
    }

    private static IServiceCollection AddDispatcher(this IServiceCollection services, DispatcherConfiguration configuration)
    {
        services.TryAddTransient<IDispatcher, Dispatcher>();
        var combinedTypes = new List<Type>();
        combinedTypes.AddRange(configuration.AssembliesToScan.SelectMany(assembly => assembly.GetTypes()));
        combinedTypes.AddRange(configuration.ExplicitTypes);

        // requests should have exactly one handler
        RegisterHandlers(services, combinedTypes, typeof(IRequest), typeof(IRequestHandler<,>), mustHaveOneHandler: true);

        // events can have zero or more handlers
        RegisterHandlers(services, combinedTypes, typeof(IEvent), typeof(IEventHandler<>), mustHaveOneHandler: false);

        // register open generic behaviors
        foreach (var behaviorType in configuration.Behaviors)
        {
            if (!behaviorType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"Behavior type {behaviorType.FullName} must be a generic type definition.");
            }
            services.AddTransient(typeof(IBehavior<,>), behaviorType);
        }
        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, IEnumerable<Type> allTypes, Type messageInterfaceType, Type messageHandlerInterfaceType, bool mustHaveOneHandler)
    {
        // find all closed implementations of IRequestHandler<,> or IEventHandler<> in the assembly
        var handlerTypes = allTypes
            .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                .Select(i => t))
            .Distinct()
            .ToList();

        // find all message implementations in the assembly
        var messageTypes = allTypes
            .Where(t => !t.IsAbstract && !t.IsInterface && messageInterfaceType.IsAssignableFrom(t))
            .ToList();

        foreach (var msgType in messageTypes)
        {
            // Find all handler types that handle this request
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
                services.TryAddTransient(match.HandlerInterface, match.HandlerType);
            }
        }
    }
}
