using System;
using System.Threading.Tasks;

namespace Product.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IItemRepository Items { get; }
        IUserRepository Users { get; }
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync();
    }
}
