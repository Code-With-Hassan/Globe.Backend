using AutoMapper;
using Globe.Shared.Entities;
using Globe.Shared.Models.Privileges;

namespace Globe.Account.Service.Services.PrivilegesService.Impl
{
    /// <summary>
    /// Factory class to build respective Model counterparts.
    /// Required due to some special cases.
    /// </summary>
    public class PrivilegesModelFactory
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for PrivilegesModelFactory.
        /// </summary>
        /// <param name="mapper">IMapper interface to convert passed object using AutoMaper, if required.</param>
        public PrivilegesModelFactory(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Builds default UserReadPrivilegesModel w/out Domain.Core.Entities.User.
        /// </summary>
        /// <returns>Created UserReadPrivilegesModel object</returns>
        public UserReadPrivilegesModel Build()
        {
            return new UserReadPrivilegesModel
            {
                // Add default preferences if does not exists.
                ScreenPrivileges = new(),

                // Add default screen privileges if does not exists
                AllowedApplications = new()
            };
        }

        /// <summary>
        /// Builds UserReadPrivilegesModel from Domain.Core.Entities.User.
        /// </summary>
        /// <param name="user">The User Model to build from.</param>
        /// <returns>Created UserReadPrivilegesModel object</returns>
        public UserReadPrivilegesModel Build(UserEntity user)
        {
            var model = _mapper.Map<UserReadPrivilegesModel>(user);

            var privilegesMergeFactory = new PrivilegesMergeFactory();
            foreach (var usrRole in user.UserRoles)
            {
                if (usrRole.Role is null || usrRole.Role.IsActive is null or false)
                    continue;

                // Add screens
                foreach (var (roleScreen, rs) in from roleScreen in usrRole.Role.RoleScreens
                                                 let rs = Build(roleScreen)
                                                 select (roleScreen, rs))
                {
                    privilegesMergeFactory.Add(rs);
                }
            }

            model.ScreenPrivileges = privilegesMergeFactory.GetMerged().Where(x => x.HasAdminCustomOrReadOnlyPrivilege()).ToList();

            // Return model
            return model;
        }

        /// <summary>
        /// Builds RoleScreenModel from RoleScreen object.
        /// </summary>
        /// <param name="roleScreen">The RoleScreen Model to build from.</param>
        /// <returns>Created RoleScreenModel object</returns>
        public RoleScreenModel Build(RoleScreenEntity roleScreen)
        {
            return new RoleScreenModel()
            {
                Id = roleScreen.Id,
                ScreenName = roleScreen.Screen.ScreenName,
                ScreenPrivileges = roleScreen.Privilege,
                RoleId = roleScreen.RoleId,
                ScreenId = roleScreen.ScreenId,
                Application = _mapper.Map<ApplicationModel>(roleScreen.Screen.Application)

            };
        }
    }
}
