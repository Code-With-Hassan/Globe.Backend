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
using System.Text;
using System.Threading.Tasks;

namespace Globe.Core.Repository.impl
{
    public class OrganizationEntityRepository<TEntity> : IOrganizationEntityRepository<TEntity> where TEntity : OrganizationIdEntity
    {
        private const int _retryAttempts = 3;

        private int _currentAttempt;

        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Gets or sets the after save event used for audit trail.
        /// </summary>
        public Action<object> AfterSave { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository"/> class.
        /// </summary>
        /// <param name="context">DbContext</param>
        public OrganizationEntityRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the Records of the TEntity, Based on the filter specified
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns>A list of TEntities.</returns>
        public virtual QueryResult<TEntity> Get(List<long> OrganizationIds,
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

            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

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
        /// Gets the Records of the TEntity, Based on the filter specified
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns>A list of TEntities.</returns>
        public virtual QueryResult<TEntity> GetByIds(List<long> OrganizationIds,
              List<int> Ids,
              string filter = null,
              string orderBy = "Id desc",
              int pageSize = 10,
              int currentPage = 1,
              params Expression<Func<TEntity, object>>[] includes)
        {
            int count = 0;
            IQueryable<TEntity> query = _dbSet;
            if (Ids.Count > 0)
            {
                foreach (Expression<Func<TEntity, object>> include in includes)
                    query = query.Include(include);

                // filter data based upon the company ids
                if (OrganizationIds != null && OrganizationIds.Count >= 1)
                    query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

                // filter data based upon the PK ids
                if (Ids.Count >= 1)
                {
                    query = query.Where(x => Ids.Contains((int)x.Id));
                }
                else
                {

                }

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
            }
            else
            {
                query = query.Where(x => x.Id == 0);
            }
            return new QueryResult<TEntity>() { Count = count, List = query.ToList() };
        }
        /// <summary>
        /// Gets the list of all active entities.
        /// </summary>
        /// <param name="includes">The sub entities to includes.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>A QueryResult.</returns>
        public virtual async Task<QueryResult<TEntity>> GetAllActiveAsync(List<long> OrganizationIds, string orderBy, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));


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
        /// Gets the query result based on passed parameters.
        /// </summary>
        /// <param name="query">The provided query to be used on entity.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="includes">The inner objects that needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> Get(List<long> OrganizationIds,
              IQueryable<TEntity> query,
              string filter = null,
              params Expression<Func<TEntity, object>>[] includes)
        {
            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);


            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

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

        /// <summary>
        /// Gets the query result of the specified identity including sub entities.
        /// </summary>
        /// <param name="includes">The sub entities needs to be included.</param>
        /// <returns>A QueryResult.</returns>
        public virtual QueryResult<TEntity> Get(List<long> OrganizationIds,
              params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

            return new QueryResult<TEntity>() { List = query.ToList() };
        }

        /// <summary>
        /// Queries the entities.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>An IQueryable.</returns>
        public virtual IQueryable<TEntity> Query(List<long> OrganizationIds,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

            return query;
        }

        /// <summary>
        /// Gets the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A ValueTask.</returns>
        public virtual ValueTask<TEntity> GetById(object id)
        {
            return _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets the first or default entity.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>A TEntity.</returns>
        public virtual TEntity GetFirstOrDefault(List<long> OrganizationIds, Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            // filter data based upon the company ids
            if (OrganizationIds != null && OrganizationIds.Count >= 1)
                query = query.Where(x => OrganizationIds.Contains(x.OrganizationId));

            return query.FirstOrDefault(filter);
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Insert(List<long> OrganizationIds, TEntity entity, bool isActive = true)
        {
            var value = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // if user is associated to 1 company only then add company id from jwt token
            if (OrganizationIds != null && OrganizationIds.Count == 1)
                entity.OrganizationId = OrganizationIds.FirstOrDefault();


            entity.CreatedAt = value;
            entity.UpdatedAt = value;
            entity.IsActive = isActive;

            _dbSet.Add(entity);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Update(List<long> OrganizationIds, TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            // if user is associated to 1 company only then add company id from jwt token
            if (OrganizationIds != null && OrganizationIds.Count == 1)
                entity.OrganizationId = OrganizationIds.FirstOrDefault();

            entity.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _context.Entry(entity).State = EntityState.Modified;
            _context.Entry(entity).Property(nameof(entity.CreatedAt)).IsModified = false;
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <param name="deactivate">If true, deactivate.</param>
        public virtual void Delete(List<long> OrganizationIds, object id, bool deactivate = true)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                if (deactivate)
                {
                    entityToDelete.IsActive = false;
                    Update(OrganizationIds, entityToDelete);
                }
                else
                {
                    _context.RemoveRange(entityToDelete);
                }
            }
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> filter)
        {
            _context.RemoveRange(_dbSet.Where(filter));
        }

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <param name="ids">The list of id of the entities to be deleted.</param>
        /// <param name="deactivate">If true, deactivate.</param>
        public virtual void Delete(List<long> OrganizationIds, int[] ids, bool deactivate = true)
        {
            var longIds = ids.ToList().ConvertAll(i => (long)i);
            var query = Query(OrganizationIds, x => longIds.Contains(x.Id));
            var result = Get(OrganizationIds, query);

            if (deactivate)
                result.List.ForEach(x => x.IsActive = false);
            else
                _context.RemoveRange(result.List);
        }

        /// <summary>
        /// Deletes multiple entities on the basis of supplied criteria.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="deactivate">If true, deactivate.</param>
        public virtual void Delete(List<long> OrganizationIds, string filter = null, bool deactivate = true)
        {
            var result = Get(OrganizationIds, Query(OrganizationIds), filter);

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
        /// Counts the number of elements.
        /// </summary>
        /// <returns>Number of elements.</returns>
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


    }
}
