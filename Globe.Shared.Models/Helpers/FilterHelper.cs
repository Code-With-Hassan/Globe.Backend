using System.Linq;
using System.Text;

namespace Globe.Shared.Models.Helpers
{
    /// <summary>
    /// The filter expression helper.
    /// </summary>
    public static class FilterHelper
    {
        /// <summary>
        /// The Quote character constant.
        /// </summary>
        const char QUOTE = '"';

        /// <summary>
        /// The Backslash character constant.
        /// </summary>
        const char BACKSLASH = '\\';

        /// <summary>
        /// The AND Logic.
        /// </summary>
        const string AND = "&&";

        /// <summary>
        /// The OR Logic.
        /// </summary>
        const string OR = "||";

        /// <summary>
        /// Gets the escaped expression.
        /// </summary>
        /// <param name="exp">The expression string to be escaped.</param>
        /// <returns>Escaped String</returns>
        public static string GetEscaped(string exp)
        {
            if (exp.Count(e => e == QUOTE) > 2)
            {
                if (exp.Contains(AND)) return HandleMultipleFilters(exp, AND);
                if (exp.Contains(OR)) return HandleMultipleFilters(exp, OR);

                int f = exp.IndexOf(QUOTE);
                int l = exp.LastIndexOf(QUOTE);

                var sb = new StringBuilder();
                char[] array = exp.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i != f && i != l && array[i] == QUOTE)
                    {
                        sb.Append(BACKSLASH);
                        sb.Append(QUOTE);
                    }
                    else if (array[i] == BACKSLASH)
                    {
                        sb.Append(BACKSLASH);
                        sb.Append(BACKSLASH);
                    }
                    else
                    {
                        sb.Append(array[i]);
                    }
                }
                return sb.ToString();
            }
            else
            {
                return exp.Replace(@"\", @"\\");
            }
        }

        /// <summary>
        /// Handle multiple filters request.
        /// </summary>
        /// <param name="exp">The exp of multiple filters</param>
        /// <returns>Return parsed filters</returns>
        private static string HandleMultipleFilters(string exp, string splitter)
        {
            var sb = new StringBuilder();
            string[] filters = exp.Split(splitter);

            for (int i = 0; i < filters.Length; i++)
            {
                if (i > 0) sb.Append(splitter);
                sb.Append(GetEscaped(filters[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string FindSubStringFromString(string strSource, string strStart, string strEnd, bool filter = false)
        {

            if (strSource.Contains(strStart))
            {

                int startIndex = strSource.IndexOf(strStart, 0);

                int endIndex;
                if (filter)
                {
                    var minusOne = startIndex - 1;

                    if (minusOne == 0)
                        startIndex--;
                    else
                        startIndex -= 3;

                    endIndex = strSource.IndexOf(strEnd, startIndex);
                }
                else
                {
                    endIndex = strSource.IndexOf(strEnd, ++startIndex);
                    endIndex--;
                }

                return strSource.Substring(startIndex, endIndex + 1 - startIndex);
            }

            return null;

        }

        public static string FindSubStringFromStringForNonRelational(string strSource, string strStart, string strEnd)
        {

            if (strSource.Contains(strStart))
            {

                int startIndex = strSource.IndexOf(strStart, 0);

                int endIndex;
                var minusOne = startIndex - 1;

                if (startIndex == 0) return strSource;

                else if (minusOne == 0)
                {
                    startIndex--;
                    endIndex = strSource.IndexOf(strEnd, startIndex);
                }
                else
                {
                    startIndex -= 2;
                    endIndex = strSource.IndexOf(strEnd, startIndex);

                    if (endIndex == -1)
                        endIndex = strSource.Length - 1;
                    else
                        endIndex--;
                }

                return strSource.Substring(startIndex, endIndex + 1 - startIndex);
            }

            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="subString"></param>
        /// <returns></returns>
        public static string RemoveSubStringFromString(string theString, string subString)
        {
            //Starting index of subfilter
            int index = theString.IndexOf(subString, 0);

            return index < 0 ? theString : theString.Remove(index, subString.Length);
        }
    }
}
