using Globe.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Globe.Shared.Entities
{
    public class RoleApplicationEntity : BaseEntity
    {
        public long ApplicationId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ApplicationEntity Application { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public RoleEntity Role { get; set; }
    }
}
