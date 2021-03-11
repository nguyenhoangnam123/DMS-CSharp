using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace DMS.Handlers
{
    public interface IHandler
    {
        string Name { get; }
        IRabbitManager RabbitManager { get; set; }
        void QueueBind(IModel channel, string queue, string exchange);
        Task Handle(DataContext context, string routingKey, string content);
    }

    public abstract class Handler : IHandler
    {
        public abstract string Name { get; }
        public IRabbitManager RabbitManager { get; set; }

        public abstract Task Handle(DataContext context, string routingKey, string content);

        public abstract void QueueBind(IModel channel, string queue, string exchange);

        protected void SystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = null,
                AppUser = "RABBITMQ",
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            RabbitManager.PublishSingle(new EventMessage<SystemLog>(SystemLog, SystemLog.RowId), RoutingKeyEnum.SystemLogSend);
        }

        protected void AuditLog(object newData, object oldData, string className, [CallerMemberName]string methodName = "")
        {
            AuditLog AuditLog = new AuditLog
            {
                AppUserId = null,
                AppUser = "RABBITMQ",
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                OldData = JsonConvert.SerializeObject(oldData),
                NewData = JsonConvert.SerializeObject(newData),
                Time = StaticParams.DateTimeNow,
                RowId = Guid.NewGuid(),
            };
            RabbitManager.PublishSingle(new EventMessage<AuditLog>(AuditLog, AuditLog.RowId), RoutingKeyEnum.AuditLogSend);
        }
    }
}
