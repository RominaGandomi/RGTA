using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Helpers
{
    public class TenantProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantProviderMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            this._next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Invoke(HttpContext context, IdentityDbContext dbContext)
        {

            var urlHost = context.Request.Host.ToString();

            if (string.IsNullOrEmpty(urlHost))
            {
                throw new ApplicationException("Url Host must be specified");
            }

            urlHost = urlHost.Remove(urlHost.IndexOf(":", StringComparison.Ordinal), urlHost.Length - urlHost.IndexOf(":", StringComparison.Ordinal)).ToLower().Trim();

            var tenant = dbContext.Tenant.FirstOrDefault(a => a.TenantName.ToLower() == urlHost) ?? dbContext.Tenant.FirstOrDefault(x => x.TenantName == "RGTA");

            if (tenant == null)
            {
                throw new ApplicationException("tenant not found based on URL, no default found");
            }

            context.Items.Add("TENANT", tenant);
            context.Items.Add("TenantId", tenant.Id);
            _httpContextAccessor.HttpContext = context;
            return _next.Invoke(_httpContextAccessor.HttpContext);
        }
    }
}
