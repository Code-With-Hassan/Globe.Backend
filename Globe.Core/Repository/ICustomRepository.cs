using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Globe.Core.Repository
{
    /// <summary>
    /// The custom repository interface.
    /// </summary>
    public interface ICustomRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the Count of rows.
        /// </summary>
        /// <returns>An int.</returns>
        int Count();

        /// <summary>
        /// Gets the query result.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(string filter = null, string orderBy = null, int pageSize = 10, int currentPage = 1, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the query result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(IQueryable<TEntity> query, string filter = null, string orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the query result.
        /// </summary>
        /// <param name="includes">The includes.</param>
        /// <returns>A QueryResult.</returns>
        QueryResult<TEntity> Get(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the all active records async.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>A Task.</returns>
        Task<QueryResult<TEntity>> GetAllActiveAsync(string orderBy, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets a record by its id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A ValueTask.</returns>
        ValueTask<TEntity> GetById(object id);

        /// <summary>
        /// Gets the first or default.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>A TEntity.</returns>
        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Creates query.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>An IQueryable.</returns>
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <param name="isolation">The System.Data.IsolationLevel to use.</param>
        /// <returns>An IDbContextTransaction.</returns>
        IDbContextTransaction GetTransaction(System.Data.IsolationLevel isolation);

        void SaveChanges(string userName, List<long> organizationIds);

        /// <summary>
        /// Deletes the entity w.r.t. filter expression.
        /// </summary>
        /// <param name="filter">The filter to be applied.</param>
        void Delete(Expression<Func<TEntity, bool>> filter);

        QueryResult<TEntity> GetPaginatedByQuery(IQueryable<TEntity> query,
                                               string filter = null,
                                               string orderBy = "Id desc",
                                               int pageSize = 10,
                                               int currentPage = 1,
                                               params Expression<Func<TEntity, object>>[] includes);
    }
}