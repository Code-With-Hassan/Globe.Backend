using System.Globalization;

namespace Globe.Shared.Models.Helpers
{
    /// <summary>
    /// The date helper class.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Date format
        /// </summary>
        const string DATE_FORMAT1 = "yyyy-MM-dd";

        /// <summary>
        /// Date format
        /// </summary>
        const string DATE_FORMAT2 = "yyyy/MM/dd";

        /// <summary>
        /// Date format
        /// </summary>
        const string DATE_FORMAT3 = "yyyy.MM.dd";

        /// <summary>
        /// Date format
        /// </summary>
        const string DATE_FORMAT4 = "yyyyMMdd";

        /// <summary>
        /// converts datetime to date string using a format.
        /// </summary>
        /// <param name="value">The datetime value.</param>
        /// <returns>A formatted datetime string.</returns>
        public static string? ToString(DateTime? value)
        {
            if (value != null)
                return value?.ToString(DATE_FORMAT1);
            return null;
        }

        /// <summary>
        /// Converts a string value to datetime value using predefined formats.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A DateTime value.</returns>
        public static DateTime? FromString(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return DateTime.ParseExact(value,
                    new string[] { DATE_FORMAT1, DATE_FORMAT2, DATE_FORMAT3, DATE_FORMAT4 },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
            }
            return null;
        }

        /// <summary>
        /// Converts a string value to datetime value using predefined formats.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A DateTime value.</returns>
        public static DateTime FromStringNotNull(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return DateTime.ParseExact(value,
                    new string[] { DATE_FORMAT1, DATE_FORMAT2, DATE_FORMAT3, DATE_FORMAT4 },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Converts a string to datetime provided valid formats
        /// </summary>
        /// <param name="dateString">date string to convert to datetime</param>
        /// <param name="formats">formats to be used to parse date string</param>
        /// <returns>null if couldn't parse or else returns valid datetime value</returns>
        public static DateTime? ConvertToDateTime(string dateString, params string[] formats)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;

            return DateTime.ParseExact(dateString,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
        }
    }
}
