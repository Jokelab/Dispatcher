namespace Dispatcher.OtherAssembly
{
    public class CreateUserHandler : ICommandHandler<CreateUserCommand, UserModel>
    {
        private readonly IDispatcher _dispatcher;
        public CreateUserHandler(IDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public Task<UserModel> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new UserModel
            {
                Name = command.Name,
                Id = new Random().Next()
            };

            _dispatcher.Publish(new UserCreated(user));

            return Task.FromResult(user);
        }
    }
}
