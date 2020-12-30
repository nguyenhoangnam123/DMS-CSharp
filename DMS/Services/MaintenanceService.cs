using DMS.Common;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Enums;

namespace DMS.Services
{
    public interface IMaintenanceService : IServiceScoped
    {
        Task CleanEventMessage();
        Task CleanHangfire();
        Task Job_Checking();
        Task CompleteStoreCheckout();
        Task CreateStoreUnchecking();
        Task AutoInactive();
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

        public async Task Job_Checking()
        {
            await CompleteStoreCheckout();
            await CreateStoreUnchecking();
        }

        public async Task CompleteStoreCheckout()
        {
            DateTime Now = StaticParams.DateTimeNow;
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => sc.CheckOutAt.HasValue == false && sc.CheckInAt.HasValue).ToListAsync();
            foreach (StoreCheckingDAO StoreCheckingDAO in StoreCheckingDAOs)
            {
                StoreCheckingDAO.CheckOutAt = Now;
            }
            await DataContext.SaveChangesAsync();
        }

        public async Task CreateStoreUnchecking()
        {
            DateTime x = new DateTime(2020, 10, 01, 16, 59, 0);
            for (DateTime i = x; i < StaticParams.DateTimeNow; i = i.AddDays(1))
            {
                DateTime End = i;
                DateTime Start = /*End.AddDays(-1);*/ End.AddDays(-1).AddMinutes(1);
                List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
                    .Where(ec => (!ec.ERoute.EndDate.HasValue || Start <= ec.ERoute.EndDate) && ec.ERoute.StartDate <= End)
                    .Include(ec => ec.ERoute)
                    .Include(ec => ec.ERouteContentDays)
                    .Where(x => x.ERoute.RequestStateId == RequestStateEnum.APPROVED.Id && x.ERoute.StatusId == StatusEnum.ACTIVE.Id)
                    .ToListAsync();
                foreach (var ERouteContentDAO in ERouteContentDAOs)
                {
                    ERouteContentDAO.RealStartDate = ERouteContentDAO.ERoute.RealStartDate;
                }
                ERouteContentDAOs = ERouteContentDAOs.Distinct().ToList();
                List<StoreUncheckingDAO> PlannedStoreUncheckingDAOs = new List<StoreUncheckingDAO>();
                List<StoreUncheckingDAO> StoreUncheckingDAOs = new List<StoreUncheckingDAO>();
                foreach (ERouteContentDAO ERouteContentDAO in ERouteContentDAOs)
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
                                    Date = End,
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
                foreach (StoreUncheckingDAO StoreUncheckingDAO in PlannedStoreUncheckingDAOs)
                {
                    if (!StoreCheckingDAOs.Any(sc => sc.SaleEmployeeId == StoreUncheckingDAO.AppUserId && sc.StoreId == StoreUncheckingDAO.StoreId))
                    {
                        StoreUncheckingDAOs.Add(StoreUncheckingDAO);
                    }
                }

                StoreUncheckingDAOs = StoreUncheckingDAOs.Distinct().ToList();
                await DataContext.StoreUnchecking.BulkInsertAsync(StoreUncheckingDAOs);
            }
            

        }

        public async Task AutoInactive()
        {
            var Now = StaticParams.DateTimeNow;
            await DataContext.ERoute.Where(x => x.EndDate.HasValue && x.EndDate.Value < Now).UpdateFromQueryAsync(x => new ERouteDAO { StatusId = StatusEnum.INACTIVE.Id });
            await DataContext.PriceList.Where(x => x.EndDate.HasValue && x.EndDate.Value < Now).UpdateFromQueryAsync(x => new PriceListDAO { StatusId = StatusEnum.INACTIVE.Id });
            await DataContext.WorkflowDefinition.Where(x => x.EndDate.HasValue && x.EndDate.Value < Now).UpdateFromQueryAsync(x => new WorkflowDefinitionDAO { StatusId = StatusEnum.INACTIVE.Id });
            await DataContext.Survey.Where(x => x.EndAt.HasValue && x.EndAt.Value < Now).UpdateFromQueryAsync(x => new SurveyDAO { StatusId = StatusEnum.INACTIVE.Id });
            await DataContext.PromotionCode.Where(x => x.EndDate.HasValue && x.EndDate.Value < Now).UpdateFromQueryAsync(x => new PromotionCodeDAO { StatusId = StatusEnum.INACTIVE.Id });
        }
    }
}
