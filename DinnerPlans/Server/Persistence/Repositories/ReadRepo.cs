using Microsoft.EntityFrameworkCore;
using DinnerPlans.Server.Persistence.IRepositories;
using System.Linq.Expressions;

namespace DinnerPlans.Server.Persistence.Repositories
{
    public class ReadRepo : IReadRepo
    {
        private readonly DinnerPlansContext _context;
        public ReadRepo(DinnerPlansContext context) => _context = context;

        public async Task<T?> GetEntityById<T>(int? id) where T : class => await _context.Set<T>().FindAsync(id);
        public async Task<T?> GetEntityWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
        public async Task<E?> GetEntityWithPredicateAndProjection<T, E>
            (Expression<Func<T, bool>> predicate, Expression<Func<T, E>> projection) 
            where T : class where E : class => 
            await _context.Set<T>().Where(predicate).Select(projection).FirstOrDefaultAsync();

        public async Task<T?> GetEntityWithPrediateAndInclude<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class
        {
            var query = _context.Set<T>().Where(predicate).AsQueryable();
            var entity = await includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).FirstOrDefaultAsync();
            if (entity != null)
                _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        public async Task<T?> GetEntityWithPredicateAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var entity = await _context.Set<T>().Where(predicate).AsNoTracking().FirstOrDefaultAsync();
            return entity;
        }

        public async Task<IList<T>> GetListWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _context.Set<T>().Where(predicate).ToListAsync();

        public async Task<IList<E>> GetListWithPredicateAndProjection<T, E>
            (Expression<Func<T, bool>> predicate, Expression<Func<T, E>> projection)
            where T : class where E : class => await _context.Set<T>().Where(predicate).Select(projection).ToListAsync();

        public async Task<IList<T>> GetListWithPredicateAndInclude<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class
        {
            var query = _context.Set<T>().Where(predicate).AsQueryable();
            return await includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).ToListAsync();
        }

        public async Task<IList<T>> GetList<T>() where T : class => await _context.Set<T>().ToListAsync();
        public async Task<IList<T>> GetListWithPredicateAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
        }
    }
}
