using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MNotification;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;

namespace DMS.Rpc.notification
{
    

    public class NotificationController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private INotificationService NotificationService;
        private ICurrentContext CurrentContext;
        public NotificationController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            INotificationService NotificationService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.NotificationService = NotificationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NotificationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
            NotificationFilter = NotificationService.ToFilter(NotificationFilter);
            int count = await NotificationService.Count(NotificationFilter);
            return count;
        }

        [Route(NotificationRoute.List), HttpPost]
        public async Task<ActionResult<List<Notification_NotificationDTO>>> List([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
            NotificationFilter = NotificationService.ToFilter(NotificationFilter);
            List<Notification> Notifications = await NotificationService.List(NotificationFilter);
            List<Notification_NotificationDTO> Notification_NotificationDTOs = Notifications
                .Select(c => new Notification_NotificationDTO(c)).ToList();
            return Notification_NotificationDTOs;
        }

        [Route(NotificationRoute.Get), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Get([FromBody]Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = await NotificationService.Get(Notification_NotificationDTO.Id);
            return new Notification_NotificationDTO(Notification);
        }

        [Route(NotificationRoute.Create), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Create([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Create(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }

        [Route(NotificationRoute.Update), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Update([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Update(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }

        [Route(NotificationRoute.Delete), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Delete([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Delete(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }
        
        [Route(NotificationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter = NotificationService.ToFilter(NotificationFilter);
            NotificationFilter.Id = new IdFilter { In = Ids };
            NotificationFilter.Selects = NotificationSelect.Id;
            NotificationFilter.Skip = 0;
            NotificationFilter.Take = int.MaxValue;

            List<Notification> Notifications = await NotificationService.List(NotificationFilter);
            Notifications = await NotificationService.BulkDelete(Notifications);
            return true;
        }
        
        [Route(NotificationRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Notification> Notifications = new List<Notification>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Notifications);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int TitleColumn = 1 + StartColumn;
                int ContentColumn = 2 + StartColumn;
                int OrganizationIdColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string TitleValue = worksheet.Cells[i + StartRow, TitleColumn].Value?.ToString();
                    string ContentValue = worksheet.Cells[i + StartRow, ContentColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    
                    Notification Notification = new Notification();
                    Notification.Title = TitleValue;
                    Notification.Content = ContentValue;
                    
                    Notifications.Add(Notification);
                }
            }
            Notifications = await NotificationService.Import(Notifications);
            if (Notifications.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Notifications.Count; i++)
                {
                    Notification Notification = Notifications[i];
                    if (!Notification.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Notification.Errors.ContainsKey(nameof(Notification.Id)))
                            Error += Notification.Errors[nameof(Notification.Id)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.Title)))
                            Error += Notification.Errors[nameof(Notification.Title)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.Content)))
                            Error += Notification.Errors[nameof(Notification.Content)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(NotificationRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Notification
                var NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
                NotificationFilter.Skip = 0;
                NotificationFilter.Take = int.MaxValue;
                NotificationFilter = NotificationService.ToFilter(NotificationFilter);
                List<Notification> Notifications = await NotificationService.List(NotificationFilter);

                var NotificationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Title",
                        "Content",
                        "AppUserId",
                        "StatusId",
                    }
                };
                List<object[]> NotificationData = new List<object[]>();
                for (int i = 0; i < Notifications.Count; i++)
                {
                    var Notification = Notifications[i];
                    NotificationData.Add(new Object[]
                    {
                        Notification.Id,
                        Notification.Title,
                        Notification.Content,
                    });
                }
                excel.GenerateWorksheet("Notification", NotificationHeaders, NotificationData);
                #endregion
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Notification.xlsx");
        }

        [Route(NotificationRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Notification
                var NotificationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Title",
                        "Content",
                        "OrganizationId",
                        "AppUserId",
                        "StatusId",
                    }
                };
                List<object[]> NotificationData = new List<object[]>();
                excel.GenerateWorksheet("Notification", NotificationHeaders, NotificationData);
                #endregion
                
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "Password",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "Position",
                        "Department",
                        "OrganizationId",
                        "SexId",
                        "StatusId",
                        "Avatar",
                        "Birthday",
                        "ProvinceId",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.Position,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.SexId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.Birthday,
                        AppUser.ProvinceId,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Notification.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter = NotificationService.ToFilter(NotificationFilter);
            if (Id == 0)
            {

            }
            else
            {
                NotificationFilter.Id = new IdFilter { Equal = Id };
                int count = await NotificationService.Count(NotificationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Notification ConvertDTOToEntity(Notification_NotificationDTO Notification_NotificationDTO)
        {
            Notification Notification = new Notification();
            Notification.Id = Notification_NotificationDTO.Id;
            Notification.Title = Notification_NotificationDTO.Title;
            Notification.Content = Notification_NotificationDTO.Content;
            Notification.OrganizationId = Notification_NotificationDTO.OrganizationId;
            Notification.BaseLanguage = CurrentContext.Language;
            return Notification;
        }

        private NotificationFilter ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter.Selects = NotificationSelect.ALL;
            NotificationFilter.Skip = Notification_NotificationFilterDTO.Skip;
            NotificationFilter.Take = Notification_NotificationFilterDTO.Take;
            NotificationFilter.OrderBy = Notification_NotificationFilterDTO.OrderBy;
            NotificationFilter.OrderType = Notification_NotificationFilterDTO.OrderType;

            NotificationFilter.Id = Notification_NotificationFilterDTO.Id;
            NotificationFilter.Title = Notification_NotificationFilterDTO.Title;
            NotificationFilter.Content = Notification_NotificationFilterDTO.Content;
            NotificationFilter.OrganizationId = Notification_NotificationFilterDTO.OrganizationId;
            return NotificationFilter;
        }

        [Route(NotificationRoute.FilterListOrganization), HttpPost]
        public async Task<List<Notification_OrganizationDTO>> FilterListOrganization([FromBody] Notification_OrganizationFilterDTO Notification_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Notification_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Notification_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Notification_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Notification_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Notification_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Notification_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Notification_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Notification_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Notification_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Notification_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Notification_OrganizationDTO> Notification_OrganizationDTOs = Organizations
                .Select(x => new Notification_OrganizationDTO(x)).ToList();
            return Notification_OrganizationDTOs;
        }

        [Route(NotificationRoute.SingleListOrganization), HttpPost]
        public async Task<List<Notification_OrganizationDTO>> SingleListOrganization([FromBody] Notification_OrganizationFilterDTO Notification_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Notification_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Notification_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Notification_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Notification_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Notification_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Notification_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = Notification_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Notification_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Notification_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Notification_OrganizationDTO> Notification_OrganizationDTOs = Organizations
                .Select(x => new Notification_OrganizationDTO(x)).ToList();
            return Notification_OrganizationDTOs;
        }

    }
}

