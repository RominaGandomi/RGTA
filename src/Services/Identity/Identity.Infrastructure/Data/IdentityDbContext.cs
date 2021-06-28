using Identity.Core.Entities;
using Identity.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data
{
    public class IdentityDbContext : IdentityDbContext<User, Role, int,
      IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
      IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext CurrentHttpContext => _httpContextAccessor.HttpContext;
        public int TenantProvider => (int)(_httpContextAccessor.HttpContext?.Items?.FirstOrDefault(x => x.Key?.ToString() == "TenantId").Value ?? 1);
        public Func<string> UserProvider = () => GetCurrentUser(_httpContextAccessor.HttpContext);
        public Func<DateTime> TimestampProvider { get; set; } = () => DateTime.Now;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {

        }
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Tenant> Tenant { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Tenant>().HasIndex(p => new { p.TenantId, p.TenantName }).IsUnique();
        }
        public virtual void Save()
        {
            base.SaveChanges();
        }
        public static string GetCurrentUser(HttpContext ctx)
        {
            var claim = ctx?.User?.FindFirst(ClaimTypes.Name);
            if (claim != null) return claim.Value;
            return Environment.UserName;
        }
        public override int SaveChanges()
        {
            TrackChanges();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        private void TrackChanges()
        {
            try
            {
                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                {
                    if (!(entry.Entity is IAuditEntity)) continue;
                    var audible = (IAuditEntity)entry.Entity;
                    if (entry.State != EntityState.Added)
                    {
                        if (entry.State == EntityState.Modified)
                        {
                            audible.UpdatedBy = UserProvider.Invoke();
                            audible.UpdatedOn = TimestampProvider();
                        }
                        else
                        {
                            audible.DeletedBy = UserProvider.Invoke();
                            audible.DeletedOn = TimestampProvider();
                        }
                    }
                    else
                    {
                        audible.CreatedBy = UserProvider.Invoke();
                        audible.CreatedOn = TimestampProvider();
                    }

                    if ((entry.Entity is IFullAuditedEntity fullAudible))
                    {
                        if (entry.State == EntityState.Added)
                        {
                            fullAudible.VersionId = 1;
                        }

                        if (entry.State == EntityState.Modified)
                        {
                            fullAudible.VersionId += 1;
                        }
                    }

                    #region Tenant Conf

                    if (entry.Entity is IMultiTenant multiTenant)
                    {
                        if (multiTenant.TenantId > 1)
                            multiTenant.TenantId = multiTenant.TenantId;
                        else
                            multiTenant.TenantId = TenantProvider;
                    }

                    #endregion
                }

            }
            catch (StackOverflowException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while tracking Data" + ex.Message);
            }
        }
    }
}
