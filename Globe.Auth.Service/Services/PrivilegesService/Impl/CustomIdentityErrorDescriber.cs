using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Identity;

namespace Globe.Account.Service.Services.PrivilegesService.Impl
{
    /// <summary>
    /// The custom identity error describer.
    /// Following error describer will be used to return identity errors as message keys.
    /// These keys will be used by front end to translate errors in different languages.
    /// </summary>
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        /// <summary>
        /// Default error.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = MsgKeys.UnknownFailure
            };
        }

        /// <summary>
        /// Concurrencie failure.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = MsgKeys.OptimisticConcurrencyFailure
            };
        }

        /// <summary>
        /// Password mismatch error.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = MsgKeys.PasswordIsIncorrect
            };
        }

        /// <summary>
        /// Invalid token.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = MsgKeys.InvalidToken
            };
        }

        /// <summary>
        /// Login already associated.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = nameof(LoginAlreadyAssociated),
                Description = MsgKeys.UserAlreadyExists
            };
        }

        /// <summary>
        /// Invalid user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = MsgKeys.UsernameIsIncorrect
            };
        }

        /// <summary>
        /// Invalid email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = MsgKeys.EmailIsInvalid
            };
        }

        /// <summary>
        /// Duplicate user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = MsgKeys.UserAlreadyExists
            };
        }

        /// <summary>
        /// Duplicate email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = MsgKeys.EmailIsTaken
            };
        }

        /// <summary>
        /// Invalid role name.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = MsgKeys.RoleNameInvalid
            };
        }

        /// <summary>
        /// Duplicate role name.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = MsgKeys.RoleNameTaken
            };
        }

        /// <summary>
        /// User already has password.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = MsgKeys.UserHasAPasswordSet
            };
        }

        /// <summary>
        /// User lockout not enabled.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = MsgKeys.LockoutNotEnabled
            };
        }

        /// <summary>
        /// User already in role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = MsgKeys.UserAlreadyInRole
            };
        }

        /// <summary>
        /// User not in role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = MsgKeys.UserNotInRole
            };
        }

        /// <summary>
        /// Password too short.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = MsgKeys.PasswordTooShort
            };
        }

        /// <summary>
        /// Password requires non alphanumeric.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = MsgKeys.PasswordMustHaveNonAlphanumeric
            };
        }

        /// <summary>
        /// Password requires digit.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = MsgKeys.PasswordMustHaveDigit
            };
        }

        /// <summary>
        /// Password requires lower.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = MsgKeys.PasswordMustHaveLowercase
            };
        }

        /// <summary>
        /// Password requires upper.
        /// </summary>
        /// <returns>An IdentityError.</returns>
        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = MsgKeys.PasswordMustHaveUppercase
            };
        }
    }
}
