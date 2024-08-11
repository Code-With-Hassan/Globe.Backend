using Globe.Audit.Api.Models;
using Globe.Core.Entities;
using Globe.Shared.Models;

namespace Globe.Audit.Api.Services
{
    /// <summary>
    /// The Audit service interface.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Create Audit Log.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>An ExportLogModel object.</returns>
        ExportLogModel Create(ExportLogModel model);

        /// <summary>
        /// Gets the paged result.
        /// </summary>
        /// <param name="queryStringParams">The query string params.</param>
        /// <returns>A PagedResult.</returns>
        PagedResult<AuditEntity> GetPagedResult(QueryStringParams queryStringParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void AddAuditTable(AuditEntity entity);

        /// <summary>
        /// Get all tables
        /// </summary>
        /// <returns></returns>
        List<AuditTableEntity> GetTables();

        /// <summary>
        /// AssociateOrganization to audits
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="OrgIds"></param>
        void AssociateOrganization(long entityId, List<long> OrgIds);
    }
}