namespace DinnerPlans.Server.Persistence.IRepositories
{
    public interface IWriteRepo
    {
        /// <summary>
        /// add entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddEntityAsync<T>(T entity) where T : class;
        /// <summary>
        /// add range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;
        /// <summary>
        /// update entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void UpdateEntity<T>(T entity) where T : class;
        /// <summary>
        /// delete entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void DeleteEntity<T>(T entity) where T : class;
        /// <summary>
        /// delete range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void DeleteRange<T>(IEnumerable<T> entities) where T : class;
    }
}
