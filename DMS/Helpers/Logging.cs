using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Helpers
{
    public interface ILogging : IServiceScoped
    {
        Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName]string methodName = "");
        Task<bool> CreateSystemLog(Exception ex, string className, [CallerMemberName]string methodName = "");
    }
    public class Logging : ILogging
    {
        private ICurrentContext CurrentContext;
        private IRabbitManager RabbitManager;
        private IUOW UOW;
        public Logging(IConfiguration Configuration,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IUOW UOW)
        {
            this.CurrentContext = CurrentContext;
            this.RabbitManager = RabbitManager;
            this.UOW = UOW;
        }
        public async Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "")
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            AuditLog AuditLog = new AuditLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = AppUser.DisplayName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                NewData = JsonConvert.SerializeObject(newData),
                Time = StaticParams.DateTimeNow
            };

            RabbitManager.Publish(AuditLog, RoutingKeyEnum.AuditLog);
            return true;

        }
        public async Task<bool> CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = AppUser.DisplayName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString()
            };

            RabbitManager.Publish(SystemLog, RoutingKeyEnum.SystemLog);
            return true;
        }
    }
}
