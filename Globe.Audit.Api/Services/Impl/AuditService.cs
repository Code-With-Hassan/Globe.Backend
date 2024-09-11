using Globe.Audit.Api.Database;
using Globe.Audit.Api.Helpers;
using Globe.Audit.Api.Models;
using Globe.Core.Entities;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.EventBus.RabbitMQ.Extensions;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;
using Globe.Shared.Helpers;
using Globe.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Globe.Audit.Api.Services.Impl
{
    /// <summary>
    /// The AuditService.
    /// </summary>
    public class AuditService : BaseService<AuditService>, IAuditService
    {
        private readonly IRepository<AuditEntity> _auditRepository;
        private readonly IRepository<AuditTableEntity> _auditTableRepository;
        private readonly IRepository<AuditOrganizationEntity> _auditOrganizationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditService"/> class.
        /// </summary>
        /// <param name="context">The audit database context.</param>
        /// <param name="logger">The audit controller logger.</param>
        /// <param name="httpContext"></param>
        public AuditService(AuditDbContext context, ILogger<AuditService> logger, IEventSender sender,
                            IHttpContextAccessor httpContext)
                            : base(logger, httpContext)
        {
            _auditRepository = new GenericRepository<AuditEntity>(context);
            _auditTableRepository = new GenericRepository<AuditTableEntity>(context);
            _auditOrganizationRepository = new GenericRepository<AuditOrganizationEntity>(context);

            sender.SetAfterSaveEvent<AuditEntity>(_auditRepository);
            sender.SetAfterSaveEvent<AuditTableEntity>(_auditTableRepository);
            sender.SetAfterSaveEvent<AuditOrganizationEntity>(_auditOrganizationRepository);
        }

        /// <summary>
        /// Gets the paged result.
        /// </summary>
        /// <param name="queryStringParams">The query string params.</param>
        /// <returns>A PagedResult.</returns>
        public PagedResult<AuditEntity> GetPagedResult(QueryStringParams queryStringParams)
        {
            _logger.LogInformation("Get Audit Called with params: {QueryStringParams}", queryStringParams);
            PagedResult<AuditEntity> result;
            QueryResult<AuditEntity> queryResult;
            try
            {

                if (IsSuperUser)
                {
                    queryResult = _auditRepository.GetPaginatedByQuery(
                    _auditRepository.Query().AsSplitQuery(),
                  queryStringParams.FilterExpression,
                  queryStringParams.OrderBy,
                  queryStringParams.PageSize,
                  queryStringParams.PageNumber);
                }
                else
                {
                    var OrgSpecificAuditsIds = _auditOrganizationRepository.Query(x => OrganizationIds.Contains(x.OrganizationId))
                                     .Select(x => x.AuditId);

                    queryResult = _auditRepository.GetPaginatedByQuery(
                   _auditRepository.Query(x => IsSuperUser || OrgSpecificAuditsIds.Contains(x.Id)).AsSplitQuery(),
                                                 queryStringParams.FilterExpression,
                                                 queryStringParams.OrderBy,
                                                 queryStringParams.PageSize,
                                                 queryStringParams.PageNumber);
                }


                int total = queryResult.Count;
                var auditList = queryResult.List;

                result = new PagedResult<AuditEntity>(
                        total,
                        queryStringParams.PageNumber,
                        auditList,
                        queryStringParams.PageSize
                    );
            }
            catch (Exception e)
            {
                // Log error message
                _logger.LogError(e, "{Message}: {Exception}", e.Message, e.ToString());

                // Rethrow exception
                throw;
            }
            return result;
        }


        //Create
        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="model">The model of the entity.</param>
        /// <returns>An ExportLogModel object.</returns>
        public ExportLogModel Create(ExportLogModel model)
        {
            var entity = AuditEntryConverter.ToAudit(UserName, model);

            _auditRepository.Insert(entity);
            _auditRepository.SaveChanges(UserName, DefaultOrganizationId);

            // associate audit logs with only users organizations
            AssocaiteOrganizationWithAudits(entity);

            AddAuditTable(entity);

            return model;
        }

        /// <summary>
        /// Associate Audits to Organizations for only users
        /// </summary>
        /// <param name="entity"></param>
        private void AssocaiteOrganizationWithAudits(AuditEntity entity)
        {
            if (SystemConstants.SystemAdmin.CompareTo(UserName) == -1
                    || SystemConstants.SystemAdmin.CompareTo(UserName) == -1)
            {
                var orgAudits = DefaultOrganizationId
                                                    .Select(x => AuditEntryConverter.AssociateOrgToAudits(x, entity.Id))
                                                    .ToList();

                orgAudits.ForEach(x => _auditOrganizationRepository.Insert(x));

                _auditOrganizationRepository.SaveChanges(UserName, DefaultOrganizationId);
            }

        }

        /// <summary>
        /// AddTableAndUser
        /// </summary>
        /// <param name="entity"></param>
        public void AddAuditTable(AuditEntity entity)
        {
            if (!_auditTableRepository.Query(x => x.TableName.Equals(entity.TableName)).Any())
            {
                _auditTableRepository.Insert(new AuditTableEntity
                {
                    TableName = entity.TableName
                });
                _auditTableRepository.SaveChanges(UserName, DefaultOrganizationId);
            }
        }

        /// <summary>
        /// Associate Audits with Organizations
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="OrgIds"></param>
        public void AssociateOrganization(long entityId, List<long> OrgIds)
        {
            foreach (var org in OrgIds)
            {
                var auditOrganization = new AuditOrganizationEntity();
                auditOrganization.AuditId = entityId;
                auditOrganization.OrganizationId = org;
                _auditOrganizationRepository.Insert(auditOrganization);
            }
            _auditOrganizationRepository.SaveChanges(SystemConstants.SystemUsername, new());
        }


        /// <summary>
        /// Get all tables.
        /// </summary>
        /// <returns>List of tables Model</returns>
        public List<AuditTableEntity> GetTables() => _auditTableRepository.Query().ToList();
    }
}
