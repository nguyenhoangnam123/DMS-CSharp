using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.indirect_sales_order;
using DMS.Services.MNotification;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MWorkflow;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MIndirectSalesOrder
{
    public interface IIndirectSalesOrderService : IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<IndirectSalesOrder> Get(long Id);
        Task<List<Item>> ListItem(ItemFilter ItemFilter, long? StoreId);
        Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Approve(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Reject(IndirectSalesOrder IndirectSalesOrder);
        Task<List<IndirectSalesOrder>> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<List<IndirectSalesOrder>> Import(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<IndirectSalesOrderFilter> ToFilter(IndirectSalesOrderFilter IndirectSalesOrderFilter);
    }

    public class IndirectSalesOrderService : BaseService, IIndirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationService NotificationService;
        private IIndirectSalesOrderValidator IndirectSalesOrderValidator;
        private IRabbitManager RabbitManager;
        private IOrganizationService OrganizationService;
        private IWorkflowService WorkflowService;
        private IItemService ItemService;

        public IndirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificationService NotificationService,
            IRabbitManager RabbitManager,
            IIndirectSalesOrderValidator IndirectSalesOrderValidator,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IWorkflowService WorkflowService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationService = NotificationService;
            this.RabbitManager = RabbitManager;
            this.IndirectSalesOrderValidator = IndirectSalesOrderValidator;
            this.OrganizationService = OrganizationService;
            this.ItemService = ItemService;
            this.WorkflowService = WorkflowService;
        }
        public async Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderRepository.Count(IndirectSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter)
        {
            try
            {
                List<IndirectSalesOrder> IndirectSalesOrders = await UOW.IndirectSalesOrderRepository.List(IndirectSalesOrderFilter);
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<List<Item>> ListItem(ItemFilter ItemFilter, long? StoreId)
        {
            try
            {
                List<Item> Items = await ItemService.List(ItemFilter);
                await ApplyPrice(Items, StoreId);
                return Items;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrder> Get(long Id)
        {
            IndirectSalesOrder IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(Id);
            if (IndirectSalesOrder == null)
                return null;
            IndirectSalesOrder.RequestState = await WorkflowService.GetRequestState(IndirectSalesOrder.RowId);
            if (IndirectSalesOrder.RequestState == null)
            {
                IndirectSalesOrder.RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
            }
            else
            {
                IndirectSalesOrder.RequestStateId = IndirectSalesOrder.RequestState.Id;
                IndirectSalesOrder.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowStepMapping(IndirectSalesOrder.RowId);
            }
            return IndirectSalesOrder;
        }

        public async Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Create(IndirectSalesOrder))
                return IndirectSalesOrder;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var SaleEmployee = await UOW.AppUserRepository.Get(IndirectSalesOrder.SaleEmployeeId);
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                IndirectSalesOrder.RequestStateId = RequestStateEnum.NEW.Id;
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                IndirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                await UOW.IndirectSalesOrderRepository.Create(IndirectSalesOrder);
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);

                //Dictionary<string, string> Paramters = await MapParameters(IndirectSalesOrder);
                //await WorkflowService.Initialize(IndirectSalesOrder.RowId, WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id, Paramters);

                await UOW.Commit();
                IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);

                var RecipientIds = await ListReceipientId(SaleEmployee, IndirectSalesOrderRoute.Approve);
                RecipientIds.Add(IndirectSalesOrder.SaleEmployeeId);
                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đơn hàng {IndirectSalesOrder.Code} đã được thêm mới lên hệ thống bởi {CurrentUser.DisplayName}",
                        LinkWebsite = $"{IndirectSalesOrderRoute.Master}/?id=*".Replace("*", IndirectSalesOrder.Id.ToString()),
                        LinkMobile = $"{IndirectSalesOrderRoute.Mobile}".Replace("*", IndirectSalesOrder.Id.ToString()),
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                NotifyUsed(IndirectSalesOrder);
                await Logging.CreateAuditLog(IndirectSalesOrder, new { }, nameof(IndirectSalesOrderService));
                return IndirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrder> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Update(IndirectSalesOrder))
                return IndirectSalesOrder;
            try
            {
                var oldData = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
                if (oldData.SaleEmployeeId != IndirectSalesOrder.SaleEmployeeId)
                {
                    var SaleEmployee = await UOW.AppUserRepository.Get(IndirectSalesOrder.SaleEmployeeId);
                    IndirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                }
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                UserNotification UserNotification = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"Đơn hàng {IndirectSalesOrder.Code} đã được cập nhật thông tin bởi {CurrentUser.DisplayName}",
                    LinkWebsite = $"{IndirectSalesOrderRoute.Master}/?id=*".Replace("*", IndirectSalesOrder.Id.ToString()),
                    LinkMobile = $"{IndirectSalesOrderRoute.Mobile}".Replace("*", IndirectSalesOrder.Id.ToString()),
                    Time = Now,
                    Unread = true,
                    SenderId = CurrentContext.UserId,
                    RecipientId = IndirectSalesOrder.SaleEmployeeId
                };
                UserNotifications.Add(UserNotification);

                await NotificationService.BulkSend(UserNotifications);

                IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
                NotifyUsed(IndirectSalesOrder);
                await Logging.CreateAuditLog(IndirectSalesOrder, oldData, nameof(IndirectSalesOrderService));
                return IndirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrder> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Delete(IndirectSalesOrder))
                return IndirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.Delete(IndirectSalesOrder);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await UOW.PermissionRepository.ListAppUser(IndirectSalesOrderRoute.Approve);
                RecipientIds.AddRange(await UOW.PermissionRepository.ListAppUser(IndirectSalesOrderRoute.Reject));
                RecipientIds.Add(CurrentContext.UserId);
                RecipientIds = RecipientIds.Distinct().ToList();

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đơn hàng {IndirectSalesOrder.Code} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName}",
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(new { }, IndirectSalesOrder, nameof(IndirectSalesOrderService));
                return IndirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<IndirectSalesOrder>> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            if (!await IndirectSalesOrderValidator.BulkDelete(IndirectSalesOrders))
                return IndirectSalesOrders;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.BulkDelete(IndirectSalesOrders);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrders, nameof(IndirectSalesOrderService));
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<IndirectSalesOrder>> Import(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            if (!await IndirectSalesOrderValidator.Import(IndirectSalesOrders))
                return IndirectSalesOrders;
            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.BulkMerge(IndirectSalesOrders);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrders, new { }, nameof(IndirectSalesOrderService));
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrderFilter> ToFilter(IndirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderFilter subFilter = new IndirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                    {
                        var organizationIds = FilterOrganization(Organizations, FilterPermissionDefinition.IdFilter);
                        IdFilter IdFilter = new IdFilter { In = organizationIds };
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, IdFilter);
                    }

                    if (FilterPermissionDefinition.Name == nameof(subFilter.BuyerStoreId))
                        subFilter.BuyerStoreId = FilterBuilder.Merge(subFilter.BuyerStoreId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.SellerStoreId))
                        subFilter.SellerStoreId = FilterBuilder.Merge(subFilter.SellerStoreId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = FilterBuilder.Merge(subFilter.Total, FilterPermissionDefinition.DecimalFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = FilterBuilder.Merge(subFilter.OrderDate, FilterPermissionDefinition.DateFilter);

                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.Equal = CurrentContext.UserId;
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.NotEqual = CurrentContext.UserId;
                        }
                    }
                }
            }
            return filter;
        }

        private async Task<IndirectSalesOrder> Calculator(IndirectSalesOrder IndirectSalesOrder)
        {
            var ProductIds = new List<long>();
            var ItemIds = new List<long>();
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                ProductIds.AddRange(IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.Item.ProductId).ToList());
                ItemIds.AddRange(IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).ToList());
            }
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                ProductIds.AddRange(IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.Item.ProductId).ToList());
                ItemIds.AddRange(IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.ItemId).ToList());
            }
            ProductIds = ProductIds.Distinct().ToList();
            ItemIds = ItemIds.Distinct().ToList();

            var Items = await UOW.ItemRepository.List(new ItemFilter
            {
                Id = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.SalePrice | ItemSelect.Id
            });

            var Products = await UOW.ProductRepository.List(new ProductFilter
            {
                Id = new IdFilter { In = ProductIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.UnitOfMeasure | ProductSelect.UnitOfMeasureGrouping | ProductSelect.Id | ProductSelect.TaxType
            });

            var UOMGs = await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.Id | UnitOfMeasureGroupingSelect.UnitOfMeasure | UnitOfMeasureGroupingSelect.UnitOfMeasureGroupingContents
            });

            //sản phẩm bán
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    var Item = Items.Where(x => x.Id == IndirectSalesOrderContent.ItemId).FirstOrDefault();
                    var Product = Products.Where(x => IndirectSalesOrderContent.Item.ProductId == x.Id).FirstOrDefault();
                    IndirectSalesOrderContent.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;
                    IndirectSalesOrderContent.PrimaryPrice = Item.SalePrice;

                    List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
                    if (Product.UnitOfMeasureGroupingId.HasValue)
                    {
                        var UOMG = UOMGs.Where(x => x.Id == Product.UnitOfMeasureGroupingId).FirstOrDefault();
                        UnitOfMeasures = UOMG.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                        {
                            Id = x.UnitOfMeasure.Id,
                            Code = x.UnitOfMeasure.Code,
                            Name = x.UnitOfMeasure.Name,
                            Description = x.UnitOfMeasure.Description,
                            StatusId = x.UnitOfMeasure.StatusId,
                            Factor = x.Factor
                        }).ToList();
                    }

                    UnitOfMeasures.Add(new UnitOfMeasure
                    {
                        Id = Product.UnitOfMeasure.Id,
                        Code = Product.UnitOfMeasure.Code,
                        Name = Product.UnitOfMeasure.Name,
                        Description = Product.UnitOfMeasure.Description,
                        StatusId = Product.UnitOfMeasure.StatusId,
                        Factor = 1
                    });
                    var UOM = UnitOfMeasures.Where(x => IndirectSalesOrderContent.UnitOfMeasureId == x.Id).FirstOrDefault();
                    //IndirectSalesOrderContent.TaxPercentage = Product.TaxType.Percentage;
                    IndirectSalesOrderContent.RequestedQuantity = IndirectSalesOrderContent.Quantity * UOM.Factor.Value;

                    //Trường hợp không sửa giá, giá bán = giá bán cơ sở của sản phẩm * hệ số quy đổi của đơn vị tính
                    if (IndirectSalesOrder.EditedPriceStatusId == Enums.EditedPriceStatusEnum.INACTIVE.Id)
                    {
                        IndirectSalesOrderContent.SalePrice = Item.SalePrice * UOM.Factor.Value;
                    }
                    //giá tiền từng line trước chiết khấu
                    var SubAmount = IndirectSalesOrderContent.Quantity * IndirectSalesOrderContent.SalePrice;
                    if (IndirectSalesOrderContent.DiscountPercentage.HasValue)
                    {
                        IndirectSalesOrderContent.DiscountAmount = SubAmount * IndirectSalesOrderContent.DiscountPercentage.Value / 100;
                        IndirectSalesOrderContent.DiscountAmount = Math.Round(IndirectSalesOrderContent.DiscountAmount ?? 0, 0);
                        IndirectSalesOrderContent.Amount = SubAmount - IndirectSalesOrderContent.DiscountAmount.Value;
                    }
                    else
                    {
                        IndirectSalesOrderContent.Amount = SubAmount;
                    }
                }

                //tổng trước chiết khấu
                IndirectSalesOrder.SubTotal = IndirectSalesOrder.IndirectSalesOrderContents.Sum(x => x.Amount);

                //tính tổng chiết khấu theo % chiết khấu chung
                if (IndirectSalesOrder.GeneralDiscountPercentage.HasValue && IndirectSalesOrder.GeneralDiscountPercentage > 0)
                {
                    IndirectSalesOrder.GeneralDiscountAmount = IndirectSalesOrder.SubTotal * (IndirectSalesOrder.GeneralDiscountPercentage / 100);
                    IndirectSalesOrder.GeneralDiscountAmount = Math.Round(IndirectSalesOrder.GeneralDiscountAmount.Value, 0);
                }
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                    IndirectSalesOrderContent.GeneralDiscountPercentage = IndirectSalesOrderContent.Amount / IndirectSalesOrder.SubTotal * 100;
                    IndirectSalesOrderContent.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount * IndirectSalesOrderContent.GeneralDiscountPercentage / 100;
                    IndirectSalesOrderContent.GeneralDiscountAmount = Math.Round(IndirectSalesOrderContent.GeneralDiscountAmount ?? 0, 0);
                    //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                    IndirectSalesOrderContent.TaxAmount = (IndirectSalesOrderContent.Amount - (IndirectSalesOrderContent.GeneralDiscountAmount.HasValue ? IndirectSalesOrderContent.GeneralDiscountAmount.Value : 0)) * IndirectSalesOrderContent.TaxPercentage / 100;
                    IndirectSalesOrderContent.TaxAmount = Math.Round(IndirectSalesOrderContent.TaxAmount ?? 0, 0);
                }

                IndirectSalesOrder.TotalTaxAmount = IndirectSalesOrder.IndirectSalesOrderContents.Where(x => x.TaxAmount.HasValue).Sum(x => x.TaxAmount.Value);
                IndirectSalesOrder.TotalTaxAmount = Math.Round(IndirectSalesOrder.TotalTaxAmount, 0);
                //tổng phải thanh toán
                IndirectSalesOrder.Total = IndirectSalesOrder.SubTotal - (IndirectSalesOrder.GeneralDiscountAmount.HasValue ? IndirectSalesOrder.GeneralDiscountAmount.Value : 0) + IndirectSalesOrder.TotalTaxAmount;
            }
            else
            {
                IndirectSalesOrder.SubTotal = 0;
                IndirectSalesOrder.GeneralDiscountPercentage = null;
                IndirectSalesOrder.GeneralDiscountAmount = null;
                IndirectSalesOrder.TotalTaxAmount = 0;
                IndirectSalesOrder.Total = 0;
            }

            //sản phẩm khuyến mãi
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    var Product = Products.Where(x => IndirectSalesOrderPromotion.Item.ProductId == x.Id).FirstOrDefault();
                    IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;

                    List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
                    if (Product.UnitOfMeasureGroupingId.HasValue)
                    {
                        var UOMG = UOMGs.Where(x => x.Id == Product.UnitOfMeasureGroupingId).FirstOrDefault();
                        UnitOfMeasures = UOMG.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                        {
                            Id = x.UnitOfMeasure.Id,
                            Code = x.UnitOfMeasure.Code,
                            Name = x.UnitOfMeasure.Name,
                            Description = x.UnitOfMeasure.Description,
                            StatusId = x.UnitOfMeasure.StatusId,
                            Factor = x.Factor
                        }).ToList();
                    }

                    UnitOfMeasures.Add(new UnitOfMeasure
                    {
                        Id = Product.UnitOfMeasure.Id,
                        Code = Product.UnitOfMeasure.Code,
                        Name = Product.UnitOfMeasure.Name,
                        Description = Product.UnitOfMeasure.Description,
                        StatusId = Product.UnitOfMeasure.StatusId,
                        Factor = 1
                    });
                    var UOM = UnitOfMeasures.Where(x => IndirectSalesOrderPromotion.UnitOfMeasureId == x.Id).FirstOrDefault();
                    IndirectSalesOrderPromotion.RequestedQuantity = IndirectSalesOrderPromotion.Quantity * UOM.Factor.Value;
                }
            }

            return IndirectSalesOrder;
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long? StoreId)
        {
            var CurrrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            SystemConfiguration SystemConfiguration = await UOW.SystemConfigurationRepository.Get();
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Path = new StringFilter { StartWith = CurrrentUser.Organization.Path },
                Selects = OrganizationSelect.Id
            };
            var OrganizationIds = (await UOW.OrganizationRepository.List(OrganizationFilter)).Select(x => x.Id).ToList();


            var ItemIds = Items.Select(x => x.Id).ToList();
            Dictionary<long, decimal> result = new Dictionary<long, decimal>();
            PriceListItemMappingFilter PriceListItemMappingFilter = new PriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListItemMappingSelect.ALL,
                PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.ALLSTORE.Id },
                SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.INDIRECT.Id },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var PriceListItemMappingAllStore = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
            List<PriceListItemMapping> PriceListItemMappings = new List<PriceListItemMapping>();
            PriceListItemMappings.AddRange(PriceListItemMappingAllStore);

            if (StoreId.HasValue)
            {
                var Store = await UOW.StoreRepository.Get(StoreId.Value);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STOREGROUPING.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.INDIRECT.Id },
                    StoreGroupingId = new IdFilter { Equal = Store.StoreGroupingId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreGrouping = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STORETYPE.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.INDIRECT.Id },
                    StoreGroupingId = new IdFilter { Equal = Store.StoreTypeId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreType = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.DETAILS.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.INDIRECT.Id },
                    StoreId = new IdFilter { Equal = StoreId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreDetail = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreGrouping);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreType);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreDetail);
            }

            foreach (var ItemId in ItemIds)
            {
                result.Add(ItemId, decimal.MaxValue);
            }

            //Áp giá theo cấu hình
            //Ưu tiên lấy giá thấp hơn
            if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 0)
            {
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MaxValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => (decimal)x.Price)
                            .DefaultIfEmpty(decimal.MaxValue)
                            .Min();
                        if (targetPrice < decimal.MaxValue)
                        {
                            result[ItemId] = targetPrice;
                            break;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MaxValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault();
                    }
                }
            }
            //Ưu tiên lấy giá cao hơn
            else if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 1)
            {
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MinValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => (decimal)x.Price)
                            .DefaultIfEmpty(decimal.MinValue)
                            .Max();
                        if (targetPrice > decimal.MinValue)
                        {
                            result[ItemId] = targetPrice;
                            break;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MinValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault();
                    }
                }
            }

            foreach (var item in Items)
            {
                item.SalePrice = result[item.Id];
            }

            return Items;
        }

        public async Task<IndirectSalesOrder> Approve(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.Id == 0)
                IndirectSalesOrder = await Create(IndirectSalesOrder);
            else
                IndirectSalesOrder = await Update(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated == false)
                return IndirectSalesOrder;
            Dictionary<string, string> Parameters = await MapParameters(IndirectSalesOrder);
            GenericEnum Action = await WorkflowService.Approve(IndirectSalesOrder.RowId, WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id, Parameters);
            if (Action != WorkflowActionEnum.OK)
                return null;
            return await Get(IndirectSalesOrder.Id);
        }

        public async Task<IndirectSalesOrder> Reject(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
            Dictionary<string, string> Parameters = await MapParameters(IndirectSalesOrder);
            GenericEnum Action = await WorkflowService.Reject(IndirectSalesOrder.RowId, WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id, Parameters);
            if (Action != WorkflowActionEnum.OK)
                return null;
            return await Get(IndirectSalesOrder.Id);
        }

        private async Task<Dictionary<string, string>> MapParameters(IndirectSalesOrder IndirectSalesOrder)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add(nameof(IndirectSalesOrder.Id), IndirectSalesOrder.Id.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.Code), IndirectSalesOrder.Code);
            Parameters.Add(nameof(IndirectSalesOrder.SaleEmployeeId), IndirectSalesOrder.SaleEmployeeId.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.BuyerStoreId), IndirectSalesOrder.BuyerStoreId.ToString());

            Parameters.Add(nameof(IndirectSalesOrder.Total), IndirectSalesOrder.Total.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.TotalDiscountAmount), IndirectSalesOrder.TotalDiscountAmount.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.TotalRequestedQuantity), IndirectSalesOrder.TotalRequestedQuantity.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.OrganizationId), IndirectSalesOrder.OrganizationId.ToString());

            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(IndirectSalesOrder.RowId);
            if (RequestWorkflowDefinitionMapping == null)
                Parameters.Add(nameof(RequestState), RequestStateEnum.NEW.Id.ToString());
            else
                Parameters.Add(nameof(RequestState), RequestWorkflowDefinitionMapping.RequestStateId.ToString());
            Parameters.Add("Username", CurrentContext.UserName);
            return Parameters;
        }

        private async Task<List<long>> ListReceipientId(AppUser AppUser, string Path)
        {
            var Ids = await UOW.PermissionRepository.ListAppUser(Path);
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(AppUser.Organization.Path) || AppUser.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();

            var AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Organization,
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            });
            var AppUserIds = AppUsers.Where(x => OrganizationIds.Contains(x.OrganizationId)).Select(x => x.Id).ToList();
            AppUserIds = AppUserIds.Intersect(Ids).ToList();
            return AppUserIds;
        }

        private void NotifyUsed(IndirectSalesOrder IndirectSalesOrder)
        {
            {
                List<long> ItemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(i => i.ItemId).ToList();
                List<EventMessage<Item>> itemMessages = ItemIds.Select(i => new EventMessage<Item>
                {
                    Content = new Item { Id = i },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(itemMessages, RoutingKeyEnum.ItemUsed);
            }

            {
                List<long> PrimaryUOMIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(i => i.PrimaryUnitOfMeasureId).ToList();
                List<long> UOMIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(i => i.UnitOfMeasureId).ToList();
                UOMIds.AddRange(PrimaryUOMIds);
                List<EventMessage<UnitOfMeasure>> UnitOfMeasureMessages = UOMIds.Select(x => new EventMessage<UnitOfMeasure>
                {
                    Content = new UnitOfMeasure { Id = x },
                    EntityName = nameof(UnitOfMeasure),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            }
            {
                List<EventMessage<Store>> storeMessages = new List<EventMessage<Store>>();
                EventMessage<Store> BuyerStore = new EventMessage<Store>
                {
                    Content = new Store { Id = IndirectSalesOrder.BuyerStoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                storeMessages.Add(BuyerStore);
                EventMessage<Store> SellerStore = new EventMessage<Store>
                {
                    Content = new Store { Id = IndirectSalesOrder.SellerStoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                storeMessages.Add(SellerStore);
                RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
            }

            {
                EventMessage<AppUser> AppUserMessage = new EventMessage<AppUser>
                {
                    Content = new AppUser { Id = IndirectSalesOrder.SaleEmployeeId },
                    EntityName = nameof(AppUser),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            }
        }
    }
}
