using Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using DMS.Models;
using DMS.Repositories;

namespace DMS.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAppUserRepository AppUserRepository { get; }
        IBrandRepository BrandRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IEditedPriceStatusRepository EditedPriceStatusRepository { get; }
        IEventMessageRepository EventMessageRepository { get; }
        IImageRepository ImageRepository { get; }
        IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; }
        IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; }
        IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; }
        IIndirectSalesOrderStatusRepository IndirectSalesOrderStatusRepository { get; }
        IItemRepository ItemRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IInventoryHistoryRepository InventoryHistoryRepository { get; }
        IMenuRepository MenuRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductProductGroupingMappingRepository ProductProductGroupingMappingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IResellerRepository ResellerRepository { get; }
        IResellerStatusRepository ResellerStatusRepository { get; }
        IResellerTypeRepository ResellerTypeRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISexRepository SexRepository { get; }
        IStatusRepository StatusRepository { get; }
        IStoreRepository StoreRepository { get; }
        IStoreGroupingRepository StoreGroupingRepository { get; }
        IStoreTypeRepository StoreTypeRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        ITaxTypeRepository TaxTypeRepository { get; }
        IUnitOfMeasureRepository UnitOfMeasureRepository { get; }
        IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; }
        IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; }
        IVariationRepository VariationRepository { get; }
        IVariationGroupingRepository VariationGroupingRepository { get; }
        IWardRepository WardRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; }
        IWorkflowStateRepository WorkflowStateRepository { get; }
        IWorkflowTypeRepository WorkflowTypeRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAppUserRepository AppUserRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IEditedPriceStatusRepository EditedPriceStatusRepository { get; private set; }
        public IEventMessageRepository EventMessageRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; private set; }
        public IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; private set; }
        public IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; private set; }
        public IIndirectSalesOrderStatusRepository IndirectSalesOrderStatusRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IInventoryHistoryRepository InventoryHistoryRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductProductGroupingMappingRepository ProductProductGroupingMappingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IProvinceRepository ProvinceRepository { get; private set; }
        public IResellerRepository ResellerRepository { get; private set; }
        public IResellerStatusRepository ResellerStatusRepository { get; private set; }
        public IResellerTypeRepository ResellerTypeRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public IStoreRepository StoreRepository { get; private set; }
        public IStoreGroupingRepository StoreGroupingRepository { get; private set; }
        public IStoreTypeRepository StoreTypeRepository { get; private set; }
        public ISupplierRepository SupplierRepository { get; private set; }
        public ITaxTypeRepository TaxTypeRepository { get; private set; }
        public IUnitOfMeasureRepository UnitOfMeasureRepository { get; private set; }
        public IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; private set; }
        public IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; private set; }
        public IVariationRepository VariationRepository { get; private set; }
        public IVariationGroupingRepository VariationGroupingRepository { get; private set; }
        public IWardRepository WardRepository { get; private set; }
        public IWarehouseRepository WarehouseRepository { get; private set; }
        public IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; private set; }
        public IWorkflowStateRepository WorkflowStateRepository { get; private set; }
        public IWorkflowTypeRepository WorkflowTypeRepository { get; private set; }
        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            BrandRepository = new BrandRepository(DataContext);
            DistrictRepository = new DistrictRepository(DataContext);
            EditedPriceStatusRepository = new EditedPriceStatusRepository(DataContext);
            EventMessageRepository = new EventMessageRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            IndirectSalesOrderContentRepository = new IndirectSalesOrderContentRepository(DataContext);
            IndirectSalesOrderRepository = new IndirectSalesOrderRepository(DataContext);
            IndirectSalesOrderPromotionRepository = new IndirectSalesOrderPromotionRepository(DataContext);
            IndirectSalesOrderStatusRepository = new IndirectSalesOrderStatusRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            InventoryRepository = new InventoryRepository(DataContext);
            InventoryHistoryRepository = new InventoryHistoryRepository(DataContext);
            MenuRepository = new MenuRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductProductGroupingMappingRepository = new ProductProductGroupingMappingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            ProvinceRepository = new ProvinceRepository(DataContext);
            ResellerRepository = new ResellerRepository(DataContext);
            ResellerStatusRepository = new ResellerStatusRepository(DataContext);
            ResellerTypeRepository = new ResellerTypeRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            StoreRepository = new StoreRepository(DataContext);
            StoreGroupingRepository = new StoreGroupingRepository(DataContext);
            StoreTypeRepository = new StoreTypeRepository(DataContext);
            SupplierRepository = new SupplierRepository(DataContext);
            TaxTypeRepository = new TaxTypeRepository(DataContext);
            UnitOfMeasureRepository = new UnitOfMeasureRepository(DataContext);
            UnitOfMeasureGroupingContentRepository = new UnitOfMeasureGroupingContentRepository(DataContext);
            UnitOfMeasureGroupingRepository = new UnitOfMeasureGroupingRepository(DataContext);
            VariationRepository = new VariationRepository(DataContext);
            VariationGroupingRepository = new VariationGroupingRepository(DataContext);
            WardRepository = new WardRepository(DataContext);
            WarehouseRepository = new WarehouseRepository(DataContext);
            WorkflowDefinitionRepository = new WorkflowDefinitionRepository(DataContext);
            WorkflowStateRepository = new WorkflowStateRepository(DataContext);
            WorkflowTypeRepository = new WorkflowTypeRepository(DataContext);
        }
        public async Task Begin()
        {
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}