namespace Chinook.Domain.Contracts
{
    public interface IUserService
    {
        Task<string> GetAuthenticatedUserIdAsync();
    }
}