using System;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace lonefire.Extensions
{
    //Override default Username Validator to allow arbitrary username rules
    public class LfUsernameValidator<TUser> : IUserValidator<TUser>
       where TUser : ApplicationUser
    {
        private readonly IStringLocalizer _localizer;

        public LfUsernameValidator(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager,
                                                  TUser user)
        {
            if (!user.UserName.Contains(Constants.AdminName) 
                && !user.UserName.Contains(Constants.EmptyUserName))
            {
                return Task.FromResult(IdentityResult.Success);
            }


            return Task.FromResult(
                     IdentityResult.Failed(new IdentityError
                     {
                         Code = _localizer["Invalid Username"],
                         Description = _localizer["Username contains reserved field"]
                     }));
        }
    }
}
