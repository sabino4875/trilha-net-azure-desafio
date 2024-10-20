namespace TrilhaNetAzureDesafio.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using TrilhaNetAzureDesafio.Models;

    public interface IRepository<TEntity> : IDisposable where TEntity : IEntity
    {
        Guid Insert(TEntity entity);
        Boolean Update(TEntity entity);
        Boolean Delete(Expression<Func<TEntity, Boolean>> criteria);
        TEntity Find(Expression<Func<TEntity, Boolean>> criteria);
        IEnumerable<TEntity> List(Expression<Func<TEntity, Boolean>> criteria, Int32 limit, Int32 offset);
        Int32 Count(Expression<Func<TEntity, Boolean>> criteria);
    }
}
