using DMS.Common;
using DMS.Models;
using DMS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MOrganization;
using DMS.Services.MAppUser;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreStatus;

namespace DMS.Rpc.reports.report_store.report_store_state_change
{
    public class ReportStoreStateChangeController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreStatusService StoreStatusService;
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        public ReportStoreStateChangeController(
            IAppUserService AppUserService, 
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreStatusService StoreStatusService, 
            IUOW UOW,
            ICurrentContext CurrentContext)
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreStatusService = StoreStatusService;
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStoreStateChangeRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreStateChange_OrganizationDTO>> FilterListOrganization()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportStoreStateChange_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportStoreStateChange_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportStoreStateChangeRoute.FilterListStore), HttpPost]
        public async Task<List<ReportStoreStateChange_StoreDTO>> FilterListStore([FromBody] ReportStoreStateChange_StoreFilterDTO ReportStoreStateChange_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportStoreStateChange_StoreFilterDTO.Id;
            StoreFilter.Code = ReportStoreStateChange_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ReportStoreStateChange_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ReportStoreStateChange_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ReportStoreStateChange_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ReportStoreStateChange_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportStoreStateChange_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportStoreStateChange_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (CurrentUser.AppUserStoreMappings != null && CurrentUser.AppUserStoreMappings.Count > 0)
            {
                StoreFilter.Id.In = CurrentUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
            }
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportStoreStateChange_StoreDTO> ReportStoreStateChange_StoreDTOs = Stores
                .Select(x => new ReportStoreStateChange_StoreDTO(x)).ToList();
            return ReportStoreStateChange_StoreDTOs;
        }

        [Route(ReportStoreStateChangeRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportStoreStateChange_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportStoreStateChange_StoreStatusFilterDTO ReportStoreStateChange_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportStoreStateChange_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportStoreStateChange_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportStoreStateChange_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportStoreStateChange_StoreStatusDTO> ReportStoreStateChange_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportStoreStateChange_StoreStatusDTO(x)).ToList();
            return ReportStoreStateChange_StoreStatusDTOs;
        }
        #endregion
    }
}
