using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.Core.DataAccess
{
    public class Repository : IRepository
    {
        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<T> Get<T>(long id) where T : class
        {
            return id == 0L ? default(T) : await _unitOfWork.DbContext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> Find<T>() where T : class
        {
            return _unitOfWork.DbContext.Set<T>();
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _unitOfWork.DbContext.Set<T>().Where(predicate);
        }

        public async Task<T> Add<T>(T entity) where T : class
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                ((IAuditEntity)entity).CreatedDate = DateTime.Now;
            }
            await _unitOfWork.DbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public void Update<T>(T entity) where T : class
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                ((IAuditEntity)entity).UpdatedDate = DateTime.Now;
            }
            _unitOfWork.DbContext.Set<T>().Update(entity);
        }

        public T Remove<T>(T entity) where T : class
        {
            _unitOfWork.DbContext.Set<T>().Remove(entity);
            return entity;
        }

        public async Task SaveChanges()
        {
            await _unitOfWork.Commit();
        }
    }
}
