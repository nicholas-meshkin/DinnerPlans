using DinnerPlans.Server.Persistence.IRepositories;
using DinnerPlans.Server.Persistence.Repositories;
using DinnerPlans.Server.Core;

namespace DinnerPlans.Server.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DinnerPlansContext _context;
        private bool _disposed;
        public IReadRepo ReadRepo { get; }
        public IWriteRepo WriteRepo { get; }

        public UnitOfWork(DinnerPlansContext context)
        {
            _context = context;
            ReadRepo = new ReadRepo(_context);
            WriteRepo = new WriteRepo(_context);
        }
        public async Task<bool> Commit() => await _context.SaveChangesAsync() > 0;
        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed && disposing)
                _context.Dispose();
            _disposed= true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
