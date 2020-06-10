using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
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
        Task<List<Item>> ListItem(ItemFilter ItemFilter, long StoreId);
        Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Approve(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Reject(IndirectSalesOrder IndirectSalesOrder);
        Task<List<IndirectSalesOrder>> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<List<IndirectSalesOrder>> Import(List<IndirectSalesOrder> IndirectSalesOrders);
        IndirectSalesOrderFilter ToFilter(IndirectSalesOrderFilter IndirectSalesOrderFilter);
    }

    public class IndirectSalesOrderService : BaseService, IIndirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderValidator IndirectSalesOrderValidator;
        private IWorkflowService WorkflowService;

        public IndirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderValidator IndirectSalesOrderValidator,
            IWorkflowService WorkflowService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderValidator = IndirectSalesOrderValidator;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<List<Item>> ListItem(ItemFilter ItemFilter, long StoreId)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                await ApplyPrice(Items, StoreId);
                return Items;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                IndirectSalesOrder.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowState(IndirectSalesOrder.RowId);
            }
            return IndirectSalesOrder;
        }

        public async Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Create(IndirectSalesOrder))
                return IndirectSalesOrder;

            try
            {
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                IndirectSalesOrder.RequestStateId = Enums.RequestStateEnum.NEW.Id;
                IndirectSalesOrder.RowId = Guid.NewGuid();
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                await UOW.IndirectSalesOrderRepository.Create(IndirectSalesOrder);
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);
                await WorkflowService.Initialize(IndirectSalesOrder.RowId, WorkflowTypeEnum.STORE.Id, MapParameters(IndirectSalesOrder));
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrder, new { }, nameof(IndirectSalesOrderService));
                return await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
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
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);
                await UOW.Commit();

                var newData = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectSalesOrderService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateAuditLog(new { }, IndirectSalesOrder, nameof(IndirectSalesOrderService));
                return IndirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public IndirectSalesOrderFilter ToFilter(IndirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderFilter subFilter = new IndirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.BuyerStoreId))
                        subFilter.BuyerStoreId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.SellerStoreId))
                        subFilter.SellerStoreId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleEmployeeId))
                        subFilter.SaleEmployeeId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = FilterPermissionDefinition.LongFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = FilterPermissionDefinition.DateFilter;
                }
            }
            return filter;
        }

        private async Task<IndirectSalesOrder> Calculator(IndirectSalesOrder IndirectSalesOrder)
        {
            var ProductIds = new List<long>();
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
                ProductIds.AddRange(IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.Item.ProductId).ToList());
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
                ProductIds.AddRange(IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.Item.ProductId).ToList());
            ProductIds = ProductIds.Distinct().ToList();

            var Items = await UOW.ItemRepository.List(new ItemFilter
            {
                ProductId = new IdFilter { In = ProductIds },
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

            //sản phẩm bán
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    var Item = Items.Where(x => x.Id == IndirectSalesOrderContent.ItemId).FirstOrDefault();
                    var Product = Products.Where(x => IndirectSalesOrderContent.Item.ProductId == x.Id).FirstOrDefault();
                    IndirectSalesOrderContent.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;

                    var UnitOfMeasures = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Factor = x.Factor
                    }).ToList();
                    if (Product.UnitOfMeasure.StatusId == Enums.StatusEnum.ACTIVE.Id)
                    {
                        UnitOfMeasures.Add(new UnitOfMeasure
                        {
                            Id = Product.UnitOfMeasure.Id,
                            Code = Product.UnitOfMeasure.Code,
                            Name = Product.UnitOfMeasure.Name,
                            Description = Product.UnitOfMeasure.Description,
                            StatusId = Product.UnitOfMeasure.StatusId,
                            Factor = 1
                        });
                    }
                    var UOM = UnitOfMeasures.Where(x => IndirectSalesOrderContent.UnitOfMeasureId == x.Id).FirstOrDefault();
                    IndirectSalesOrderContent.TaxPercentage = Product.TaxType.Percentage;
                    IndirectSalesOrderContent.RequestedQuantity = IndirectSalesOrderContent.Quantity * UOM.Factor.Value;

                    //giá tiền từng line = số lượng yc*đơn giá*(100-%chiết khấu)
                    if (IndirectSalesOrder.EditedPriceStatusId == Enums.EditedPriceStatusEnum.INACTIVE.Id)
                    {
                        //Trường hợp không sửa giá, giá bán = giá bán cơ sở của sản phẩm * hệ số quy đổi của đơn vị tính
                        IndirectSalesOrderContent.SalePrice = Convert.ToInt64(Item.SalePrice * UOM.Factor.Value);
                        IndirectSalesOrderContent.Amount = Convert.ToInt64((IndirectSalesOrderContent.Quantity * IndirectSalesOrderContent.SalePrice) * ((100 - IndirectSalesOrderContent.DiscountPercentage.Value) / 100));
                    }
                    else if (IndirectSalesOrder.EditedPriceStatusId == Enums.EditedPriceStatusEnum.ACTIVE.Id)
                    {
                        //Trường hợp cho phép sửa giá, sử dụng giá bán do người dùng nhập
                        IndirectSalesOrderContent.Amount = Convert.ToInt64((IndirectSalesOrderContent.Quantity * IndirectSalesOrderContent.SalePrice) * ((100 - IndirectSalesOrderContent.DiscountPercentage.Value) / 100));
                    }
                }

                //tổng trước chiết khấu
                IndirectSalesOrder.SubTotal = IndirectSalesOrder.IndirectSalesOrderContents.Sum(x => x.Amount);

                //tính tổng chiết khấu theo % chiết khấu chung
                if (IndirectSalesOrder.GeneralDiscountPercentage.HasValue)
                {
                    IndirectSalesOrder.GeneralDiscountAmount = Convert.ToInt64(IndirectSalesOrder.SubTotal * IndirectSalesOrder.GeneralDiscountPercentage / 100);
                }

                if (IndirectSalesOrder.GeneralDiscountAmount.HasValue && IndirectSalesOrder.GeneralDiscountAmount > 0)
                {
                    foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                    {
                        //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                        IndirectSalesOrderContent.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount * (IndirectSalesOrderContent.Amount / IndirectSalesOrder.SubTotal);
                    }
                }

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                    IndirectSalesOrderContent.TaxAmount = Convert.ToInt64((IndirectSalesOrderContent.Amount - (IndirectSalesOrderContent.GeneralDiscountAmount.HasValue ? IndirectSalesOrderContent.GeneralDiscountAmount.Value : 0)) * IndirectSalesOrderContent.TaxPercentage / 100);
                }
                IndirectSalesOrder.TotalTaxAmount = IndirectSalesOrder.IndirectSalesOrderContents.Sum(x => x.TaxAmount.Value);
                IndirectSalesOrder.Total = IndirectSalesOrder.SubTotal - IndirectSalesOrder.GeneralDiscountAmount ?? 0 + IndirectSalesOrder.TotalTaxAmount;
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

                    var UnitOfMeasures = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Factor = x.Factor
                    }).ToList();
                    if (Product.UnitOfMeasure.StatusId == Enums.StatusEnum.ACTIVE.Id)
                    {
                        UnitOfMeasures.Add(new UnitOfMeasure
                        {
                            Id = Product.UnitOfMeasure.Id,
                            Code = Product.UnitOfMeasure.Code,
                            Name = Product.UnitOfMeasure.Name,
                            Description = Product.UnitOfMeasure.Description,
                            StatusId = Product.UnitOfMeasure.StatusId,
                            Factor = 1
                        });
                    }
                    var UOM = UnitOfMeasures.Where(x => IndirectSalesOrderPromotion.UnitOfMeasureId == x.Id).FirstOrDefault();
                    IndirectSalesOrderPromotion.RequestedQuantity = IndirectSalesOrderPromotion.Quantity * UOM.Factor.Value;
                }
            }

            return IndirectSalesOrder;
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long StoreId)
        {
            var CurrrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            var OrganizationIds = CurrrentUser.Organization.Path.Split('.').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => long.Parse(x)).OrderByDescending(x => x).ToList();
            var Store = await UOW.StoreRepository.Get(StoreId);
            var ItemIds = Items.Select(x => x.Id).ToList();
            Dictionary<long, long> result = new Dictionary<long, long>();
            IndirectPriceListItemMappingFilter IndirectPriceListItemMappingFilter = new IndirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectPriceListItemMappingSelect.ALL,
                IndirectPriceListTypeId = new IdFilter { Equal = Enums.IndirectPriceListTypeEnum.ALLSTORE.Id },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var IndirectPriceListItemMappingAllStore = await UOW.IndirectPriceListItemMappingItemMappingRepository.List(IndirectPriceListItemMappingFilter);

            IndirectPriceListItemMappingFilter = new IndirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectPriceListItemMappingSelect.ALL,
                IndirectPriceListTypeId = new IdFilter { Equal = Enums.IndirectPriceListTypeEnum.STOREGROUPING.Id },
                StoreGroupingId = new IdFilter { Equal = Store.StoreGroupingId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var IndirectPriceListItemMappingStoreGrouping = await UOW.IndirectPriceListItemMappingItemMappingRepository.List(IndirectPriceListItemMappingFilter);

            IndirectPriceListItemMappingFilter = new IndirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectPriceListItemMappingSelect.ALL,
                IndirectPriceListTypeId = new IdFilter { Equal = Enums.IndirectPriceListTypeEnum.STORETYPE.Id },
                StoreGroupingId = new IdFilter { Equal = Store.StoreTypeId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var IndirectPriceListItemMappingStoreType = await UOW.IndirectPriceListItemMappingItemMappingRepository.List(IndirectPriceListItemMappingFilter);

            IndirectPriceListItemMappingFilter = new IndirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectPriceListItemMappingSelect.ALL,
                IndirectPriceListTypeId = new IdFilter { Equal = Enums.IndirectPriceListTypeEnum.DETAILS.Id },
                StoreId = new IdFilter { Equal = StoreId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var IndirectPriceListItemMappingStoreDetail = await UOW.IndirectPriceListItemMappingItemMappingRepository.List(IndirectPriceListItemMappingFilter);

            List<IndirectPriceListItemMapping> IndirectPriceListItemMappings = new List<IndirectPriceListItemMapping>();
            IndirectPriceListItemMappings.AddRange(IndirectPriceListItemMappingAllStore);
            IndirectPriceListItemMappings.AddRange(IndirectPriceListItemMappingStoreGrouping);
            IndirectPriceListItemMappings.AddRange(IndirectPriceListItemMappingStoreType);
            IndirectPriceListItemMappings.AddRange(IndirectPriceListItemMappingStoreDetail);
            foreach (var ItemId in ItemIds)
            {
                result.Add(ItemId, long.MaxValue);
            }

            foreach (var ItemId in ItemIds)
            {
                foreach (var OrganizationId in OrganizationIds)
                {
                    long targetPrice = long.MaxValue;
                    targetPrice = IndirectPriceListItemMappings.Where(x => x.ItemId == ItemId && x.IndirectPriceList.OrganizationId == OrganizationId).Select(x => x.Price).DefaultIfEmpty(long.MaxValue).Min();
                    if (targetPrice < long.MaxValue)
                    {
                        result[ItemId] = targetPrice;
                        break;
                    }
                }
            }

            foreach (var ItemId in ItemIds)
            {
                if (result[ItemId] == long.MaxValue)
                {
                    result[ItemId] = Convert.ToInt64(Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault());
                }
            }
            return Items;
        }

        public async Task<IndirectSalesOrder> Approve(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.Id == 0)
                IndirectSalesOrder = await Create(IndirectSalesOrder);
            else
                IndirectSalesOrder = await Update(IndirectSalesOrder);
            Dictionary<string, string> Parameters = MapParameters(IndirectSalesOrder);
            bool Approved = await WorkflowService.Approve(IndirectSalesOrder.RowId, WorkflowTypeEnum.ORDER.Id, Parameters);
            if (Approved == false)
                return null;
            return await Get(IndirectSalesOrder.Id);
        }

        public async Task<IndirectSalesOrder> Reject(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
            Dictionary<string, string> Parameters = MapParameters(IndirectSalesOrder);
            bool Rejected = await WorkflowService.Reject(IndirectSalesOrder.RowId, WorkflowTypeEnum.ORDER.Id, Parameters);
            if (Rejected == false)
                return null;
            return await Get(IndirectSalesOrder.Id);
        }

        private Dictionary<string, string> MapParameters(IndirectSalesOrder IndirectSalesOrder)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add(nameof(IndirectSalesOrder.Id), IndirectSalesOrder.Id.ToString());
            Parameters.Add(nameof(IndirectSalesOrder.Code), IndirectSalesOrder.Code);
            Parameters.Add("Username", CurrentContext.UserName);
            return Parameters;
        }
    }
}
