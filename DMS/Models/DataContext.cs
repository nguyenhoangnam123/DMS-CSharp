using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture;

namespace DMS.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ActionDAO> Action { get; set; }
        public virtual DbSet<ActionPageMappingDAO> ActionPageMapping { get; set; }
        public virtual DbSet<AlbumDAO> Album { get; set; }
        public virtual DbSet<AlbumImageMappingDAO> AlbumImageMapping { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserPermissionDAO> AppUserPermission { get; set; }
        public virtual DbSet<AppUserRoleMappingDAO> AppUserRoleMapping { get; set; }
        public virtual DbSet<AppUserStoreMappingDAO> AppUserStoreMapping { get; set; }
        public virtual DbSet<BannerDAO> Banner { get; set; }
        public virtual DbSet<BrandDAO> Brand { get; set; }
        public virtual DbSet<CategoryDAO> Category { get; set; }
        public virtual DbSet<ColorDAO> Color { get; set; }
        public virtual DbSet<DirectSalesOrderDAO> DirectSalesOrder { get; set; }
        public virtual DbSet<DirectSalesOrderContentDAO> DirectSalesOrderContent { get; set; }
        public virtual DbSet<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotion { get; set; }
        public virtual DbSet<DirectSalesOrderTransactionDAO> DirectSalesOrderTransaction { get; set; }
        public virtual DbSet<DistrictDAO> District { get; set; }
        public virtual DbSet<ERouteDAO> ERoute { get; set; }
        public virtual DbSet<ERouteChangeRequestDAO> ERouteChangeRequest { get; set; }
        public virtual DbSet<ERouteChangeRequestContentDAO> ERouteChangeRequestContent { get; set; }
        public virtual DbSet<ERouteContentDAO> ERouteContent { get; set; }
        public virtual DbSet<ERouteContentDayDAO> ERouteContentDay { get; set; }
        public virtual DbSet<ERouteTypeDAO> ERouteType { get; set; }
        public virtual DbSet<EditedPriceStatusDAO> EditedPriceStatus { get; set; }
        public virtual DbSet<EventMessageDAO> EventMessage { get; set; }
        public virtual DbSet<FieldDAO> Field { get; set; }
        public virtual DbSet<FieldTypeDAO> FieldType { get; set; }
        public virtual DbSet<IdGeneratorDAO> IdGenerator { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<IndirectSalesOrderDAO> IndirectSalesOrder { get; set; }
        public virtual DbSet<IndirectSalesOrderContentDAO> IndirectSalesOrderContent { get; set; }
        public virtual DbSet<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotion { get; set; }
        public virtual DbSet<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransaction { get; set; }
        public virtual DbSet<InventoryDAO> Inventory { get; set; }
        public virtual DbSet<InventoryHistoryDAO> InventoryHistory { get; set; }
        public virtual DbSet<ItemDAO> Item { get; set; }
        public virtual DbSet<ItemHistoryDAO> ItemHistory { get; set; }
        public virtual DbSet<ItemImageMappingDAO> ItemImageMapping { get; set; }
        public virtual DbSet<KpiCriteriaGeneralDAO> KpiCriteriaGeneral { get; set; }
        public virtual DbSet<KpiCriteriaItemDAO> KpiCriteriaItem { get; set; }
        public virtual DbSet<KpiGeneralDAO> KpiGeneral { get; set; }
        public virtual DbSet<KpiGeneralContentDAO> KpiGeneralContent { get; set; }
        public virtual DbSet<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMapping { get; set; }
        public virtual DbSet<KpiItemDAO> KpiItem { get; set; }
        public virtual DbSet<KpiItemContentDAO> KpiItemContent { get; set; }
        public virtual DbSet<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMapping { get; set; }
        public virtual DbSet<KpiPeriodDAO> KpiPeriod { get; set; }
        public virtual DbSet<KpiYearDAO> KpiYear { get; set; }
        public virtual DbSet<LuckyNumberDAO> LuckyNumber { get; set; }
        public virtual DbSet<LuckyNumberGroupingDAO> LuckyNumberGrouping { get; set; }
        public virtual DbSet<MenuDAO> Menu { get; set; }
        public virtual DbSet<NotificationDAO> Notification { get; set; }
        public virtual DbSet<NotificationStatusDAO> NotificationStatus { get; set; }
        public virtual DbSet<OrganizationDAO> Organization { get; set; }
        public virtual DbSet<PageDAO> Page { get; set; }
        public virtual DbSet<PermissionDAO> Permission { get; set; }
        public virtual DbSet<PermissionActionMappingDAO> PermissionActionMapping { get; set; }
        public virtual DbSet<PermissionContentDAO> PermissionContent { get; set; }
        public virtual DbSet<PermissionOperatorDAO> PermissionOperator { get; set; }
        public virtual DbSet<PositionDAO> Position { get; set; }
        public virtual DbSet<PriceListDAO> PriceList { get; set; }
        public virtual DbSet<PriceListItemHistoryDAO> PriceListItemHistory { get; set; }
        public virtual DbSet<PriceListItemMappingDAO> PriceListItemMapping { get; set; }
        public virtual DbSet<PriceListStoreGroupingMappingDAO> PriceListStoreGroupingMapping { get; set; }
        public virtual DbSet<PriceListStoreMappingDAO> PriceListStoreMapping { get; set; }
        public virtual DbSet<PriceListStoreTypeMappingDAO> PriceListStoreTypeMapping { get; set; }
        public virtual DbSet<PriceListTypeDAO> PriceListType { get; set; }
        public virtual DbSet<ProblemDAO> Problem { get; set; }
        public virtual DbSet<ProblemHistoryDAO> ProblemHistory { get; set; }
        public virtual DbSet<ProblemImageMappingDAO> ProblemImageMapping { get; set; }
        public virtual DbSet<ProblemStatusDAO> ProblemStatus { get; set; }
        public virtual DbSet<ProblemTypeDAO> ProblemType { get; set; }
        public virtual DbSet<ProductDAO> Product { get; set; }
        public virtual DbSet<ProductGroupingDAO> ProductGrouping { get; set; }
        public virtual DbSet<ProductImageMappingDAO> ProductImageMapping { get; set; }
        public virtual DbSet<ProductProductGroupingMappingDAO> ProductProductGroupingMapping { get; set; }
        public virtual DbSet<ProductTypeDAO> ProductType { get; set; }
        public virtual DbSet<PromotionDAO> Promotion { get; set; }
        public virtual DbSet<PromotionCodeDAO> PromotionCode { get; set; }
        public virtual DbSet<PromotionCodeHistoryDAO> PromotionCodeHistory { get; set; }
        public virtual DbSet<PromotionCodeOrganizationMappingDAO> PromotionCodeOrganizationMapping { get; set; }
        public virtual DbSet<PromotionCodeProductMappingDAO> PromotionCodeProductMapping { get; set; }
        public virtual DbSet<PromotionCodeStoreMappingDAO> PromotionCodeStoreMapping { get; set; }
        public virtual DbSet<PromotionComboDAO> PromotionCombo { get; set; }
        public virtual DbSet<PromotionComboInItemMappingDAO> PromotionComboInItemMapping { get; set; }
        public virtual DbSet<PromotionComboOutItemMappingDAO> PromotionComboOutItemMapping { get; set; }
        public virtual DbSet<PromotionDirectSalesOrderDAO> PromotionDirectSalesOrder { get; set; }
        public virtual DbSet<PromotionDirectSalesOrderItemMappingDAO> PromotionDirectSalesOrderItemMapping { get; set; }
        public virtual DbSet<PromotionDiscountTypeDAO> PromotionDiscountType { get; set; }
        public virtual DbSet<PromotionPolicyDAO> PromotionPolicy { get; set; }
        public virtual DbSet<PromotionProductDAO> PromotionProduct { get; set; }
        public virtual DbSet<PromotionProductAppliedTypeDAO> PromotionProductAppliedType { get; set; }
        public virtual DbSet<PromotionProductGroupingDAO> PromotionProductGrouping { get; set; }
        public virtual DbSet<PromotionProductGroupingItemMappingDAO> PromotionProductGroupingItemMapping { get; set; }
        public virtual DbSet<PromotionProductItemMappingDAO> PromotionProductItemMapping { get; set; }
        public virtual DbSet<PromotionProductTypeDAO> PromotionProductType { get; set; }
        public virtual DbSet<PromotionProductTypeItemMappingDAO> PromotionProductTypeItemMapping { get; set; }
        public virtual DbSet<PromotionPromotionPolicyMappingDAO> PromotionPromotionPolicyMapping { get; set; }
        public virtual DbSet<PromotionSamePriceDAO> PromotionSamePrice { get; set; }
        public virtual DbSet<PromotionStoreDAO> PromotionStore { get; set; }
        public virtual DbSet<PromotionStoreGroupingDAO> PromotionStoreGrouping { get; set; }
        public virtual DbSet<PromotionStoreGroupingItemMappingDAO> PromotionStoreGroupingItemMapping { get; set; }
        public virtual DbSet<PromotionStoreGroupingMappingDAO> PromotionStoreGroupingMapping { get; set; }
        public virtual DbSet<PromotionStoreItemMappingDAO> PromotionStoreItemMapping { get; set; }
        public virtual DbSet<PromotionStoreMappingDAO> PromotionStoreMapping { get; set; }
        public virtual DbSet<PromotionStoreTypeDAO> PromotionStoreType { get; set; }
        public virtual DbSet<PromotionStoreTypeItemMappingDAO> PromotionStoreTypeItemMapping { get; set; }
        public virtual DbSet<PromotionStoreTypeMappingDAO> PromotionStoreTypeMapping { get; set; }
        public virtual DbSet<PromotionTypeDAO> PromotionType { get; set; }
        public virtual DbSet<ProvinceDAO> Province { get; set; }
        public virtual DbSet<RequestStateDAO> RequestState { get; set; }
        public virtual DbSet<RequestWorkflowDefinitionMappingDAO> RequestWorkflowDefinitionMapping { get; set; }
        public virtual DbSet<RequestWorkflowHistoryDAO> RequestWorkflowHistory { get; set; }
        public virtual DbSet<RequestWorkflowParameterMappingDAO> RequestWorkflowParameterMapping { get; set; }
        public virtual DbSet<RequestWorkflowStepMappingDAO> RequestWorkflowStepMapping { get; set; }
        public virtual DbSet<ResellerDAO> Reseller { get; set; }
        public virtual DbSet<ResellerStatusDAO> ResellerStatus { get; set; }
        public virtual DbSet<ResellerTypeDAO> ResellerType { get; set; }
        public virtual DbSet<RewardHistoryDAO> RewardHistory { get; set; }
        public virtual DbSet<RewardHistoryContentDAO> RewardHistoryContent { get; set; }
        public virtual DbSet<RewardStatusDAO> RewardStatus { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SalesOrderTypeDAO> SalesOrderType { get; set; }
        public virtual DbSet<SexDAO> Sex { get; set; }
        public virtual DbSet<StatusDAO> Status { get; set; }
        public virtual DbSet<StoreDAO> Store { get; set; }
        public virtual DbSet<StoreCheckingDAO> StoreChecking { get; set; }
        public virtual DbSet<StoreCheckingImageMappingDAO> StoreCheckingImageMapping { get; set; }
        public virtual DbSet<StoreGroupingDAO> StoreGrouping { get; set; }
        public virtual DbSet<StoreImageDAO> StoreImage { get; set; }
        public virtual DbSet<StoreImageMappingDAO> StoreImageMapping { get; set; }
        public virtual DbSet<StoreScoutingDAO> StoreScouting { get; set; }
        public virtual DbSet<StoreScoutingImageMappingDAO> StoreScoutingImageMapping { get; set; }
        public virtual DbSet<StoreScoutingStatusDAO> StoreScoutingStatus { get; set; }
        public virtual DbSet<StoreScoutingTypeDAO> StoreScoutingType { get; set; }
        public virtual DbSet<StoreStatusDAO> StoreStatus { get; set; }
        public virtual DbSet<StoreTypeDAO> StoreType { get; set; }
        public virtual DbSet<StoreUncheckingDAO> StoreUnchecking { get; set; }
        public virtual DbSet<SupplierDAO> Supplier { get; set; }
        public virtual DbSet<SurveyDAO> Survey { get; set; }
        public virtual DbSet<SurveyOptionDAO> SurveyOption { get; set; }
        public virtual DbSet<SurveyOptionTypeDAO> SurveyOptionType { get; set; }
        public virtual DbSet<SurveyQuestionDAO> SurveyQuestion { get; set; }
        public virtual DbSet<SurveyQuestionTypeDAO> SurveyQuestionType { get; set; }
        public virtual DbSet<SurveyRespondentTypeDAO> SurveyRespondentType { get; set; }
        public virtual DbSet<SurveyResultDAO> SurveyResult { get; set; }
        public virtual DbSet<SurveyResultCellDAO> SurveyResultCell { get; set; }
        public virtual DbSet<SurveyResultSingleDAO> SurveyResultSingle { get; set; }
        public virtual DbSet<SurveyResultTextDAO> SurveyResultText { get; set; }
        public virtual DbSet<SystemConfigurationDAO> SystemConfiguration { get; set; }
        public virtual DbSet<TaxTypeDAO> TaxType { get; set; }
        public virtual DbSet<UnitOfMeasureDAO> UnitOfMeasure { get; set; }
        public virtual DbSet<UnitOfMeasureGroupingDAO> UnitOfMeasureGrouping { get; set; }
        public virtual DbSet<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContent { get; set; }
        public virtual DbSet<UsedVariationDAO> UsedVariation { get; set; }
        public virtual DbSet<VariationDAO> Variation { get; set; }
        public virtual DbSet<VariationGroupingDAO> VariationGrouping { get; set; }
        public virtual DbSet<WardDAO> Ward { get; set; }
        public virtual DbSet<WarehouseDAO> Warehouse { get; set; }
        public virtual DbSet<WorkflowDefinitionDAO> WorkflowDefinition { get; set; }
        public virtual DbSet<WorkflowDirectionDAO> WorkflowDirection { get; set; }
        public virtual DbSet<WorkflowDirectionConditionDAO> WorkflowDirectionCondition { get; set; }
        public virtual DbSet<WorkflowOperatorDAO> WorkflowOperator { get; set; }
        public virtual DbSet<WorkflowParameterDAO> WorkflowParameter { get; set; }
        public virtual DbSet<WorkflowParameterTypeDAO> WorkflowParameterType { get; set; }
        public virtual DbSet<WorkflowStateDAO> WorkflowState { get; set; }
        public virtual DbSet<WorkflowStepDAO> WorkflowStep { get; set; }
        public virtual DbSet<WorkflowTypeDAO> WorkflowType { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=192.168.20.200;initial catalog=dms;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureTempTable<long>();
            modelBuilder.Entity<ActionDAO>(entity =>
            {
                entity.ToTable("Action", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Action_Menu");
            });

            modelBuilder.Entity<ActionPageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PageId });

                entity.ToTable("ActionPageMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Action");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Page");
            });

            modelBuilder.Entity<AlbumDAO>(entity =>
            {
                entity.ToTable("Album", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Album_Status");
            });

            modelBuilder.Entity<AlbumImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ImageId, e.AlbumId });

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ShootingAt).HasColumnType("datetime");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumImageMappings)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlbumImageMapping_Album");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.AlbumImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlbumImageMapping_Image");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.AlbumImageMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlbumImageMapping_Store");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.ToTable("AppUser", "MDM");

                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ nhà");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(4000)
                    .HasComment("Ảnh đại diện");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasComment("Ngày sinh");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Department)
                    .HasMaxLength(500)
                    .HasComment("Phòng ban");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(500)
                    .HasComment("Tên hiển thị");

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ email");

                entity.Property(e => e.GPSUpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.OrganizationId).HasComment("Đơn vị công tác");

                entity.Property(e => e.Phone)
                    .HasMaxLength(500)
                    .HasComment("Số điện thoại liên hệ");

                entity.Property(e => e.PositionId).HasComment("Vị trí công tác");

                entity.Property(e => e.ProvinceId).HasComment("Tỉnh thành");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên đăng nhập");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_Organization");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_AppUser_Position");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_AppUser_Province");

                entity.HasOne(d => d.Sex)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.SexId)
                    .HasConstraintName("FK_AppUser_Sex");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_UserStatus");
            });

            modelBuilder.Entity<AppUserPermissionDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("AppUserPermission", "PER");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<AppUserRoleMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.RoleId })
                    .HasName("PK_UserRoleMapping");

                entity.ToTable("AppUserRoleMapping", "MDM");

                entity.Property(e => e.AppUserId).HasComment("Id nhân viên");

                entity.Property(e => e.RoleId).HasComment("Id nhóm quyền");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_AppUser");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_Role");
            });

            modelBuilder.Entity<AppUserStoreMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.StoreId });

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserStoreMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserStoreMapping_AppUser");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.AppUserStoreMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserStoreMapping_Store");
            });

            modelBuilder.Entity<BannerDAO>(entity =>
            {
                entity.ToTable("Banner", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Banner_AppUser");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Banner_Image");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Banner_Status");
            });

            modelBuilder.Entity<BrandDAO>(entity =>
            {
                entity.ToTable("Brand", "MDM");

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Mã nhãn hiệu");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Description)
                    .HasMaxLength(2000)
                    .HasComment("Mô tả");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên nhãn nhiệu");

                entity.Property(e => e.StatusId).HasComment("Trạng thái hoạt động");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_Status");
            });

            modelBuilder.Entity<CategoryDAO>(entity =>
            {
                entity.ToTable("Category", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Category_Image");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Category_Category");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_Status");
            });

            modelBuilder.Entity<ColorDAO>(entity =>
            {
                entity.ToTable("Color", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DirectSalesOrderDAO>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.BuyerStoreId).HasComment("Cửa hàng mua");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Mã đơn hàng");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(4000)
                    .HasComment("Địa chỉ giao hàng");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày giao hàng");

                entity.Property(e => e.EditedPriceStatusId).HasComment("Sửa giá");

                entity.Property(e => e.GeneralDiscountAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền chiết khấu tổng");

                entity.Property(e => e.GeneralDiscountPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasComment("% chiết khấu tổng");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasComment("Ghi chú");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày đặt hàng");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.PromotionCode).HasMaxLength(50);

                entity.Property(e => e.PromotionValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.RowId).HasComment("Id global cho phê duyệt");

                entity.Property(e => e.SaleEmployeeId).HasComment("Nhân viên kinh doanh");

                entity.Property(e => e.StoreAddress).HasMaxLength(4000);

                entity.Property(e => e.SubTotal)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng tiền trước thuế");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng tiền sau thuế");

                entity.Property(e => e.TotalAfterTax).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.TotalTaxAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng thuế");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.BuyerStore)
                    .WithMany(p => p.DirectSalesOrders)
                    .HasForeignKey(d => d.BuyerStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_Store");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.DirectSalesOrderCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_AppUser1");

                entity.HasOne(d => d.EditedPriceStatus)
                    .WithMany(p => p.DirectSalesOrders)
                    .HasForeignKey(d => d.EditedPriceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_EditedPriceStatus");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.DirectSalesOrders)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_Organization");

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.DirectSalesOrders)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_RequestState");

                entity.HasOne(d => d.SaleEmployee)
                    .WithMany(p => p.DirectSalesOrderSaleEmployees)
                    .HasForeignKey(d => d.SaleEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrder_AppUser");

                entity.HasOne(d => d.StoreChecking)
                    .WithMany(p => p.DirectSalesOrders)
                    .HasForeignKey(d => d.StoreCheckingId)
                    .HasConstraintName("FK_DirectSalesOrder_StoreChecking");
            });

            modelBuilder.Entity<DirectSalesOrderContentDAO>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.GeneralDiscountAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.GeneralDiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.PrimaryPrice)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Giá theo đơn vị lưu kho");

                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Giá bán theo đơn vị xuất hàng");

                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.TaxPercentage).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.DirectSalesOrder)
                    .WithMany(p => p.DirectSalesOrderContents)
                    .HasForeignKey(d => d.DirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderContent_DirectSalesOrder");

                entity.HasOne(d => d.EditedPriceStatus)
                    .WithMany(p => p.DirectSalesOrderContents)
                    .HasForeignKey(d => d.EditedPriceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderContent_EditedPriceStatus");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.DirectSalesOrderContents)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderContent_Item");

                entity.HasOne(d => d.PrimaryUnitOfMeasure)
                    .WithMany(p => p.DirectSalesOrderContentPrimaryUnitOfMeasures)
                    .HasForeignKey(d => d.PrimaryUnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderContent_UnitOfMeasure1");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.DirectSalesOrderContentUnitOfMeasures)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderContent_UnitOfMeasure");
            });

            modelBuilder.Entity<DirectSalesOrderPromotionDAO>(entity =>
            {
                entity.Property(e => e.Note).HasMaxLength(4000);

                entity.HasOne(d => d.DirectSalesOrder)
                    .WithMany(p => p.DirectSalesOrderPromotions)
                    .HasForeignKey(d => d.DirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderPromotion_DirectSalesOrder");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.DirectSalesOrderPromotions)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderPromotion_Item");

                entity.HasOne(d => d.PrimaryUnitOfMeasure)
                    .WithMany(p => p.DirectSalesOrderPromotionPrimaryUnitOfMeasures)
                    .HasForeignKey(d => d.PrimaryUnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderPromotion_UnitOfMeasure1");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.DirectSalesOrderPromotionUnitOfMeasures)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderPromotion_UnitOfMeasure");
            });

            modelBuilder.Entity<DirectSalesOrderTransactionDAO>(entity =>
            {
                entity.Property(e => e.Discount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.DirectSalesOrder)
                    .WithMany(p => p.DirectSalesOrderTransactions)
                    .HasForeignKey(d => d.DirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderTransaction_DirectSalesOrder");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.DirectSalesOrderTransactions)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderTransaction_Item");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.DirectSalesOrderTransactions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderTransaction_Organization");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.DirectSalesOrderTransactions)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectSalesOrderTransaction_UnitOfMeasure");
            });

            modelBuilder.Entity<DistrictDAO>(entity =>
            {
                entity.ToTable("District", "MDM");

                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(500)
                    .HasComment("Mã quận huyện");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên quận huyện");

                entity.Property(e => e.Priority).HasComment("Thứ tự ưu tiên");

                entity.Property(e => e.ProvinceId).HasComment("Tỉnh phụ thuộc");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái hoạt động");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Status");
            });

            modelBuilder.Entity<ERouteDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.RealStartDate).HasColumnType("datetime");

                entity.Property(e => e.RowId).HasComment("Id global cho phê duyệt");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.ERouteCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_AppUser");

                entity.HasOne(d => d.ERouteType)
                    .WithMany(p => p.ERoutes)
                    .HasForeignKey(d => d.ERouteTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_ERouteType");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ERoutes)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_Organization");

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.ERoutes)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_RequestState");

                entity.HasOne(d => d.SaleEmployee)
                    .WithMany(p => p.ERouteSaleEmployees)
                    .HasForeignKey(d => d.SaleEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_AppUser1");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ERoutes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERoute_Status");
            });

            modelBuilder.Entity<ERouteChangeRequestDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.ERouteChangeRequests)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteChangeRequest_AppUser");

                entity.HasOne(d => d.ERoute)
                    .WithMany(p => p.ERouteChangeRequests)
                    .HasForeignKey(d => d.ERouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteChangeRequest_ERoute");

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.ERouteChangeRequests)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteChangeRequest_RequestState");
            });

            modelBuilder.Entity<ERouteChangeRequestContentDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.ERouteChangeRequest)
                    .WithMany(p => p.ERouteChangeRequestContents)
                    .HasForeignKey(d => d.ERouteChangeRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteChangeRequestContent_ERouteChangeRequest");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.ERouteChangeRequestContents)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteChangeRequestContent_Store");
            });

            modelBuilder.Entity<ERouteContentDAO>(entity =>
            {
                entity.HasOne(d => d.ERoute)
                    .WithMany(p => p.ERouteContents)
                    .HasForeignKey(d => d.ERouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteContent_ERoute");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.ERouteContents)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteContent_Store");
            });

            modelBuilder.Entity<ERouteContentDayDAO>(entity =>
            {
                entity.HasOne(d => d.ERouteContent)
                    .WithMany(p => p.ERouteContentDays)
                    .HasForeignKey(d => d.ERouteContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ERouteContentDay_ERouteContent");
            });

            modelBuilder.Entity<ERouteTypeDAO>(entity =>
            {
                entity.ToTable("ERouteType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EditedPriceStatusDAO>(entity =>
            {
                entity.ToTable("EditedPriceStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EventMessageDAO>(entity =>
            {
                entity.ToTable("EventMessage", "MDM");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.RoutingKey)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("datetime");
            });

            modelBuilder.Entity<FieldDAO>(entity =>
            {
                entity.ToTable("Field", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Field_FieldType");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionField_Menu");
            });

            modelBuilder.Entity<FieldTypeDAO>(entity =>
            {
                entity.ToTable("FieldType", "PER");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<IdGeneratorDAO>(entity =>
            {
                entity.ToTable("IdGenerator", "MDM");
            });

            modelBuilder.Entity<ImageDAO>(entity =>
            {
                entity.ToTable("Image", "MDM");

                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComment("Tên");

                entity.Property(e => e.ThumbnailUrl)
                    .HasMaxLength(4000)
                    .HasComment("Đường dẫn Url");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComment("Đường dẫn Url");
            });

            modelBuilder.Entity<IndirectSalesOrderDAO>(entity =>
            {
                entity.HasIndex(e => e.BuyerStoreId);

                entity.HasIndex(e => e.Id)
                    .HasName("IX_IndirectSalesOrder");

                entity.HasIndex(e => e.OrganizationId);

                entity.HasIndex(e => e.RequestStateId);

                entity.HasIndex(e => e.SaleEmployeeId);

                entity.HasIndex(e => e.SellerStoreId);

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.BuyerStoreId).HasComment("Cửa hàng mua");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Mã đơn hàng");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(4000)
                    .HasComment("Địa chỉ giao hàng");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày giao hàng");

                entity.Property(e => e.EditedPriceStatusId).HasComment("Sửa giá");

                entity.Property(e => e.GeneralDiscountAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền chiết khấu tổng");

                entity.Property(e => e.GeneralDiscountPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasComment("% chiết khấu tổng");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasComment("Ghi chú");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày đặt hàng");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.RowId).HasComment("Id global cho phê duyệt");

                entity.Property(e => e.SaleEmployeeId).HasComment("Nhân viên kinh doanh");

                entity.Property(e => e.SellerStoreId).HasComment("Cửa hàng bán");

                entity.Property(e => e.StoreAddress).HasMaxLength(4000);

                entity.Property(e => e.SubTotal)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng tiền trước thuế");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng tiền sau thuế");

                entity.Property(e => e.TotalTaxAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.BuyerStore)
                    .WithMany(p => p.IndirectSalesOrderBuyerStores)
                    .HasForeignKey(d => d.BuyerStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_Store");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.IndirectSalesOrderCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_AppUser1");

                entity.HasOne(d => d.EditedPriceStatus)
                    .WithMany(p => p.IndirectSalesOrders)
                    .HasForeignKey(d => d.EditedPriceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_EditedPriceStatus");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.IndirectSalesOrders)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_Organization");

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.IndirectSalesOrders)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_RequestState");

                entity.HasOne(d => d.SaleEmployee)
                    .WithMany(p => p.IndirectSalesOrderSaleEmployees)
                    .HasForeignKey(d => d.SaleEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_AppUser");

                entity.HasOne(d => d.SellerStore)
                    .WithMany(p => p.IndirectSalesOrderSellerStores)
                    .HasForeignKey(d => d.SellerStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrder_Store1");
            });

            modelBuilder.Entity<IndirectSalesOrderContentDAO>(entity =>
            {
                entity.HasIndex(e => e.IndirectSalesOrderId);

                entity.HasIndex(e => e.ItemId);

                entity.HasIndex(e => e.PrimaryUnitOfMeasureId);

                entity.HasIndex(e => e.UnitOfMeasureId);

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Tổng số tiền từng dòng trước chiết khấu tổng");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền chiết khấu theo dòng");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasComment("% chiết khấu theo dòng");

                entity.Property(e => e.GeneralDiscountAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền sau chiết khấu từng dòng");

                entity.Property(e => e.GeneralDiscountPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasComment("% chiết khấu tổng ");

                entity.Property(e => e.IndirectSalesOrderId).HasComment("FK của đơn hàng gián tiếp");

                entity.Property(e => e.ItemId).HasComment("Sản phẩm");

                entity.Property(e => e.PrimaryPrice)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Giá theo đơn vị lưu kho");

                entity.Property(e => e.PrimaryUnitOfMeasureId).HasComment("Đơn vị lưu kho");

                entity.Property(e => e.Quantity).HasComment("Số lượng xuất hàng");

                entity.Property(e => e.RequestedQuantity).HasComment("Số lượng xuất hàng theo đơn vị lưu kho");

                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Giá bán theo đơn vị xuất hàng");

                entity.Property(e => e.TaxAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền thuế sau tất cả các loại chiết khấu");

                entity.Property(e => e.TaxPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasComment("% thuế lấy theo sản phẩm");

                entity.Property(e => e.UnitOfMeasureId).HasComment("Đơn vị để xuất hàng");

                entity.HasOne(d => d.EditedPriceStatus)
                    .WithMany(p => p.IndirectSalesOrderContents)
                    .HasForeignKey(d => d.EditedPriceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderContent_EditedPriceStatus");

                entity.HasOne(d => d.IndirectSalesOrder)
                    .WithMany(p => p.IndirectSalesOrderContents)
                    .HasForeignKey(d => d.IndirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderContent_IndirectSalesOrder");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.IndirectSalesOrderContents)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderContent_Item");

                entity.HasOne(d => d.PrimaryUnitOfMeasure)
                    .WithMany(p => p.IndirectSalesOrderContentPrimaryUnitOfMeasures)
                    .HasForeignKey(d => d.PrimaryUnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderContent_UnitOfMeasure1");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.IndirectSalesOrderContentUnitOfMeasures)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderContent_UnitOfMeasure");
            });

            modelBuilder.Entity<IndirectSalesOrderPromotionDAO>(entity =>
            {
                entity.HasIndex(e => e.IndirectSalesOrderId);

                entity.HasIndex(e => e.ItemId);

                entity.HasIndex(e => e.PrimaryUnitOfMeasureId);

                entity.HasIndex(e => e.UnitOfMeasureId);

                entity.Property(e => e.Id).HasComment("Primary Key");

                entity.Property(e => e.IndirectSalesOrderId).HasComment("Id đơn hàng gián tiếp");

                entity.Property(e => e.ItemId).HasComment("Sản phẩm khuyến mãi");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasComment("Ghi chú");

                entity.Property(e => e.PrimaryUnitOfMeasureId).HasComment("Đơn vị lưu kho");

                entity.Property(e => e.Quantity).HasComment("Số lượng bán");

                entity.Property(e => e.RequestedQuantity).HasComment("Số lượng yêu cầu xuất kho");

                entity.Property(e => e.UnitOfMeasureId).HasComment("Đơn vị xuất kho");

                entity.HasOne(d => d.IndirectSalesOrder)
                    .WithMany(p => p.IndirectSalesOrderPromotions)
                    .HasForeignKey(d => d.IndirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderPromotion_IndirectSalesOrder");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.IndirectSalesOrderPromotions)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderPromotion_Item");

                entity.HasOne(d => d.PrimaryUnitOfMeasure)
                    .WithMany(p => p.IndirectSalesOrderPromotionPrimaryUnitOfMeasures)
                    .HasForeignKey(d => d.PrimaryUnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderPromotion_UnitOfMeasure1");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.IndirectSalesOrderPromotionUnitOfMeasures)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderPromotion_UnitOfMeasure");
            });

            modelBuilder.Entity<IndirectSalesOrderTransactionDAO>(entity =>
            {
                entity.HasIndex(e => e.IndirectSalesOrderId);

                entity.HasIndex(e => e.ItemId);

                entity.HasIndex(e => e.OrganizationId);

                entity.HasIndex(e => e.UnitOfMeasureId);

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.IndirectSalesOrder)
                    .WithMany(p => p.IndirectSalesOrderTransactions)
                    .HasForeignKey(d => d.IndirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderTransaction_IndirectSalesOrder");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.IndirectSalesOrderTransactions)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderTransaction_Item");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.IndirectSalesOrderTransactions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderTransaction_Organization");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.IndirectSalesOrderTransactions)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectSalesOrderTransaction_UnitOfMeasure");
            });

            modelBuilder.Entity<InventoryDAO>(entity =>
            {
                entity.ToTable("Inventory", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Inventory_Item");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WarehouseProductMapping_Warehouse");
            });

            modelBuilder.Entity<InventoryHistoryDAO>(entity =>
            {
                entity.ToTable("InventoryHistory", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.InventoryHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryHistory_AppUser");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryHistories)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryHistory_Inventory");
            });

            modelBuilder.Entity<ItemDAO>(entity =>
            {
                entity.ToTable("Item", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ScanCode).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Product");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Status");
            });

            modelBuilder.Entity<ItemHistoryDAO>(entity =>
            {
                entity.ToTable("ItemHistory", "MDM");

                entity.Property(e => e.NewPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemHistories)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemHistory_Item");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.ItemHistories)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemHistory_AppUser");
            });

            modelBuilder.Entity<ItemImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.ImageId });

                entity.ToTable("ItemImageMapping", "MDM");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ItemImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemImageMapping_Image");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemImageMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemImageMapping_Item");
            });

            modelBuilder.Entity<KpiCriteriaGeneralDAO>(entity =>
            {
                entity.ToTable("KpiCriteriaGeneral", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<KpiCriteriaItemDAO>(entity =>
            {
                entity.ToTable("KpiCriteriaItem", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<KpiGeneralDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.KpiGeneralCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpi_AppUser1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.KpiGeneralEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpi_AppUser");

                entity.HasOne(d => d.KpiYear)
                    .WithMany(p => p.KpiGenerals)
                    .HasForeignKey(d => d.KpiYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiGeneral_KpiYear");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.KpiGenerals)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpi_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.KpiGenerals)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpi_Status");
            });

            modelBuilder.Entity<KpiGeneralContentDAO>(entity =>
            {
                entity.HasOne(d => d.KpiCriteriaGeneral)
                    .WithMany(p => p.KpiGeneralContents)
                    .HasForeignKey(d => d.KpiCriteriaGeneralId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpiCriteriaMapping_GeneralCriteria");

                entity.HasOne(d => d.KpiGeneral)
                    .WithMany(p => p.KpiGeneralContents)
                    .HasForeignKey(d => d.KpiGeneralId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpiCriteriaMapping_GeneralKpi");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.KpiGeneralContents)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneralKpiCriteriaMapping_Status");
            });

            modelBuilder.Entity<KpiGeneralContentKpiPeriodMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.KpiGeneralContentId, e.KpiPeriodId });

                entity.Property(e => e.Value).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.KpiGeneralContent)
                    .WithMany(p => p.KpiGeneralContentKpiPeriodMappings)
                    .HasForeignKey(d => d.KpiGeneralContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiGeneralContentKpiPeriodMapping_KpiGeneralContent");

                entity.HasOne(d => d.KpiPeriod)
                    .WithMany(p => p.KpiGeneralContentKpiPeriodMappings)
                    .HasForeignKey(d => d.KpiPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiGeneralContentKpiPeriodMapping_KpiPeriod");
            });

            modelBuilder.Entity<KpiItemDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.KpiItemCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_AppUser1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.KpiItemEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_AppUser");

                entity.HasOne(d => d.KpiPeriod)
                    .WithMany(p => p.KpiItems)
                    .HasForeignKey(d => d.KpiPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_KpiPeriod");

                entity.HasOne(d => d.KpiYear)
                    .WithMany(p => p.KpiItems)
                    .HasForeignKey(d => d.KpiYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_KpiYear");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.KpiItems)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.KpiItems)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItem_Status");
            });

            modelBuilder.Entity<KpiItemContentDAO>(entity =>
            {
                entity.HasOne(d => d.Item)
                    .WithMany(p => p.KpiItemContents)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemSpecificKpiContent_Item");

                entity.HasOne(d => d.KpiItem)
                    .WithMany(p => p.KpiItemContents)
                    .HasForeignKey(d => d.KpiItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItemContent_KpiItem");
            });

            modelBuilder.Entity<KpiItemContentKpiCriteriaItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.KpiItemContentId, e.KpiCriteriaItemId })
                    .HasName("PK_ItemSpecificKpiContentItemSpecificKpiCriteriaMapping");

                entity.HasOne(d => d.KpiCriteriaItem)
                    .WithMany(p => p.KpiItemContentKpiCriteriaItemMappings)
                    .HasForeignKey(d => d.KpiCriteriaItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItemContentKpiCriteriaItemMapping_KpiCriteriaItem1");

                entity.HasOne(d => d.KpiItemContent)
                    .WithMany(p => p.KpiItemContentKpiCriteriaItemMappings)
                    .HasForeignKey(d => d.KpiItemContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KpiItemContentKpiCriteriaItemMapping_KpiItemContent");
            });

            modelBuilder.Entity<KpiPeriodDAO>(entity =>
            {
                entity.ToTable("KpiPeriod", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<KpiYearDAO>(entity =>
            {
                entity.ToTable("KpiYear", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<LuckyNumberDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UsedAt).HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.LuckyNumberGrouping)
                    .WithMany(p => p.LuckyNumbers)
                    .HasForeignKey(d => d.LuckyNumberGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuckyNumber_LuckyNumberGrouping");

                entity.HasOne(d => d.RewardStatus)
                    .WithMany(p => p.LuckyNumbers)
                    .HasForeignKey(d => d.RewardStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuckyNumber_RewardStatus");
            });

            modelBuilder.Entity<LuckyNumberGroupingDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.LuckyNumberGroupings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuckyNumberGrouping_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.LuckyNumberGroupings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuckyNumberGrouping_Status");
            });

            modelBuilder.Entity<MenuDAO>(entity =>
            {
                entity.ToTable("Menu", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Path).HasMaxLength(3000);
            });

            modelBuilder.Entity<NotificationDAO>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.NotificationStatus)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotificationStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_NotificationStatus");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Notification_Organization");
            });

            modelBuilder.Entity<NotificationStatusDAO>(entity =>
            {
                entity.ToTable("NotificationStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<OrganizationDAO>(entity =>
            {
                entity.ToTable("Organization", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Organization_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Organizations)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_Status");
            });

            modelBuilder.Entity<PageDAO>(entity =>
            {
                entity.ToTable("Page", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<PermissionDAO>(entity =>
            {
                entity.ToTable("Permission", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Menu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Status");
            });

            modelBuilder.Entity<PermissionActionMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PermissionId })
                    .HasName("PK_ActionPermissionMapping");

                entity.ToTable("PermissionActionMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPermissionMapping_Action");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPermissionMapping_Permission");
            });

            modelBuilder.Entity<PermissionContentDAO>(entity =>
            {
                entity.ToTable("PermissionContent", "PER");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Field");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Permission");

                entity.HasOne(d => d.PermissionOperator)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionOperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_PermissionOperator");
            });

            modelBuilder.Entity<PermissionOperatorDAO>(entity =>
            {
                entity.ToTable("PermissionOperator", "PER");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.PermissionOperators)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionOperator_FieldType");
            });

            modelBuilder.Entity<PositionDAO>(entity =>
            {
                entity.ToTable("Position", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Positions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Position_Status");
            });

            modelBuilder.Entity<PriceListDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.RowId).HasComment("Id global cho phê duyệt");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceList_AppUser");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceList_Organization");

                entity.HasOne(d => d.PriceListType)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.PriceListTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceList_PriceListType");

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceList_RequestState");

                entity.HasOne(d => d.SalesOrderType)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.SalesOrderTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceList_SalesOrderType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndirectPriceList_Status");
            });

            modelBuilder.Entity<PriceListItemHistoryDAO>(entity =>
            {
                entity.Property(e => e.NewPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PriceListItemHistories)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListItemHistory_Item");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.PriceListItemHistories)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListItemHistory_AppUser");

                entity.HasOne(d => d.PriceList)
                    .WithMany(p => p.PriceListItemHistories)
                    .HasForeignKey(d => d.PriceListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListItemHistory_PriceList");
            });

            modelBuilder.Entity<PriceListItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.ItemId })
                    .HasName("PK_IndirectPriceListItemMapping");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PriceListItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListItemMapping_Item");

                entity.HasOne(d => d.PriceList)
                    .WithMany(p => p.PriceListItemMappings)
                    .HasForeignKey(d => d.PriceListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListItemMapping_PriceList");
            });

            modelBuilder.Entity<PriceListStoreGroupingMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.StoreGroupingId })
                    .HasName("PK_IndirectPriceListStoreGroupingId");

                entity.HasOne(d => d.PriceList)
                    .WithMany(p => p.PriceListStoreGroupingMappings)
                    .HasForeignKey(d => d.PriceListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreGroupingMapping_PriceList");

                entity.HasOne(d => d.StoreGrouping)
                    .WithMany(p => p.PriceListStoreGroupingMappings)
                    .HasForeignKey(d => d.StoreGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreGroupingMapping_StoreGrouping");
            });

            modelBuilder.Entity<PriceListStoreMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.StoreId })
                    .HasName("PK_IndirectPriceListStoreMapping");

                entity.HasOne(d => d.PriceList)
                    .WithMany(p => p.PriceListStoreMappings)
                    .HasForeignKey(d => d.PriceListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreMapping_PriceList");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.PriceListStoreMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreMapping_Store");
            });

            modelBuilder.Entity<PriceListStoreTypeMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.StoreTypeId })
                    .HasName("PK_IndirectPriceListStoreTypeMapping");

                entity.HasOne(d => d.PriceList)
                    .WithMany(p => p.PriceListStoreTypeMappings)
                    .HasForeignKey(d => d.PriceListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreTypeMapping_PriceList");

                entity.HasOne(d => d.StoreType)
                    .WithMany(p => p.PriceListStoreTypeMappings)
                    .HasForeignKey(d => d.StoreTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceListStoreTypeMapping_StoreType");
            });

            modelBuilder.Entity<PriceListTypeDAO>(entity =>
            {
                entity.ToTable("PriceListType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ProblemDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CompletedAt).HasColumnType("datetime");

                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.NoteAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Problem_AppUser");

                entity.HasOne(d => d.ProblemStatus)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.ProblemStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Problem_ProblemStatus");

                entity.HasOne(d => d.ProblemType)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.ProblemTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Problem_ProblemType");

                entity.HasOne(d => d.StoreChecking)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.StoreCheckingId)
                    .HasConstraintName("FK_Problem_StoreChecking");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Problem_Store");
            });

            modelBuilder.Entity<ProblemHistoryDAO>(entity =>
            {
                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.ProblemHistories)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProblemHistory_AppUser");

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.ProblemHistories)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProblemHistory_Problem");

                entity.HasOne(d => d.ProblemStatus)
                    .WithMany(p => p.ProblemHistories)
                    .HasForeignKey(d => d.ProblemStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProblemHistory_ProblemStatus");
            });

            modelBuilder.Entity<ProblemImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ProblemId, e.ImageId });

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProblemImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProblemImageMapping_Image");

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.ProblemImageMappings)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProblemImageMapping_Problem");
            });

            modelBuilder.Entity<ProblemStatusDAO>(entity =>
            {
                entity.ToTable("ProblemStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ProblemTypeDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductDAO>(entity =>
            {
                entity.ToTable("Product", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.ERPCode).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Note).HasMaxLength(3000);

                entity.Property(e => e.OtherName).HasMaxLength(1000);

                entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ScanCode).HasMaxLength(500);

                entity.Property(e => e.SupplierCode).HasMaxLength(500);

                entity.Property(e => e.TechnicalName).HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Product_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.ProductType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Status");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Product_Supplier");

                entity.HasOne(d => d.TaxType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.TaxTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_TaxType");

                entity.HasOne(d => d.UnitOfMeasureGrouping)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitOfMeasureGroupingId)
                    .HasConstraintName("FK_Product_UnitOfMeasureGrouping");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_UnitOfMeasure");

                entity.HasOne(d => d.UsedVariation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UsedVariationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_UsedVariation");
            });

            modelBuilder.Entity<ProductGroupingDAO>(entity =>
            {
                entity.ToTable("ProductGrouping", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_GroupItem_GroupItem");
            });

            modelBuilder.Entity<ProductImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ImageId });

                entity.ToTable("ProductImageMapping", "MDM");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProductImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductImageMapping_Image");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImageMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductImageMapping_Product");
            });

            modelBuilder.Entity<ProductProductGroupingMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ProductGroupingId })
                    .HasName("PK_ProductProductGrouping");

                entity.ToTable("ProductProductGroupingMapping", "MDM");

                entity.HasOne(d => d.ProductGrouping)
                    .WithMany(p => p.ProductProductGroupingMappings)
                    .HasForeignKey(d => d.ProductGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductProductGroupingMapping_ProductGrouping");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductProductGroupingMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductProductGroupingMapping_Product");
            });

            modelBuilder.Entity<ProductTypeDAO>(entity =>
            {
                entity.ToTable("ProductType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ProductTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductType_Status");
            });

            modelBuilder.Entity<PromotionDAO>(entity =>
            {
                entity.ToTable("Promotion", "PRO");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promotion_Organization");

                entity.HasOne(d => d.PromotionType)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.PromotionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promotion_PromotionType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promotion_Status");
            });

            modelBuilder.Entity<PromotionCodeDAO>(entity =>
            {
                entity.ToTable("PromotionCode", "PRO");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.MaxValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PromotionCodes)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCode_Organization");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionCodes)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCode_PromotionDiscountType");

                entity.HasOne(d => d.PromotionProductAppliedType)
                    .WithMany(p => p.PromotionCodes)
                    .HasForeignKey(d => d.PromotionProductAppliedTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCode_PromotionProductAppliedType");

                entity.HasOne(d => d.PromotionType)
                    .WithMany(p => p.PromotionCodes)
                    .HasForeignKey(d => d.PromotionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCode_PromotionType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PromotionCodes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCode_Status");
            });

            modelBuilder.Entity<PromotionCodeHistoryDAO>(entity =>
            {
                entity.ToTable("PromotionCodeHistory", "PRO");

                entity.Property(e => e.AppliedAt).HasColumnType("datetime");

                entity.HasOne(d => d.PromotionCode)
                    .WithMany(p => p.PromotionCodeHistories)
                    .HasForeignKey(d => d.PromotionCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeHistory_PromotionCode");
            });

            modelBuilder.Entity<PromotionCodeOrganizationMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionCodeId, e.OrganizationId })
                    .HasName("PK_PromotionCodeOrganization");

                entity.ToTable("PromotionCodeOrganizationMapping", "PRO");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PromotionCodeOrganizationMappings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeOrganizationMapping_Organization");

                entity.HasOne(d => d.PromotionCode)
                    .WithMany(p => p.PromotionCodeOrganizationMappings)
                    .HasForeignKey(d => d.PromotionCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeOrganizationMapping_PromotionCode");
            });

            modelBuilder.Entity<PromotionCodeProductMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionCodeId, e.ProductId });

                entity.ToTable("PromotionCodeProductMapping", "PRO");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PromotionCodeProductMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeProductMapping_Product");

                entity.HasOne(d => d.PromotionCode)
                    .WithMany(p => p.PromotionCodeProductMappings)
                    .HasForeignKey(d => d.PromotionCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeProductMapping_PromotionCode");
            });

            modelBuilder.Entity<PromotionCodeStoreMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionCodeId, e.StoreId })
                    .HasName("PK_PromotionCodeStore");

                entity.ToTable("PromotionCodeStoreMapping", "PRO");

                entity.HasOne(d => d.PromotionCode)
                    .WithMany(p => p.PromotionCodeStoreMappings)
                    .HasForeignKey(d => d.PromotionCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeStoreMapping_PromotionCode");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.PromotionCodeStoreMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCodeStoreMapping_Store");
            });

            modelBuilder.Entity<PromotionComboDAO>(entity =>
            {
                entity.ToTable("PromotionCombo", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionCombos)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCombo_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionCombos)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCombo_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionCombos)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionCombo_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionComboInItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionComboId, e.ItemId })
                    .HasName("PK_ComboInItemMapping");

                entity.ToTable("PromotionComboInItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionComboInItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionComboInItemMapping_Item");

                entity.HasOne(d => d.PromotionCombo)
                    .WithMany(p => p.PromotionComboInItemMappings)
                    .HasForeignKey(d => d.PromotionComboId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionComboInItemMapping_PromotionCombo");
            });

            modelBuilder.Entity<PromotionComboOutItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionComboId, e.ItemId })
                    .HasName("PK_ComboOutItemMapping");

                entity.ToTable("PromotionComboOutItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionComboOutItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionComboOutItemMapping_Item");

                entity.HasOne(d => d.PromotionCombo)
                    .WithMany(p => p.PromotionComboOutItemMappings)
                    .HasForeignKey(d => d.PromotionComboId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionComboOutItemMapping_PromotionCombo");
            });

            modelBuilder.Entity<PromotionDirectSalesOrderDAO>(entity =>
            {
                entity.ToTable("PromotionDirectSalesOrder", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionDirectSalesOrders)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionDirectSalesOrder_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionDirectSalesOrders)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionDirectSalesOrder_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionDirectSalesOrders)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionDirectSalesOrder_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionDirectSalesOrderItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionDirectSalesOrderId, e.ItemId });

                entity.ToTable("PromotionDirectSalesOrderItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionDirectSalesOrderItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionDirectSalesOrderItemMapping_Item");

                entity.HasOne(d => d.PromotionDirectSalesOrder)
                    .WithMany(p => p.PromotionDirectSalesOrderItemMappings)
                    .HasForeignKey(d => d.PromotionDirectSalesOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionDirectSalesOrderItemMapping_PromotionDirectSalesOrder");
            });

            modelBuilder.Entity<PromotionDiscountTypeDAO>(entity =>
            {
                entity.ToTable("PromotionDiscountType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PromotionPolicyDAO>(entity =>
            {
                entity.ToTable("PromotionPolicy", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PromotionProductDAO>(entity =>
            {
                entity.ToTable("PromotionProduct", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProduct_Product");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProduct_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProduct_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProduct_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionProductAppliedTypeDAO>(entity =>
            {
                entity.ToTable("PromotionProductAppliedType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PromotionProductGroupingDAO>(entity =>
            {
                entity.ToTable("PromotionProductGrouping", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.ProductGrouping)
                    .WithMany(p => p.PromotionProductGroupings)
                    .HasForeignKey(d => d.ProductGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGrouping_ProductGrouping");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionProductGroupings)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGrouping_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionProductGroupings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGrouping_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionProductGroupings)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGrouping_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionProductGroupingItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionProductGroupingId, e.ItemId });

                entity.ToTable("PromotionProductGroupingItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionProductGroupingItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGroupingItemMapping_Item");

                entity.HasOne(d => d.PromotionProductGrouping)
                    .WithMany(p => p.PromotionProductGroupingItemMappings)
                    .HasForeignKey(d => d.PromotionProductGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductGroupingItemMapping_PromotionProductGrouping");
            });

            modelBuilder.Entity<PromotionProductItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionProductId, e.ItemId });

                entity.ToTable("PromotionProductItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionProductItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductItemMapping_Item");

                entity.HasOne(d => d.PromotionProduct)
                    .WithMany(p => p.PromotionProductItemMappings)
                    .HasForeignKey(d => d.PromotionProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductItemMapping_PromotionProduct");
            });

            modelBuilder.Entity<PromotionProductTypeDAO>(entity =>
            {
                entity.ToTable("PromotionProductType", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.ProductType)
                    .WithMany(p => p.PromotionProductTypes)
                    .HasForeignKey(d => d.ProductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductType_ProductType");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionProductTypes)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductType_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionProductTypes)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductType_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionProductTypes)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductType_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionProductTypeItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionProductTypeId, e.ItemId });

                entity.ToTable("PromotionProductTypeItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionProductTypeItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductTypeItemMapping_Item");

                entity.HasOne(d => d.PromotionProductType)
                    .WithMany(p => p.PromotionProductTypeItemMappings)
                    .HasForeignKey(d => d.PromotionProductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionProductTypeItemMapping_PromotionProductType");
            });

            modelBuilder.Entity<PromotionPromotionPolicyMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.PromotionPolicyId });

                entity.ToTable("PromotionPromotionPolicyMapping", "PRO");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionPromotionPolicyMappings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionPromotionPolicyMapping_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionPromotionPolicyMappings)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionPromotionPolicyMapping_PromotionPolicy");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PromotionPromotionPolicyMappings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionPromotionPolicyMapping_Status");
            });

            modelBuilder.Entity<PromotionSamePriceDAO>(entity =>
            {
                entity.ToTable("PromotionSamePrice", "PRO");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionSamePrices)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionSamePrice_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionSamePrices)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionSamePrice_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionStoreDAO>(entity =>
            {
                entity.ToTable("PromotionStore", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionStores)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStore_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStores)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStore_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionStores)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStore_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionStoreGroupingDAO>(entity =>
            {
                entity.ToTable("PromotionStoreGrouping", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionStoreGroupings)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGrouping_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStoreGroupings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGrouping_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionStoreGroupings)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGrouping_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionStoreGroupingItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionStoreGroupingId, e.itemId });

                entity.ToTable("PromotionStoreGroupingItemMapping", "PRO");

                entity.Property(e => e.Quantity)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.HasOne(d => d.PromotionStoreGrouping)
                    .WithMany(p => p.PromotionStoreGroupingItemMappings)
                    .HasForeignKey(d => d.PromotionStoreGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGroupingItemMapping_PromotionStoreGrouping");

                entity.HasOne(d => d.item)
                    .WithMany(p => p.PromotionStoreGroupingItemMappings)
                    .HasForeignKey(d => d.itemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGroupingItemMapping_Item");
            });

            modelBuilder.Entity<PromotionStoreGroupingMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.StoreGroupingId });

                entity.ToTable("PromotionStoreGroupingMapping", "PRO");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStoreGroupingMappings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGroupingMapping_Promotion");

                entity.HasOne(d => d.StoreGrouping)
                    .WithMany(p => p.PromotionStoreGroupingMappings)
                    .HasForeignKey(d => d.StoreGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreGroupingMapping_StoreGrouping");
            });

            modelBuilder.Entity<PromotionStoreItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionStoreId, e.ItemId });

                entity.ToTable("PromotionStoreItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionStoreItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreItemMapping_Item");

                entity.HasOne(d => d.PromotionStore)
                    .WithMany(p => p.PromotionStoreItemMappings)
                    .HasForeignKey(d => d.PromotionStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreItemMapping_PromotionStore");
            });

            modelBuilder.Entity<PromotionStoreMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.StoreId });

                entity.ToTable("PromotionStoreMapping", "PRO");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStoreMappings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreMapping_Promotion");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.PromotionStoreMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreMapping_Store");
            });

            modelBuilder.Entity<PromotionStoreTypeDAO>(entity =>
            {
                entity.ToTable("PromotionStoreType", "PRO");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FromValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ToValue).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.PromotionDiscountType)
                    .WithMany(p => p.PromotionStoreTypes)
                    .HasForeignKey(d => d.PromotionDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreType_PromotionDiscountType");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStoreTypes)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreType_Promotion");

                entity.HasOne(d => d.PromotionPolicy)
                    .WithMany(p => p.PromotionStoreTypes)
                    .HasForeignKey(d => d.PromotionPolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreType_PromotionPolicy");
            });

            modelBuilder.Entity<PromotionStoreTypeItemMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionStoreTypeId, e.ItemId });

                entity.ToTable("PromotionStoreTypeItemMapping", "PRO");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PromotionStoreTypeItemMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreTypeItemMapping_Item");

                entity.HasOne(d => d.PromotionStoreType)
                    .WithMany(p => p.PromotionStoreTypeItemMappings)
                    .HasForeignKey(d => d.PromotionStoreTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreTypeItemMapping_PromotionStoreType");
            });

            modelBuilder.Entity<PromotionStoreTypeMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.StoreTypeId });

                entity.ToTable("PromotionStoreTypeMapping", "PRO");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStoreTypeMappings)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreTypeMapping_Promotion");

                entity.HasOne(d => d.StoreType)
                    .WithMany(p => p.PromotionStoreTypeMappings)
                    .HasForeignKey(d => d.StoreTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PromotionStoreTypeMapping_StoreType");
            });

            modelBuilder.Entity<PromotionTypeDAO>(entity =>
            {
                entity.ToTable("PromotionType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ProvinceDAO>(entity =>
            {
                entity.ToTable("Province", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Province_Status");
            });

            modelBuilder.Entity<RequestStateDAO>(entity =>
            {
                entity.ToTable("RequestState", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RequestWorkflowDefinitionMappingDAO>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("RequestWorkflowDefinitionMapping", "WF");

                entity.HasIndex(e => e.CreatorId);

                entity.HasIndex(e => e.RequestId);

                entity.HasIndex(e => e.RequestStateId);

                entity.HasIndex(e => e.WorkflowDefinitionId);

                entity.HasIndex(e => new { e.RequestId, e.RequestStateId });

                entity.Property(e => e.RequestId).ValueGeneratedNever();

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.RequestWorkflowDefinitionMappings)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowDefinitionMapping_RequestState");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.RequestWorkflowDefinitionMappings)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowDefinitionMapping_WorkflowDefinition");
            });

            modelBuilder.Entity<RequestWorkflowHistoryDAO>(entity =>
            {
                entity.HasKey(e => new { e.Counter, e.RequestId, e.WorkflowStepId });

                entity.ToTable("RequestWorkflowHistory", "WF");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("FK_RequestWorkflowHistory_AppUser");

                entity.HasOne(d => d.WorkflowState)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.WorkflowStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowHistory_WorkflowState");

                entity.HasOne(d => d.WorkflowStep)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.WorkflowStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowHistory_WorkflowStep");
            });

            modelBuilder.Entity<RequestWorkflowParameterMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.WorkflowParameterId, e.RequestId });

                entity.ToTable("RequestWorkflowParameterMapping", "WF");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameter)
                    .WithMany(p => p.RequestWorkflowParameterMappings)
                    .HasForeignKey(d => d.WorkflowParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreWorkflowParameterMapping_WorkflowParameter");
            });

            modelBuilder.Entity<RequestWorkflowStepMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.WorkflowStepId });

                entity.ToTable("RequestWorkflowStepMapping", "WF");

                entity.HasIndex(e => e.AppUserId);

                entity.HasIndex(e => new { e.WorkflowStepId, e.WorkflowStateId });

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("FK_StoreWorkflow_AppUser");

                entity.HasOne(d => d.WorkflowState)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.WorkflowStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreWorkflow_WorkflowState");

                entity.HasOne(d => d.WorkflowStep)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.WorkflowStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflow_WorkflowStep");
            });

            modelBuilder.Entity<ResellerDAO>(entity =>
            {
                entity.ToTable("Reseller", "MDM");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DeputyName).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TaxCode).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Resellers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reseller_Organization");

                entity.HasOne(d => d.ResellerStatus)
                    .WithMany(p => p.Resellers)
                    .HasForeignKey(d => d.ResellerStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reseller_ResellerStatus");

                entity.HasOne(d => d.ResellerType)
                    .WithMany(p => p.Resellers)
                    .HasForeignKey(d => d.ResellerTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reseller_ResellerType");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Resellers)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reseller_AppUser");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Resellers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reseller_Status");
            });

            modelBuilder.Entity<ResellerStatusDAO>(entity =>
            {
                entity.ToTable("ResellerStatus", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ResellerTypeDAO>(entity =>
            {
                entity.ToTable("ResellerType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<RewardHistoryDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.RewardHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RewardHistory_AppUser");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.RewardHistories)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RewardHistory_Store");
            });

            modelBuilder.Entity<RewardHistoryContentDAO>(entity =>
            {
                entity.HasOne(d => d.LuckyNumber)
                    .WithMany(p => p.RewardHistoryContents)
                    .HasForeignKey(d => d.LuckyNumberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RewardHistoryContent_LuckyNumber");

                entity.HasOne(d => d.RewardHistory)
                    .WithMany(p => p.RewardHistoryContents)
                    .HasForeignKey(d => d.RewardHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RewardHistoryContent_RewardHistory");
            });

            modelBuilder.Entity<RewardStatusDAO>(entity =>
            {
                entity.ToTable("RewardStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.ToTable("Role", "PER");

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Status");
            });

            modelBuilder.Entity<SalesOrderTypeDAO>(entity =>
            {
                entity.ToTable("SalesOrderType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SexDAO>(entity =>
            {
                entity.ToTable("Sex", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StatusDAO>(entity =>
            {
                entity.ToTable("Status", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<StoreDAO>(entity =>
            {
                entity.ToTable("Store", "MDM");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("IX_Store_Organization");

                entity.Property(e => e.Address).HasMaxLength(3000);

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.CodeDraft).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DeliveryAddress).HasMaxLength(3000);

                entity.Property(e => e.DeliveryLatitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.DeliveryLongitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.LegalEntity).HasMaxLength(500);

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.OwnerEmail).HasMaxLength(500);

                entity.Property(e => e.OwnerName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OwnerPhone)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TaxCode).HasMaxLength(500);

                entity.Property(e => e.Telephone).HasMaxLength(500);

                entity.Property(e => e.UnsignAddress).HasMaxLength(3000);

                entity.Property(e => e.UnsignName).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("FK_Store_AppUser");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Store_District");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Organization");

                entity.HasOne(d => d.ParentStore)
                    .WithMany(p => p.InverseParentStore)
                    .HasForeignKey(d => d.ParentStoreId)
                    .HasConstraintName("FK_Store_Store");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Store_Province");

                entity.HasOne(d => d.Reseller)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.ResellerId)
                    .HasConstraintName("FK_Store_Reseller");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Status");

                entity.HasOne(d => d.StoreGrouping)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.StoreGroupingId)
                    .HasConstraintName("FK_Store_StoreGrouping");

                entity.HasOne(d => d.StoreScouting)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.StoreScoutingId)
                    .HasConstraintName("FK_Store_StoreScouting");

                entity.HasOne(d => d.StoreStatus)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.StoreStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_StoreStatus");

                entity.HasOne(d => d.StoreType)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.StoreTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_StoreType");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Store_Ward");
            });

            modelBuilder.Entity<StoreCheckingDAO>(entity =>
            {
                entity.Property(e => e.CheckInAt).HasColumnType("datetime");

                entity.Property(e => e.CheckOutAt).HasColumnType("datetime");

                entity.Property(e => e.CheckOutLatitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.CheckOutLongitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.DeviceName).HasMaxLength(200);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 15)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.StoreCheckings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreChecking_Organization");

                entity.HasOne(d => d.SaleEmployee)
                    .WithMany(p => p.StoreCheckings)
                    .HasForeignKey(d => d.SaleEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreChecking_AppUser");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreCheckings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreChecking_Store");
            });

            modelBuilder.Entity<StoreCheckingImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ImageId, e.StoreCheckingId })
                    .HasName("PK_ImageStoreCheckingMapping");

                entity.Property(e => e.ShootingAt).HasColumnType("datetime");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.StoreCheckingImageMappings)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageStoreCheckingMapping_Album");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.StoreCheckingImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageStoreCheckingMapping_Image");

                entity.HasOne(d => d.SaleEmployee)
                    .WithMany(p => p.StoreCheckingImageMappings)
                    .HasForeignKey(d => d.SaleEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageStoreCheckingMapping_AppUser");

                entity.HasOne(d => d.StoreChecking)
                    .WithMany(p => p.StoreCheckingImageMappings)
                    .HasForeignKey(d => d.StoreCheckingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageStoreCheckingMapping_StoreChecking");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreCheckingImageMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageStoreCheckingMapping_Store");
            });

            modelBuilder.Entity<StoreGroupingDAO>(entity =>
            {
                entity.ToTable("StoreGrouping", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_StoreGrouping_StoreGrouping");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StoreGroupings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreGrouping_Status");
            });

            modelBuilder.Entity<StoreImageDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StoreImage");

                entity.Property(e => e.AlbumName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DisplayName).HasMaxLength(500);

                entity.Property(e => e.ShootingAt).HasColumnType("datetime");

                entity.Property(e => e.StoreAddress).HasMaxLength(3000);

                entity.Property(e => e.StoreName).HasMaxLength(500);

                entity.Property(e => e.ThumbnailUrl).HasMaxLength(4000);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<StoreImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.ImageId });

                entity.ToTable("StoreImageMapping", "MDM");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.StoreImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreImageMapping_Image");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreImageMappings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreImageMapping_Store");
            });

            modelBuilder.Entity<StoreScoutingDAO>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OwnerPhone).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScouting_AppUser");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_StoreScouting_District");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScouting_Organization");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_StoreScouting_Province");

                entity.HasOne(d => d.StoreScoutingStatus)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.StoreScoutingStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScouting_StoreScouting");

                entity.HasOne(d => d.StoreScoutingType)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.StoreScoutingTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScouting_StoreScoutingType");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.StoreScoutings)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_StoreScouting_Ward");
            });

            modelBuilder.Entity<StoreScoutingImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.StoreScoutingId, e.ImageId });

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.StoreScoutingImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScoutingImageMapping_Image");

                entity.HasOne(d => d.StoreScouting)
                    .WithMany(p => p.StoreScoutingImageMappings)
                    .HasForeignKey(d => d.StoreScoutingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScoutingImageMapping_StoreScouting");
            });

            modelBuilder.Entity<StoreScoutingStatusDAO>(entity =>
            {
                entity.ToTable("StoreScoutingStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StoreScoutingTypeDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StoreScoutingTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreScoutingType_Status");
            });

            modelBuilder.Entity<StoreStatusDAO>(entity =>
            {
                entity.ToTable("StoreStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StoreTypeDAO>(entity =>
            {
                entity.ToTable("StoreType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.StoreTypes)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_StoreType_Color");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StoreTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreType_Status");
            });

            modelBuilder.Entity<StoreUncheckingDAO>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.StoreUncheckings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreUnchecking_AppUser");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.StoreUncheckings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreUnchecking_Organization");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreUncheckings)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreUnchecking_Store");
            });

            modelBuilder.Entity<SupplierDAO>(entity =>
            {
                entity.ToTable("Supplier", "MDM");

                entity.Property(e => e.Address).HasMaxLength(2000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OwnerName).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.TaxCode).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Supplier_District");

                entity.HasOne(d => d.PersonInCharge)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.PersonInChargeId)
                    .HasConstraintName("FK_Supplier_AppUser");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Supplier_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Supplier_Status");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Supplier_Ward");
            });

            modelBuilder.Entity<SurveyDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.EndAt).HasColumnType("datetime");

                entity.Property(e => e.StartAt).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Survey_AppUser");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Survey_Status");
            });

            modelBuilder.Entity<SurveyOptionDAO>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.SurveyOptionType)
                    .WithMany(p => p.SurveyOptions)
                    .HasForeignKey(d => d.SurveyOptionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyOption_SurveyOptionType");

                entity.HasOne(d => d.SurveyQuestion)
                    .WithMany(p => p.SurveyOptions)
                    .HasForeignKey(d => d.SurveyQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyOption_SurveyQuestion");
            });

            modelBuilder.Entity<SurveyOptionTypeDAO>(entity =>
            {
                entity.ToTable("SurveyOptionType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<SurveyQuestionDAO>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.SurveyQuestions)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestion_Survey");

                entity.HasOne(d => d.SurveyQuestionType)
                    .WithMany(p => p.SurveyQuestions)
                    .HasForeignKey(d => d.SurveyQuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestion_SurveyQuestionType");
            });

            modelBuilder.Entity<SurveyQuestionTypeDAO>(entity =>
            {
                entity.ToTable("SurveyQuestionType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<SurveyRespondentTypeDAO>(entity =>
            {
                entity.ToTable("SurveyRespondentType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SurveyResultDAO>(entity =>
            {
                entity.Property(e => e.RespondentAddress).HasMaxLength(500);

                entity.Property(e => e.RespondentEmail).HasMaxLength(500);

                entity.Property(e => e.RespondentName).HasMaxLength(500);

                entity.Property(e => e.RespondentPhone).HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResult_AppUser");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResult_Organization");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_SurveyResult_Store2");

                entity.HasOne(d => d.StoreScouting)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.StoreScoutingId)
                    .HasConstraintName("FK_SurveyResult_StoreScouting");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResult_Survey");

                entity.HasOne(d => d.SurveyRespondentType)
                    .WithMany(p => p.SurveyResults)
                    .HasForeignKey(d => d.SurveyRespondentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResult_SurveyRespondentType");
            });

            modelBuilder.Entity<SurveyResultCellDAO>(entity =>
            {
                entity.HasKey(e => new { e.SurveyResultId, e.SurveyQuestionId, e.RowOptionId, e.ColumnOptionId })
                    .HasName("PK_SurveyResultTable");

                entity.HasOne(d => d.ColumnOption)
                    .WithMany(p => p.SurveyResultCellColumnOptions)
                    .HasForeignKey(d => d.ColumnOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultTable_SurveyOption1");

                entity.HasOne(d => d.RowOption)
                    .WithMany(p => p.SurveyResultCellRowOptions)
                    .HasForeignKey(d => d.RowOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultTable_SurveyOption");

                entity.HasOne(d => d.SurveyQuestion)
                    .WithMany(p => p.SurveyResultCells)
                    .HasForeignKey(d => d.SurveyQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultCell_SurveyQuestion");

                entity.HasOne(d => d.SurveyResult)
                    .WithMany(p => p.SurveyResultCells)
                    .HasForeignKey(d => d.SurveyResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultCell_SurveyResult");
            });

            modelBuilder.Entity<SurveyResultSingleDAO>(entity =>
            {
                entity.HasKey(e => new { e.SurveyResultId, e.SurveyQuestionId, e.SurveyOptionId })
                    .HasName("PK_SurveyResultSingle_1");

                entity.HasOne(d => d.SurveyOption)
                    .WithMany(p => p.SurveyResultSingles)
                    .HasForeignKey(d => d.SurveyOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultSingle_SurveyOption");

                entity.HasOne(d => d.SurveyQuestion)
                    .WithMany(p => p.SurveyResultSingles)
                    .HasForeignKey(d => d.SurveyQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultSingle_SurveyQuestion");

                entity.HasOne(d => d.SurveyResult)
                    .WithMany(p => p.SurveyResultSingles)
                    .HasForeignKey(d => d.SurveyResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResultSingle_SurveyResult");
            });

            modelBuilder.Entity<SurveyResultTextDAO>(entity =>
            {
                entity.HasKey(e => new { e.SurveyResultId, e.SurveyQuestionId })
                    .HasName("PK_SurveyRessultText");

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.HasOne(d => d.SurveyQuestion)
                    .WithMany(p => p.SurveyResultTexts)
                    .HasForeignKey(d => d.SurveyQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyRessultText_SurveyQuestion");

                entity.HasOne(d => d.SurveyResult)
                    .WithMany(p => p.SurveyResultTexts)
                    .HasForeignKey(d => d.SurveyResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyRessultText_SurveyResult");
            });

            modelBuilder.Entity<SystemConfigurationDAO>(entity =>
            {
                entity.ToTable("SystemConfiguration", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Value).HasMaxLength(500);
            });

            modelBuilder.Entity<TaxTypeDAO>(entity =>
            {
                entity.ToTable("TaxType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaxTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TaxType_Status");
            });

            modelBuilder.Entity<UnitOfMeasureDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasure", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UnitOfMeasures)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasure_Status");
            });

            modelBuilder.Entity<UnitOfMeasureGroupingDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasureGrouping", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UnitOfMeasureGroupings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGrouping_Status");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.UnitOfMeasureGroupings)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGrouping_UnitOfMeasure");
            });

            modelBuilder.Entity<UnitOfMeasureGroupingContentDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasureGroupingContent", "MDM");

                entity.HasOne(d => d.UnitOfMeasureGrouping)
                    .WithMany(p => p.UnitOfMeasureGroupingContents)
                    .HasForeignKey(d => d.UnitOfMeasureGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGroupingContent_UnitOfMeasureGrouping");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.UnitOfMeasureGroupingContents)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGroupingContent_UnitOfMeasure");
            });

            modelBuilder.Entity<UsedVariationDAO>(entity =>
            {
                entity.ToTable("UsedVariation", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VariationDAO>(entity =>
            {
                entity.ToTable("Variation", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.VariationGrouping)
                    .WithMany(p => p.Variations)
                    .HasForeignKey(d => d.VariationGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Variation_VariationGrouping");
            });

            modelBuilder.Entity<VariationGroupingDAO>(entity =>
            {
                entity.ToTable("VariationGrouping", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VariationGroupings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VariationGrouping_Product");
            });

            modelBuilder.Entity<WardDAO>(entity =>
            {
                entity.ToTable("Ward", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_District");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_Status");
            });

            modelBuilder.Entity<WarehouseDAO>(entity =>
            {
                entity.ToTable("Warehouse", "MDM");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Warehouse_District");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Warehouse_Organization");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Warehouse_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Warehouse_Status");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Warehouse_Ward");
            });

            modelBuilder.Entity<WorkflowDefinitionDAO>(entity =>
            {
                entity.ToTable("WorkflowDefinition", "WF");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.WorkflowDefinitionCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_AppUser");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.WorkflowDefinitionModifiers)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_AppUser1");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_Status");

                entity.HasOne(d => d.WorkflowType)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.WorkflowTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_WorkflowType");
            });

            modelBuilder.Entity<WorkflowDirectionDAO>(entity =>
            {
                entity.ToTable("WorkflowDirection", "WF");

                entity.Property(e => e.BodyMailForCreator).HasMaxLength(4000);

                entity.Property(e => e.BodyMailForCurrentStep).HasMaxLength(4000);

                entity.Property(e => e.BodyMailForNextStep).HasMaxLength(4000);

                entity.Property(e => e.SubjectMailForCreator).HasMaxLength(500);

                entity.Property(e => e.SubjectMailForCurrentStep).HasMaxLength(500);

                entity.Property(e => e.SubjectMailForNextStep).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.FromStep)
                    .WithMany(p => p.WorkflowDirectionFromSteps)
                    .HasForeignKey(d => d.FromStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowStep");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowDirections)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_Status");

                entity.HasOne(d => d.ToStep)
                    .WithMany(p => p.WorkflowDirectionToSteps)
                    .HasForeignKey(d => d.ToStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowStep1");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.WorkflowDirections)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowDefinition");
            });

            modelBuilder.Entity<WorkflowDirectionConditionDAO>(entity =>
            {
                entity.ToTable("WorkflowDirectionCondition", "WF");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowDirection)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowDirectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowDirection");

                entity.HasOne(d => d.WorkflowOperator)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowOperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowOperator");

                entity.HasOne(d => d.WorkflowParameter)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowParameter");
            });

            modelBuilder.Entity<WorkflowOperatorDAO>(entity =>
            {
                entity.ToTable("WorkflowOperator", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameterType)
                    .WithMany(p => p.WorkflowOperators)
                    .HasForeignKey(d => d.WorkflowParameterTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowOperator_WorkflowParameterType");
            });

            modelBuilder.Entity<WorkflowParameterDAO>(entity =>
            {
                entity.ToTable("WorkflowParameter", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameterType)
                    .WithMany(p => p.WorkflowParameters)
                    .HasForeignKey(d => d.WorkflowParameterTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowParameter_WorkflowParameterType");

                entity.HasOne(d => d.WorkflowType)
                    .WithMany(p => p.WorkflowParameters)
                    .HasForeignKey(d => d.WorkflowTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowParameter_WorkflowType");
            });

            modelBuilder.Entity<WorkflowParameterTypeDAO>(entity =>
            {
                entity.ToTable("WorkflowParameterType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<WorkflowStateDAO>(entity =>
            {
                entity.ToTable("WorkflowState", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<WorkflowStepDAO>(entity =>
            {
                entity.ToTable("WorkflowStep", "WF");

                entity.Property(e => e.BodyMailForReject).HasMaxLength(4000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.SubjectMailForReject).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_Status");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_WorkflowDefinition");
            });

            modelBuilder.Entity<WorkflowTypeDAO>(entity =>
            {
                entity.ToTable("WorkflowType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
