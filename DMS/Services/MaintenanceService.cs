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
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => sc.CheckOutAt.HasValue == false && sc.CheckInAt.HasValue).ToListAsync();
            foreach(StoreCheckingDAO StoreCheckingDAO in StoreCheckingDAOs)
            {
                StoreCheckingDAO.CheckOutAt = StoreCheckingDAO.CheckInAt.Value.Date.AddDays(1).AddSeconds(-1);
            }
            await DataContext.SaveChangesAsync();
        }
    }
}
