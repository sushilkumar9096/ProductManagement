using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Product.Application.Interfaces;
using Product.Infrastructure.Data.Repositories;

namespace Product.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public IProductRepository Products { get; }
        public IItemRepository Items { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IProductRepository products,
            IItemRepository items,
            IUserRepository users)
        {
            _context = context;
            Products = products;
            Items = items;
            Users = users;
            _repositories = new ConcurrentDictionary<string, object>();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;

            return (IGenericRepository<T>)_repositories.GetOrAdd(type, _ =>
            {
                if (typeof(T) == typeof(Domain.Entities.Product)) return Products;
                if (typeof(T) == typeof(Domain.Entities.Item)) return Items;
                if (typeof(T) == typeof(Domain.Entities.User)) return Users;

                return new GenericRepository<T>(_context);
            });
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
