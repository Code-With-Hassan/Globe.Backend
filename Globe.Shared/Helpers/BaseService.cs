using Microsoft.AspNetCore.Http;
using Globe.Shared.MVC.Extensions;

namespace Globe.Shared.Helpers
{
    /// <summary>
    /// The base service.
    /// </summary>
    public class BaseService
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="accessor">The accessor.</param>
        public BaseService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UserName => _accessor.HttpContext.GetUserName();

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        public int UserId => _accessor.HttpContext.GetUserId();

        /// <summary>
        /// Gets the user Id.
        /// </summary> 
        public List<long> OrganizationIds => _accessor.HttpContext.GetOrganizationIds();

        /// <summary>
        /// Get the default organization id
        /// </summary>
        // public long DefaultOrganizationId => IsSuperUser ? 0 : OrganizationIds.FirstOrDefault();
        public List<long> DefaultOrganizationId => _accessor.HttpContext.GetOrganizationIds();

        /// <summary>
        /// Gets the corelation id.
        /// </summary>
        public string CorelationId => _accessor.HttpContext.GetCorelationId();

        /// <summary>
        /// Is Super User.
        /// </summary>
        public bool IsSuperUser => _accessor.HttpContext.IsSuperUser();
    }
}
