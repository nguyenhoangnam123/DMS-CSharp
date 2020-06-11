using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
            Banners = new HashSet<BannerDAO>();
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            ERouteChangeRequests = new HashSet<ERouteChangeRequestDAO>();
            ERouteCreators = new HashSet<ERouteDAO>();
            ERouteSaleEmployees = new HashSet<ERouteDAO>();
            GeneralKpiCreators = new HashSet<GeneralKpiDAO>();
            GeneralKpiEmployees = new HashSet<GeneralKpiDAO>();
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
            InventoryHistories = new HashSet<InventoryHistoryDAO>();
            ItemHistories = new HashSet<ItemHistoryDAO>();
            KpiItemCreators = new HashSet<KpiItemDAO>();
            KpiItemEmployees = new HashSet<KpiItemDAO>();
            ProblemHistories = new HashSet<ProblemHistoryDAO>();
            Problems = new HashSet<ProblemDAO>();
            RequestWorkflowStepMappings = new HashSet<RequestWorkflowStepMappingDAO>();
            Resellers = new HashSet<ResellerDAO>();
            StoreCheckingImageMappings = new HashSet<StoreCheckingImageMappingDAO>();
            StoreCheckings = new HashSet<StoreCheckingDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            SurveyResults = new HashSet<SurveyResultDAO>();
            Surveys = new HashSet<SurveyDAO>();
            WorkflowDefinitionCreators = new HashSet<WorkflowDefinitionDAO>();
            WorkflowDefinitionModifiers = new HashSet<WorkflowDefinitionDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? PositionId { get; set; }
        public string Department { get; set; }
        public long? OrganizationId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Avatar { get; set; }
        public Guid RowId { get; set; }
        public long? ProvinceId { get; set; }
        public long? SexId { get; set; }
        public DateTime? Birthday { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
        public virtual PositionDAO Position { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual SexDAO Sex { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
        public virtual ICollection<BannerDAO> Banners { get; set; }
        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<ERouteChangeRequestDAO> ERouteChangeRequests { get; set; }
        public virtual ICollection<ERouteDAO> ERouteCreators { get; set; }
        public virtual ICollection<ERouteDAO> ERouteSaleEmployees { get; set; }
        public virtual ICollection<GeneralKpiDAO> GeneralKpiCreators { get; set; }
        public virtual ICollection<GeneralKpiDAO> GeneralKpiEmployees { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
        public virtual ICollection<InventoryHistoryDAO> InventoryHistories { get; set; }
        public virtual ICollection<ItemHistoryDAO> ItemHistories { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItemCreators { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItemEmployees { get; set; }
        public virtual ICollection<ProblemHistoryDAO> ProblemHistories { get; set; }
        public virtual ICollection<ProblemDAO> Problems { get; set; }
        public virtual ICollection<RequestWorkflowStepMappingDAO> RequestWorkflowStepMappings { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<StoreCheckingImageMappingDAO> StoreCheckingImageMappings { get; set; }
        public virtual ICollection<StoreCheckingDAO> StoreCheckings { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<SurveyResultDAO> SurveyResults { get; set; }
        public virtual ICollection<SurveyDAO> Surveys { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitionCreators { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitionModifiers { get; set; }
    }
}
