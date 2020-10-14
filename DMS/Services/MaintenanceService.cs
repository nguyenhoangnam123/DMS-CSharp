using Common;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface IMaintenanceService : IServiceScoped
    {
        Task CleanEventMessage();
        Task CleanHangfire();
        Task CompleteStoreCheckout();
        Task CreateStoreUnchecking();
    }
    public class MaintenanceService : IMaintenanceService
    {
        private DataContext DataContext;
        public MaintenanceService(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task CleanEventMessage()
        {
            DateTime Checkpoint = StaticParams.DateTimeNow.AddDays(-1);
            await DataContext.EventMessage.Where(em => em.Time < Checkpoint).DeleteFromQueryAsync();
        }

        public async Task CleanHangfire()
        {
            var commandText = @"
                TRUNCATE TABLE [HangFire].[AggregatedCounter]
                TRUNCATE TABLE[HangFire].[Counter]
                TRUNCATE TABLE[HangFire].[JobParameter]
                TRUNCATE TABLE[HangFire].[JobQueue]
                TRUNCATE TABLE[HangFire].[List]
                TRUNCATE TABLE[HangFire].[State]
                DELETE FROM[HangFire].[Job]
                DBCC CHECKIDENT('[HangFire].[Job]', reseed, 0)
                UPDATE[HangFire].[Hash] SET Value = 1 WHERE Field = 'LastJobId'";
            var result = await DataContext.Database.ExecuteSqlRawAsync(commandText);
        }

        public async Task CompleteStoreCheckout()
        {
            DateTime Now = StaticParams.DateTimeNow;
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => sc.CheckOutAt.HasValue == false && sc.CheckInAt.HasValue).ToListAsync();
            foreach(StoreCheckingDAO StoreCheckingDAO in StoreCheckingDAOs)
            {
                StoreCheckingDAO.CheckOutAt = Now;
            }
            await DataContext.SaveChangesAsync();
        }

        public async Task CreateStoreUnchecking()
        {
            DateTime End = StaticParams.DateTimeNow.Date;
            DateTime Start = End.AddDays(-1);
            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
                .Where(ec => (!ec.ERoute.EndDate.HasValue || Start <= ec.ERoute.EndDate) && ec.ERoute.StartDate <= End)
                .Include(ec => ec.ERoute)
                .Include(ec => ec.ERouteContentDays)
                .ToListAsync();
            List<StoreUncheckingDAO> PlannedStoreUncheckingDAOs = new List<StoreUncheckingDAO>();
            List<StoreUncheckingDAO> StoreUncheckingDAOs = new List<StoreUncheckingDAO>();
            foreach(ERouteContentDAO ERouteContentDAO in ERouteContentDAOs)
            {
                StoreUncheckingDAO StoreUncheckingDAO = PlannedStoreUncheckingDAOs.Where(su =>
                    su.Date == Start &&
                    su.AppUserId == ERouteContentDAO.ERoute.SaleEmployeeId &&
                    su.StoreId == ERouteContentDAO.StoreId
                ).FirstOrDefault();
                if (StoreUncheckingDAO == null)
                {
                    if (Start >= ERouteContentDAO.ERoute.RealStartDate)
                    {
                        long gap = (Start - ERouteContentDAO.ERoute.RealStartDate).Days % 28;
                        if (ERouteContentDAO.ERouteContentDays.Any(ecd => ecd.OrderDay == gap && ecd.Planned))
                        {
                            StoreUncheckingDAO = new StoreUncheckingDAO
                            {
                                AppUserId = ERouteContentDAO.ERoute.SaleEmployeeId,
                                Date = Start,
                                StoreId = ERouteContentDAO.StoreId,
                                OrganizationId = ERouteContentDAO.ERoute.OrganizationId
                            };
                            PlannedStoreUncheckingDAOs.Add(StoreUncheckingDAO);
                        }
                    }
                }
            }
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(sc => sc.CheckOutAt.HasValue &&
                Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End).ToListAsync();
            foreach(StoreUncheckingDAO StoreUncheckingDAO in PlannedStoreUncheckingDAOs)
            {
                if (!StoreCheckingDAOs.Any(sc => sc.SaleEmployeeId == StoreUncheckingDAO.AppUserId && sc.StoreId == StoreUncheckingDAO.StoreId))
                {
                    StoreUncheckingDAOs.Add(StoreUncheckingDAO);
                }
            }

            await DataContext.StoreUnchecking.BulkInsertAsync(StoreUncheckingDAOs);
        }
    }
}
