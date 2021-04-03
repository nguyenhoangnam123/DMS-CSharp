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
        Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder);
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
                await CalculateOrder(DirectSalesOrder);
                DirectSalesOrder.StoreUserCreatorId = CurrentContext.StoreUserId;
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                DirectSalesOrder.DirectSalesOrderSourceTypeId = DirectSalesOrderSourceTypeEnum.FROM_AMS.Id; // set source Type
                DirectSalesOrder.OrderDate = StaticParams.DateTimeNow; // khi tao moi don hang trne mobile mac dinh OrderDate = DateTime Now
                AppUser SaleEmployee = await UOW.AppUserRepository.Get(DirectSalesOrder.SaleEmployeeId);
                if(SaleEmployee != null)
                {
                    DirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                }
                await NotifyUsed(DirectSalesOrder); // update nội bộ
                List<EventMessage<DirectSalesOrder>> EventMessageDirectSalesOrders = new List<EventMessage<DirectSalesOrder>>();
                EventMessage<DirectSalesOrder> EventMessageDirectSalesOrder = new EventMessage<DirectSalesOrder>(DirectSalesOrder, DirectSalesOrder.RowId);
                EventMessageDirectSalesOrders.Add(EventMessageDirectSalesOrder);
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

        public async Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Update(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                //await CalculateOrder(DirectSalesOrder);
                //await NotifyUsed(DirectSalesOrder); // update nội bộ
                List<EventMessage<DirectSalesOrder>> EventMessageDirectSalesOrders = new List<EventMessage<DirectSalesOrder>>();
                EventMessage<DirectSalesOrder> EventMessageDirectSalesOrder = new EventMessage<DirectSalesOrder>(DirectSalesOrder, DirectSalesOrder.RowId);
                EventMessageDirectSalesOrders.Add(EventMessageDirectSalesOrder);
                //RabbitManager.PublishList(EventMessageDirectSalesOrders, RoutingKeyEnum.DirectSalesOrderUpdate); // tạo message update tới DMS
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
                DirectSalesOrderFilter.StoreUserCreatorId = new IdFilter { Equal = CurrentContext.StoreUserId }; // filter nguoi tao la currentUser
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
                DirectSalesOrderFilter.StoreUserCreatorId = new IdFilter { Equal = CurrentContext.StoreUserId }; // filter nguoi tao la currentUser
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

        private async Task<DirectSalesOrder> CalculateOrder(DirectSalesOrder DirectSalesOrder)
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

            //List<ItemBasePrice> ItemBasePrices = await UOW.ItemBasePriceRepository.List(new ItemBasePriceFilter {
            //    Skip = 0,
            //    Take = int.MaxValue,
            //    ItemId = new IdFilter { In = ItemIds  },
            //    Selects = ItemBasePriceSelect.ALL,
            //});

            List<Item> Items = await UOW.ItemRepository.List(new ItemFilter {
                Skip = 0,
                Take = ItemIds.Count,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.ALL,
            });

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

            //foreach (Item Item in Items)
            //{
            //    ItemBasePrice ItemBasePrice = ItemBasePrices.Where(x => x.ItemId == Item.Id).FirstOrDefault();
            //    if(ItemBasePrice != null)
            //    {
            //        Item.SalePrice = ItemBasePrice.BasePrice;
            //    }
            //} // nếu không có itemBasePrice thì lấy giá item từ client

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

        private async Task NotifyUsed(DirectSalesOrder DirectSalesOrder)
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
    }
}
