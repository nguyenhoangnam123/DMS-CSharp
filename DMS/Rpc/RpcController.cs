using Common;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DMS.Rpc
{
    public class RpcController : ControllerBase
    {
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement()
        {
        }
    }
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public PermissionHandler(ICurrentContext CurrentContext, IConfiguration Configuration)
        {
            this.CurrentContext = CurrentContext;
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext = new DataContext(optionsBuilder.Options);
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var mvcContext = context.Resource as AuthorizationFilterContext;
            var HttpContext = mvcContext.HttpContext;
            string url = HttpContext.Request.Path.Value;
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            CurrentContext.UserId = UserId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";

            List<long> permissionIds = await
                (from p in DataContext.Permission
                 join ru in DataContext.AppUserRoleMapping on p.RoleId equals ru.RoleId
                 join ppm in DataContext.PermissionPageMapping on p.Id equals ppm.PermissionId
                 join page in DataContext.Page on ppm.PageId equals page.Id
                 where page.Path == url && ru.AppUserId == UserId
                 select p.Id).Distinct().ToListAsync();

            List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
                .Include(p => p.PermissionFieldMappings).ThenInclude(pf => pf.Field)
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();
            CurrentContext.RoleIds = PermissionDAOs.Select(p => p.RoleId).Distinct().ToList();
            CurrentContext.Filters = new Dictionary<long, List<FilterPermissionDefinition>>();
            foreach (PermissionDAO PermissionDAO in PermissionDAOs)
            {
                List<FilterPermissionDefinition> FilterPermissionDefinitions = new List<FilterPermissionDefinition>();
                CurrentContext.Filters.Add(PermissionDAO.Id, FilterPermissionDefinitions);
                foreach (PermissionFieldMappingDAO PermissionFieldMappingDAO in PermissionDAO.PermissionFieldMappings)
                {
                    FilterPermissionDefinition FilterPermissionDefinition = new FilterPermissionDefinition(PermissionFieldMappingDAO.Field.Name, PermissionFieldMappingDAO.Field.Type);
                    FilterPermissionDefinition.Value = PermissionFieldMappingDAO.Value;
                    FilterPermissionDefinitions.Add(FilterPermissionDefinition);
                }

            }
            context.Succeed(requirement);
        }
    }
}
