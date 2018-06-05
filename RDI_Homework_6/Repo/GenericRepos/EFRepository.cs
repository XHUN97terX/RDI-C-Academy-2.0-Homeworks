using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repo.GenericRepos
{
    public abstract class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public abstract TEntity GetByID(int id);

        protected DbContext context;

        public EFRepository(DbContext context)
        {
            this.context = context;
        }
        public EFRepository(DbContext context, Action<string> logger) : this(context)
        {
            this.context.Database.Log = logger;
        }

        public void Delete(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = GetByID(id);
            if (entity == null)
                throw new ArgumentException("NO DATA");
            Delete(entity);
        }
        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> condition)
        {
            return GetAll().Where(condition);
        }
        public IQueryable<TEntity> GetAll()
        {
            return context.Set<TEntity>();
        }
        public void Insert(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
