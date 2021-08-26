using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.Core.DataAccess
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        public UnitOfWork(IDbContextFactory<DemoContext> contextFactory, ILogger<UnitOfWork> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        private readonly IDbContextFactory<DemoContext> _contextFactory;
        private readonly ILogger<UnitOfWork> _logger;
        private DbContext _dbContext;
        private bool _disposed;

        public DbContext DbContext => _dbContext ?? (_dbContext = CreateDbContext());

        public async Task Commit()
        {
            CheckDisposed();

            try
            {
                if (_dbContext != null)
                {
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                CloseSession();
                throw new Exception("Error in Commit", ex);
            }
        }

        private DbContext CreateDbContext()
        {
            CheckDisposed();

            return _contextFactory.CreateDbContext();
        }

        private void CheckDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        private void CloseSession()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    CloseSession();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                _dbContext = null;
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~UnitOfWork()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
