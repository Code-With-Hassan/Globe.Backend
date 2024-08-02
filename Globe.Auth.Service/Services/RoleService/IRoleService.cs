using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globe.Account.Service.Services.RoleService
{
    public interface IRoleService
    {
        Task AssignRoleAsync(string userId, string role);
        Task EnsureRoleExistsAsync(string role);
    }
}
