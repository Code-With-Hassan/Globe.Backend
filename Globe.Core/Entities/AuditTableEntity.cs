using Globe.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globe.Core.Entities
{
    public class AuditTableEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [StringLength(200)]
        public string TableName { get; set; }
    }
}
