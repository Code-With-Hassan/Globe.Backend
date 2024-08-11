using System.Collections.Generic;

namespace Globe.Core.Repository
{
    /// <summary>
    /// The generic query result class.
    /// </summary>
    public class QueryResult<TEntity>
    {
        /// <summary>
        /// Gets or sets the count of rows in query result.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of generic entity.
        /// </summary>
        public List<TEntity> List { get; set; }
    }
}
