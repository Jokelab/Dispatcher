namespace Dispatcher.OtherAssembly
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, UserModel>
    {
        private readonly IDispatcher _dispatcher;
        public CreateUserHandler(IDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public Task<UserModel> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = new UserModel
            {
                Name = request.Name,
                Id = new Random().Next()
            };

            _dispatcher.Publish(new UserCreated(user), cancellationToken);

            return Task.FromResult(user);
        }
    }
}
