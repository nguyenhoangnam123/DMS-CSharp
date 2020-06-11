using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IERoutePerformanceRepository
    {
        Task<bool> Save(ERoutePerformance ERoutePerformance);
    }
    public class ERoutePerformanceRepository : IERoutePerformanceRepository
    {
        private DataContext DataContext;
        public ERoutePerformanceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }
        public async Task<bool> Save(ERoutePerformance ERoutePerformance)
        {
            ERoutePerformanceDAO ERoutePerformanceDAO = await DataContext.ERoutePerformance
                .Where(ep => ep.Date == ERoutePerformance.Date && ep.SaleEmployeeId == ERoutePerformance.SaleEmployeeId).FirstOrDefaultAsync();
            if (ERoutePerformanceDAO == null)
            {
                ERoutePerformanceDAO = new ERoutePerformanceDAO
                {
                    Date = ERoutePerformance.Date,
                    PlanCounter = ERoutePerformance.PlanCounter,
                    SaleEmployeeId = ERoutePerformance.SaleEmployeeId,
                };
                DataContext.ERoutePerformance.Add(ERoutePerformanceDAO);
            }
            else
            {
                ERoutePerformanceDAO.PlanCounter = ERoutePerformance.PlanCounter;
            }
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
