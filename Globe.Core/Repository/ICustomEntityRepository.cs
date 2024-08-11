using Globe.Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Globe.Core.Repository
{
    /// <summary>
    /// The repository for Entities.
    /// </summary>
    public interface ICustomEntityRepository<TEntity> where TEntity : CustomBaseEntity
    {
        /// <summary>
        /// Gets the Entity.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(string filter = null,
              string orderBy = "Id desc",
              int pageSize = 10,
              int currentPage = 1,
              params Expression<Func<TEntity, object>>[] includes);

        QueryResult<TEntity> GetPaginatedByQuery(IQueryable<TEntity> query,
                                               string filter = null,
                                               string orderBy = "Id desc",
                                               int pageSize = 10,
                                               int currentPage = 1,
                                               params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the Entity.
        /// </summary>
        /// <param name="query">The provided query to be used on entity.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> GetInMemory(IQueryable<TEntity> query,
              string filter = null,
              string orderBy = "Id desc",
              int pageSize = 10,
              int currentPage = 1,
              params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the list of all active entities. (i.e. entities with IsActive equals to true)
        /// </summary>
        /// <param name="includes">The sub entities to include.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>A QueryResult.</returns>
        Task<QueryResult<TEntity>> GetAllActiveAsync(string orderBy, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the list of all active entities. (i.e. entities with IsActive equals to true) from cache
        /// </summary>
        /// <param name="query">The provided query to be used on entity.</param>
        /// <param name="includes">The sub entities to include.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> GetAllActiveInMemory(IQueryable<TEntity> query, string orderBy, string filter = null, params Expression<Func<TEntity, object>>[] includes);


        /// <summary>
        /// Gets the query result based on passed parameters.
        /// </summary>
        /// <returns>A List Of Entity.</returns>
        List<TEntity> GetAll();

        /// <summary>
        /// Gets the query result based on passed parameters.
        /// </summary>
        /// <param name="query">The provided query to be used on entity.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="includes">The inner objects that needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(
              IQueryable<TEntity> query,
              string filter = null,
              params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the query result of the specified identity including sub entities.
        /// </summary>
        /// <param name="includes">The sub entities needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Queries the repository.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>An IQueryable of generic entity.</returns>
        IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        /// <summary>
        /// Gets the entity by its id.
        /// </summary>
        /// <param name="id">The id of entity.</param>
        /// <returns>The entity.</returns>
        ValueTask<TEntity> GetById(object id);

        /// <summary>
        /// Gets the first or default entity.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="includes">The sub entities to include.</param>
        /// <returns>A TEntity.</returns>
        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        void Insert(TEntity entity, bool isActive = true);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        void UpdateRelatedEntity(TEntity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="id">The id of the entity to be delete.</param>
        /// <param name="deactivate">If true, deactivate | else permanently delete.</param>
        void Delete(object id, bool deactivate = true);

        /// <summary>
        /// Deletes the entity w.r.t. filter expression.
        /// </summary>
        /// <param name="filter">The filter to be applied.</param>
        void Delete(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <param name="ids">The list of id of the entities to be deleted.</param>
        /// <param name="deactivate">If true, deactivate | else delete entity permanently.</param>
        void Delete(long[] ids, bool deactivate = true);

        /// <summary>
        /// Deletes multiple entities on the basis of supplied criteria.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="deactivate">If true, deactivate | else delete entity permanently.</param>
        void Delete(string filter = null, bool deactivate = true);

        /// <summary>
        /// Counts the number of rows.
        /// </summary>
        /// <returns>An int values of number of rows.</returns>
        int Count();

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <returns>An IDbContextTransaction.</returns>
        IDbContextTransaction GetTransaction();

        /// <summary>
        /// Saves the changes in database.
        /// </summary>
        void SaveChanges(string userName, List<long> organizationIds);

        /// <summary>
        /// Saves the changes in database.
        /// </summary>
        Task SaveChangesAsync(string userName, List<long> organizationIds);
    }
}
