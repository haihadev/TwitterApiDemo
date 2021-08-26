using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TwitterAPIDemo.Core.DataAccess
{
    public interface IRepository
    {
        Task<T> Get<T>(long id) where T : class;

        IQueryable<T> Find<T>() where T : class;

        IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class;

        Task<T> Add<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        T Remove<T>(T entity) where T : class;

        Task SaveChanges();
    }
}
