using System;using Thinktecture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DMS.DWModels
{
    public partial class DWContext : DbContext
    {
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<Dim_AlbumDAO> Dim_Album { get; set; }
        public virtual DbSet<Dim_AppUserDAO> Dim_AppUser { get; set; }
        public virtual DbSet<Dim_BrandDAO> Dim_Brand { get; set; }
        public virtual DbSet<Dim_CategoryDAO> Dim_Category { get; set; }
        public virtual DbSet<Dim_DateDAO> Dim_Date { get; set; }
        public virtual DbSet<Dim_DirectSalesOrderSourceTypeDAO> Dim_DirectSalesOrderSourceType { get; set; }
        public virtual DbSet<Dim_DistrictDAO> Dim_District { get; set; }
        public virtual DbSet<Dim_ERouteTypeDAO> Dim_ERouteType { get; set; }
        public virtual DbSet<Dim_ItemDAO> Dim_Item { get; set; }
        public virtual DbSet<Dim_OrganizationDAO> Dim_Organization { get; set; }
        public virtual DbSet<Dim_POSMTransactionTypeDAO> Dim_POSMTransactionType { get; set; }
        public virtual DbSet<Dim_ProblemTypeDAO> Dim_ProblemType { get; set; }
        public virtual DbSet<Dim_ProductDAO> Dim_Product { get; set; }
        public virtual DbSet<Dim_ProductGroupingDAO> Dim_ProductGrouping { get; set; }
        public virtual DbSet<Dim_ProductTypeDAO> Dim_ProductType { get; set; }
        public virtual DbSet<Dim_ProvinceDAO> Dim_Province { get; set; }
        public virtual DbSet<Dim_RequestStateDAO> Dim_RequestState { get; set; }
        public virtual DbSet<Dim_StatusDAO> Dim_Status { get; set; }
        public virtual DbSet<Dim_StoreDAO> Dim_Store { get; set; }
        public virtual DbSet<Dim_StoreGroupingDAO> Dim_StoreGrouping { get; set; }
        public virtual DbSet<Dim_StoreScoutingDAO> Dim_StoreScouting { get; set; }
        public virtual DbSet<Dim_StoreScoutingTypeDAO> Dim_StoreScoutingType { get; set; }
        public virtual DbSet<Dim_StoreStatusDAO> Dim_StoreStatus { get; set; }
        public virtual DbSet<Dim_StoreTypeDAO> Dim_StoreType { get; set; }
        public virtual DbSet<Dim_SupplierDAO> Dim_Supplier { get; set; }
        public virtual DbSet<Dim_TaxTypeDAO> Dim_TaxType { get; set; }
        public virtual DbSet<Dim_UnitOfMeasureDAO> Dim_UnitOfMeasure { get; set; }
        public virtual DbSet<Dim_UsedVariationDAO> Dim_UsedVariation { get; set; }
        public virtual DbSet<Dim_WardDAO> Dim_Ward { get; set; }
        public virtual DbSet<Dim_WarehouseDAO> Dim_Warehouse { get; set; }
        public virtual DbSet<Fact_DirectSalesOrderDAO> Fact_DirectSalesOrder { get; set; }
        public virtual DbSet<Fact_ERouteDAO> Fact_ERoute { get; set; }
        public virtual DbSet<Fact_IndirectSalesOrderDAO> Fact_IndirectSalesOrder { get; set; }
        public virtual DbSet<Fact_POSMTransactionDAO> Fact_POSMTransaction { get; set; }
        public virtual DbSet<Fact_ProblemDAO> Fact_Problem { get; set; }
        public virtual DbSet<Fact_StoreCheckingDAO> Fact_StoreChecking { get; set; }
        public virtual DbSet<Fact_StoreUncheckingDAO> Fact_StoreUnchecking { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }

        public DWContext(DbContextOptions<DWContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=192.168.20.200;initial catalog=dw_dms;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregatedCounterDAO>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<CounterDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Dim_AlbumDAO>(entity =>
            {
                entity.HasKey(e => e.AlbumId);

                entity.Property(e => e.AlbumId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Dim_AppUserDAO>(entity =>
            {
                entity.HasKey(e => e.AppUserId);

                entity.Property(e => e.AppUserId)
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

                entity.Property(e => e.OrganizationId).HasComment("Đơn vị công tác");

                entity.Property(e => e.Phone)
                    .HasMaxLength(500)
                    .HasComment("Số điện thoại liên hệ");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên đăng nhập");
            });

            modelBuilder.Entity<Dim_BrandDAO>(entity =>
            {
                entity.HasKey(e => e.BrandId);

                entity.Property(e => e.BrandId)
                    .HasComment("Id")
                    .ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_CategoryDAO>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_DateDAO>(entity =>
            {
                entity.HasKey(e => e.DateKey);

                entity.Property(e => e.DateKey).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.MonthName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.QuarterName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WeekDayName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_DirectSalesOrderSourceTypeDAO>(entity =>
            {
                entity.HasKey(e => e.DirectSalesOrderSourceTypeId);

                entity.Property(e => e.DirectSalesOrderSourceTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_DistrictDAO>(entity =>
            {
                entity.HasKey(e => e.DistrictId);

                entity.Property(e => e.DistrictId)
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
            });

            modelBuilder.Entity<Dim_ERouteTypeDAO>(entity =>
            {
                entity.HasKey(e => e.ERouteTypeId);

                entity.Property(e => e.ERouteTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_ItemDAO>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.Property(e => e.ItemId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_OrganizationDAO>(entity =>
            {
                entity.HasKey(e => e.OrganizationId);

                entity.Property(e => e.OrganizationId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_POSMTransactionTypeDAO>(entity =>
            {
                entity.HasKey(e => e.POSMTransactionTypeId);

                entity.Property(e => e.POSMTransactionTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Dim_ProblemTypeDAO>(entity =>
            {
                entity.HasKey(e => e.ProblemTypeId);

                entity.Property(e => e.ProblemTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_ProductDAO>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_ProductGroupingDAO>(entity =>
            {
                entity.HasKey(e => e.ProductGroupingId);

                entity.Property(e => e.ProductGroupingId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_ProductTypeDAO>(entity =>
            {
                entity.HasKey(e => e.ProductTypeId);

                entity.Property(e => e.ProductTypeId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_ProvinceDAO>(entity =>
            {
                entity.HasKey(e => e.ProvinceId);

                entity.Property(e => e.ProvinceId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Dim_RequestStateDAO>(entity =>
            {
                entity.HasKey(e => e.RequestStateId);

                entity.Property(e => e.RequestStateId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_StatusDAO>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Dim_StoreDAO>(entity =>
            {
                entity.HasKey(e => e.StoreId);

                entity.Property(e => e.StoreId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(3000);

                entity.Property(e => e.Code).HasMaxLength(400);

                entity.Property(e => e.CodeDraft).HasMaxLength(500);

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
            });

            modelBuilder.Entity<Dim_StoreGroupingDAO>(entity =>
            {
                entity.HasKey(e => e.StoreGroupingId);

                entity.Property(e => e.StoreGroupingId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<Dim_StoreScoutingDAO>(entity =>
            {
                entity.HasKey(e => e.StoreScoutingId);

                entity.Property(e => e.StoreScoutingId).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OwnerPhone).HasMaxLength(500);
            });

            modelBuilder.Entity<Dim_StoreScoutingTypeDAO>(entity =>
            {
                entity.HasKey(e => e.StoreScoutingTypeId);

                entity.Property(e => e.StoreScoutingTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_StoreStatusDAO>(entity =>
            {
                entity.HasKey(e => e.StoreStatusId);

                entity.Property(e => e.StoreStatusId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_StoreTypeDAO>(entity =>
            {
                entity.HasKey(e => e.StoreTypeId);

                entity.Property(e => e.StoreTypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Dim_SupplierDAO>(entity =>
            {
                entity.HasKey(e => e.SupplierId);

                entity.Property(e => e.SupplierId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_TaxTypeDAO>(entity =>
            {
                entity.HasKey(e => e.TaxTypeId);

                entity.Property(e => e.TaxTypeId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_UnitOfMeasureDAO>(entity =>
            {
                entity.HasKey(e => e.UnitOfMeasureId);

                entity.Property(e => e.UnitOfMeasureId).ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Dim_UsedVariationDAO>(entity =>
            {
                entity.HasKey(e => e.UsedVariationId);

                entity.Property(e => e.UsedVariationId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dim_WardDAO>(entity =>
            {
                entity.HasKey(e => e.WardId);

                entity.Property(e => e.WardId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Dim_WarehouseDAO>(entity =>
            {
                entity.HasKey(e => e.WarehouseId);

                entity.Property(e => e.WarehouseId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Fact_DirectSalesOrderDAO>(entity =>
            {
                entity.HasKey(e => e.DirectSaleOrderTransactionId);

                entity.Property(e => e.DirectSaleOrderTransactionId).ValueGeneratedNever();

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

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.EditedPriceStatusId).HasComment("Sửa giá");

                entity.Property(e => e.GeneralDiscount)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Số tiền chiết khấu tổng");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasComment("Ghi chú");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.PromotionCode).HasMaxLength(50);

                entity.Property(e => e.PromotionValue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 4)");

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
            });

            modelBuilder.Entity<Fact_ERouteDAO>(entity =>
            {
                entity.HasKey(e => e.ERouteContentDayId);

                entity.Property(e => e.ERouteContentDayId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.RealStartDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Fact_IndirectSalesOrderDAO>(entity =>
            {
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

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.EditedPriceStatusId).HasComment("Sửa giá");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasComment("Ghi chú");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SellerStoreId).HasComment("Cửa hàng bán");

                entity.Property(e => e.StoreAddress).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Fact_POSMTransactionDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.TransactionAmount).HasColumnType("decimal(18, 4)");
            });

            modelBuilder.Entity<Fact_ProblemDAO>(entity =>
            {
                entity.HasKey(e => e.ProblemId);

                entity.Property(e => e.ProblemId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CompletedAt).HasColumnType("datetime");

                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.NoteAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Fact_StoreCheckingDAO>(entity =>
            {
                entity.HasKey(e => e.StoreCheckingId);

                entity.Property(e => e.StoreCheckingId).ValueGeneratedNever();

                entity.Property(e => e.CheckInAt).HasColumnType("datetime");

                entity.Property(e => e.CheckOutAt).HasColumnType("datetime");

                entity.Property(e => e.CheckOutLatitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.CheckOutLongitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.DeviceName).HasMaxLength(200);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 15)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 15)");
            });

            modelBuilder.Entity<Fact_StoreUncheckingDAO>(entity =>
            {
                entity.HasKey(e => e.StoreUnchecking);

                entity.Property(e => e.StoreUnchecking).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<HashDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<JobDAO>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameterDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueueDAO>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<ServerDAO>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<StateDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
