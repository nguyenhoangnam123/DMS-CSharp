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
        IFieldRepository FieldRepository { get; }
        IImageRepository ImageRepository { get; }
        IItemRepository ItemRepository { get; }
        IMenuRepository MenuRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPageRepository PageRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IRoleRepository RoleRepository { get; }
        IStatusRepository StatusRepository { get; }
        IStoreRepository StoreRepository { get; }
        IStoreGroupingRepository StoreGroupingRepository { get; }
        IStoreTypeRepository StoreTypeRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        ITaxTypeRepository TaxTypeRepository { get; }
        IUnitOfMeasureRepository UnitOfMeasureRepository { get; }
        IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; }
        IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; }
        IUserStatusRepository UserStatusRepository { get; }
        IVariationRepository VariationRepository { get; }
        IVariationGroupingRepository VariationGroupingRepository { get; }
        IWardRepository WardRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAppUserRepository AppUserRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPageRepository PageRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IProvinceRepository ProvinceRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public IStoreRepository StoreRepository { get; private set; }
        public IStoreGroupingRepository StoreGroupingRepository { get; private set; }
        public IStoreTypeRepository StoreTypeRepository { get; private set; }
        public ISupplierRepository SupplierRepository { get; private set; }
        public ITaxTypeRepository TaxTypeRepository { get; private set; }
        public IUnitOfMeasureRepository UnitOfMeasureRepository { get; private set; }
        public IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; private set; }
        public IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; private set; }
        public IUserStatusRepository UserStatusRepository { get; private set; }
        public IVariationRepository VariationRepository { get; private set; }
        public IVariationGroupingRepository VariationGroupingRepository { get; private set; }
        public IWardRepository WardRepository { get; private set; }

        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            BrandRepository = new BrandRepository(DataContext);
            DistrictRepository = new DistrictRepository(DataContext);
            FieldRepository = new FieldRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            MenuRepository = new MenuRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PageRepository = new PageRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            ProvinceRepository = new ProvinceRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            StoreRepository = new StoreRepository(DataContext);
            StoreGroupingRepository = new StoreGroupingRepository(DataContext);
            StoreTypeRepository = new StoreTypeRepository(DataContext);
            SupplierRepository = new SupplierRepository(DataContext);
            TaxTypeRepository = new TaxTypeRepository(DataContext);
            UnitOfMeasureRepository = new UnitOfMeasureRepository(DataContext);
            UnitOfMeasureGroupingContentRepository = new UnitOfMeasureGroupingContentRepository(DataContext);
            UnitOfMeasureGroupingRepository = new UnitOfMeasureGroupingRepository(DataContext);
            UserStatusRepository = new UserStatusRepository(DataContext);
            VariationRepository = new VariationRepository(DataContext);
            VariationGroupingRepository = new VariationGroupingRepository(DataContext);
            WardRepository = new WardRepository(DataContext);
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