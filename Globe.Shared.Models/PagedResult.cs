using System.Collections.Generic;
using System.Linq;

namespace Globe.Shared.Models
{
    /// <summary>
    /// The paged list that will contain the list of paged records
    /// each record will be of type TItem
    /// </summary>
    public class PagedResult<TItem>
    {
        /// <summary>
        /// Gets or sets the total number of records (without paging).
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the items in the current page
        /// </summary>
        public IEnumerable<TItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the number of pages.
        /// </summary>
        public int NumberOfPages { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the item count in current page.
        /// </summary>
        public int ItemCountInCurrentPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult"/> class.
        /// </summary>
        /// <param name="total">The total number of items</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="items">The items list on the current page</param>
        /// <param name="pageSize">The page size.</param>
        public PagedResult(int total, int currentPage, IEnumerable<TItem> items, int pageSize)
        {
            Total = total;
            CurrentPage = currentPage;
            Items = items;
            PageSize = pageSize < 1 ? 1 : pageSize;
            NumberOfPages = total / PageSize + (total % PageSize > 0 ? 1 : 0);
            ItemCountInCurrentPage = items.Count();
        }
    }
}
