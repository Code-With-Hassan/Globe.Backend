using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Globe.Account.Service.Services
{
    /// <summary>
    /// The default data protector token provider.
    /// </summary>
    public class DefaultDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        /// <summary>
        /// Initializes a new instance of the DefaultDataProtectorTokenProvider class.
        /// </summary>
        /// <param name="dataProtectionProvider">The data protection provider.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public DefaultDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DefaultDataProtectorTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger) { }
    }

    /// <summary>
    /// The default data protector token provider options.
    /// </summary>
    public class DefaultDataProtectorTokenProviderOptions : DataProtectionTokenProviderOptions { }
}