using Identity.Core.Entities;
using Identity.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Identity.Infrastructure.Data
{
    public class Seed
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IdentityDbContext _context;

        public Seed(
            IUserService userService, 
            UserManager<User> userManager, 
            RoleManager<Role> roleManager, 
            IdentityDbContext context)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void SeedUsers()
        {
            var usersToDeletion = _userService.GetAll();
            foreach (var user in usersToDeletion)
            {
                _userService.DeleteAsync(user);
            }

            if (!_userManager.Users.Any())
            {
                var roles = new List<Role>
                {
                    new Role{Name = "Admin"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "Member"},
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin",
                    DateOfBirth = DateTime.Now,
                    LastEnterance = DateTime.Now,
                    CreatedOn = DateTime.Now,
                };

                var adminResult = _userManager.CreateAsync(adminUser, "AdminPassword").Result;

                if (adminResult.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }).Wait();
                }

                var modUser = new User
                {
                    UserName = "Moderator",
                    DateOfBirth = DateTime.Now,
                    LastEnterance = DateTime.Now,
                    CreatedOn = DateTime.Now,
                };

                var modResult = _userManager.CreateAsync(modUser, "ModPassword").Result;

                if (modResult.Succeeded)
                {
                    var moderator = _userManager.FindByNameAsync("Moderator").Result;
                    _userManager.AddToRolesAsync(moderator, new[] { "Moderator" }).Wait();
                }

                var memberUser = new User
                {
                    UserName = "Member",
                    DateOfBirth = DateTime.Now,
                    LastEnterance = DateTime.Now,
                    CreatedOn = DateTime.Now,
                };

                var memberResult = _userManager.CreateAsync(memberUser, "password").Result;

                if (memberResult.Succeeded)
                {
                    var member = _userManager.FindByNameAsync("Member").Result;
                    _userManager.AddToRolesAsync(member, new[] { "Member" }).Wait();
                }

            }
        }
        public bool SeedTenant()
        {
            _context.Tenant.RemoveRange(_context.Tenant);
            _context.SaveChanges();

            _context.Tenant.Add(new Tenant()
            {
                TenantId = 1,
                TenantName = "RGTA"
            });
            _context.Tenant.Add(new Tenant()
            {
                TenantId = 2,
                TenantName = "X Company"
            });
            _context.Tenant.Add(new Tenant()
            {
                TenantId = 3,
                TenantName = "Y Company"
            });

            return _context.SaveChanges() > 0;
        }
    }
}
