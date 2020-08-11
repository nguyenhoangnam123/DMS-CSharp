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
using DMS.Services.MPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MPriceListType;
using DMS.Services.MSalesOrderType;
using DMS.Services.MProduct;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Enums;
using DMS.Services.MProvince;

namespace DMS.Rpc.price_list
{
    public partial class PriceListController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IItemService ItemService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IPriceListTypeService PriceListTypeService;
        private ISalesOrderTypeService SalesOrderTypeService;
        private IStatusService StatusService;
        private IPriceListService PriceListService;
        private IProductTypeService ProductTypeService;
        private IProvinceService ProvinceService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public PriceListController(
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IStoreGroupingService StoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IPriceListTypeService PriceListTypeService,
            ISalesOrderTypeService SalesOrderTypeService,
            IStatusService StatusService,
            IPriceListService PriceListService,
            IProvinceService ProvinceService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.ItemService = ItemService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.PriceListTypeService = PriceListTypeService;
            this.SalesOrderTypeService = SalesOrderTypeService;
            this.StatusService = StatusService;
            this.PriceListService = PriceListService;
            this.ProvinceService = ProvinceService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            int count = await PriceListService.Count(PriceListFilter);
            return count;
        }

        [Route(PriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListDTO>>> List([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            List<PriceList_PriceListDTO> PriceList_PriceListDTOs = PriceLists
                .Select(c => new PriceList_PriceListDTO(c)).ToList();
            return PriceList_PriceListDTOs;
        }

        [Route(PriceListRoute.Get), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Get([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = await PriceListService.Get(PriceList_PriceListDTO.Id);
            return new PriceList_PriceListDTO(PriceList);
        }

        [Route(PriceListRoute.Create), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Create([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Create(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Update), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Update([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Update(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Delete), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Delete([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Delete(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            PriceListFilter.Id = new IdFilter { In = Ids };
            PriceListFilter.Selects = PriceListSelect.Id;
            PriceListFilter.Skip = 0;
            PriceListFilter.Take = int.MaxValue;

            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            PriceLists = await PriceListService.BulkDelete(PriceLists);
            if (PriceLists.Any(x => !x.IsValidated))
                return BadRequest(PriceLists.Where(x => !x.IsValidated));
            return true;
        }

        [Route(PriceListRoute.ExportTemplateItem), HttpPost]
        public async Task<FileResult> ExportTemplate()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var ItemFilter = new ItemFilter
            {
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name,
                Skip = 0,
                Take = int.MaxValue,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var Items = await ItemService.List(ItemFilter);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var InventoryHeaders = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã sản phẩm",
                        "Giá bán"
                    }
                };
                List<object[]> data = new List<object[]>();
                data.Add(new object[]
                {
                    
                });
                excel.GenerateWorksheet("Gia ban", InventoryHeaders, data);

                data.Clear();
                var PriceListHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã sản phẩm",
                        "Tên sản phẩm",
                    }
                };
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    data.Add(new Object[]
                    {
                        Item.Code,
                        Item.Name,
                    });
                }
                
                excel.GenerateWorksheet("San pham", PriceListHeader, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Template_PriceList_Item.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            if (Id == 0)
            {

            }
            else
            {
                PriceListFilter.Id = new IdFilter { Equal = Id };
                int count = await PriceListService.Count(PriceListFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private PriceList ConvertDTOToEntity(PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            PriceList PriceList = new PriceList();
            PriceList.Id = PriceList_PriceListDTO.Id;
            PriceList.Code = PriceList_PriceListDTO.Code;
            PriceList.Name = PriceList_PriceListDTO.Name;
            PriceList.StartDate = PriceList_PriceListDTO.StartDate;
            PriceList.EndDate = PriceList_PriceListDTO.EndDate;
            PriceList.StatusId = PriceList_PriceListDTO.StatusId;
            PriceList.OrganizationId = PriceList_PriceListDTO.OrganizationId;
            PriceList.PriceListTypeId = PriceList_PriceListDTO.PriceListTypeId;
            PriceList.SalesOrderTypeId = PriceList_PriceListDTO.SalesOrderTypeId;
            PriceList.Organization = PriceList_PriceListDTO.Organization == null ? null : new Organization
            {
                Id = PriceList_PriceListDTO.Organization.Id,
                Code = PriceList_PriceListDTO.Organization.Code,
                Name = PriceList_PriceListDTO.Organization.Name,
                ParentId = PriceList_PriceListDTO.Organization.ParentId,
                Path = PriceList_PriceListDTO.Organization.Path,
                Level = PriceList_PriceListDTO.Organization.Level,
                StatusId = PriceList_PriceListDTO.Organization.StatusId,
                Phone = PriceList_PriceListDTO.Organization.Phone,
                Email = PriceList_PriceListDTO.Organization.Email,
                Address = PriceList_PriceListDTO.Organization.Address,
            };
            PriceList.PriceListType = PriceList_PriceListDTO.PriceListType == null ? null : new PriceListType
            {
                Id = PriceList_PriceListDTO.PriceListType.Id,
                Code = PriceList_PriceListDTO.PriceListType.Code,
                Name = PriceList_PriceListDTO.PriceListType.Name,
            };
            PriceList.SalesOrderType = PriceList_PriceListDTO.SalesOrderType == null ? null : new SalesOrderType
            {
                Id = PriceList_PriceListDTO.SalesOrderType.Id,
                Code = PriceList_PriceListDTO.SalesOrderType.Code,
                Name = PriceList_PriceListDTO.SalesOrderType.Name,
            };
            PriceList.Status = PriceList_PriceListDTO.Status == null ? null : new Status
            {
                Id = PriceList_PriceListDTO.Status.Id,
                Code = PriceList_PriceListDTO.Status.Code,
                Name = PriceList_PriceListDTO.Status.Name,
            };
            PriceList.BaseLanguage = CurrentContext.Language;
            return PriceList;
        }

        private PriceListFilter ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter.Selects = PriceListSelect.ALL;
            PriceListFilter.Skip = PriceList_PriceListFilterDTO.Skip;
            PriceListFilter.Take = PriceList_PriceListFilterDTO.Take;
            PriceListFilter.OrderBy = PriceList_PriceListFilterDTO.OrderBy;
            PriceListFilter.OrderType = PriceList_PriceListFilterDTO.OrderType;

            PriceListFilter.Id = PriceList_PriceListFilterDTO.Id;
            PriceListFilter.Code = PriceList_PriceListFilterDTO.Code;
            PriceListFilter.Name = PriceList_PriceListFilterDTO.Name;
            PriceListFilter.StartDate = PriceList_PriceListFilterDTO.StartDate;
            PriceListFilter.EndDate = PriceList_PriceListFilterDTO.EndDate;
            PriceListFilter.StatusId = PriceList_PriceListFilterDTO.StatusId;
            PriceListFilter.OrganizationId = PriceList_PriceListFilterDTO.OrganizationId;
            PriceListFilter.PriceListTypeId = PriceList_PriceListFilterDTO.PriceListTypeId;
            PriceListFilter.SalesOrderTypeId = PriceList_PriceListFilterDTO.SalesOrderTypeId;
            PriceListFilter.CreatedAt = PriceList_PriceListFilterDTO.CreatedAt;
            PriceListFilter.UpdatedAt = PriceList_PriceListFilterDTO.UpdatedAt;
            return PriceListFilter;
        }
    }
}

