using System.Linq.Expressions;

namespace API.EF.Repository.Base
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> Get();
        Task<T> GetById(Expression<Func<T, bool >> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
