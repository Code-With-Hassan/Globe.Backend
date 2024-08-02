using System.Collections.Generic;

namespace Globe.Api.Gateway.Middleware
{
    /// <summary>
    /// The request validator configuration options.
    /// </summary>
    public class AuthMiddlewareSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the allowed paths.
        /// </summary>
        public List<AllowedPath> AllowedPaths { get; set; }

        /// <summary>
        /// Contains the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="method">The method.</param>
        /// <returns>A bool.</returns>
        public bool ContainPath(string path, string method)
        {
            return AllowedPaths != null &&
                    AllowedPaths.Exists(x => path.ToUpper().Contains(x.PathKeyword.ToUpper()) &&
                                                x.Methods.Exists(y => string.Compare(y, method, true) == 0));
        }
    }

    /// <summary>
    /// The allowed path.
    /// </summary>
    public class AllowedPath
    {
        /// <summary>
        /// Gets or sets the path keyword.
        /// </summary>
        public string PathKeyword { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public List<string> Methods { get; set; }
    }
}