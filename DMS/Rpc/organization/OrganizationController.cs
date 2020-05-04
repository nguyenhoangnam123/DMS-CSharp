using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MSex;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MWard;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.organization
{
    public class OrganizationRoute : Root
    {
        public const string Master = Module + "/organization/organization-master";
        public const string Detail = Module + "/organization/organization-detail";
        private const string Default = Rpc + Module + "/organization";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(OrganizationFilter.Id), FieldType.ID },
            { nameof(OrganizationFilter.Code), FieldType.STRING },
            { nameof(OrganizationFilter.Name), FieldType.STRING },
            { nameof(OrganizationFilter.ParentId), FieldType.ID },
            { nameof(OrganizationFilter.Path), FieldType.STRING },
            { nameof(OrganizationFilter.Level), FieldType.LONG },
            { nameof(OrganizationFilter.StatusId), FieldType.ID },
            { nameof(OrganizationFilter.Phone), FieldType.STRING },
            { nameof(OrganizationFilter.Email), FieldType.STRING },
            { nameof(OrganizationFilter.Address), FieldType.STRING },
        };
    }

    public class OrganizationController : RpcController
    {
        private IStatusService StatusService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public OrganizationController(
            IStatusService StatusService,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(OrganizationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            int count = await OrganizationService.Count(OrganizationFilter);
            return count;
        }

        [Route(OrganizationRoute.List), HttpPost]
        public async Task<ActionResult<List<Organization_OrganizationDTO>>> List([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Organization_OrganizationDTO> Organization_OrganizationDTOs = Organizations
                .Select(c => new Organization_OrganizationDTO(c)).ToList();
            return Organization_OrganizationDTOs;
        }

        [Route(OrganizationRoute.Get), HttpPost]
        public async Task<ActionResult<Organization_OrganizationDTO>> Get([FromBody]Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Organization_OrganizationDTO.Id))
                return Forbid();

            Organization Organization = await OrganizationService.Get(Organization_OrganizationDTO.Id);
            return new Organization_OrganizationDTO(Organization);
        }

        [Route(OrganizationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);

            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] {  "Mã tổ chức","Tên tổ chức","Mã tổ chức"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    data.Add(new Object[]
                    {
                        Organization.Name,
                        Organization.Code,
                        Organization.Parent?.Code ?? "",
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, data);
                excel.Save();
            }

            return File(memoryStream.ToArray(), "application/octet-stream", "Organization.xlsx");

        }
        private async Task<bool> HasPermission(long Id)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            if (Id == 0)
            {

            }
            else
            {
                OrganizationFilter.Id = new IdFilter { Equal = Id };
                int count = await OrganizationService.Count(OrganizationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Organization ConvertDTOToEntity(Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            Organization Organization = new Organization();
            Organization.Id = Organization_OrganizationDTO.Id;
            Organization.Code = Organization_OrganizationDTO.Code;
            Organization.Name = Organization_OrganizationDTO.Name;
            Organization.ParentId = Organization_OrganizationDTO.ParentId;
            Organization.Path = Organization_OrganizationDTO.Path;
            Organization.Level = Organization_OrganizationDTO.Level;
            Organization.StatusId = Organization_OrganizationDTO.StatusId;
            Organization.Phone = Organization_OrganizationDTO.Phone;
            Organization.Address = Organization_OrganizationDTO.Address;
            Organization.Email = Organization_OrganizationDTO.Email;
            Organization.Parent = Organization_OrganizationDTO.Parent == null ? null : new Organization
            {
                Id = Organization_OrganizationDTO.Parent.Id,
                Code = Organization_OrganizationDTO.Parent.Code,
                Name = Organization_OrganizationDTO.Parent.Name,
                ParentId = Organization_OrganizationDTO.Parent.ParentId,
                Path = Organization_OrganizationDTO.Parent.Path,
                Level = Organization_OrganizationDTO.Parent.Level,
                StatusId = Organization_OrganizationDTO.Parent.StatusId,
                Phone = Organization_OrganizationDTO.Parent.Phone,
                Address = Organization_OrganizationDTO.Parent.Address,
                Email = Organization_OrganizationDTO.Parent.Email,
            };
            Organization.Status = Organization_OrganizationDTO.Status == null ? null : new Status
            {
                Id = Organization_OrganizationDTO.Status.Id,
                Code = Organization_OrganizationDTO.Status.Code,
                Name = Organization_OrganizationDTO.Status.Name,
            };
            Organization.AppUsers = Organization_OrganizationDTO.AppUsers?
                .Select(x => new AppUser
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Email = x.Email,
                    Phone = x.Phone,
                    Department = x.Department,
                    SexId = x.SexId,
                    StatusId = x.StatusId,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            Organization.BaseLanguage = CurrentContext.Language;
            return Organization;
        }

        private OrganizationFilter ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = Organization_OrganizationFilterDTO.OrderBy;
            OrganizationFilter.OrderType = Organization_OrganizationFilterDTO.OrderType;

            OrganizationFilter.Id = Organization_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Organization_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Organization_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Organization_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Organization_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Organization_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Organization_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Organization_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = Organization_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = Organization_OrganizationFilterDTO.Email;
            return OrganizationFilter;
        }

        [Route(OrganizationRoute.SingleListOrganization), HttpPost]
        public async Task<List<Organization_OrganizationDTO>> SingleListOrganization([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Organization_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Organization_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Organization_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Organization_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Organization_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Organization_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Organization_OrganizationDTO> Organization_OrganizationDTOs = Organizations
                .Select(x => new Organization_OrganizationDTO(x)).ToList();
            return Organization_OrganizationDTOs;
        }
        [Route(OrganizationRoute.SingleListStatus), HttpPost]
        public async Task<List<Organization_StatusDTO>> SingleListStatus([FromBody] Organization_StatusFilterDTO Organization_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Organization_StatusFilterDTO.Id;
            StatusFilter.Code = Organization_StatusFilterDTO.Code;
            StatusFilter.Name = Organization_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Organization_StatusDTO> Organization_StatusDTOs = Statuses
                .Select(x => new Organization_StatusDTO(x)).ToList();
            return Organization_StatusDTOs;
        }
        [Route(OrganizationRoute.SingleListAppUser), HttpPost]
        public async Task<List<Organization_AppUserDTO>> SingleListAppUser([FromBody] Organization_AppUserFilterDTO Organization_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Organization_AppUserFilterDTO.Id;
            AppUserFilter.Username = Organization_AppUserFilterDTO.Username;
            AppUserFilter.Password = Organization_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Organization_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Organization_AppUserFilterDTO.Address;
            AppUserFilter.Email = Organization_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Organization_AppUserFilterDTO.Phone;
            AppUserFilter.Department = Organization_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Organization_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = Organization_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Organization_AppUserDTO> Organization_AppUserDTOs = AppUsers
                .Select(x => new Organization_AppUserDTO(x)).ToList();
            return Organization_AppUserDTOs;
        }
    }
}

