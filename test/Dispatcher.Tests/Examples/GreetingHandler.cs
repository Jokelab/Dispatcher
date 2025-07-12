using Dispatcher.Interfaces;

namespace Dispatcher.Tests.Examples
{
    public class GreetingHandler : IRequestHandler<GreetingRequest, string>
    {
        public Task<string> Handle(GreetingRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Hello, {request.Name}!");
        }
    }
}
