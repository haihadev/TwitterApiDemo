using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TwitterAPIDemo.Core.DataAccess
{
    public interface IUnitOfWork
    {
        DbContext DbContext { get; }

        Task Commit();
    }
}
