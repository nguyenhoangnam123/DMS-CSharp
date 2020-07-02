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
