using System.Linq.Expressions;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        T Get(Expression<Func<T,bool>> filter);
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        public IEnumerable<T> GetRange(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

  
    }
}
