using AutoMapper;
using Globe.Domain.Core.Data;
using Globe.Shared.Models.Privileges;
using Microsoft.EntityFrameworkCore;

namespace Globe.Account.Service.Services.PrivilegesService.Impl
{
    /// <summary>
    /// The privileges service.
    /// </summary>
    public class PrivilegesService : IPrivilegesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivilegesService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="mapper">The auto mapper.</param>
        public PrivilegesService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the user privileges Model asynchronously.
        /// Adds user's screen and screen element privileges in Model.
        /// Adds user's column privileges and preferences in Model.
        /// </summary>
        /// <param name="userId">The user id for privileges are required.</param>
        /// <returns>An UserReadPrivilegesModel object.</returns>
        public async Task<UserReadPrivilegesModel> GetUserPrivilegesAsync(long userId)
        {
            var user = await _context.Users.AsSplitQuery()
                    .OrderBy(x => x.Id)
                    .Where(u => u.Id == userId)
                    .Include(ur => ur.UserRoles)
                        .ThenInclude(urr => urr.Role)
                        .ThenInclude(urs => urs.RoleScreens)
                        .ThenInclude(rs => rs.Screen)
                    .Include(ur => ur.UserRoles)
                        .ThenInclude(urr => urr.Role)
                    .Include(a => a.UserRoles)
                        .ThenInclude(ar => ar.Role)
                    .FirstOrDefaultAsync();

            var model = new PrivilegesModelFactory(_mapper).Build(user);

            await AddModulesAndApplicationsAsync(model);

            if (user.UserRoles.Select(x => x.Role.Id).FirstOrDefault() > 0)
                await AddAllowedApplicationAndDefaultApplication(model,
                                                                user.UserRoles.Select(x => x.Role.Id).ToList(),
                                                                (long)user.UserRoles.Select(x => x.Role.DefaultApplicationId).FirstOrDefault());

            return model;
        }

        private async Task AddAllowedApplicationAndDefaultApplication(UserReadPrivilegesModel model, List<long> roleIds, long defaultApplicationId)
        {
            //Get all applications equal to that role id.
            var allowedApplications = await _context.RoleApplications.Where(x => roleIds.Contains(x.RoleId))
                                                                     .Select(x => x.Application)
                                                                     .Distinct()
                                                                     .ToListAsync();

            //Add default application in first index. 
            var allowedApplicationNameList = allowedApplications.Where(x => x.Id == defaultApplicationId)
                                                                .Select(x => x.Name).ToList();

            //Remove the default application.
            allowedApplications.Remove(allowedApplications.FirstOrDefault(x => x.Id == defaultApplicationId));

            //Add rest of application.
            allowedApplicationNameList.AddRange(allowedApplications.Select(x => x.Name).ToList());

            model.AllowedApplications = allowedApplicationNameList;
        }

        /// <summary>
        /// Adds the modules and applications async.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A Task.</returns>
        private async Task AddModulesAndApplicationsAsync(UserReadPrivilegesModel model)
        {
            var entitiesList = await _context.Screens.Include(x => x.Application).ToListAsync();

            var screensList = entitiesList.Select(x => _mapper.Map<ScreensModel>(x)).ToList();

            model.ScreenPrivileges.ForEach(x =>
            {
                x.Application = screensList.FirstOrDefault(y => y.Id == x.ScreenId).Application;
            });

        }
    }
}
