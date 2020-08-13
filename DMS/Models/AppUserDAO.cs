using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
            AppUserStoreMappings = new HashSet<AppUserStoreMappingDAO>();
            Banners = new HashSet<BannerDAO>();
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            ERouteChangeRequests = new HashSet<ERouteChangeRequestDAO>();
            ERouteCreators = new HashSet<ERouteDAO>();
            ERouteSaleEmployees = new HashSet<ERouteDAO>();
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
            InventoryHistories = new HashSet<InventoryHistoryDAO>();
            ItemHistories = new HashSet<ItemHistoryDAO>();
            KpiGeneralCreators = new HashSet<KpiGeneralDAO>();
            KpiGeneralEmployees = new HashSet<KpiGeneralDAO>();
            KpiItemCreators = new HashSet<KpiItemDAO>();
            KpiItemEmployees = new HashSet<KpiItemDAO>();
            PriceListItemHistories = new HashSet<PriceListItemHistoryDAO>();
            ProblemHistories = new HashSet<ProblemHistoryDAO>();
            Problems = new HashSet<ProblemDAO>();
            RequestWorkflowStepMappings = new HashSet<RequestWorkflowStepMappingDAO>();
            Resellers = new HashSet<ResellerDAO>();
            StoreCheckingImageMappings = new HashSet<StoreCheckingImageMappingDAO>();
            StoreCheckings = new HashSet<StoreCheckingDAO>();
            StoreScoutings = new HashSet<StoreScoutingDAO>();
            StoreUncheckings = new HashSet<StoreUncheckingDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            SurveyResults = new HashSet<SurveyResultDAO>();
            Surveys = new HashSet<SurveyDAO>();
            WorkflowDefinitionCreators = new HashSet<WorkflowDefinitionDAO>();
            WorkflowDefinitionModifiers = new HashSet<WorkflowDefinitionDAO>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Địa chỉ nhà
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string Phone { get; set; }
        public long? SexId { get; set; }
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// Vị trí công tác
        /// </summary>
        public long? PositionId { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// Đơn vị công tác
        /// </summary>
        public long? OrganizationId { get; set; }
        /// <summary>
        /// Tỉnh thành
        /// </summary>
        public long? ProvinceId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public long StatusId { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// Ngày xoá
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Trường để đồng bộ
        /// </summary>
        public Guid RowId { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
        public virtual PositionDAO Position { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual SexDAO Sex { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
        public virtual ICollection<AppUserStoreMappingDAO> AppUserStoreMappings { get; set; }
        public virtual ICollection<BannerDAO> Banners { get; set; }
        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<ERouteChangeRequestDAO> ERouteChangeRequests { get; set; }
        public virtual ICollection<ERouteDAO> ERouteCreators { get; set; }
        public virtual ICollection<ERouteDAO> ERouteSaleEmployees { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
        public virtual ICollection<InventoryHistoryDAO> InventoryHistories { get; set; }
        public virtual ICollection<ItemHistoryDAO> ItemHistories { get; set; }
        public virtual ICollection<KpiGeneralDAO> KpiGeneralCreators { get; set; }
        public virtual ICollection<KpiGeneralDAO> KpiGeneralEmployees { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItemCreators { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItemEmployees { get; set; }
        public virtual ICollection<PriceListItemHistoryDAO> PriceListItemHistories { get; set; }
        public virtual ICollection<ProblemHistoryDAO> ProblemHistories { get; set; }
        public virtual ICollection<ProblemDAO> Problems { get; set; }
        public virtual ICollection<RequestWorkflowStepMappingDAO> RequestWorkflowStepMappings { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<StoreCheckingImageMappingDAO> StoreCheckingImageMappings { get; set; }
        public virtual ICollection<StoreCheckingDAO> StoreCheckings { get; set; }
        public virtual ICollection<StoreScoutingDAO> StoreScoutings { get; set; }
        public virtual ICollection<StoreUncheckingDAO> StoreUncheckings { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<SurveyResultDAO> SurveyResults { get; set; }
        public virtual ICollection<SurveyDAO> Surveys { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitionCreators { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitionModifiers { get; set; }
    }
}
