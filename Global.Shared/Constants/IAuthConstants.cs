namespace Globe.Shared.Constants
{
    /// <summary>
    /// The auth configuration property names.
    /// </summary>
    public interface IAuthConstants
    {
        /// <summary>
        /// The jwt token issuer configuration property name.
        /// </summary>
        public const string JwtIssuer = "Jwt:Issuer";

        /// <summary>
        /// The jwt secret key configuration property name.
        /// The secret key will be used to encrypt jwt token.
        /// </summary>
        public const string JwtSecretKey = "Jwt:SecretKey";

        /// <summary>
        /// The user name claim configuration property name.
        /// The user name of the user for which the token is being generated.
        /// </summary>
        public const string UserName = nameof(UserName);

        /// <summary>
        /// The user ID claim configuration property ID.
        /// The user ID of the user for which the token is being generated.
        /// </summary>
        public const string UserId = nameof(UserId);

        /// <summary>
        /// The user ID claim configuration property ID.
        /// The user ID of the user for which the token is being generated.
        /// </summary>
        public const string OrganizationIds = nameof(OrganizationIds);

        /// <summary>
        /// The privileges claim name.
        /// </summary>
        public const string Privileges = nameof(Privileges);

        /// <summary>
        /// The corelation id.
        /// </summary>
        public const string CorelationId = nameof(CorelationId);

        /// <summary>
        /// The IsSuperUser check.
        /// </summary>
        public const string IsSuperUser = nameof(IsSuperUser);

        /// <summary>
        /// The Applications claim name.
        /// </summary>
        public const string Applications = nameof(Applications);

        /// <summary>
        /// The scope name.
        /// </summary>
        public const string Scope = "scope";

        /// <summary>
        /// The System name.
        /// </summary>
        public const string System = "sys";

        /// <summary>
        /// The TMA name.
        /// </summary>
        public const string TMA = "tma";

        /// <summary>
        /// Location code claim to add in tma jwt token
        /// </summary>
        public const string LocationCode = nameof(LocationCode);
    }
}
