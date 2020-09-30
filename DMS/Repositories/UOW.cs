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
        IColorRepository ColorRepository { get; }
        IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; }
        IDirectSalesOrderRepository DirectSalesOrderRepository { get; }
        IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IEditedPriceStatusRepository EditedPriceStatusRepository { get; }
        IERouteChangeRequestContentRepository ERouteChangeRequestContentRepository { get; }
        IERouteChangeRequestRepository ERouteChangeRequestRepository { get; }
        IERouteContentRepository ERouteContentRepository { get; }
        IERouteRepository ERouteRepository { get; }
        IERouteTypeRepository ERouteTypeRepository { get; }
        IEventMessageRepository EventMessageRepository { get; }
        IFieldRepository FieldRepository { get; }
        IIdGenerateRepository IdGenerateRepository { get; }
        IImageRepository ImageRepository { get; }
        IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; }
        IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; }
        IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; }
        IIndirectSalesOrderTransactionRepository IndirectSalesOrderTransactionRepository { get; }
        IItemRepository ItemRepository { get; }
        IItemHistoryRepository ItemHistoryRepository { get; }
        IKpiCriteriaGeneralRepository KpiCriteriaGeneralRepository { get; }
        IKpiCriteriaItemRepository KpiCriteriaItemRepository { get; }
        IKpiCriteriaTotalRepository KpiCriteriaTotalRepository { get; }
        IKpiGeneralRepository KpiGeneralRepository { get; }
        IKpiGeneralContentRepository KpiGeneralContentRepository { get; }
        IKpiItemContentRepository KpiItemContentRepository { get; }
        IKpiItemRepository KpiItemRepository { get; }
        IKpiPeriodRepository KpiPeriodRepository { get; }
        IKpiYearRepository KpiYearRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IInventoryHistoryRepository InventoryHistoryRepository { get; }
        IMenuRepository MenuRepository { get; }
        INotificationRepository NotificationRepository { get; }
        INotificationStatusRepository NotificationStatusRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPermissionOperatorRepository PermissionOperatorRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IPositionRepository PositionRepository { get; }
        IPriceListRepository PriceListRepository { get; }
        IPriceListTypeRepository PriceListTypeRepository { get; }
        IPriceListItemHistoryRepository PriceListItemHistoryRepository { get; }
        IPriceListItemMappingItemMappingRepository PriceListItemMappingItemMappingRepository { get; }
        IProblemRepository ProblemRepository { get; }
        IProblemHistoryRepository ProblemHistoryRepository { get; }
        IProblemTypeRepository ProblemTypeRepository { get; }
        IProblemStatusRepository ProblemStatusRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IPromotionComboRepository PromotionComboRepository { get; }
        IPromotionRepository PromotionRepository { get; }
        IPromotionDirectSalesOrderRepository PromotionDirectSalesOrderRepository { get; }
        IPromotionDiscountTypeRepository PromotionDiscountTypeRepository { get; }
        IPromotionPolicyRepository PromotionPolicyRepository { get; }
        IPromotionProductRepository PromotionProductRepository { get; }
        IPromotionProductGroupingRepository PromotionProductGroupingRepository { get; }
        IPromotionProductTypeRepository PromotionProductTypeRepository { get; }
        IPromotionSamePriceRepository PromotionSamePriceRepository { get; }
        IPromotionStoreRepository PromotionStoreRepository { get; }
        IPromotionStoreGroupingRepository PromotionStoreGroupingRepository { get; }
        IPromotionStoreTypeRepository PromotionStoreTypeRepository { get; }
        IPromotionTypeRepository PromotionTypeRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IRequestStateRepository RequestStateRepository { get; }
        IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; }
        IRequestWorkflowHistoryRepository RequestWorkflowHistoryRepository { get; }
        IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; }
        IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; }
        IResellerRepository ResellerRepository { get; }
        IResellerStatusRepository ResellerStatusRepository { get; }
        IResellerTypeRepository ResellerTypeRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISalesOrderTypeRepository SalesOrderTypeRepository { get; }
        ISexRepository SexRepository { get; }
        IStatusRepository StatusRepository { get; }
        IStoreScoutingRepository StoreScoutingRepository { get; }
        IStoreScoutingTypeRepository StoreScoutingTypeRepository { get; }
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
        ISystemConfigurationRepository SystemConfigurationRepository { get; }
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
        IWorkflowOperatorRepository WorkflowOperatorRepository { get; }
        IWorkflowParameterRepository WorkflowParameterRepository { get; }
        IWorkflowParameterTypeRepository WorkflowParameterTypeRepository { get; }
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
        public IColorRepository ColorRepository { get; private set; }
        public IDirectSalesOrderContentRepository DirectSalesOrderContentRepository { get; private set; }
        public IDirectSalesOrderRepository DirectSalesOrderRepository { get; private set; }
        public IDirectSalesOrderPromotionRepository DirectSalesOrderPromotionRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IEditedPriceStatusRepository EditedPriceStatusRepository { get; private set; }
        public IERouteChangeRequestContentRepository ERouteChangeRequestContentRepository { get; private set; }
        public IERouteChangeRequestRepository ERouteChangeRequestRepository { get; private set; }
        public IERouteContentRepository ERouteContentRepository { get; private set; }
        public IERouteRepository ERouteRepository { get; private set; }
        public IERouteTypeRepository ERouteTypeRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IEventMessageRepository EventMessageRepository { get; private set; }
        public IIdGenerateRepository IdGenerateRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IIndirectSalesOrderContentRepository IndirectSalesOrderContentRepository { get; private set; }
        public IIndirectSalesOrderRepository IndirectSalesOrderRepository { get; private set; }
        public IIndirectSalesOrderPromotionRepository IndirectSalesOrderPromotionRepository { get; private set; }
        public IIndirectSalesOrderTransactionRepository IndirectSalesOrderTransactionRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IItemHistoryRepository ItemHistoryRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IInventoryHistoryRepository InventoryHistoryRepository { get; private set; }
        public IKpiCriteriaGeneralRepository KpiCriteriaGeneralRepository { get; private set; }
        public IKpiCriteriaItemRepository KpiCriteriaItemRepository { get; private set; }
        public IKpiCriteriaTotalRepository KpiCriteriaTotalRepository { get; private set; }
        public IKpiGeneralRepository KpiGeneralRepository { get; private set; }
        public IKpiGeneralContentRepository KpiGeneralContentRepository { get; private set; }
        public IKpiItemContentRepository KpiItemContentRepository { get; private set; } 
        public IKpiItemRepository KpiItemRepository { get; private set; }
        public IKpiPeriodRepository KpiPeriodRepository { get; private set; }
        public IKpiYearRepository KpiYearRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public INotificationRepository NotificationRepository { get; private set; }
        public INotificationStatusRepository NotificationStatusRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPermissionOperatorRepository PermissionOperatorRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IPositionRepository PositionRepository { get; private set; }
        public IPriceListRepository PriceListRepository { get; private set; }
        public IPriceListTypeRepository PriceListTypeRepository { get; private set; }
        public IPriceListItemHistoryRepository PriceListItemHistoryRepository { get; private set; }
        public IPriceListItemMappingItemMappingRepository PriceListItemMappingItemMappingRepository { get; private set; }
        public IProblemRepository ProblemRepository { get; private set; }
        public IProblemHistoryRepository ProblemHistoryRepository { get; private set; }
        public IProblemTypeRepository ProblemTypeRepository { get; private set; }
        public IProblemStatusRepository ProblemStatusRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IPromotionComboRepository PromotionComboRepository { get; private set; }
        public IPromotionRepository PromotionRepository { get; private set; }
        public IPromotionDirectSalesOrderRepository PromotionDirectSalesOrderRepository { get; private set; }
        public IPromotionDiscountTypeRepository PromotionDiscountTypeRepository { get; private set; }
        public IPromotionPolicyRepository PromotionPolicyRepository { get; private set; }
        public IPromotionProductRepository PromotionProductRepository { get; private set; }
        public IPromotionProductGroupingRepository PromotionProductGroupingRepository { get; private set; }
        public IPromotionProductTypeRepository PromotionProductTypeRepository { get; private set; }
        public IPromotionSamePriceRepository PromotionSamePriceRepository { get; private set; }
        public IPromotionStoreRepository PromotionStoreRepository { get; private set; }
        public IPromotionStoreGroupingRepository PromotionStoreGroupingRepository { get; private set; }
        public IPromotionStoreTypeRepository PromotionStoreTypeRepository { get; private set; }
        public IPromotionTypeRepository PromotionTypeRepository { get; private set; }
        public IProvinceRepository ProvinceRepository { get; private set; }
        public IRequestStateRepository RequestStateRepository { get; private set; }
        public IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; private set; }
        public IRequestWorkflowHistoryRepository RequestWorkflowHistoryRepository { get; private set; }
        public IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; private set; }
        public IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; private set; }
        public IResellerRepository ResellerRepository { get; private set; }
        public IResellerStatusRepository ResellerStatusRepository { get; private set; }
        public IResellerTypeRepository ResellerTypeRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISalesOrderTypeRepository SalesOrderTypeRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public IStoreScoutingRepository StoreScoutingRepository { get; private set; }
        public IStoreScoutingTypeRepository StoreScoutingTypeRepository { get; private set; }
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
        public ISystemConfigurationRepository SystemConfigurationRepository { get; private set; }
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
        public IWorkflowOperatorRepository WorkflowOperatorRepository { get; private set; }
        public IWorkflowParameterRepository WorkflowParameterRepository { get; private set; }
        public IWorkflowParameterTypeRepository WorkflowParameterTypeRepository { get; private set; }
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
            ColorRepository = new ColorRepository(DataContext);
            DirectSalesOrderContentRepository = new DirectSalesOrderContentRepository(DataContext);
            DirectSalesOrderRepository = new DirectSalesOrderRepository(DataContext);
            DirectSalesOrderPromotionRepository = new DirectSalesOrderPromotionRepository(DataContext);
            DistrictRepository = new DistrictRepository(DataContext);
            EditedPriceStatusRepository = new EditedPriceStatusRepository(DataContext);
            ERouteChangeRequestContentRepository = new ERouteChangeRequestContentRepository(DataContext);
            ERouteChangeRequestRepository = new ERouteChangeRequestRepository(DataContext);
            ERouteContentRepository = new ERouteContentRepository(DataContext);
            ERouteRepository = new ERouteRepository(DataContext);
            ERouteTypeRepository = new ERouteTypeRepository(DataContext);
            FieldRepository = new FieldRepository(DataContext);
            EventMessageRepository = new EventMessageRepository(DataContext);
            IdGenerateRepository = new IdGenerateRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            IndirectSalesOrderContentRepository = new IndirectSalesOrderContentRepository(DataContext);
            IndirectSalesOrderRepository = new IndirectSalesOrderRepository(DataContext);
            IndirectSalesOrderPromotionRepository = new IndirectSalesOrderPromotionRepository(DataContext);
            IndirectSalesOrderTransactionRepository = new IndirectSalesOrderTransactionRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            ItemHistoryRepository = new ItemHistoryRepository(DataContext);
            InventoryRepository = new InventoryRepository(DataContext);
            InventoryHistoryRepository = new InventoryHistoryRepository(DataContext);
            KpiCriteriaGeneralRepository = new KpiCriteriaGeneralRepository(DataContext);
            KpiCriteriaItemRepository = new KpiCriteriaItemRepository(DataContext);
            KpiCriteriaTotalRepository = new KpiCriteriaTotalRepository(DataContext);
            KpiGeneralRepository = new KpiGeneralRepository(DataContext);
            KpiGeneralContentRepository = new KpiGeneralContentRepository(DataContext);
            KpiItemContentRepository = new KpiItemContentRepository(DataContext);
            KpiItemRepository = new KpiItemRepository(DataContext);
            KpiPeriodRepository = new KpiPeriodRepository(DataContext);
            KpiYearRepository = new KpiYearRepository(DataContext);
            MenuRepository = new MenuRepository(DataContext);
            NotificationRepository = new NotificationRepository(DataContext);
            NotificationStatusRepository = new NotificationStatusRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PermissionOperatorRepository = new PermissionOperatorRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            PositionRepository = new PositionRepository(DataContext);
            PriceListRepository = new PriceListRepository(DataContext);
            PriceListTypeRepository = new PriceListTypeRepository(DataContext);
            PriceListItemHistoryRepository = new PriceListItemHistoryRepository(DataContext);
            PriceListItemMappingItemMappingRepository = new PriceListItemMappingItemMappingRepository(DataContext);
            ProblemRepository = new ProblemRepository(DataContext);
            ProblemTypeRepository = new ProblemTypeRepository(DataContext);
            ProblemHistoryRepository = new ProblemHistoryRepository(DataContext);
            ProblemStatusRepository = new ProblemStatusRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            PromotionComboRepository = new PromotionComboRepository(DataContext);
            PromotionRepository = new PromotionRepository(DataContext);
            PromotionDirectSalesOrderRepository = new PromotionDirectSalesOrderRepository(DataContext);
            PromotionDiscountTypeRepository = new PromotionDiscountTypeRepository(DataContext);
            PromotionPolicyRepository = new PromotionPolicyRepository(DataContext);
            PromotionProductRepository = new PromotionProductRepository(DataContext);
            PromotionProductGroupingRepository = new PromotionProductGroupingRepository(DataContext);
            PromotionProductTypeRepository = new PromotionProductTypeRepository(DataContext);
            PromotionSamePriceRepository = new PromotionSamePriceRepository(DataContext);
            PromotionStoreRepository = new PromotionStoreRepository(DataContext);
            PromotionStoreGroupingRepository = new PromotionStoreGroupingRepository(DataContext);
            PromotionStoreTypeRepository = new PromotionStoreTypeRepository(DataContext);
            PromotionTypeRepository = new PromotionTypeRepository(DataContext);
            ProvinceRepository = new ProvinceRepository(DataContext);
            RequestStateRepository = new RequestStateRepository(DataContext);
            RequestWorkflowDefinitionMappingRepository = new RequestWorkflowDefinitionMappingRepository(DataContext);
            RequestWorkflowHistoryRepository = new RequestWorkflowHistoryRepository(DataContext);
            RequestWorkflowParameterMappingRepository = new RequestWorkflowParameterMappingRepository(DataContext);
            RequestWorkflowStepMappingRepository = new RequestWorkflowStepMappingRepository(DataContext);
            ResellerRepository = new ResellerRepository(DataContext);
            ResellerStatusRepository = new ResellerStatusRepository(DataContext);
            ResellerTypeRepository = new ResellerTypeRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SalesOrderTypeRepository = new SalesOrderTypeRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            StoreScoutingStatusRepository = new StoreScoutingStatusRepository(DataContext);
            StoreScoutingRepository = new StoreScoutingRepository(DataContext);
            StoreScoutingTypeRepository = new StoreScoutingTypeRepository(DataContext);
            StoreRepository = new StoreRepository(DataContext);
            StoreCheckingRepository = new StoreCheckingRepository(DataContext);
            StoreGroupingRepository = new StoreGroupingRepository(DataContext);
            StoreTypeRepository = new StoreTypeRepository(DataContext);
            SupplierRepository = new SupplierRepository(DataContext);
            SurveyOptionTypeRepository = new SurveyOptionTypeRepository(DataContext);
            SurveyQuestionTypeRepository = new SurveyQuestionTypeRepository(DataContext);
            SurveyRepository = new SurveyRepository(DataContext);
            SurveyResultRepository = new SurveyResultRepository(DataContext);
            SystemConfigurationRepository = new SystemConfigurationRepository(DataContext);
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
            WorkflowOperatorRepository = new WorkflowOperatorRepository(DataContext);
            WorkflowParameterRepository = new WorkflowParameterRepository(DataContext);
            WorkflowParameterTypeRepository = new WorkflowParameterTypeRepository(DataContext);
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