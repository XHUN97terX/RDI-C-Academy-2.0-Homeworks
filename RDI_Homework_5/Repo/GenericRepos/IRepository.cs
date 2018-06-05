using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repo.GenericRepos
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Insert(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
        TEntity GetByID(int id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> condition);
    }
}
