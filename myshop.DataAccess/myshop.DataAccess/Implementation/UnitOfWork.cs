using myshop.DataAccess.Data;
using myshop.Entities.Repositories;
using System.Collections;

namespace myshop.DataAccess.Implementation
{
    public class UnitOfWork : IUnitOfWork 
	{
		private readonly AppDBContext context;
        public Hashtable _Repositories { get; set; }

		// public IGenericRepository<Category> CategoryRepository => new GenericRepository<Category>(context);

		public UnitOfWork(AppDBContext context) 
		{
			this.context = context;
		}

		public int Complete()
		{
			return context.SaveChanges();
		}

		public IGenericRepository<T> Repository<T>() where T : class
		{
			if (_Repositories == null)	
				_Repositories = new Hashtable();

			var repositoryKey = typeof(T).Name;

			if (!_Repositories.ContainsKey(repositoryKey))
			{
				var repositoryType = typeof(GenericRepository<>);

				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);

				_Repositories.Add(repositoryKey, repositoryInstance);
			}

			return (IGenericRepository<T>)_Repositories[repositoryKey];

		}
	}
}
