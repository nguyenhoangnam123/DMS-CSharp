using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DMS.Helpers
{
    public interface ILogging : IServiceScoped
    {
        Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName]string methodName = "");
        Task CreateSystemLog(Exception ex, string className, [CallerMemberName]string methodName = "");
    }
    public class Logging : ILogging
    {
        private ICurrentContext CurrentContext;
        private IRabbitManager RabbitManager;
        private IUOW UOW;
        public Logging(
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager
            )
        {
            this.CurrentContext = CurrentContext;
            this.RabbitManager = RabbitManager;
        }
        public async Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "")
        {
            AuditLog AuditLog = new AuditLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = CurrentContext.UserName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                OldData = JsonConvert.SerializeObject(oldData),
                NewData = JsonConvert.SerializeObject(newData),
                Time = StaticParams.DateTimeNow,
                RowId = Guid.NewGuid(),
            };
            RabbitManager.PublishSingle(AuditLog, RoutingKeyEnum.AuditLogSend);
            return true;
        }
        public async Task CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = CurrentContext.UserName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            RabbitManager.PublishSingle(SystemLog, RoutingKeyEnum.SystemLogSend);
            throw new MessageException(ex);
        }
    }
}
