using System.Threading.Tasks;
using Product.Domain.Entities;

namespace Product.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameWithRefreshTokensAsync(string username);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
