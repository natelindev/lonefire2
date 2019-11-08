using System;
using System.Threading.Tasks;
using lonefire.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace lonefire.Extensions
{
    //Override default Username Validator
    public class LfUsernameValidator<TUser> : IUserValidator<TUser>
       where TUser : IdentityUser
    {
       
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager,
                                                  TUser user)
        {
            if (!user.UserName.Contains(Startup.Configuration["ReservedNames.Admin"]) 
                && !user.UserName.Contains(Startup.Configuration["ReservedNames.Admin"))
            {
                return Task.FromResult(IdentityResult.Success);
            }


            return Task.FromResult(
                     IdentityResult.Failed(new IdentityError
                     {
                         Code = "非法用户名",
                         Description = "用户名包含系统预留字段"
                     }));
        }
    }
}
