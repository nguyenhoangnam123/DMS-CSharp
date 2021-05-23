using DMS.ABE.Common;
using DMS.ABE.Models;
using System;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAppUserRepository AppUserRepository { get; }
        IBannerRepository BannerRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; }
        IDirectSalesOrderRepository DirectSalesOrderRepository { get; }
        IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; }
        IFieldRepository FieldRepository { get; }
        IImageRepository ImageRepository { get; }
        IItemRepository ItemRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IPriceListItemMappingItemMappingRepository PriceListItemMappingItemMappingRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IStoreRepository StoreRepository { get; }
        IStoreGroupingRepository StoreGroupingRepository { get; }
        IStoreTypeRepository StoreTypeRepository { get; }
        ISystemConfigurationRepository SystemConfigurationRepository { get; }
        IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; }
        IVariationRepository VariationRepository { get; }
        IVariationGroupingRepository VariationGroupingRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IStoreUserRepository StoreUserRepository { get; }
        IStoreUserFavoriteProductMappingRepository StoreUserFavoriteProductMappingRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAppUserRepository AppUserRepository { get; private set; }
        public IBannerRepository BannerRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; private set; }
        public IDirectSalesOrderRepository DirectSalesOrderRepository { get; private set; }
        public IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IPriceListItemMappingItemMappingRepository PriceListItemMappingItemMappingRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IStoreRepository StoreRepository { get; private set; }
        public IStoreGroupingRepository StoreGroupingRepository { get; private set; }
        public IStoreTypeRepository StoreTypeRepository { get; private set; }
        public ISystemConfigurationRepository SystemConfigurationRepository { get; private set; }
        public IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; private set; }
        public IVariationRepository VariationRepository { get; private set; }
        public IVariationGroupingRepository VariationGroupingRepository { get; private set; }
        public IWarehouseRepository WarehouseRepository { get; private set; }
        public IStoreUserRepository StoreUserRepository { get; private set; }
        public IStoreUserFavoriteProductMappingRepository StoreUserFavoriteProductMappingRepository { get; private set; }

        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            BannerRepository = new BannerRepository(DataContext);
            CategoryRepository = new CategoryRepository(DataContext);
            DirectSalesOrderContentRepository = new DirectSalesOrderContentRepository(DataContext);
            DirectSalesOrderRepository = new DirectSalesOrderRepository(DataContext);
            DirectSalesOrderPromotionRepository = new DirectSalesOrderPromotionRepository(DataContext);
            FieldRepository = new FieldRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            InventoryRepository = new InventoryRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            PriceListItemMappingItemMappingRepository = new PriceListItemMappingItemMappingRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            StoreRepository = new StoreRepository(DataContext);
            StoreGroupingRepository = new StoreGroupingRepository(DataContext);
            StoreTypeRepository = new StoreTypeRepository(DataContext);
            SystemConfigurationRepository = new SystemConfigurationRepository(DataContext);
            UnitOfMeasureGroupingRepository = new UnitOfMeasureGroupingRepository(DataContext);
            VariationRepository = new VariationRepository(DataContext);
            VariationGroupingRepository = new VariationGroupingRepository(DataContext);
            WarehouseRepository = new WarehouseRepository(DataContext);
            StoreUserRepository = new StoreUserRepository(DataContext);
            StoreUserFavoriteProductMappingRepository = new StoreUserFavoriteProductMappingRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
        }

        public Task Commit()
        {
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }

            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}