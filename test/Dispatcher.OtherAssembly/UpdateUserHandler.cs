namespace Dispatcher.OtherAssembly
{
    public class UpdateUserHandler: IRequestHandler<UpdateUserRequest, bool>
    {
        public Task<bool> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            // Simulate updating a user in a database or other storage
            Console.WriteLine($"Updating user {request.UserId} with Name: {request.UserName}, Email: {request.Email}");
            return Task.FromResult(true); // Simulate success
        }
    }

}
