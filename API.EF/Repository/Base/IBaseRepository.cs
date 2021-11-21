﻿using System.Linq.Expressions;

namespace API.EF.Repository.Base
{
    public interface IBaseRepository<T>
    {
        List<T> Get();
        T GetById(Expression<Func<T, bool >> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
