using System;
using System.Threading.Tasks;
using lonefire.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace lonefire.Extensions
{
    //Override default Username Validator to allow arbitrary username rules
    public class LfUsernameValidator<TUser> : IUserValidator<TUser>
       where TUser : IdentityUser
    {
        private readonly IStringLocalizer _localizer;

        public LfUsernameValidator(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager,
                                                  TUser user)
        {
            if (!user.UserName.Contains(_localizer[Constants.AdminName]) 
                && !user.UserName.Contains(_localizer[Constants.EmptyUserName]))
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
