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
using DMS.Services.MPromotion;
using DMS.Services.MOrganization;
using DMS.Services.MPromotionType;
using DMS.Services.MStatus;
using DMS.Services.MPromotionCombo;
using DMS.Services.MPromotionDirectSalesOrder;
using DMS.Services.MPromotionDiscountType;
using DMS.Services.MPromotionPolicy;
using DMS.Services.MPromotionSamePrice;
using DMS.Services.MStoreGrouping;
using DMS.Services.MPromotionStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;
using DMS.Services.MPromotionStoreType;
using DMS.Services.MPromotionStore;

namespace DMS.Rpc.promotion
{
    public partial class PromotionController : RpcController
    {
        [Route(PromotionRoute.FilterListOrganization), HttpPost]
        public async Task<List<Promotion_OrganizationDTO>> FilterListOrganization([FromBody] Promotion_OrganizationFilterDTO Promotion_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Promotion_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Promotion_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Promotion_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Promotion_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Promotion_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Promotion_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Promotion_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Promotion_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Promotion_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Promotion_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Promotion_OrganizationDTO> Promotion_OrganizationDTOs = Organizations
                .Select(x => new Promotion_OrganizationDTO(x)).ToList();
            return Promotion_OrganizationDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionType), HttpPost]
        public async Task<List<Promotion_PromotionTypeDTO>> FilterListPromotionType([FromBody] Promotion_PromotionTypeFilterDTO Promotion_PromotionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionTypeFilter PromotionTypeFilter = new PromotionTypeFilter();
            PromotionTypeFilter.Skip = 0;
            PromotionTypeFilter.Take = int.MaxValue;
            PromotionTypeFilter.Take = 20;
            PromotionTypeFilter.OrderBy = PromotionTypeOrder.Id;
            PromotionTypeFilter.OrderType = OrderType.ASC;
            PromotionTypeFilter.Selects = PromotionTypeSelect.ALL;

            List<PromotionType> PromotionTypes = await PromotionTypeService.List(PromotionTypeFilter);
            List<Promotion_PromotionTypeDTO> Promotion_PromotionTypeDTOs = PromotionTypes
                .Select(x => new Promotion_PromotionTypeDTO(x)).ToList();
            return Promotion_PromotionTypeDTOs;
        }
        [Route(PromotionRoute.FilterListStatus), HttpPost]
        public async Task<List<Promotion_StatusDTO>> FilterListStatus([FromBody] Promotion_StatusFilterDTO Promotion_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Promotion_StatusDTO> Promotion_StatusDTOs = Statuses
                .Select(x => new Promotion_StatusDTO(x)).ToList();
            return Promotion_StatusDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionCombo), HttpPost]
        public async Task<List<Promotion_PromotionComboDTO>> FilterListPromotionCombo([FromBody] Promotion_PromotionComboFilterDTO Promotion_PromotionComboFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionComboFilter PromotionComboFilter = new PromotionComboFilter();
            PromotionComboFilter.Skip = 0;
            PromotionComboFilter.Take = 20;
            PromotionComboFilter.OrderBy = PromotionComboOrder.Id;
            PromotionComboFilter.OrderType = OrderType.ASC;
            PromotionComboFilter.Selects = PromotionComboSelect.ALL;
            PromotionComboFilter.Id = Promotion_PromotionComboFilterDTO.Id;
            PromotionComboFilter.Note = Promotion_PromotionComboFilterDTO.Note;
            PromotionComboFilter.Name = Promotion_PromotionComboFilterDTO.Name;
            PromotionComboFilter.PromotionId = Promotion_PromotionComboFilterDTO.PromotionId;

            List<PromotionCombo> PromotionCombos = await PromotionComboService.List(PromotionComboFilter);
            List<Promotion_PromotionComboDTO> Promotion_PromotionComboDTOs = PromotionCombos
                .Select(x => new Promotion_PromotionComboDTO(x)).ToList();
            return Promotion_PromotionComboDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionDirectSalesOrder), HttpPost]
        public async Task<List<Promotion_PromotionDirectSalesOrderDTO>> FilterListPromotionDirectSalesOrder([FromBody] Promotion_PromotionDirectSalesOrderFilterDTO Promotion_PromotionDirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter = new PromotionDirectSalesOrderFilter();
            PromotionDirectSalesOrderFilter.Skip = 0;
            PromotionDirectSalesOrderFilter.Take = 20;
            PromotionDirectSalesOrderFilter.OrderBy = PromotionDirectSalesOrderOrder.Id;
            PromotionDirectSalesOrderFilter.OrderType = OrderType.ASC;
            PromotionDirectSalesOrderFilter.Selects = PromotionDirectSalesOrderSelect.ALL;
            PromotionDirectSalesOrderFilter.Id = Promotion_PromotionDirectSalesOrderFilterDTO.Id;
            PromotionDirectSalesOrderFilter.Name = Promotion_PromotionDirectSalesOrderFilterDTO.Name;
            PromotionDirectSalesOrderFilter.PromotionId = Promotion_PromotionDirectSalesOrderFilterDTO.PromotionId;
            PromotionDirectSalesOrderFilter.Note = Promotion_PromotionDirectSalesOrderFilterDTO.Note;
            PromotionDirectSalesOrderFilter.FromValue = Promotion_PromotionDirectSalesOrderFilterDTO.FromValue;
            PromotionDirectSalesOrderFilter.ToValue = Promotion_PromotionDirectSalesOrderFilterDTO.ToValue;
            PromotionDirectSalesOrderFilter.PromotionDiscountTypeId = Promotion_PromotionDirectSalesOrderFilterDTO.PromotionDiscountTypeId;
            PromotionDirectSalesOrderFilter.DiscountPercentage = Promotion_PromotionDirectSalesOrderFilterDTO.DiscountPercentage;
            PromotionDirectSalesOrderFilter.DiscountValue = Promotion_PromotionDirectSalesOrderFilterDTO.DiscountValue;

            List<PromotionDirectSalesOrder> PromotionDirectSalesOrders = await PromotionDirectSalesOrderService.List(PromotionDirectSalesOrderFilter);
            List<Promotion_PromotionDirectSalesOrderDTO> Promotion_PromotionDirectSalesOrderDTOs = PromotionDirectSalesOrders
                .Select(x => new Promotion_PromotionDirectSalesOrderDTO(x)).ToList();
            return Promotion_PromotionDirectSalesOrderDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionDiscountType), HttpPost]
        public async Task<List<Promotion_PromotionDiscountTypeDTO>> FilterListPromotionDiscountType([FromBody] Promotion_PromotionDiscountTypeFilterDTO Promotion_PromotionDiscountTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionDiscountTypeFilter PromotionDiscountTypeFilter = new PromotionDiscountTypeFilter();
            PromotionDiscountTypeFilter.Skip = 0;
            PromotionDiscountTypeFilter.Take = int.MaxValue;
            PromotionDiscountTypeFilter.Take = 20;
            PromotionDiscountTypeFilter.OrderBy = PromotionDiscountTypeOrder.Id;
            PromotionDiscountTypeFilter.OrderType = OrderType.ASC;
            PromotionDiscountTypeFilter.Selects = PromotionDiscountTypeSelect.ALL;

            List<PromotionDiscountType> PromotionDiscountTypes = await PromotionDiscountTypeService.List(PromotionDiscountTypeFilter);
            List<Promotion_PromotionDiscountTypeDTO> Promotion_PromotionDiscountTypeDTOs = PromotionDiscountTypes
                .Select(x => new Promotion_PromotionDiscountTypeDTO(x)).ToList();
            return Promotion_PromotionDiscountTypeDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionPolicy), HttpPost]
        public async Task<List<Promotion_PromotionPolicyDTO>> FilterListPromotionPolicy([FromBody] Promotion_PromotionPolicyFilterDTO Promotion_PromotionPolicyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionPolicyFilter PromotionPolicyFilter = new PromotionPolicyFilter();
            PromotionPolicyFilter.Skip = 0;
            PromotionPolicyFilter.Take = int.MaxValue;
            PromotionPolicyFilter.Take = 20;
            PromotionPolicyFilter.OrderBy = PromotionPolicyOrder.Id;
            PromotionPolicyFilter.OrderType = OrderType.ASC;
            PromotionPolicyFilter.Selects = PromotionPolicySelect.ALL;

            List<PromotionPolicy> PromotionPolicies = await PromotionPolicyService.List(PromotionPolicyFilter);
            List<Promotion_PromotionPolicyDTO> Promotion_PromotionPolicyDTOs = PromotionPolicies
                .Select(x => new Promotion_PromotionPolicyDTO(x)).ToList();
            return Promotion_PromotionPolicyDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionSamePrice), HttpPost]
        public async Task<List<Promotion_PromotionSamePriceDTO>> FilterListPromotionSamePrice([FromBody] Promotion_PromotionSamePriceFilterDTO Promotion_PromotionSamePriceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionSamePriceFilter PromotionSamePriceFilter = new PromotionSamePriceFilter();
            PromotionSamePriceFilter.Skip = 0;
            PromotionSamePriceFilter.Take = 20;
            PromotionSamePriceFilter.OrderBy = PromotionSamePriceOrder.Id;
            PromotionSamePriceFilter.OrderType = OrderType.ASC;
            PromotionSamePriceFilter.Selects = PromotionSamePriceSelect.ALL;
            PromotionSamePriceFilter.Id = Promotion_PromotionSamePriceFilterDTO.Id;
            PromotionSamePriceFilter.Note = Promotion_PromotionSamePriceFilterDTO.Note;
            PromotionSamePriceFilter.Name = Promotion_PromotionSamePriceFilterDTO.Name;
            PromotionSamePriceFilter.PromotionId = Promotion_PromotionSamePriceFilterDTO.PromotionId;
            PromotionSamePriceFilter.Price = Promotion_PromotionSamePriceFilterDTO.Price;

            List<PromotionSamePrice> PromotionSamePrices = await PromotionSamePriceService.List(PromotionSamePriceFilter);
            List<Promotion_PromotionSamePriceDTO> Promotion_PromotionSamePriceDTOs = PromotionSamePrices
                .Select(x => new Promotion_PromotionSamePriceDTO(x)).ToList();
            return Promotion_PromotionSamePriceDTOs;
        }
        [Route(PromotionRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<Promotion_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] Promotion_StoreGroupingFilterDTO Promotion_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = Promotion_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Promotion_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Promotion_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = Promotion_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = Promotion_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = Promotion_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = Promotion_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Promotion_StoreGroupingDTO> Promotion_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Promotion_StoreGroupingDTO(x)).ToList();
            return Promotion_StoreGroupingDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionStoreGrouping), HttpPost]
        public async Task<List<Promotion_PromotionStoreGroupingDTO>> FilterListPromotionStoreGrouping([FromBody] Promotion_PromotionStoreGroupingFilterDTO Promotion_PromotionStoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreGroupingFilter PromotionStoreGroupingFilter = new PromotionStoreGroupingFilter();
            PromotionStoreGroupingFilter.Skip = 0;
            PromotionStoreGroupingFilter.Take = 20;
            PromotionStoreGroupingFilter.OrderBy = PromotionStoreGroupingOrder.Id;
            PromotionStoreGroupingFilter.OrderType = OrderType.ASC;
            PromotionStoreGroupingFilter.Selects = PromotionStoreGroupingSelect.ALL;
            PromotionStoreGroupingFilter.Id = Promotion_PromotionStoreGroupingFilterDTO.Id;
            PromotionStoreGroupingFilter.PromotionId = Promotion_PromotionStoreGroupingFilterDTO.PromotionId;
            PromotionStoreGroupingFilter.Note = Promotion_PromotionStoreGroupingFilterDTO.Note;
            PromotionStoreGroupingFilter.FromValue = Promotion_PromotionStoreGroupingFilterDTO.FromValue;
            PromotionStoreGroupingFilter.ToValue = Promotion_PromotionStoreGroupingFilterDTO.ToValue;
            PromotionStoreGroupingFilter.PromotionDiscountTypeId = Promotion_PromotionStoreGroupingFilterDTO.PromotionDiscountTypeId;
            PromotionStoreGroupingFilter.DiscountPercentage = Promotion_PromotionStoreGroupingFilterDTO.DiscountPercentage;
            PromotionStoreGroupingFilter.DiscountValue = Promotion_PromotionStoreGroupingFilterDTO.DiscountValue;

            List<PromotionStoreGrouping> PromotionStoreGroupings = await PromotionStoreGroupingService.List(PromotionStoreGroupingFilter);
            List<Promotion_PromotionStoreGroupingDTO> Promotion_PromotionStoreGroupingDTOs = PromotionStoreGroupings
                .Select(x => new Promotion_PromotionStoreGroupingDTO(x)).ToList();
            return Promotion_PromotionStoreGroupingDTOs;
        }
        [Route(PromotionRoute.FilterListStore), HttpPost]
        public async Task<List<Promotion_StoreDTO>> FilterListStore([FromBody] Promotion_StoreFilterDTO Promotion_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Promotion_StoreFilterDTO.Id;
            StoreFilter.Code = Promotion_StoreFilterDTO.Code;
            StoreFilter.Name = Promotion_StoreFilterDTO.Name;
            StoreFilter.UnsignName = Promotion_StoreFilterDTO.UnsignName;
            StoreFilter.ParentStoreId = Promotion_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Promotion_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Promotion_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Promotion_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Promotion_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Promotion_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Promotion_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Promotion_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Promotion_StoreFilterDTO.WardId;
            StoreFilter.Address = Promotion_StoreFilterDTO.Address;
            StoreFilter.UnsignAddress = Promotion_StoreFilterDTO.UnsignAddress;
            StoreFilter.DeliveryAddress = Promotion_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Promotion_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Promotion_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Promotion_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Promotion_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Promotion_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Promotion_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Promotion_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Promotion_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Promotion_StoreDTO> Promotion_StoreDTOs = Stores
                .Select(x => new Promotion_StoreDTO(x)).ToList();
            return Promotion_StoreDTOs;
        }
        [Route(PromotionRoute.FilterListStoreType), HttpPost]
        public async Task<List<Promotion_StoreTypeDTO>> FilterListStoreType([FromBody] Promotion_StoreTypeFilterDTO Promotion_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Promotion_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Promotion_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Promotion_StoreTypeFilterDTO.Name;
            StoreTypeFilter.ColorId = Promotion_StoreTypeFilterDTO.ColorId;
            StoreTypeFilter.StatusId = Promotion_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Promotion_StoreTypeDTO> Promotion_StoreTypeDTOs = StoreTypes
                .Select(x => new Promotion_StoreTypeDTO(x)).ToList();
            return Promotion_StoreTypeDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionStoreType), HttpPost]
        public async Task<List<Promotion_PromotionStoreTypeDTO>> FilterListPromotionStoreType([FromBody] Promotion_PromotionStoreTypeFilterDTO Promotion_PromotionStoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreTypeFilter PromotionStoreTypeFilter = new PromotionStoreTypeFilter();
            PromotionStoreTypeFilter.Skip = 0;
            PromotionStoreTypeFilter.Take = 20;
            PromotionStoreTypeFilter.OrderBy = PromotionStoreTypeOrder.Id;
            PromotionStoreTypeFilter.OrderType = OrderType.ASC;
            PromotionStoreTypeFilter.Selects = PromotionStoreTypeSelect.ALL;
            PromotionStoreTypeFilter.Id = Promotion_PromotionStoreTypeFilterDTO.Id;
            PromotionStoreTypeFilter.PromotionId = Promotion_PromotionStoreTypeFilterDTO.PromotionId;
            PromotionStoreTypeFilter.Note = Promotion_PromotionStoreTypeFilterDTO.Note;
            PromotionStoreTypeFilter.FromValue = Promotion_PromotionStoreTypeFilterDTO.FromValue;
            PromotionStoreTypeFilter.ToValue = Promotion_PromotionStoreTypeFilterDTO.ToValue;
            PromotionStoreTypeFilter.PromotionDiscountTypeId = Promotion_PromotionStoreTypeFilterDTO.PromotionDiscountTypeId;
            PromotionStoreTypeFilter.DiscountPercentage = Promotion_PromotionStoreTypeFilterDTO.DiscountPercentage;
            PromotionStoreTypeFilter.DiscountValue = Promotion_PromotionStoreTypeFilterDTO.DiscountValue;

            List<PromotionStoreType> PromotionStoreTypes = await PromotionStoreTypeService.List(PromotionStoreTypeFilter);
            List<Promotion_PromotionStoreTypeDTO> Promotion_PromotionStoreTypeDTOs = PromotionStoreTypes
                .Select(x => new Promotion_PromotionStoreTypeDTO(x)).ToList();
            return Promotion_PromotionStoreTypeDTOs;
        }
        [Route(PromotionRoute.FilterListPromotionStore), HttpPost]
        public async Task<List<Promotion_PromotionStoreDTO>> FilterListPromotionStore([FromBody] Promotion_PromotionStoreFilterDTO Promotion_PromotionStoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreFilter PromotionStoreFilter = new PromotionStoreFilter();
            PromotionStoreFilter.Skip = 0;
            PromotionStoreFilter.Take = 20;
            PromotionStoreFilter.OrderBy = PromotionStoreOrder.Id;
            PromotionStoreFilter.OrderType = OrderType.ASC;
            PromotionStoreFilter.Selects = PromotionStoreSelect.ALL;
            PromotionStoreFilter.Id = Promotion_PromotionStoreFilterDTO.Id;
            PromotionStoreFilter.PromotionId = Promotion_PromotionStoreFilterDTO.PromotionId;
            PromotionStoreFilter.Note = Promotion_PromotionStoreFilterDTO.Note;
            PromotionStoreFilter.FromValue = Promotion_PromotionStoreFilterDTO.FromValue;
            PromotionStoreFilter.ToValue = Promotion_PromotionStoreFilterDTO.ToValue;
            PromotionStoreFilter.PromotionDiscountTypeId = Promotion_PromotionStoreFilterDTO.PromotionDiscountTypeId;
            PromotionStoreFilter.DiscountPercentage = Promotion_PromotionStoreFilterDTO.DiscountPercentage;
            PromotionStoreFilter.DiscountValue = Promotion_PromotionStoreFilterDTO.DiscountValue;

            List<PromotionStore> PromotionStores = await PromotionStoreService.List(PromotionStoreFilter);
            List<Promotion_PromotionStoreDTO> Promotion_PromotionStoreDTOs = PromotionStores
                .Select(x => new Promotion_PromotionStoreDTO(x)).ToList();
            return Promotion_PromotionStoreDTOs;
        }

        [Route(PromotionRoute.SingleListOrganization), HttpPost]
        public async Task<List<Promotion_OrganizationDTO>> SingleListOrganization([FromBody] Promotion_OrganizationFilterDTO Promotion_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Promotion_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Promotion_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Promotion_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Promotion_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Promotion_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Promotion_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Promotion_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Promotion_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Promotion_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Promotion_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Promotion_OrganizationDTO> Promotion_OrganizationDTOs = Organizations
                .Select(x => new Promotion_OrganizationDTO(x)).ToList();
            return Promotion_OrganizationDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionType), HttpPost]
        public async Task<List<Promotion_PromotionTypeDTO>> SingleListPromotionType([FromBody] Promotion_PromotionTypeFilterDTO Promotion_PromotionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionTypeFilter PromotionTypeFilter = new PromotionTypeFilter();
            PromotionTypeFilter.Skip = 0;
            PromotionTypeFilter.Take = int.MaxValue;
            PromotionTypeFilter.Take = 20;
            PromotionTypeFilter.OrderBy = PromotionTypeOrder.Id;
            PromotionTypeFilter.OrderType = OrderType.ASC;
            PromotionTypeFilter.Selects = PromotionTypeSelect.ALL;

            List<PromotionType> PromotionTypes = await PromotionTypeService.List(PromotionTypeFilter);
            List<Promotion_PromotionTypeDTO> Promotion_PromotionTypeDTOs = PromotionTypes
                .Select(x => new Promotion_PromotionTypeDTO(x)).ToList();
            return Promotion_PromotionTypeDTOs;
        }
        [Route(PromotionRoute.SingleListStatus), HttpPost]
        public async Task<List<Promotion_StatusDTO>> SingleListStatus([FromBody] Promotion_StatusFilterDTO Promotion_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Promotion_StatusDTO> Promotion_StatusDTOs = Statuses
                .Select(x => new Promotion_StatusDTO(x)).ToList();
            return Promotion_StatusDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionCombo), HttpPost]
        public async Task<List<Promotion_PromotionComboDTO>> SingleListPromotionCombo([FromBody] Promotion_PromotionComboFilterDTO Promotion_PromotionComboFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionComboFilter PromotionComboFilter = new PromotionComboFilter();
            PromotionComboFilter.Skip = 0;
            PromotionComboFilter.Take = 20;
            PromotionComboFilter.OrderBy = PromotionComboOrder.Id;
            PromotionComboFilter.OrderType = OrderType.ASC;
            PromotionComboFilter.Selects = PromotionComboSelect.ALL;
            PromotionComboFilter.Id = Promotion_PromotionComboFilterDTO.Id;
            PromotionComboFilter.Note = Promotion_PromotionComboFilterDTO.Note;
            PromotionComboFilter.Name = Promotion_PromotionComboFilterDTO.Name;
            PromotionComboFilter.PromotionId = Promotion_PromotionComboFilterDTO.PromotionId;

            List<PromotionCombo> PromotionCombos = await PromotionComboService.List(PromotionComboFilter);
            List<Promotion_PromotionComboDTO> Promotion_PromotionComboDTOs = PromotionCombos
                .Select(x => new Promotion_PromotionComboDTO(x)).ToList();
            return Promotion_PromotionComboDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionDirectSalesOrder), HttpPost]
        public async Task<List<Promotion_PromotionDirectSalesOrderDTO>> SingleListPromotionDirectSalesOrder([FromBody] Promotion_PromotionDirectSalesOrderFilterDTO Promotion_PromotionDirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter = new PromotionDirectSalesOrderFilter();
            PromotionDirectSalesOrderFilter.Skip = 0;
            PromotionDirectSalesOrderFilter.Take = 20;
            PromotionDirectSalesOrderFilter.OrderBy = PromotionDirectSalesOrderOrder.Id;
            PromotionDirectSalesOrderFilter.OrderType = OrderType.ASC;
            PromotionDirectSalesOrderFilter.Selects = PromotionDirectSalesOrderSelect.ALL;
            PromotionDirectSalesOrderFilter.Id = Promotion_PromotionDirectSalesOrderFilterDTO.Id;
            PromotionDirectSalesOrderFilter.Name = Promotion_PromotionDirectSalesOrderFilterDTO.Name;
            PromotionDirectSalesOrderFilter.PromotionId = Promotion_PromotionDirectSalesOrderFilterDTO.PromotionId;
            PromotionDirectSalesOrderFilter.Note = Promotion_PromotionDirectSalesOrderFilterDTO.Note;
            PromotionDirectSalesOrderFilter.FromValue = Promotion_PromotionDirectSalesOrderFilterDTO.FromValue;
            PromotionDirectSalesOrderFilter.ToValue = Promotion_PromotionDirectSalesOrderFilterDTO.ToValue;
            PromotionDirectSalesOrderFilter.PromotionDiscountTypeId = Promotion_PromotionDirectSalesOrderFilterDTO.PromotionDiscountTypeId;
            PromotionDirectSalesOrderFilter.DiscountPercentage = Promotion_PromotionDirectSalesOrderFilterDTO.DiscountPercentage;
            PromotionDirectSalesOrderFilter.DiscountValue = Promotion_PromotionDirectSalesOrderFilterDTO.DiscountValue;

            List<PromotionDirectSalesOrder> PromotionDirectSalesOrders = await PromotionDirectSalesOrderService.List(PromotionDirectSalesOrderFilter);
            List<Promotion_PromotionDirectSalesOrderDTO> Promotion_PromotionDirectSalesOrderDTOs = PromotionDirectSalesOrders
                .Select(x => new Promotion_PromotionDirectSalesOrderDTO(x)).ToList();
            return Promotion_PromotionDirectSalesOrderDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionDiscountType), HttpPost]
        public async Task<List<Promotion_PromotionDiscountTypeDTO>> SingleListPromotionDiscountType([FromBody] Promotion_PromotionDiscountTypeFilterDTO Promotion_PromotionDiscountTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionDiscountTypeFilter PromotionDiscountTypeFilter = new PromotionDiscountTypeFilter();
            PromotionDiscountTypeFilter.Skip = 0;
            PromotionDiscountTypeFilter.Take = int.MaxValue;
            PromotionDiscountTypeFilter.Take = 20;
            PromotionDiscountTypeFilter.OrderBy = PromotionDiscountTypeOrder.Id;
            PromotionDiscountTypeFilter.OrderType = OrderType.ASC;
            PromotionDiscountTypeFilter.Selects = PromotionDiscountTypeSelect.ALL;

            List<PromotionDiscountType> PromotionDiscountTypes = await PromotionDiscountTypeService.List(PromotionDiscountTypeFilter);
            List<Promotion_PromotionDiscountTypeDTO> Promotion_PromotionDiscountTypeDTOs = PromotionDiscountTypes
                .Select(x => new Promotion_PromotionDiscountTypeDTO(x)).ToList();
            return Promotion_PromotionDiscountTypeDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionPolicy), HttpPost]
        public async Task<List<Promotion_PromotionPolicyDTO>> SingleListPromotionPolicy([FromBody] Promotion_PromotionPolicyFilterDTO Promotion_PromotionPolicyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionPolicyFilter PromotionPolicyFilter = new PromotionPolicyFilter();
            PromotionPolicyFilter.Skip = 0;
            PromotionPolicyFilter.Take = int.MaxValue;
            PromotionPolicyFilter.Take = 20;
            PromotionPolicyFilter.OrderBy = PromotionPolicyOrder.Id;
            PromotionPolicyFilter.OrderType = OrderType.ASC;
            PromotionPolicyFilter.Selects = PromotionPolicySelect.ALL;

            List<PromotionPolicy> PromotionPolicies = await PromotionPolicyService.List(PromotionPolicyFilter);
            List<Promotion_PromotionPolicyDTO> Promotion_PromotionPolicyDTOs = PromotionPolicies
                .Select(x => new Promotion_PromotionPolicyDTO(x)).ToList();
            return Promotion_PromotionPolicyDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionSamePrice), HttpPost]
        public async Task<List<Promotion_PromotionSamePriceDTO>> SingleListPromotionSamePrice([FromBody] Promotion_PromotionSamePriceFilterDTO Promotion_PromotionSamePriceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionSamePriceFilter PromotionSamePriceFilter = new PromotionSamePriceFilter();
            PromotionSamePriceFilter.Skip = 0;
            PromotionSamePriceFilter.Take = 20;
            PromotionSamePriceFilter.OrderBy = PromotionSamePriceOrder.Id;
            PromotionSamePriceFilter.OrderType = OrderType.ASC;
            PromotionSamePriceFilter.Selects = PromotionSamePriceSelect.ALL;
            PromotionSamePriceFilter.Id = Promotion_PromotionSamePriceFilterDTO.Id;
            PromotionSamePriceFilter.Note = Promotion_PromotionSamePriceFilterDTO.Note;
            PromotionSamePriceFilter.Name = Promotion_PromotionSamePriceFilterDTO.Name;
            PromotionSamePriceFilter.PromotionId = Promotion_PromotionSamePriceFilterDTO.PromotionId;
            PromotionSamePriceFilter.Price = Promotion_PromotionSamePriceFilterDTO.Price;

            List<PromotionSamePrice> PromotionSamePrices = await PromotionSamePriceService.List(PromotionSamePriceFilter);
            List<Promotion_PromotionSamePriceDTO> Promotion_PromotionSamePriceDTOs = PromotionSamePrices
                .Select(x => new Promotion_PromotionSamePriceDTO(x)).ToList();
            return Promotion_PromotionSamePriceDTOs;
        }
        [Route(PromotionRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<Promotion_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] Promotion_StoreGroupingFilterDTO Promotion_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = Promotion_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Promotion_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Promotion_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = Promotion_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = Promotion_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = Promotion_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = Promotion_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Promotion_StoreGroupingDTO> Promotion_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Promotion_StoreGroupingDTO(x)).ToList();
            return Promotion_StoreGroupingDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionStoreGrouping), HttpPost]
        public async Task<List<Promotion_PromotionStoreGroupingDTO>> SingleListPromotionStoreGrouping([FromBody] Promotion_PromotionStoreGroupingFilterDTO Promotion_PromotionStoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreGroupingFilter PromotionStoreGroupingFilter = new PromotionStoreGroupingFilter();
            PromotionStoreGroupingFilter.Skip = 0;
            PromotionStoreGroupingFilter.Take = 20;
            PromotionStoreGroupingFilter.OrderBy = PromotionStoreGroupingOrder.Id;
            PromotionStoreGroupingFilter.OrderType = OrderType.ASC;
            PromotionStoreGroupingFilter.Selects = PromotionStoreGroupingSelect.ALL;
            PromotionStoreGroupingFilter.Id = Promotion_PromotionStoreGroupingFilterDTO.Id;
            PromotionStoreGroupingFilter.PromotionId = Promotion_PromotionStoreGroupingFilterDTO.PromotionId;
            PromotionStoreGroupingFilter.Note = Promotion_PromotionStoreGroupingFilterDTO.Note;
            PromotionStoreGroupingFilter.FromValue = Promotion_PromotionStoreGroupingFilterDTO.FromValue;
            PromotionStoreGroupingFilter.ToValue = Promotion_PromotionStoreGroupingFilterDTO.ToValue;
            PromotionStoreGroupingFilter.PromotionDiscountTypeId = Promotion_PromotionStoreGroupingFilterDTO.PromotionDiscountTypeId;
            PromotionStoreGroupingFilter.DiscountPercentage = Promotion_PromotionStoreGroupingFilterDTO.DiscountPercentage;
            PromotionStoreGroupingFilter.DiscountValue = Promotion_PromotionStoreGroupingFilterDTO.DiscountValue;

            List<PromotionStoreGrouping> PromotionStoreGroupings = await PromotionStoreGroupingService.List(PromotionStoreGroupingFilter);
            List<Promotion_PromotionStoreGroupingDTO> Promotion_PromotionStoreGroupingDTOs = PromotionStoreGroupings
                .Select(x => new Promotion_PromotionStoreGroupingDTO(x)).ToList();
            return Promotion_PromotionStoreGroupingDTOs;
        }
        [Route(PromotionRoute.SingleListStore), HttpPost]
        public async Task<List<Promotion_StoreDTO>> SingleListStore([FromBody] Promotion_StoreFilterDTO Promotion_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Promotion_StoreFilterDTO.Id;
            StoreFilter.Code = Promotion_StoreFilterDTO.Code;
            StoreFilter.Name = Promotion_StoreFilterDTO.Name;
            StoreFilter.UnsignName = Promotion_StoreFilterDTO.UnsignName;
            StoreFilter.ParentStoreId = Promotion_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Promotion_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Promotion_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Promotion_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Promotion_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Promotion_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Promotion_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Promotion_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Promotion_StoreFilterDTO.WardId;
            StoreFilter.Address = Promotion_StoreFilterDTO.Address;
            StoreFilter.UnsignAddress = Promotion_StoreFilterDTO.UnsignAddress;
            StoreFilter.DeliveryAddress = Promotion_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Promotion_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Promotion_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Promotion_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Promotion_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Promotion_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Promotion_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Promotion_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Promotion_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Promotion_StoreDTO> Promotion_StoreDTOs = Stores
                .Select(x => new Promotion_StoreDTO(x)).ToList();
            return Promotion_StoreDTOs;
        }
        [Route(PromotionRoute.SingleListStoreType), HttpPost]
        public async Task<List<Promotion_StoreTypeDTO>> SingleListStoreType([FromBody] Promotion_StoreTypeFilterDTO Promotion_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Promotion_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Promotion_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Promotion_StoreTypeFilterDTO.Name;
            StoreTypeFilter.ColorId = Promotion_StoreTypeFilterDTO.ColorId;
            StoreTypeFilter.StatusId = Promotion_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Promotion_StoreTypeDTO> Promotion_StoreTypeDTOs = StoreTypes
                .Select(x => new Promotion_StoreTypeDTO(x)).ToList();
            return Promotion_StoreTypeDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionStoreType), HttpPost]
        public async Task<List<Promotion_PromotionStoreTypeDTO>> SingleListPromotionStoreType([FromBody] Promotion_PromotionStoreTypeFilterDTO Promotion_PromotionStoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreTypeFilter PromotionStoreTypeFilter = new PromotionStoreTypeFilter();
            PromotionStoreTypeFilter.Skip = 0;
            PromotionStoreTypeFilter.Take = 20;
            PromotionStoreTypeFilter.OrderBy = PromotionStoreTypeOrder.Id;
            PromotionStoreTypeFilter.OrderType = OrderType.ASC;
            PromotionStoreTypeFilter.Selects = PromotionStoreTypeSelect.ALL;
            PromotionStoreTypeFilter.Id = Promotion_PromotionStoreTypeFilterDTO.Id;
            PromotionStoreTypeFilter.PromotionId = Promotion_PromotionStoreTypeFilterDTO.PromotionId;
            PromotionStoreTypeFilter.Note = Promotion_PromotionStoreTypeFilterDTO.Note;
            PromotionStoreTypeFilter.FromValue = Promotion_PromotionStoreTypeFilterDTO.FromValue;
            PromotionStoreTypeFilter.ToValue = Promotion_PromotionStoreTypeFilterDTO.ToValue;
            PromotionStoreTypeFilter.PromotionDiscountTypeId = Promotion_PromotionStoreTypeFilterDTO.PromotionDiscountTypeId;
            PromotionStoreTypeFilter.DiscountPercentage = Promotion_PromotionStoreTypeFilterDTO.DiscountPercentage;
            PromotionStoreTypeFilter.DiscountValue = Promotion_PromotionStoreTypeFilterDTO.DiscountValue;

            List<PromotionStoreType> PromotionStoreTypes = await PromotionStoreTypeService.List(PromotionStoreTypeFilter);
            List<Promotion_PromotionStoreTypeDTO> Promotion_PromotionStoreTypeDTOs = PromotionStoreTypes
                .Select(x => new Promotion_PromotionStoreTypeDTO(x)).ToList();
            return Promotion_PromotionStoreTypeDTOs;
        }
        [Route(PromotionRoute.SingleListPromotionStore), HttpPost]
        public async Task<List<Promotion_PromotionStoreDTO>> SingleListPromotionStore([FromBody] Promotion_PromotionStoreFilterDTO Promotion_PromotionStoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionStoreFilter PromotionStoreFilter = new PromotionStoreFilter();
            PromotionStoreFilter.Skip = 0;
            PromotionStoreFilter.Take = 20;
            PromotionStoreFilter.OrderBy = PromotionStoreOrder.Id;
            PromotionStoreFilter.OrderType = OrderType.ASC;
            PromotionStoreFilter.Selects = PromotionStoreSelect.ALL;
            PromotionStoreFilter.Id = Promotion_PromotionStoreFilterDTO.Id;
            PromotionStoreFilter.PromotionId = Promotion_PromotionStoreFilterDTO.PromotionId;
            PromotionStoreFilter.Note = Promotion_PromotionStoreFilterDTO.Note;
            PromotionStoreFilter.FromValue = Promotion_PromotionStoreFilterDTO.FromValue;
            PromotionStoreFilter.ToValue = Promotion_PromotionStoreFilterDTO.ToValue;
            PromotionStoreFilter.PromotionDiscountTypeId = Promotion_PromotionStoreFilterDTO.PromotionDiscountTypeId;
            PromotionStoreFilter.DiscountPercentage = Promotion_PromotionStoreFilterDTO.DiscountPercentage;
            PromotionStoreFilter.DiscountValue = Promotion_PromotionStoreFilterDTO.DiscountValue;

            List<PromotionStore> PromotionStores = await PromotionStoreService.List(PromotionStoreFilter);
            List<Promotion_PromotionStoreDTO> Promotion_PromotionStoreDTOs = PromotionStores
                .Select(x => new Promotion_PromotionStoreDTO(x)).ToList();
            return Promotion_PromotionStoreDTOs;
        }

        [Route(PromotionRoute.CountPromotionPolicy), HttpPost]
        public async Task<long> CountPromotionPolicy([FromBody] Promotion_PromotionPolicyFilterDTO Promotion_PromotionPolicyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionPolicyFilter PromotionPolicyFilter = new PromotionPolicyFilter();
            PromotionPolicyFilter.Id = Promotion_PromotionPolicyFilterDTO.Id;
            PromotionPolicyFilter.Code = Promotion_PromotionPolicyFilterDTO.Code;
            PromotionPolicyFilter.Name = Promotion_PromotionPolicyFilterDTO.Name;

            return await PromotionPolicyService.Count(PromotionPolicyFilter);
        }

        [Route(PromotionRoute.ListPromotionPolicy), HttpPost]
        public async Task<List<Promotion_PromotionPolicyDTO>> ListPromotionPolicy([FromBody] Promotion_PromotionPolicyFilterDTO Promotion_PromotionPolicyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionPolicyFilter PromotionPolicyFilter = new PromotionPolicyFilter();
            PromotionPolicyFilter.Skip = Promotion_PromotionPolicyFilterDTO.Skip;
            PromotionPolicyFilter.Take = Promotion_PromotionPolicyFilterDTO.Take;
            PromotionPolicyFilter.OrderBy = PromotionPolicyOrder.Id;
            PromotionPolicyFilter.OrderType = OrderType.ASC;
            PromotionPolicyFilter.Selects = PromotionPolicySelect.ALL;
            PromotionPolicyFilter.Id = Promotion_PromotionPolicyFilterDTO.Id;
            PromotionPolicyFilter.Code = Promotion_PromotionPolicyFilterDTO.Code;
            PromotionPolicyFilter.Name = Promotion_PromotionPolicyFilterDTO.Name;

            List<PromotionPolicy> PromotionPolicies = await PromotionPolicyService.List(PromotionPolicyFilter);
            List<Promotion_PromotionPolicyDTO> Promotion_PromotionPolicyDTOs = PromotionPolicies
                .Select(x => new Promotion_PromotionPolicyDTO(x)).ToList();
            return Promotion_PromotionPolicyDTOs;
        }
        [Route(PromotionRoute.CountStoreGrouping), HttpPost]
        public async Task<long> CountStoreGrouping([FromBody] Promotion_StoreGroupingFilterDTO Promotion_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Id = Promotion_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Promotion_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Promotion_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = Promotion_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = Promotion_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = Promotion_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = Promotion_StoreGroupingFilterDTO.StatusId;

            return await StoreGroupingService.Count(StoreGroupingFilter);
        }

        [Route(PromotionRoute.ListStoreGrouping), HttpPost]
        public async Task<List<Promotion_StoreGroupingDTO>> ListStoreGrouping([FromBody] Promotion_StoreGroupingFilterDTO Promotion_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = Promotion_StoreGroupingFilterDTO.Skip;
            StoreGroupingFilter.Take = Promotion_StoreGroupingFilterDTO.Take;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = Promotion_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Promotion_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Promotion_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = Promotion_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = Promotion_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = Promotion_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = Promotion_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Promotion_StoreGroupingDTO> Promotion_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Promotion_StoreGroupingDTO(x)).ToList();
            return Promotion_StoreGroupingDTOs;
        }
        [Route(PromotionRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] Promotion_StoreFilterDTO Promotion_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = Promotion_StoreFilterDTO.Id;
            StoreFilter.Code = Promotion_StoreFilterDTO.Code;
            StoreFilter.Name = Promotion_StoreFilterDTO.Name;
            StoreFilter.UnsignName = Promotion_StoreFilterDTO.UnsignName;
            StoreFilter.ParentStoreId = Promotion_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Promotion_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Promotion_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Promotion_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Promotion_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Promotion_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Promotion_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Promotion_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Promotion_StoreFilterDTO.WardId;
            StoreFilter.Address = Promotion_StoreFilterDTO.Address;
            StoreFilter.UnsignAddress = Promotion_StoreFilterDTO.UnsignAddress;
            StoreFilter.DeliveryAddress = Promotion_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Promotion_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Promotion_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Promotion_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Promotion_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Promotion_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Promotion_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Promotion_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Promotion_StoreFilterDTO.StatusId;

            return await StoreService.Count(StoreFilter);
        }

        [Route(PromotionRoute.ListStore), HttpPost]
        public async Task<List<Promotion_StoreDTO>> ListStore([FromBody] Promotion_StoreFilterDTO Promotion_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = Promotion_StoreFilterDTO.Skip;
            StoreFilter.Take = Promotion_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Promotion_StoreFilterDTO.Id;
            StoreFilter.Code = Promotion_StoreFilterDTO.Code;
            StoreFilter.Name = Promotion_StoreFilterDTO.Name;
            StoreFilter.UnsignName = Promotion_StoreFilterDTO.UnsignName;
            StoreFilter.ParentStoreId = Promotion_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Promotion_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Promotion_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Promotion_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Promotion_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Promotion_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Promotion_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Promotion_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Promotion_StoreFilterDTO.WardId;
            StoreFilter.Address = Promotion_StoreFilterDTO.Address;
            StoreFilter.UnsignAddress = Promotion_StoreFilterDTO.UnsignAddress;
            StoreFilter.DeliveryAddress = Promotion_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Promotion_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Promotion_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Promotion_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Promotion_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Promotion_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Promotion_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Promotion_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Promotion_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Promotion_StoreDTO> Promotion_StoreDTOs = Stores
                .Select(x => new Promotion_StoreDTO(x)).ToList();
            return Promotion_StoreDTOs;
        }
        [Route(PromotionRoute.CountStoreType), HttpPost]
        public async Task<long> CountStoreType([FromBody] Promotion_StoreTypeFilterDTO Promotion_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = Promotion_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Promotion_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Promotion_StoreTypeFilterDTO.Name;
            StoreTypeFilter.ColorId = Promotion_StoreTypeFilterDTO.ColorId;
            StoreTypeFilter.StatusId = Promotion_StoreTypeFilterDTO.StatusId;

            return await StoreTypeService.Count(StoreTypeFilter);
        }

        [Route(PromotionRoute.ListStoreType), HttpPost]
        public async Task<List<Promotion_StoreTypeDTO>> ListStoreType([FromBody] Promotion_StoreTypeFilterDTO Promotion_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = Promotion_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = Promotion_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Promotion_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Promotion_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Promotion_StoreTypeFilterDTO.Name;
            StoreTypeFilter.ColorId = Promotion_StoreTypeFilterDTO.ColorId;
            StoreTypeFilter.StatusId = Promotion_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Promotion_StoreTypeDTO> Promotion_StoreTypeDTOs = StoreTypes
                .Select(x => new Promotion_StoreTypeDTO(x)).ToList();
            return Promotion_StoreTypeDTOs;
        }
    }
}

