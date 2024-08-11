using Globe.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Globe.Audit.Api.Models
{
    /// <summary>
    /// The export log model.
    /// Following model will be used to pass data to audit, api when user exports on a screen from front end.
    /// On each export, screen name and filter expression is logged along with current user name.
    /// </summary>
    public class ExportLogModel
    {
        private string _filterExpression;

        /// <summary>
        /// Gets the audit type.
        /// </summary>
        public string AuditType { get; private set; } = AuditTypes.Export;

        /// <summary>
        /// Gets or sets the screen name.
        /// Screen on which export action is performed.
        /// </summary>
        [Required]
        public string ScreenName { get; set; }

        /// <summary>
        /// Gets or sets the filter expression.
        /// Current filter expression applied when user performed export.
        /// </summary>
        [Required]
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
                    _filterExpression = Encoding.Default.GetString(data);
                }
                catch (FormatException)
                {
                    _filterExpression = string.Empty;
                }
            }
        }
    }
}
