using System.Linq.Expressions;

namespace DinnerPlans.Server.Persistence.IRepositories
{
    public interface IReadRepo
    {
        /// <summary>
        /// get entity by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetEntityById<T>(int? id) where T : class;
        /// <summary>
        /// get entity with predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> GetEntityWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class;
        /// <summary>
        /// get entity with predicate and projection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        Task<E?> GetEntityWithPredicateAndProjection<T, E>
            (Expression<Func<T, bool>> predicate, Expression<Func<T, E>> projection)
            where T : class where E : class;
        /// <summary>
        /// get entity with predicate and include associated entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T?> GetEntityWithPrediateAndInclude<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class;
        /// <summary>
        /// get entity with predicate as no tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> GetEntityWithPredicateAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
        /// <summary>
        /// get list with predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IList<T>> GetListWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class;
        /// <summary>
        /// get list with predicate and projection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        Task<IList<E>> GetListWithPredicateAndProjection<T, E>
            (Expression<Func<T, bool>> predicate, Expression<Func<T, E>> projection)
            where T : class where E : class;
        /// <summary>
        /// get list with predicate and include associated entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IList<T>> GetListWithPredicateAndInclude<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class;
        /// <summary>
        /// get list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IList<T>> GetList<T>() where T : class;
        /// <summary>
        /// get list with predicate as no tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IList<T>> GetListWithPredicateAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
}
