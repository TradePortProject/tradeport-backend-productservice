using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProductManagement.Data
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDbContext AppDbContext { get; set; }

        public RepositoryBase(AppDbContext _AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }

        public void Create(T entity)
        {
            AppDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            AppDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            AppDbContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            return AppDbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return AppDbContext.Set<T>().Where(expression).AsNoTracking();
        }
    }
}
