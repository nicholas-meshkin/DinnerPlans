using DinnerPlans.Server.Persistence.IRepositories;

namespace DinnerPlans.Server.Core
{
    public interface IUnitOfWork
    {
        IReadRepo ReadRepo { get; }
        IWriteRepo WriteRepo { get; }
        Task<bool> Commit();
    }
}
