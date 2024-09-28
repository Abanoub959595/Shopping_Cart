using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDBContext context;
		private readonly DbSet<T> dbSet;	

		public GenericRepository(AppDBContext context)
        {
			this.context = context;
			dbSet = context.Set<T>();
		}
        public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public void DeleteRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeWord = null)
		{
			IQueryable<T> query = dbSet;
			if (expression is not null)
				query = query.Where(expression);
			if (includeWord is not null)
			{
				foreach(var include in includeWord.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries) )
					query = query.Include(include);
			}
			return query.ToList();
		}

		public T GetFirstOrDefault(Expression<Func<T, bool>>? expression = null, string? includeWord = null)
		{
			IQueryable<T> query = dbSet;
			if (expression is not null)
				query = query.Where(expression);
			if (includeWord is not null)	
				foreach(var include in includeWord.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
					query = query.Include(include);

			return query.SingleOrDefault();
		}

		public void Update(T entity)
		{
			dbSet.Update(entity);
		}
	}
}
