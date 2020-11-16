using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Entities;
using SharedKernel.Common;

namespace SharedKernel.Data
{
    public interface IRepository<TEntity>
        where TEntity : Entity
    {
        Task<ActionResult<TEntity>> AddAsync(TEntity entity);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);

        Task<bool> ExistsAsync(string id);

        Task<IEnumerable<Entity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> GetAsync(string id);

        Task<ActionResult<TEntity>> UpdateAsync(TEntity entity);

        Task<ActionResult> DeleteAsync(string id);
    }
}