using lonefire.Data;
using lonefire.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

public class SeedData
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SeedData> _logger;
    public SeedData(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<SeedData> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAdminUser()
    {
        _context.Database.EnsureCreated();

        var user = new ApplicationUser
        {
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            LockoutEnabled = false,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        if (!_context.Roles.Any(r => r.Name == Constants.AdministratorsRole))
        {
            try
            {
                var AdminRole = new IdentityRole<Guid>
                {
                    Name = Constants.AdministratorsRole,
                    NormalizedName = Constants.AdministratorsRole.ToUpper()
                };
                _context.Roles.Add(AdminRole);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                _logger.LogError("admin role seeding failed.");
            }
        }
        
        if (!_context.Users.Any(u => u.UserName == user.UserName))
        {
            var result = await _userManager.CreateAsync(user, "admin123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.AdministratorsRole);
            }
            else
            {
                _logger.LogError("admin user seeding failed.");
            }
        }
    }
}