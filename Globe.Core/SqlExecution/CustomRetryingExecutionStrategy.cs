using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Globe.Core.SqlExecution
{
    public class CustomRetryingExecutionStrategy : SqlServerRetryingExecutionStrategy
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CustomRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="context">The context on which the operations will be invoked.</param>
        /// <param name="maxRetryCount">The maximum number of retry attempts.</param>
        /// <param name="maxRetryDelay">The maximum delay between retries.</param>
        /// <param name="errorNumbersToAdd">Additional SQL error numbers that should be considered transient.</param>
        public CustomRetryingExecutionStrategy(
            DbContext context,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            ICollection<int>? errorNumbersToAdd)
            : base(
                context,
                maxRetryCount,
                maxRetryDelay,
                errorNumbersToAdd)
        { }

        /// <summary>
        ///     Method called before the first operation execution
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-connection-resiliency">Connection resiliency and database retries</see>
        ///     for more information.
        /// </remarks>
        override protected void OnFirstExecution() { }
    }
}
