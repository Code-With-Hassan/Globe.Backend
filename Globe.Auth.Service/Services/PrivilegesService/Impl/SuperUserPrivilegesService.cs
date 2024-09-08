using AutoMapper;
using Globe.Domain.Core.Data;
using Globe.Shared.Models.Privileges;
using Microsoft.EntityFrameworkCore;

namespace Globe.Account.Service.Services.PrivilegesService.Impl
{
    /// <summary>
    /// The super user privileges service.
    /// </summary>
    public class SuperUserPrivilegesService : ISuperUserPrivilegesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperUserPrivilegesService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="mapper">The auto mapper.</param>
        public SuperUserPrivilegesService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the super user privileges Model async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>An UserReadPrivilegesModel object.</returns>
        public async Task<UserReadPrivilegesModel> GetSuperUserPrivilegesAsync(long userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            var model = _mapper.Map<UserReadPrivilegesModel>(user);

            //allow all screens privileges and screen element privileges
            var allScreens = await _context.Screens.Include(x => x.Application)
                .Where(x => x.ScreenPrivilege == true)
                .ToListAsync();

            model.ScreenPrivileges = allScreens.Select(x =>
            {
                return new RoleScreenModel
                {
                    Id = x.Id,
                    ScreenName = x.ScreenName,
                    ScreenPrivileges = RoleScreenModel.ADMIN_PRIVILEGE,
                    RoleId = 0,
                    ScreenId = x.Id,
                    Application = _mapper.Map<ApplicationModel>(x.Application),
                };
            }).ToList();

            model.AllowedApplications = await _context.Applications.Select(x => x.Name).ToListAsync();

            return model;
        }
    }
}
