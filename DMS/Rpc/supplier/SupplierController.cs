using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MWard;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.supplier
{
    public class SupplierController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public SupplierController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            IWardService WardService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.WardService = WardService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SupplierRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            int count = await SupplierService.Count(SupplierFilter);
            return count;
        }

        [Route(SupplierRoute.List), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> List([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Get), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Get([FromBody]Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = await SupplierService.Get(Supplier_SupplierDTO.Id);
            return new Supplier_SupplierDTO(Supplier);
        }

        //[Route(SupplierRoute.Create), HttpPost]
        //public async Task<ActionResult<Supplier_SupplierDTO>> Create([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    if (!await HasPermission(Supplier_SupplierDTO.Id))
        //        return Forbid();

        //    Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
        //    Supplier = await SupplierService.Create(Supplier);
        //    Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
        //    if (Supplier.IsValidated)
        //        return Supplier_SupplierDTO;
        //    else
        //        return BadRequest(Supplier_SupplierDTO);
        //}

        //[Route(SupplierRoute.Update), HttpPost]
        //public async Task<ActionResult<Supplier_SupplierDTO>> Update([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    if (!await HasPermission(Supplier_SupplierDTO.Id))
        //        return Forbid();

        //    Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
        //    Supplier = await SupplierService.Update(Supplier);
        //    Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
        //    if (Supplier.IsValidated)
        //        return Supplier_SupplierDTO;
        //    else
        //        return BadRequest(Supplier_SupplierDTO);
        //}

        //[Route(SupplierRoute.Delete), HttpPost]
        //public async Task<ActionResult<Supplier_SupplierDTO>> Delete([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    if (!await HasPermission(Supplier_SupplierDTO.Id))
        //        return Forbid();

        //    Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
        //    Supplier = await SupplierService.Delete(Supplier);
        //    Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
        //    if (Supplier.IsValidated)
        //        return Supplier_SupplierDTO;
        //    else
        //        return BadRequest(Supplier_SupplierDTO);
        //}

        //[Route(SupplierRoute.BulkDelete), HttpPost]
        //public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    SupplierFilter SupplierFilter = new SupplierFilter();
        //    SupplierFilter.Id = new IdFilter { In = Ids };
        //    SupplierFilter.Selects = SupplierSelect.Id;
        //    SupplierFilter.Skip = 0;
        //    SupplierFilter.Take = int.MaxValue;

        //    List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
        //    Suppliers = await SupplierService.BulkDelete(Suppliers);
        //    if (Suppliers.Any(x => !x.IsValidated))
        //        return BadRequest(Suppliers.Where(x => !x.IsValidated));
        //    return true;
        //}

        private async Task<bool> HasPermission(long Id)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            if (Id == 0)
            {

            }
            else
            {
                SupplierFilter.Id = new IdFilter { Equal = Id };
                int count = await SupplierService.Count(SupplierFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Supplier ConvertDTOToEntity(Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            Supplier Supplier = new Supplier();
            Supplier.Id = Supplier_SupplierDTO.Id;
            Supplier.Code = Supplier_SupplierDTO.Code;
            Supplier.Name = Supplier_SupplierDTO.Name;
            Supplier.TaxCode = Supplier_SupplierDTO.TaxCode;
            Supplier.Phone = Supplier_SupplierDTO.Phone;
            Supplier.Email = Supplier_SupplierDTO.Email;
            Supplier.Address = Supplier_SupplierDTO.Address;
            Supplier.ProvinceId = Supplier_SupplierDTO.ProvinceId;
            Supplier.DistrictId = Supplier_SupplierDTO.DistrictId;
            Supplier.WardId = Supplier_SupplierDTO.WardId;
            Supplier.OwnerName = Supplier_SupplierDTO.OwnerName;
            Supplier.PersonInChargeId = Supplier_SupplierDTO.PersonInChargeId;
            Supplier.Description = Supplier_SupplierDTO.Description;
            Supplier.StatusId = Supplier_SupplierDTO.StatusId;
            Supplier.Province = Supplier_SupplierDTO.Province == null ? null : new Province
            {
                Id = Supplier_SupplierDTO.Province.Id,
                Code = Supplier_SupplierDTO.Province.Code,
                Name = Supplier_SupplierDTO.Province.Name,
                Priority = Supplier_SupplierDTO.Province.Priority,
                StatusId = Supplier_SupplierDTO.Province.StatusId
            };
            Supplier.District = Supplier_SupplierDTO.District == null ? null : new District
            {
                Id = Supplier_SupplierDTO.District.Id,
                Code = Supplier_SupplierDTO.District.Code,
                Name = Supplier_SupplierDTO.District.Name,
                Priority = Supplier_SupplierDTO.District.Priority,
                ProvinceId = Supplier_SupplierDTO.District.ProvinceId,
                StatusId = Supplier_SupplierDTO.District.StatusId
            };
            Supplier.Ward = Supplier_SupplierDTO.Ward == null ? null : new Ward
            {
                Id = Supplier_SupplierDTO.Ward.Id,
                Code = Supplier_SupplierDTO.Ward.Code,
                Name = Supplier_SupplierDTO.Ward.Name,
                Priority = Supplier_SupplierDTO.Ward.Priority,
                DistrictId = Supplier_SupplierDTO.Ward.DistrictId,
                StatusId = Supplier_SupplierDTO.Ward.StatusId
            };
            Supplier.PersonInCharge = Supplier_SupplierDTO.PersonInCharge == null ? null : new AppUser
            {
                Id = Supplier_SupplierDTO.PersonInCharge.Id,
                DisplayName = Supplier_SupplierDTO.PersonInCharge.DisplayName,
                Email = Supplier_SupplierDTO.PersonInCharge.Email,
                Phone = Supplier_SupplierDTO.PersonInCharge.Phone,
                Address = Supplier_SupplierDTO.PersonInCharge.Phone,
                StatusId = Supplier_SupplierDTO.PersonInCharge.StatusId
            };
            Supplier.Status = Supplier_SupplierDTO.Status == null ? null : new Status
            {
                Id = Supplier_SupplierDTO.Status.Id,
                Code = Supplier_SupplierDTO.Status.Code,
                Name = Supplier_SupplierDTO.Status.Name,
            };
            Supplier.BaseLanguage = CurrentContext.Language;
            return Supplier;
        }

        private SupplierFilter ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Skip = Supplier_SupplierFilterDTO.Skip;
            SupplierFilter.Take = Supplier_SupplierFilterDTO.Take;
            SupplierFilter.OrderBy = Supplier_SupplierFilterDTO.OrderBy;
            SupplierFilter.OrderType = Supplier_SupplierFilterDTO.OrderType;

            SupplierFilter.Id = Supplier_SupplierFilterDTO.Id;
            SupplierFilter.Code = Supplier_SupplierFilterDTO.Code;
            SupplierFilter.Name = Supplier_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Supplier_SupplierFilterDTO.TaxCode;
            SupplierFilter.Phone = Supplier_SupplierFilterDTO.Phone;
            SupplierFilter.Email = Supplier_SupplierFilterDTO.Email;
            SupplierFilter.Address = Supplier_SupplierFilterDTO.Address;
            SupplierFilter.ProvinceId = Supplier_SupplierFilterDTO.ProvinceId;
            SupplierFilter.DistrictId = Supplier_SupplierFilterDTO.DistrictId;
            SupplierFilter.WardId = Supplier_SupplierFilterDTO.WardId;
            SupplierFilter.OwnerName = Supplier_SupplierFilterDTO.OwnerName;
            SupplierFilter.PersonInChargeId = Supplier_SupplierFilterDTO.PersonInChargeId;
            SupplierFilter.Description = Supplier_SupplierFilterDTO.Description;
            SupplierFilter.StatusId = Supplier_SupplierFilterDTO.StatusId;
            SupplierFilter.UpdatedTime = Supplier_SupplierFilterDTO.UpdatedTime;
            return SupplierFilter;
        }

        [Route(SupplierRoute.SingleListPersonInCharge), HttpPost]
        public async Task<List<Supplier_AppUserDTO>> SingleListAppUser([FromBody] Supplier_AppUserFilterDTO Supplier_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Supplier_AppUserFilterDTO.Id;
            AppUserFilter.Username = Supplier_AppUserFilterDTO.Username;
            AppUserFilter.Password = Supplier_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Supplier_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Supplier_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Supplier_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Supplier_AppUserDTO> Supplier_AppUserDTOs = AppUsers
                .Select(x => new Supplier_AppUserDTO(x)).ToList();
            return Supplier_AppUserDTOs;
        }
        [Route(SupplierRoute.SingleListDistrict), HttpPost]
        public async Task<List<Supplier_DistrictDTO>> SingleListDistrict([FromBody] Supplier_DistrictFilterDTO Supplier_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Supplier_DistrictFilterDTO.Id;
            DistrictFilter.Name = Supplier_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Supplier_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Supplier_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Supplier_DistrictDTO> Supplier_DistrictDTOs = Districts
                .Select(x => new Supplier_DistrictDTO(x)).ToList();
            return Supplier_DistrictDTOs;
        }
        [Route(SupplierRoute.SingleListProvince), HttpPost]
        public async Task<List<Supplier_ProvinceDTO>> SingleListProvince([FromBody] Supplier_ProvinceFilterDTO Supplier_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Supplier_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Supplier_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Supplier_ProvinceDTO> Supplier_ProvinceDTOs = Provinces
                .Select(x => new Supplier_ProvinceDTO(x)).ToList();
            return Supplier_ProvinceDTOs;
        }
        [Route(SupplierRoute.SingleListStatus), HttpPost]
        public async Task<List<Supplier_StatusDTO>> SingleListStatus([FromBody] Supplier_StatusFilterDTO Supplier_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Supplier_StatusDTO> Supplier_StatusDTOs = Statuses
                .Select(x => new Supplier_StatusDTO(x)).ToList();
            return Supplier_StatusDTOs;
        }
        [Route(SupplierRoute.SingleListWard), HttpPost]
        public async Task<List<Supplier_WardDTO>> SingleListWard([FromBody] Supplier_WardFilterDTO Supplier_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Supplier_WardFilterDTO.Id;
            WardFilter.Name = Supplier_WardFilterDTO.Name;
            WardFilter.DistrictId = Supplier_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Supplier_WardDTO> Supplier_WardDTOs = Wards
                .Select(x => new Supplier_WardDTO(x)).ToList();
            return Supplier_WardDTOs;
        }

        [Route(SupplierRoute.FilterListStatus), HttpPost]
        public async Task<List<Supplier_StatusDTO>> FilterListStatus([FromBody] Supplier_StatusFilterDTO Supplier_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Supplier_StatusDTO> Supplier_StatusDTOs = Statuses
                .Select(x => new Supplier_StatusDTO(x)).ToList();
            return Supplier_StatusDTOs;
        }
    }
}

