using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Repositories
{
	public interface IUnitOfWork
	{
		IGenericRepository<T> Repository<T> () where T : class;
        // public IGenericRepository<Category> CategoryRepository { get; }
        int Complete();
	}
}
