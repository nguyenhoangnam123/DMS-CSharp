using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MAppUser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class AppUserHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "AppUser.Sync";
        public AppUserHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<AppUser>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        Selects = EventMessageSelect.ALL,
                        EntityName = new StringFilter { Equal = nameof(AppUser) },
                    };
                    List<EventMessage<AppUser>> AppUserEventMessages = await UOW.EventMessageRepository.List<AppUser>(EventMessageFilter);

                    List<AppUser> AppUsers = new List<AppUser>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<AppUser> EventMessage = AppUserEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            AppUsers.Add(EventMessage.Content);
                    }
                    try
                    {
                        List<AppUserDAO> AppUserDAOs = AppUsers.Select(au => new AppUserDAO
                        {
                            Address = au.Address,
                            Avatar = au.Avatar,
                            CreatedAt = au.CreatedAt,
                            UpdatedAt = au.UpdatedAt,
                            DeletedAt = au.DeletedAt,
                            Department = au.Department,
                            DisplayName = au.DisplayName,
                            Email = au.Email,
                            Id = au.Id,
                            OrganizationId = au.OrganizationId,
                            Phone = au.Phone,
                            PositionId = au.PositionId,
                            ProvinceId = au.ProvinceId,
                            RowId = au.RowId,
                            StatusId = au.StatusId,
                            Username = au.Username,
                            SexId = au.SexId,
                            Birthday = au.Birthday,
                        }).ToList();
                        await context.BulkMergeAsync(AppUserDAOs);
                    }
                    catch (Exception ex)
                    {

                    }
                    break;
            }
            return true;
        }
    }
}
