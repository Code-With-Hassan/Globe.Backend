using Globe.Core.AuditHelpers;
using Globe.Core.Constants;
using Globe.Core.Entities.Base;
using Globe.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Globe.Core.Repository.impl
{
    /// <summary>
    /// The generic repository class.
    /// </summary>
    public class CustomEntityRepository<TEntity> : ICustomEntityRepository<TEntity> where TEntity : CustomBaseEntity
    {
        private const int _retryAttempts = 3;

        private int _currentAttempt;

        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// After save callback function, used for audit trail.
        /// </summary>
        public Action<object> AfterSave { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">DbContext</param>
        public CustomEntityRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the Entity.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> Get(
              string filter = null,
              string orderBy = "Id desc",
              int pageSize = 10,
              int currentPage = 1,
              params Expression<Func<TEntity, object>>[] includes)
        {
            int count;
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            try
            {
                if (filter != null)
                    query = query.Where(filter);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                count = query.Count();

                query = query.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize);
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = count, List = query.ToList() };
        }

        public virtual QueryResult<TEntity> GetPaginatedByQuery(IQueryable<TEntity> query,
                                               string filter = null,
                                               string orderBy = "Id desc",
                                               int pageSize = 10,
                                               int currentPage = 1,
                                               params Expression<Func<TEntity, object>>[] includes)
        {
            int count;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            try
            {
                if (filter != null)
                    query = query.Where(filter);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                count = query.Count();

                query = query.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize);
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = count, List = query.ToList() };
        }

        /// <summary>
        /// Gets the list of all active entities. (i.e. entities with IsActive equals to true)
        /// </summary>
        /// <param name="includes">The sub entities to include.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>A QueryResult.</returns>
        public virtual async Task<QueryResult<TEntity>> GetAllActiveAsync(string orderBy
                                                                , params Expression<Func<TEntity,
                                                                    object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            List<TEntity> list;
            try
            {
                query = query.Where(x => x.IsActive == true);
                query = query.OrderBy(orderBy);

                list = await query.ToListAsync();
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = list.Count, List = list };
        }

        /// <summary>
        /// Gets the list of all active entities. (i.e. entities with IsActive equals to true) from cache.
        /// </summary>
        /// <param name="includes">The sub entities to include.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> GetAllActiveInMemory(IQueryable<TEntity> query, string orderBy, string filter = null, params Expression<Func<TEntity, object>>[] includes)
        {

            foreach (var include in includes)
                query = query.Include(include);

            List<TEntity> list;
            try
            {
                if (filter != null)
                    query = query.Where(filter);

                query = query.Where(x => x.IsActive == true);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                list = query.ToList();
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = list.Count, List = list };
        }

        /// <summary>
        /// Gets the query result based on passed parameters.
        /// </summary>
        /// <param name="query">The provided query to be used on entity.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="includes">The inner objects that needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> Get(
              IQueryable<TEntity> query,
              string filter = null,
              params Expression<Func<TEntity, object>>[] includes)
        {
            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            int count;

            try
            {
                if (filter != null)
                    query = query.Where(filter);

                count = query.Count();
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = count, List = query.ToList() };
        }

        public virtual QueryResult<TEntity> GetInMemory(IQueryable<TEntity> query, string filter = null,
                                                        string orderBy = "Id desc", int pageSize = 10,
                                                        int currentPage = 1,
                                                        params Expression<Func<TEntity,
                                                        object>>[] includes)
        {
            int count;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            try
            {
                if (filter != null)
                    query = query.Where(filter);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                count = query.Count();

                query = query.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize);
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid Parameters", e);
            }

            return new QueryResult<TEntity>() { Count = count, List = query.ToList() };
        }
        /// <summary>
        /// Gets the query result of the specified identity including sub entities.
        /// </summary>
        /// <param name="includes">The sub entities needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> Get(
              params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return new QueryResult<TEntity>() { List = query.ToList() };
        }

        /// <summary>
        /// Queries the repository.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>An IQueryable of generic entity.</returns>
        public virtual IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        /// <summary>
        /// Gets the entity by its id.
        /// </summary>
        /// <param name="id">The id of entity.</param>
        /// <returns>The entity.</returns>
        public virtual ValueTask<TEntity> GetById(object id)
        {
            return _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets the first or default entity.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="includes">The sub entities to include.</param>
        /// <returns>A TEntity.</returns>
        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefault(filter);
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        public virtual void Insert(TEntity entity, bool isActive = true)
        {
            entity.UpdatedAt = entity.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.IsActive = isActive;
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        public virtual void Update(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entity.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.IsActive ??= false;
            _context.Entry(entity).State = EntityState.Modified;
            _context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        public virtual void UpdateRelatedEntity(TEntity entity)
        {
            entity.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.IsActive ??= false;
            _context.Update(entity);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="id">The id of the entity to be delete.</param>
        /// <param name="deactivate">If true, deactivate | else permanently delete.</param>
        public virtual void Delete(object id, bool deactivate = true)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                if (deactivate)
                {
                    entityToDelete.IsActive = false;
                    Update(entityToDelete);
                }
                else
                {
                    _context.RemoveRange(entityToDelete);
                }
            }
        }

        /// <summary>
        /// Deletes the entity w.r.t. filter expression.
        /// </summary>
        /// <param name="filter">The filter to be applied.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> filter)
        {
            _context.RemoveRange(_dbSet.Where(filter));
        }

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <param name="ids">The list of id of the entities to be deleted.</param>
        /// <param name="deactivate">If true, deactivate | else delete entity permanently.</param>
        public virtual void Delete(long[] ids, bool deactivate = true)
        {
            var query = Query(x => ids.Contains(x.Id));
            var result = Get(query);

            if (deactivate)
                result.List.ForEach(x => x.IsActive = false);
            else
                _context.RemoveRange(result.List);
        }

        /// <summary>
        /// Deletes multiple entities on the basis of supplied criteria.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="deactivate">If true, deactivate | else delete entity permanently.</param>
        public virtual void Delete(string filter = null, bool deactivate = true)
        {
            var result = Get(Query(), filter);

            if (deactivate)
                result.List.ForEach(x => x.IsActive = false);
            else
                _context.RemoveRange(result.List);
        }

        /// <summary>
        /// Saves the changes in database.
        /// </summary>
        public virtual void SaveChanges(string userName, List<long> organizationIds)
        {
            List<AuditEntry> logs = null;

            if (string.Compare(userName, RepositoryConstants.System, StringComparison.OrdinalIgnoreCase) == 0
                            || string.Compare(userName, RepositoryConstants.Admin, StringComparison.OrdinalIgnoreCase) == 0)
            {
                organizationIds = new List<long>();
            }

            if (AfterSave != null) logs = new AuditHelper(_context).CreateAuditLogs(userName, organizationIds);

            _currentAttempt = 0;
            while (_currentAttempt < _retryAttempts)
            {
                try
                {
                    // Access the DbContext to handle subsequent requests
                    // Process the received data, interact with the DbContext, and send responses

                    _context.SaveChanges();
                    _context.ChangeTracker.Clear();

                    break; // Operation succeeded, exit the retry loop
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        entry.Reload();
                    }

                    _currentAttempt++;
                    // Delay before retrying, you can implement a backoff algorithm here
                    Task.Delay(100); // wait for 100 milliseconds before retrying
                }
            }

            AfterSave?.Invoke(logs);
        }

        /// <summary>
        /// Counts the number of rows.
        /// </summary>
        /// <returns>An int values of number of rows.</returns>
        public int Count()
        {
            return _dbSet.Count();
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <returns>An IDbContextTransaction.</returns>
        public IDbContextTransaction GetTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public List<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        /// <summary>
        /// Saves the changes in database asynchronously.
        /// </summary>
        public virtual async Task SaveChangesAsync(string userName, List<long> organizationIds)
        {
            List<AuditEntry> logs = null;

            if (string.Compare(userName, RepositoryConstants.System, StringComparison.OrdinalIgnoreCase) == 0
                            || string.Compare(userName, RepositoryConstants.Admin, StringComparison.OrdinalIgnoreCase) == 0)
            {
                organizationIds = new List<long>();
            }

            if (AfterSave != null) logs = new AuditHelper(_context).CreateAuditLogs(userName, organizationIds);

            _currentAttempt = 0;
            while (_currentAttempt < _retryAttempts)
            {
                try
                {
                    // Access the DbContext to handle subsequent requests
                    // Process the received data, interact with the DbContext, and send responses

                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();

                    break; // Operation succeeded, exit the retry loop
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }

                    _currentAttempt++;
                    // Delay before retrying, you can implement a backoff algorithm here
                    await Task.Delay(100); // wait for 100 milliseconds before retrying
                }
            }

            AfterSave?.Invoke(logs);
        }
    }
}
