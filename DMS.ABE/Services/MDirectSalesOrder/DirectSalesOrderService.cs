using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Enums;
using DMS.ABE.Handlers;
using DMS.ABE.Helpers;
using DMS.ABE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderService : IServiceScoped
    {
        Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<DirectSalesOrder> Get(long Id);
        Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder);
    }
    public class DirectSalesOrderService : BaseService, IDirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private IRabbitManager RabbitManager;
        private IDirectSalesOrderValidator DirectSalesOrderValidator;
        private ICurrentContext CurrentContext;

        public DirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IDirectSalesOrderValidator DirectSalesOrderValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RabbitManager = RabbitManager;
            this.DirectSalesOrderValidator = DirectSalesOrderValidator;
        }

        public async Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Create(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                Store Store = await GetStore();
                if (Store == null)
                    return null;
                DirectSalesOrder.StoreUserCreatorId = CurrentContext.StoreUserId;
                DirectSalesOrder.BuyerStoreId = Store.Id; // cửa hàng mua là cửa hàng của storeUser hiện tại
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                DirectSalesOrder.DirectSalesOrderSourceTypeId = DirectSalesOrderSourceTypeEnum.FROM_STORE.Id; // set source Type
                DirectSalesOrder.OrderDate = StaticParams.DateTimeNow; // khi tao moi don hang trne mobile mac dinh OrderDate = DateTime Now
                DirectSalesOrder.RequestStateId = RequestStateEnum.APPROVED.Id; // don hang tao tren mobile ko co wf => requestStateId = Approved
                DirectSalesOrder.StoreApprovalStateId = StoreApprovalStateEnum.APPROVED.Id; // don tao boi app dai ly co trang thai phe duyet 
                DirectSalesOrder.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id; // don hang tao tu ams.abe mac dinh khong cho sua gia
                AppUser SaleEmployee = await UOW.AppUserRepository.Get(DirectSalesOrder.SaleEmployeeId);
                if(SaleEmployee != null)
                {
                    DirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                }
                await CalculateOrder(DirectSalesOrder, Store.Id); // tính toán nội dung đơn hàng
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder); // tạo mới đơn
                await UOW.Commit();
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString(); // gán lại Code đơn hàng
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                NotifyUsed(DirectSalesOrder); // update nội bộ
                Sync(DirectSalesOrder); // sync đơn hàng
                await Logging.CreateAuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderService)); // ghi log
                return DirectSalesOrder;
            }
            catch (Exception Exception)
            {
                await UOW.Rollback();

                if (Exception.InnerException == null)
                {
                    await Logging.CreateSystemLog(Exception, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception);
                }
                else
                {
                    await Logging.CreateSystemLog(Exception.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception.InnerException);
                }
            }
        }

        public async Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Approve(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                oldData.StoreApprovalStateId = StoreApprovalStateEnum.APPROVED.Id; // update StoreApprovalStateId
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                await UOW.Commit();
                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                Sync(DirectSalesOrder);
                await Logging.CreateAuditLog(DirectSalesOrder, oldData, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception Exception)
            {
                await UOW.Rollback();

                if (Exception.InnerException == null)
                {
                    await Logging.CreateSystemLog(Exception, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception);
                }
                else
                {
                    await Logging.CreateSystemLog(Exception.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception.InnerException);
                }
            }

        }

        public async Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Approve(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                oldData.StoreApprovalStateId = StoreApprovalStateEnum.REJECTED.Id; // update StoreApprovalStateId
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                await UOW.Commit();
                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                Sync(DirectSalesOrder);
                await Logging.CreateAuditLog(DirectSalesOrder, oldData, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception Exception)
            {
                await UOW.Rollback();

                if (Exception.InnerException == null)
                {
                    await Logging.CreateSystemLog(Exception, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception);
                }
                else
                {
                    await Logging.CreateSystemLog(Exception.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(Exception.InnerException);
                }
            }

        }

        public async Task<DirectSalesOrder> Get(long Id)
        {
            DirectSalesOrder DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(Id);
            if (DirectSalesOrder == null)
                return null;
            return DirectSalesOrder;
        }

        public async Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                long StoreUserId = CurrentContext.StoreUserId;
                List<StoreUser> StoreUsers = await UOW.StoreUserRepository.List(new StoreUserFilter
                {
                    Id = new IdFilter { Equal = StoreUserId },
                    Selects = StoreUserSelect.Id | StoreUserSelect.Store
                });
                StoreUser StoreUser = StoreUsers.FirstOrDefault();
                DirectSalesOrderFilter.BuyerStoreId = new IdFilter { Equal = StoreUser.StoreId }; // filter đơn hàng có của hàng mua là cửa hàng của storeUser
                DirectSalesOrderFilter.RequestStateId = new IdFilter { Equal = RequestStateEnum.APPROVED.Id }; // chi hien thi don hang co trang thai wf la approved
                List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.List(DirectSalesOrderFilter);
                return DirectSalesOrders;
            } catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                long StoreUserId = CurrentContext.StoreUserId;
                List<StoreUser> StoreUsers = await UOW.StoreUserRepository.List(new StoreUserFilter
                {
                    Id = new IdFilter { Equal = StoreUserId },
                    Selects = StoreUserSelect.Id | StoreUserSelect.Store
                });
                StoreUser StoreUser = StoreUsers.FirstOrDefault();
                DirectSalesOrderFilter.BuyerStoreId = new IdFilter { Equal = StoreUser.StoreId }; // filter đơn hàng có của hàng mua là cửa hàng của storeUser
                DirectSalesOrderFilter.RequestStateId = new IdFilter { Equal = RequestStateEnum.APPROVED.Id }; // chi hien thi don hang co trang thai wf la approved
                int result = await UOW.DirectSalesOrderRepository.Count(DirectSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private async Task<Store> GetStore()
        {
            var StoreUserId = CurrentContext.StoreUserId;
            StoreUser StoreUser = await UOW.StoreUserRepository.Get(StoreUserId);
            if (StoreUser == null)
            {
                return null;
            } // check storeUser co ton tai khong
            Store Store = await UOW.StoreRepository.Get(StoreUser.StoreId);
            if (Store == null)
            {
                return null;
            } // check store tuong ung vs storeUser co ton tai khong
            return Store;
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long StoreId)
        {
            var Store = await UOW.StoreRepository.Get(StoreId);
            SystemConfiguration SystemConfiguration = await UOW.SystemConfigurationRepository.Get();
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(Store.Organization.Path) || Store.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();

            var ItemIds = Items.Select(x => x.Id).Distinct().ToList();
            Dictionary<long, decimal> result = new Dictionary<long, decimal>();

            PriceListItemMappingFilter PriceListItemMappingFilter = new PriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListItemMappingSelect.ALL,
                PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.ALLSTORE.Id },
                SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var PriceListItemMappingAllStore = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
            List<PriceListItemMapping> PriceListItemMappings = new List<PriceListItemMapping>();
            PriceListItemMappings.AddRange(PriceListItemMappingAllStore);

            PriceListItemMappingFilter = new PriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListItemMappingSelect.ALL,
                PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STOREGROUPING.Id },
                SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
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
                SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                StoreTypeId = new IdFilter { Equal = Store.StoreTypeId },
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
                SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                StoreId = new IdFilter { Equal = StoreId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var PriceListItemMappingStoreDetail = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
            PriceListItemMappings.AddRange(PriceListItemMappingStoreGrouping);
            PriceListItemMappings.AddRange(PriceListItemMappingStoreType);
            PriceListItemMappings.AddRange(PriceListItemMappingStoreDetail);

            //Áp giá theo cấu hình
            //Ưu tiên lấy giá thấp hơn
            if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 0)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MaxValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MaxValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MaxValue)
                            .Min();
                        if (targetPrice < result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MaxValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }
            //Ưu tiên lấy giá cao hơn
            else if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 1)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MinValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MinValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MinValue)
                            .Max();
                        if (targetPrice > result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MinValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }

            //nhân giá với thuế
            foreach (var item in Items)
            {
                item.SalePrice = result[item.Id] * (1 + item.Product.TaxType.Percentage / 100);
            }
            return Items;
        } // áp giá cho list item

        private async Task<DirectSalesOrder> CalculateOrder(DirectSalesOrder DirectSalesOrder, long StoreId)
        {
            var ProductIds = new List<long>();
            var ItemIds = new List<long>();
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                ItemIds.AddRange(DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList());
                List<Item> ListItems = await UOW.ItemRepository.List(new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = ItemIds },
                    Selects = ItemSelect.Id | ItemSelect.SalePrice | ItemSelect.ProductId
                });
                ProductIds.AddRange(ListItems.Select(x => x.ProductId).ToList());
            }
            ProductIds = ProductIds.Distinct().ToList();
            ItemIds = ItemIds.Distinct().ToList();

            List<Item> Items = await UOW.ItemRepository.List(new ItemFilter {
                Skip = 0,
                Take = ItemIds.Count,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.ALL,
            });

            Items = await ApplyPrice(Items, StoreId); // áp giá cho Item trong đơn hàng

            List<Product> Products = await UOW.ProductRepository.List(new ProductFilter
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

            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    var Item = Items.Where(x => x.Id == DirectSalesOrderContent.ItemId).FirstOrDefault();
                    var Product = Products.Where(x => Item.ProductId == x.Id).FirstOrDefault();
                    DirectSalesOrderContent.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;
                    DirectSalesOrderContent.UnitOfMeasureId = Product.UnitOfMeasureId; // đơn hàng tạo từ Mobile mặc định UOM của product
                    UnitOfMeasure UOM = new UnitOfMeasure
                    {
                        Id = Product.UnitOfMeasure.Id,
                        Code = Product.UnitOfMeasure.Code,
                        Name = Product.UnitOfMeasure.Name,
                        Description = Product.UnitOfMeasure.Description,
                        StatusId = Product.UnitOfMeasure.StatusId,
                        Factor = 1
                    };
                    DirectSalesOrderContent.RequestedQuantity = DirectSalesOrderContent.Quantity * UOM.Factor.Value;
                    DirectSalesOrderContent.PrimaryPrice = Item.SalePrice.GetValueOrDefault(0); // mặc định = giá của item vì không cho sửa giá trên mobile
                    DirectSalesOrderContent.SalePrice = DirectSalesOrderContent.PrimaryPrice * UOM.Factor.Value; // giá của một đơn vị item
                    DirectSalesOrderContent.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id; // mặc định ko cho sửa giá

                    decimal SubAmount = DirectSalesOrderContent.Quantity * DirectSalesOrderContent.SalePrice; // giá của một dòng theo đơn vị tính
                    DirectSalesOrderContent.Amount = SubAmount; // tạm tính
                    if (DirectSalesOrderContent.DiscountPercentage.HasValue)
                    {
                        DirectSalesOrderContent.DiscountAmount = SubAmount * DirectSalesOrderContent.DiscountPercentage.Value / 100;
                        DirectSalesOrderContent.DiscountAmount = Math.Round(DirectSalesOrderContent.DiscountAmount ?? 0, 0);
                        DirectSalesOrderContent.Amount = SubAmount - DirectSalesOrderContent.DiscountAmount.Value;
                    } // áp giảm giá nếu có
                } // gán primaryUOMId, RequestedQuantity
                DirectSalesOrder.SubTotal = DirectSalesOrder.DirectSalesOrderContents.Sum(x => x.Amount); //tổng trước chiết khấu
                if (DirectSalesOrder.GeneralDiscountPercentage.HasValue && DirectSalesOrder.GeneralDiscountPercentage > 0)
                {
                    DirectSalesOrder.GeneralDiscountAmount = DirectSalesOrder.SubTotal * (DirectSalesOrder.GeneralDiscountPercentage / 100);
                    DirectSalesOrder.GeneralDiscountAmount = Math.Round(DirectSalesOrder.GeneralDiscountAmount.Value, 0);
                }  //tính tổng chiết khấu theo % chiết khấu chung

                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                    DirectSalesOrderContent.GeneralDiscountPercentage = DirectSalesOrderContent.Amount / DirectSalesOrder.SubTotal * 100;
                    DirectSalesOrderContent.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount * DirectSalesOrderContent.GeneralDiscountPercentage / 100;
                    DirectSalesOrderContent.GeneralDiscountAmount = Math.Round(DirectSalesOrderContent.GeneralDiscountAmount ?? 0, 0);
                    //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                    DirectSalesOrderContent.TaxAmount = (DirectSalesOrderContent.Amount - (DirectSalesOrderContent.GeneralDiscountAmount.HasValue ? DirectSalesOrderContent.GeneralDiscountAmount.Value : 0)) * DirectSalesOrderContent.TaxPercentage / 100;
                    DirectSalesOrderContent.TaxAmount = Math.Round(DirectSalesOrderContent.TaxAmount ?? 0, 0);
                } // chiết khấu phân bổ theo dòng
                DirectSalesOrder.TotalTaxAmount = DirectSalesOrder.DirectSalesOrderContents.Where(x => x.TaxAmount.HasValue).Sum(x => x.TaxAmount.Value); // tính tổng thuê theo dòng
                DirectSalesOrder.TotalTaxAmount = Math.Round(DirectSalesOrder.TotalTaxAmount, 0);
                DirectSalesOrder.TotalAfterTax = DirectSalesOrder.SubTotal - (DirectSalesOrder.GeneralDiscountAmount.HasValue ? DirectSalesOrder.GeneralDiscountAmount.Value : 0) + DirectSalesOrder.TotalTaxAmount;  //tổng phải thanh toán
                DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax; // không có promotion nên tổng sau thuế là tổng cuối
            }
            else
            {
                DirectSalesOrder.SubTotal = 0;
                DirectSalesOrder.GeneralDiscountPercentage = null;
                DirectSalesOrder.GeneralDiscountAmount = null;
                DirectSalesOrder.TotalTaxAmount = 0;
                DirectSalesOrder.TotalAfterTax = 0;
                DirectSalesOrder.Total = 0;
            }
            return DirectSalesOrder;
        } // tinh subtotal, totalTaxAmount,  totalAfterTax, Total, phần trăm chiết khẩu chung, tổng chiết khấu

        private void NotifyUsed(DirectSalesOrder DirectSalesOrder)
        {
            {
                List<long> ItemIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.ItemId).ToList();
                List<EventMessage<Item>> itemMessages = ItemIds.Select(i => new EventMessage<Item>
                {
                    Content = new Item { Id = i },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(itemMessages, RoutingKeyEnum.ItemUsed);
            } // thông báo lên MDM
            {
                List<long> PrimaryUOMIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.PrimaryUnitOfMeasureId).ToList();
                List<long> UOMIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.UnitOfMeasureId).ToList();
                UOMIds.AddRange(PrimaryUOMIds);
                List<EventMessage<UnitOfMeasure>> UnitOfMeasureMessages = UOMIds.Select(x => new EventMessage<UnitOfMeasure>
                {
                    Content = new UnitOfMeasure { Id = x },
                    EntityName = nameof(UnitOfMeasure),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            } // thông báo lên MDM
            {
                List<EventMessage<Store>> storeMessages = new List<EventMessage<Store>>();
                EventMessage<Store> BuyerStore = new EventMessage<Store>
                {
                    Content = new Store { Id = DirectSalesOrder.BuyerStoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                storeMessages.Add(BuyerStore);
                RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
            } // thông báo lên DMS
            {
                EventMessage<AppUser> AppUserMessage = new EventMessage<AppUser>
                {
                    Content = new AppUser { Id = DirectSalesOrder.SaleEmployeeId },
                    EntityName = nameof(AppUser),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            } // thông báo lên PORTAL
        } // thông báo store, item, UOM đã được sử dụng

        private void Sync(DirectSalesOrder DirectSalesOrder)
        {
            List<EventMessage<DirectSalesOrder>> EventMessageDirectSalesOrders = new List<EventMessage<DirectSalesOrder>>();
            EventMessage<DirectSalesOrder> EventMessageDirectSalesOrder = new EventMessage<DirectSalesOrder>(DirectSalesOrder, DirectSalesOrder.RowId);
            EventMessageDirectSalesOrders.Add(EventMessageDirectSalesOrder);

            RabbitManager.PublishList(EventMessageDirectSalesOrders, RoutingKeyEnum.DirectSalesOrderSync); // đồng bộ
        }
    }
}
