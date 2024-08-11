using Globe.Shared.Models.Helpers;
using System.Text.Json;

namespace Globe.Shared.Models
{
    /// <summary>
    /// The paging request params. 
    /// These will be used for Get Requests to return paged results.
    /// </summary>
    public class QueryStringParams
    {
        private const string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Default page size.
        /// </summary>
        protected int _pageSize = 100;

        /// <summary>
        /// Filter expression.
        /// </summary>
        protected string _filterExpression;

        /// <summary>
        /// Stores datetime in datetime parameter
        /// </summary>
        protected DateTime _dateTime = new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, System.DateTime.Today.Day, 23, 59, 59);

        public void SetFilterExpression(string filterExpression)
        {
            _filterExpression = filterExpression;

        }

        public void SetOrderBy(string OrderBy)
        {
            this.OrderBy = OrderBy;
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Get Or Set DateTime in Epoch
        /// </summary>
        public string DateTime
        {
            get
            {
                return _dateTime.ToString(_dateTimeFormat);
            }
            set
            {
                var temp = DateHelper.ConvertToDateTime(value, _dateTimeFormat);
                _dateTime = temp != null ? temp.Value : new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, System.DateTime.Today.Day, 23, 59, 59);
            }
        }

        /// <summary>
        /// Gets datetime as datetime object
        /// </summary>
        public DateTime GetDateTime => _dateTime;

        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        public string FilterExpression
        {
            get
            {
                return _filterExpression;
            }
            set
            {
                try
                {
                    byte[] data = Convert.FromBase64String(value.Replace(' ', '+'));
                    _filterExpression = System.Text.Encoding.Default.GetString(data);
                    _filterExpression = FilterHelper.GetEscaped(_filterExpression);
                }
                catch (FormatException)
                {
                    _filterExpression = string.Empty;
                }

            }
        }

        /// <summary>
        /// Gets or sets the filter data.
        /// </summary>
        public string FilterData { get; set; }

        /// <summary>
        /// Gets or sets the Filter Data By Ids.
        /// </summary>
        public string FilterDataByIds { get; set; }

        /// <summary>
        /// Gets or sets the Filter Data By Node Ids.
        /// </summary>
        public string FilterDataByNodeIds { get; set; }

        /// <summary>
        /// Overriding the ToString to generate string representation of the object
        /// Mainly to be used to logging.
        /// </summary>
        /// <returns>JSON string of the current object.</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringParams"/> class.
        /// By Default every list will be sorted based on the primary key
        /// Since the primary key is auto increment we'll have the latest records at the top
        /// If a use case needs to change this behaviour it can extend this class
        /// </summary>
        public QueryStringParams()
        {
            OrderBy = "Id desc";
        }
    }
}
