using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Repositories
{
	public interface IGenericRepository<T> where T : class
	{
		T GetFirstOrDefault(Expression<Func<T, bool>>? expression = null, string? includeWord = null);
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeWord = null);
		void Add (T entity);
		void Update (T entity);
		void Delete (T entity);
		void DeleteRange(IEnumerable<T> entities);
	}
}
