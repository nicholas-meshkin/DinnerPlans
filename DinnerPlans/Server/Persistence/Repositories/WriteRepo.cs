using DinnerPlans.Server.Persistence.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DinnerPlans.Server.Persistence.Repositories
{
    public class WriteRepo : IWriteRepo
    {
        private readonly DinnerPlansContext _context;
        public WriteRepo(DinnerPlansContext context) => _context = context;

        public async Task AddEntityAsync<T>(T entity) where T : class =>
            await _context.Set<T>().AddAsync(entity);
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class =>
            await _context.Set<T>().AddRangeAsync(entities);
        public void UpdateEntity<T>(T entity) where T : class
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void DeleteEntity<T>(T entity) where T : class
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _context.Set<T>().Attach(entity);
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}
