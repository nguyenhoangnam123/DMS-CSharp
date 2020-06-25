using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Rpc.monitor
{

    public class MonitorController : RpcController
    {
        protected IAppUserService AppUserService;
        protected IOrganizationService OrganizationService;
        protected ICurrentContext CurrentContext;
        public MonitorController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext)
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }

        protected async Task<List<long>> FilterOrganization()
        {
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return new List<long>();
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            List<long> In = null;
            List<long> NotIn = null;
            foreach (var currentFilter in CurrentContext.Filters)
            {

                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "OrganizationId")
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal != null)
                        {
                            if (In == null) In = new List<long>();
                            In.Add(FilterPermissionDefinition.IdFilter.Equal.Value);
                        }
                        if (FilterPermissionDefinition.IdFilter.In != null)
                        {
                            if (In == null) In = new List<long>();
                            In.AddRange(FilterPermissionDefinition.IdFilter.In);
                        }

                        if (FilterPermissionDefinition.IdFilter.NotEqual != null)
                        {
                            if (NotIn == null) NotIn = new List<long>();
                            NotIn.Add(FilterPermissionDefinition.IdFilter.NotEqual.Value);
                        }
                        if (FilterPermissionDefinition.IdFilter.NotIn != null)
                        {
                            if (NotIn == null) NotIn = new List<long>();
                            NotIn.AddRange(FilterPermissionDefinition.IdFilter.NotIn);
                        }
                    }
                }
            }

            if (In != null)
            {
                List<string> InPaths = Organizations.Where(o => In.Count == 0 || In.Contains(o.Id)).Select(o => o.Path).ToList();
                Organizations = Organizations.Where(o => InPaths.Any(p => o.Path.StartsWith(p))).ToList();
            }
            if (NotIn != null)
            {
                List<string> NotInPaths = Organizations.Where(o => NotIn.Count == 0 || NotIn.Contains(o.Id)).Select(o => o.Path).ToList();
                Organizations = Organizations.Where(o => !NotInPaths.Any(p => o.Path.StartsWith(p))).ToList();
            }

            List<long> organizationIds = Organizations.Select(o => o.Id).ToList();

            return organizationIds;
        }
        protected async Task<List<long>> FilterAppUser()
        {
            List<long> organizationIds = await FilterOrganization();
            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                OrganizationId = new IdFilter { In = organizationIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id,
            });
            List<long> AppUserIds = AppUsers.Select(a => a.Id).ToList();
            return AppUserIds;
        }

        protected int CountPlan(DateTime Date, long SalesEmployeeId, List<ERouteContentDAO> ERouteContentDAOs)
        {
            int PlanCounter = 0;
            ERouteContentDAOs = ERouteContentDAOs.Where(ec => ec.ERoute.SaleEmployeeId == SalesEmployeeId &&
               ec.ERoute.RealStartDate <= Date &&
               (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Date)
            ).ToList();
            foreach (var ERouteContent in ERouteContentDAOs)
            {
                var index = (Date - ERouteContent.ERoute.RealStartDate).Days % 28;
                if (index >= 0 && ERouteContent.ERouteContentDays.Count > index)
                {
                    if (ERouteContent.ERouteContentDays.ElementAt(index).Planned == true)
                        PlanCounter++;
                }
            }
            return PlanCounter;
        }
    }
}
