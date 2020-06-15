using Common;
using DMS.Models;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAlbumRepository AlbumRepository { get; }
        IAppUserRepository AppUserRepository { get; }
        IBannerRepository BannerRepository { get; }
        IBrandRepository BrandRepository { get; }
        IDirectPriceListRepository DirectPriceListRepository { get; }
        IDirectPriceListTypeRepository DirectPriceListTypeRepository { get; }
        IDirectPriceListItemMappingItemMappingRepository DirectPriceListItemMappingItemMappingRepository { get; }
        IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; }
        IDirectSalesOrderRepository DirectSalesOrderRepository { get; }
        IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IEditedPriceStatusRepository EditedPriceStatusRepository { get; }
        IERouteChangeRequestContentRepository ERouteChangeRequestContentRepository { get; }
        IERouteChangeRequestRepository ERouteChangeRequestRepository { get; }
        IERouteContentRepository ERouteContentRepository { get; }
        IERoutePerformanceRepository ERoutePerformanceRepository { get; }
        IERouteRepository ERouteRepository { get; }
        IERouteTypeRepository ERouteTypeRepository { get; }
        IEventMessageRepository EventMessageRepository { get; }
        IFieldRepository FieldRepository { get; }
        IGeneralCriteriaRepository GeneralCriteriaRepository { get; }
        IGeneralKpiRepository GeneralKpiRepository { get; }
        IImageRepository ImageRepository { get; }
        IIndirectPriceListRepository IndirectPriceListRepository { get; }
        IIndirectPriceListTypeRepository IndirectPriceListTypeRepository { get; }
        IIndirectPriceListItemMappingItemMappingRepository IndirectPriceListItemMappingItemMappingRepository { get; }
        IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; }
        IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; }
        IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; }
        IItemRepository ItemRepository { get; }
        IItemHistoryRepository ItemHistoryRepository { get; }
        IKpiCriteriaItemRepository KpiCriteriaItemRepository { get; }
        IKpiCriteriaTotalRepository KpiCriteriaTotalRepository { get; }
        IKpiItemRepository KpiItemRepository { get; }
        IKpiPeriodRepository KpiPeriodRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IInventoryHistoryRepository InventoryHistoryRepository { get; }
        IMenuRepository MenuRepository { get; }
        INotificationRepository NotificationRepository { get; }
        INotificationStatusRepository NotificationStatusRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPermissionOperatorRepository PermissionOperatorRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IPositionRepository PositionRepository { get; }
        IProblemRepository ProblemRepository { get; }
        IProblemHistoryRepository ProblemHistoryRepository { get; }
        IProblemTypeRepository ProblemTypeRepository { get; }
        IProblemStatusRepository ProblemStatusRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IRequestStateRepository RequestStateRepository { get; }
        IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; }
        IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; }
        IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; }
        IResellerRepository ResellerRepository { get; }
        IResellerStatusRepository ResellerStatusRepository { get; }
        IResellerTypeRepository ResellerTypeRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISexRepository SexRepository { get; }
        IStatusRepository StatusRepository { get; }
        IStoreScoutingRepository StoreScoutingRepository { get; }
        IStoreScoutingStatusRepository StoreScoutingStatusRepository { get; }
        IStoreRepository StoreRepository { get; }
        IStoreCheckingRepository StoreCheckingRepository { get; }
        IStoreGroupingRepository StoreGroupingRepository { get; }
        IStoreTypeRepository StoreTypeRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        ISurveyOptionTypeRepository SurveyOptionTypeRepository { get; }
        ISurveyQuestionTypeRepository SurveyQuestionTypeRepository { get; }
        ISurveyRepository SurveyRepository { get; }
        ISurveyResultRepository SurveyResultRepository { get; }
        ITaxTypeRepository TaxTypeRepository { get; }
        IUnitOfMeasureRepository UnitOfMeasureRepository { get; }
        IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; }
        IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; }
        IUsedVariationRepository UsedVariationRepository { get; }
        IVariationRepository VariationRepository { get; }
        IVariationGroupingRepository VariationGroupingRepository { get; }
        IWardRepository WardRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; }
        IWorkflowDirectionRepository WorkflowDirectionRepository { get; }
        IWorkflowParameterRepository WorkflowParameterRepository { get; }
        IWorkflowStateRepository WorkflowStateRepository { get; }
        IWorkflowStepRepository WorkflowStepRepository { get; }
        IWorkflowTypeRepository WorkflowTypeRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAlbumRepository AlbumRepository { get; private set; }
        public IAppUserRepository AppUserRepository { get; private set; }
        public IBannerRepository BannerRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }
        public IDirectPriceListRepository DirectPriceListRepository { get; private set; }
        public IDirectPriceListTypeRepository DirectPriceListTypeRepository { get; private set; }
        public IDirectPriceListItemMappingItemMappingRepository DirectPriceListItemMappingItemMappingRepository { get; private set; }
        public IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; private set; }
        public IDirectSalesOrderRepository DirectSalesOrderRepository { get; private set; }
        public IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IEditedPriceStatusRepository EditedPriceStatusRepository { get; private set; }
        public IERouteChangeRequestContentRepository ERouteChangeRequestContentRepository { get; private set; }
        public IERouteChangeRequestRepository ERouteChangeRequestRepository { get; private set; }
        public IERouteContentRepository ERouteContentRepository { get; private set; }
        public IERoutePerformanceRepository ERoutePerformanceRepository { get; private set; }
        public IERouteRepository ERouteRepository { get; private set; }
        public IERouteTypeRepository ERouteTypeRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IEventMessageRepository EventMessageRepository { get; private set; }
        public IGeneralCriteriaRepository GeneralCriteriaRepository { get; private set; }
        public IGeneralKpiRepository GeneralKpiRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IIndirectPriceListRepository IndirectPriceListRepository { get; private set; }
        public IIndirectPriceListTypeRepository IndirectPriceListTypeRepository { get; private set; }
        public IIndirectPriceListItemMappingItemMappingRepository IndirectPriceListItemMappingItemMappingRepository { get; private set; }
        public IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; private set; }
        public IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; private set; }
        public IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IItemHistoryRepository ItemHistoryRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IInventoryHistoryRepository InventoryHistoryRepository { get; private set; }
        public IKpiCriteriaItemRepository KpiCriteriaItemRepository { get; private set; }
        public IKpiCriteriaTotalRepository KpiCriteriaTotalRepository { get; private set; }
        public IKpiItemRepository KpiItemRepository { get; private set; }
        public IKpiPeriodRepository KpiPeriodRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public INotificationRepository NotificationRepository { get; private set; }
        public INotificationStatusRepository NotificationStatusRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPermissionOperatorRepository PermissionOperatorRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IPositionRepository PositionRepository { get; private set; }
        public IProblemRepository ProblemRepository { get; private set; }
        public IProblemHistoryRepository ProblemHistoryRepository { get; private set; }
        public IProblemTypeRepository ProblemTypeRepository { get; private set; }
        public IProblemStatusRepository ProblemStatusRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IProvinceRepository ProvinceRepository { get; private set; }
        public IRequestStateRepository RequestStateRepository { get; private set; }
        public IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; private set; }
        public IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; private set; }
        public IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; private set; }
        public IResellerRepository ResellerRepository { get; private set; }
        public IResellerStatusRepository ResellerStatusRepository { get; private set; }
        public IResellerTypeRepository ResellerTypeRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public IStoreScoutingRepository StoreScoutingRepository { get; private set; }
        public IStoreScoutingStatusRepository StoreScoutingStatusRepository { get; private set; }
        public IStoreRepository StoreRepository { get; private set; }
        public IStoreCheckingRepository StoreCheckingRepository { get; private set; }
        public IStoreGroupingRepository StoreGroupingRepository { get; private set; }
        public IStoreTypeRepository StoreTypeRepository { get; private set; }
        public ISupplierRepository SupplierRepository { get; private set; }
        public ISurveyOptionTypeRepository SurveyOptionTypeRepository { get; private set; }
        public ISurveyQuestionTypeRepository SurveyQuestionTypeRepository { get; private set; }
        public ISurveyRepository SurveyRepository { get; private set; }
        public ISurveyResultRepository SurveyResultRepository { get; private set; }
        public ITaxTypeRepository TaxTypeRepository { get; private set; }

        public IUnitOfMeasureRepository UnitOfMeasureRepository { get; private set; }
        public IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; private set; }
        public IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; private set; }
        public IUsedVariationRepository UsedVariationRepository { get; private set; }
        public IVariationRepository VariationRepository { get; private set; }
        public IVariationGroupingRepository VariationGroupingRepository { get; private set; }
        public IWardRepository WardRepository { get; private set; }
        public IWarehouseRepository WarehouseRepository { get; private set; }
        public IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; private set; }
        public IWorkflowDirectionRepository WorkflowDirectionRepository { get; private set; }
        public IWorkflowParameterRepository WorkflowParameterRepository { get; private set; }
        public IWorkflowStateRepository WorkflowStateRepository { get; private set; }
        public IWorkflowStepRepository WorkflowStepRepository { get; private set; }
        public IWorkflowTypeRepository WorkflowTypeRepository { get; private set; }
        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AlbumRepository = new AlbumRepository(DataContext);
            AppUserRepository = new AppUserRepository(DataContext);
            BrandRepository = new BrandRepository(DataContext);
            BannerRepository = new BannerRepository(DataContext);
            DirectPriceListRepository = new DirectPriceListRepository(DataContext);
            DirectPriceListTypeRepository = new DirectPriceListTypeRepository(DataContext);
            DirectPriceListItemMappingItemMappingRepository = new DirectPriceListItemMappingItemMappingRepository(DataContext);
            DirectSalesOrderContentRepository = new DirectSalesOrderContentRepository(DataContext);
            DirectSalesOrderRepository = new DirectSalesOrderRepository(DataContext);
            DirectSalesOrderPromotionRepository = new DirectSalesOrderPromotionRepository(DataContext);
            DistrictRepository = new DistrictRepository(DataContext);
            EditedPriceStatusRepository = new EditedPriceStatusRepository(DataContext);
            ERouteChangeRequestContentRepository = new ERouteChangeRequestContentRepository(DataContext);
            ERouteChangeRequestRepository = new ERouteChangeRequestRepository(DataContext);
            ERouteContentRepository = new ERouteContentRepository(DataContext);
            ERoutePerformanceRepository = new ERoutePerformanceRepository(DataContext);
            ERouteRepository = new ERouteRepository(DataContext);
            ERouteTypeRepository = new ERouteTypeRepository(DataContext);
            FieldRepository = new FieldRepository(DataContext);
            EventMessageRepository = new EventMessageRepository(DataContext);
            GeneralCriteriaRepository = new GeneralCriteriaRepository(DataContext);
            GeneralKpiRepository = new GeneralKpiRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            IndirectPriceListRepository = new IndirectPriceListRepository(DataContext);
            IndirectPriceListTypeRepository = new IndirectPriceListTypeRepository(DataContext);
            IndirectPriceListItemMappingItemMappingRepository = new IndirectPriceListItemMappingItemMappingRepository(DataContext);
            IndirectSalesOrderContentRepository = new IndirectSalesOrderContentRepository(DataContext);
            IndirectSalesOrderRepository = new IndirectSalesOrderRepository(DataContext);
            IndirectSalesOrderPromotionRepository = new IndirectSalesOrderPromotionRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            ItemHistoryRepository = new ItemHistoryRepository(DataContext);
            InventoryRepository = new InventoryRepository(DataContext);
            InventoryHistoryRepository = new InventoryHistoryRepository(DataContext);
            KpiCriteriaItemRepository = new KpiCriteriaItemRepository(DataContext);
            KpiCriteriaTotalRepository = new KpiCriteriaTotalRepository(DataContext);
            KpiItemRepository = new KpiItemRepository(DataContext);
            KpiPeriodRepository = new KpiPeriodRepository(DataContext);
            MenuRepository = new MenuRepository(DataContext);
            NotificationRepository = new NotificationRepository(DataContext);
            NotificationStatusRepository = new NotificationStatusRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PermissionOperatorRepository = new PermissionOperatorRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            PositionRepository = new PositionRepository(DataContext);
            ProblemRepository = new ProblemRepository(DataContext);
            ProblemTypeRepository = new ProblemTypeRepository(DataContext);
            ProblemHistoryRepository = new ProblemHistoryRepository(DataContext);
            ProblemStatusRepository = new ProblemStatusRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            ProvinceRepository = new ProvinceRepository(DataContext);
            RequestStateRepository = new RequestStateRepository(DataContext);
            RequestWorkflowDefinitionMappingRepository = new RequestWorkflowDefinitionMappingRepository(DataContext);
            RequestWorkflowParameterMappingRepository = new RequestWorkflowParameterMappingRepository(DataContext);
            RequestWorkflowStepMappingRepository = new RequestWorkflowStepMappingRepository(DataContext);
            ResellerRepository = new ResellerRepository(DataContext);
            ResellerStatusRepository = new ResellerStatusRepository(DataContext);
            ResellerTypeRepository = new ResellerTypeRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            StoreScoutingStatusRepository = new StoreScoutingStatusRepository(DataContext);
            StoreScoutingRepository = new StoreScoutingRepository(DataContext);
            StoreRepository = new StoreRepository(DataContext);
            StoreCheckingRepository = new StoreCheckingRepository(DataContext);
            StoreGroupingRepository = new StoreGroupingRepository(DataContext);
            StoreTypeRepository = new StoreTypeRepository(DataContext);
            SupplierRepository = new SupplierRepository(DataContext);
            SurveyOptionTypeRepository = new SurveyOptionTypeRepository(DataContext);
            SurveyQuestionTypeRepository = new SurveyQuestionTypeRepository(DataContext);
            SurveyRepository = new SurveyRepository(DataContext);
            SurveyResultRepository = new SurveyResultRepository(DataContext);
            TaxTypeRepository = new TaxTypeRepository(DataContext);
            UnitOfMeasureRepository = new UnitOfMeasureRepository(DataContext);
            UnitOfMeasureGroupingContentRepository = new UnitOfMeasureGroupingContentRepository(DataContext);
            UnitOfMeasureGroupingRepository = new UnitOfMeasureGroupingRepository(DataContext);
            UsedVariationRepository = new UsedVariationRepository(DataContext);
            VariationRepository = new VariationRepository(DataContext);
            VariationGroupingRepository = new VariationGroupingRepository(DataContext);
            WardRepository = new WardRepository(DataContext);
            WarehouseRepository = new WarehouseRepository(DataContext);
            WorkflowDefinitionRepository = new WorkflowDefinitionRepository(DataContext);
            WorkflowDirectionRepository = new WorkflowDirectionRepository(DataContext);
            WorkflowParameterRepository = new WorkflowParameterRepository(DataContext);
            WorkflowStateRepository = new WorkflowStateRepository(DataContext);
            WorkflowStepRepository = new WorkflowStepRepository(DataContext);
            WorkflowTypeRepository = new WorkflowTypeRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            return Task.CompletedTask;
            DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
            DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}