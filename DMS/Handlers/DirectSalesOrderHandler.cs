using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DMS.Enums;
using DMS.Helpers;

namespace DMS.Handlers
{
    public class DirectSalesOrderHandler : Handler
    {

        private string CreateKey => "DMS." + Name + ".Create";

        private string UpdateKey => "DMS." + Name + ".Update";

        public override string Name => nameof(DirectSalesOrder);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"DMS.{Name}.*", null); // lấy ra cả queue create và update
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == CreateKey)
                await Create(context, content); // handle queue tạo mới đơn hàng
            if (routingKey == UpdateKey)
                await Update(context, content); // handle queue cập nhật đơn hàng
        }

        private async Task Create(DataContext context, string json)
        {
            List<EventMessage<DirectSalesOrder>> DirectSalesOrderEventMessages = JsonConvert.DeserializeObject<List<EventMessage<DirectSalesOrder>>>(json);
            List<DirectSalesOrder> DirectSalesOrders = DirectSalesOrderEventMessages.Select(x => x.Content).ToList();
            foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
            {
                try
                {
                    DirectSalesOrderDAO DirectSalesOrderDAO = new DirectSalesOrderDAO
                    {
                        Code = DirectSalesOrder.Code,
                        OrganizationId = DirectSalesOrder.OrganizationId,
                        BuyerStoreId = DirectSalesOrder.BuyerStoreId,
                        SaleEmployeeId = DirectSalesOrder.SaleEmployeeId,
                        RequestStateId = RequestStateEnum.APPROVED.Id, // don hang tao tu ams.abe mac dinh khong co wf -> wf requestState = approved 
                        EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id, // don hang tao tu ams.abe mac dinh khong cho sua gia
                        SubTotal = DirectSalesOrder.SubTotal,
                        TotalTaxAmount = DirectSalesOrder.TotalTaxAmount,
                        TotalAfterTax = DirectSalesOrder.TotalAfterTax,
                        Total = DirectSalesOrder.Total,
                        RowId = DirectSalesOrder.RowId,
                        StoreUserCreatorId = DirectSalesOrder.StoreUserCreatorId, // luu acc cua store tao don hang
                        OrderDate = DirectSalesOrder.OrderDate, // ams.abe tao ra neu client ko gui ve
                        CreatedAt = DirectSalesOrder.CreatedAt, // lay tu ams.abe ra neu client ko gui ve
                        UpdatedAt = DirectSalesOrder.UpdatedAt, // lay tu ams.abe ra neu client ko gui ve
                    };
                    await context.BulkMergeAsync(new List<DirectSalesOrderDAO> { DirectSalesOrderDAO });
                    DirectSalesOrder.Id = DirectSalesOrderDAO.Id;
                    await SaveReference(context, DirectSalesOrder);
                    AuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderHandler)); // ghi log
                    await NotifyUsed(DirectSalesOrder);
                    await SyncDirectSalesOrder(DirectSalesOrder); // public sync for AMS Web
                }
                catch (Exception ex)
                {
                    SystemLog(ex, nameof(DirectSalesOrderHandler));
                }
            }
        }

        private async Task Update(DataContext context, string json)
        {
            List<EventMessage<DirectSalesOrder>> DirectSalesOrderEventMessages = JsonConvert.DeserializeObject<List<EventMessage<DirectSalesOrder>>>(json);
            List<DirectSalesOrder> DirectSalesOrders = DirectSalesOrderEventMessages.Select(x => x.Content).ToList();
            foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
            {
                try
                {
                    DirectSalesOrderDAO DirectSalesOrderDAO = await context.DirectSalesOrder.Where(x => x.Id == DirectSalesOrder.Id).FirstOrDefaultAsync(); // lay don hang tu db
                    DirectSalesOrderDAO.Code = DirectSalesOrder.Code;
                    DirectSalesOrderDAO.OrganizationId = DirectSalesOrder.OrganizationId;
                    DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
                    DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
                    DirectSalesOrderDAO.RequestStateId = RequestStateEnum.APPROVED.Id; // don hang tao tu ams.abe mac dinh khong co wf -> wf requestState = approved 
                    DirectSalesOrderDAO.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id; // don hang tao tu ams.abe mac dinh khong cho sua gia
                    DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
                    DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
                    DirectSalesOrderDAO.TotalAfterTax = DirectSalesOrder.TotalAfterTax;
                    DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
                    DirectSalesOrderDAO.RowId = DirectSalesOrder.RowId;
                    DirectSalesOrderDAO.StoreUserCreatorId = DirectSalesOrder.StoreUserCreatorId; // luu acc cua store tao don hang
                    DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate; // ams.abe tao ra neu client ko gui ve
                    DirectSalesOrderDAO.CreatedAt = DirectSalesOrder.CreatedAt; // lay tu ams.abe ra neu client ko gui ve
                    DirectSalesOrderDAO.UpdatedAt = DirectSalesOrder.UpdatedAt; // lay tu ams.abe ra neu client ko gui ve
                    await context.BulkMergeAsync(new List<DirectSalesOrderDAO> { DirectSalesOrderDAO }); // luu vao db
                    await SaveReference(context, DirectSalesOrder);
                    await NotifyUsed(DirectSalesOrder);
                    await SyncDirectSalesOrder(DirectSalesOrder);
                }
                catch (Exception ex)
                {
                    SystemLog(ex, nameof(DirectSalesOrderHandler));
                }
            }
        }


        private async Task SyncDirectSalesOrder(DirectSalesOrder DirectSalesOrder)
        {
            List<EventMessage<DirectSalesOrder>> EventMessageDirectSalesOrders = new List<EventMessage<DirectSalesOrder>>();
            EventMessage<DirectSalesOrder> EventMessageDirectSalesOrder = new EventMessage<DirectSalesOrder>(DirectSalesOrder, DirectSalesOrder.RowId);
            EventMessageDirectSalesOrders.Add(EventMessageDirectSalesOrder);

            RabbitManager.PublishList(EventMessageDirectSalesOrders, RoutingKeyEnum.DirectSalesOrderSync); // đồng bộ lên AMS
        }

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
                if (DirectSalesOrder.StoreUserCreatorId.HasValue)
                {
                    EventMessage<StoreUser> StoreUserMessage = new EventMessage<StoreUser>
                    {
                        Content = new StoreUser { Id = DirectSalesOrder.StoreUserCreatorId.Value },
                        EntityName = nameof(StoreUser),
                        RowId = Guid.NewGuid(),
                        Time = StaticParams.DateTimeNow,
                    };
                    RabbitManager.PublishSingle(StoreUserMessage, RoutingKeyEnum.StoreUserUsed);
                }
            } // thong bao len DMS
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
        } // thông báo item, UOM, Store đã sử dụng

        private async Task SaveReference(DataContext context, DirectSalesOrder DirectSalesOrder)
        {
            try
            {
                await context.DirectSalesOrderContent
                .Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id)
                .DeleteFromQueryAsync();
                List<DirectSalesOrderContentDAO> DirectSalesOrderContentDAOs = new List<DirectSalesOrderContentDAO>();
                if (DirectSalesOrder.DirectSalesOrderContents != null)
                {
                    foreach (DirectSalesOrderContent DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                    {
                        DirectSalesOrderContentDAO DirectSalesOrderContentDAO = new DirectSalesOrderContentDAO();
                        DirectSalesOrderContentDAO.Id = DirectSalesOrderContent.Id;
                        DirectSalesOrderContentDAO.DirectSalesOrderId = DirectSalesOrder.Id;
                        DirectSalesOrderContentDAO.ItemId = DirectSalesOrderContent.ItemId;
                        DirectSalesOrderContentDAO.UnitOfMeasureId = DirectSalesOrderContent.UnitOfMeasureId;
                        DirectSalesOrderContentDAO.Quantity = DirectSalesOrderContent.Quantity;
                        DirectSalesOrderContentDAO.PrimaryUnitOfMeasureId = DirectSalesOrderContent.PrimaryUnitOfMeasureId;
                        DirectSalesOrderContentDAO.RequestedQuantity = DirectSalesOrderContent.RequestedQuantity;
                        DirectSalesOrderContentDAO.PrimaryPrice = DirectSalesOrderContent.PrimaryPrice;
                        DirectSalesOrderContentDAO.SalePrice = DirectSalesOrderContent.SalePrice;
                        DirectSalesOrderContentDAO.EditedPriceStatusId = DirectSalesOrderContent.EditedPriceStatusId;
                        DirectSalesOrderContentDAO.DiscountPercentage = DirectSalesOrderContent.DiscountPercentage;
                        DirectSalesOrderContentDAO.DiscountAmount = DirectSalesOrderContent.DiscountAmount;
                        DirectSalesOrderContentDAO.GeneralDiscountPercentage = DirectSalesOrderContent.GeneralDiscountPercentage;
                        DirectSalesOrderContentDAO.GeneralDiscountAmount = DirectSalesOrderContent.GeneralDiscountAmount;
                        DirectSalesOrderContentDAO.Amount = DirectSalesOrderContent.Amount;
                        DirectSalesOrderContentDAO.TaxPercentage = DirectSalesOrderContent.TaxPercentage;
                        DirectSalesOrderContentDAO.TaxAmount = DirectSalesOrderContent.TaxAmount;
                        DirectSalesOrderContentDAO.Factor = DirectSalesOrderContent.Factor;
                        DirectSalesOrderContentDAOs.Add(DirectSalesOrderContentDAO);
                    }
                    await context.DirectSalesOrderContent.BulkMergeAsync(DirectSalesOrderContentDAOs);
                    DirectSalesOrder.DirectSalesOrderContents = await context.DirectSalesOrderContent.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id)
                                                                            .Select(c => new DirectSalesOrderContent
                                                                            {
                                                                                Id = c.Id,
                                                                                DirectSalesOrderId = c.DirectSalesOrderId,
                                                                                ItemId = c.ItemId,
                                                                                UnitOfMeasureId = c.UnitOfMeasureId,
                                                                                PrimaryUnitOfMeasureId = c.PrimaryUnitOfMeasureId,
                                                                                Quantity = c.Quantity,
                                                                                RequestedQuantity = c.RequestedQuantity,
                                                                                PrimaryPrice = c.PrimaryPrice,
                                                                                SalePrice = c.SalePrice,
                                                                                EditedPriceStatusId =c.EditedPriceStatusId,
                                                                                Amount = c.Amount,
                                                                                TaxPercentage = c.TaxPercentage,
                                                                                TaxAmount = c.TaxAmount,
                                                                                Factor = c.Factor,
                                                                                DiscountPercentage = c.DiscountPercentage,
                                                                                DiscountAmount = c.DiscountAmount,
                                                                                GeneralDiscountPercentage =c.GeneralDiscountPercentage,
                                                                                GeneralDiscountAmount = c.GeneralDiscountAmount,
                                                                            }).ToListAsync(); // sync to ams
                }

                await context.DirectSalesOrderTransaction.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id).DeleteFromQueryAsync();
                List<DirectSalesOrderTransactionDAO> DirectSalesOrderTransactionDAOs = new List<DirectSalesOrderTransactionDAO>();
                if (DirectSalesOrder.DirectSalesOrderContents != null)
                {
                    foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                    {
                        DirectSalesOrderTransactionDAO DirectSalesOrderTransactionDAO = new DirectSalesOrderTransactionDAO
                        {
                            DirectSalesOrderId = DirectSalesOrder.Id,
                            ItemId = DirectSalesOrderContent.ItemId,
                            OrganizationId = DirectSalesOrder.OrganizationId,
                            BuyerStoreId = DirectSalesOrder.BuyerStoreId,
                            SalesEmployeeId = DirectSalesOrder.SaleEmployeeId,
                            OrderDate = DirectSalesOrder.OrderDate,
                            TypeId = TransactionTypeEnum.SALES_CONTENT.Id,
                            UnitOfMeasureId = DirectSalesOrderContent.PrimaryUnitOfMeasureId,
                            Quantity = DirectSalesOrderContent.RequestedQuantity,
                            Revenue = DirectSalesOrderContent.Amount - (DirectSalesOrderContent.GeneralDiscountAmount ?? 0) + (DirectSalesOrderContent.TaxAmount ?? 0),
                            Discount = (DirectSalesOrderContent.DiscountAmount ?? 0) + (DirectSalesOrderContent.GeneralDiscountAmount ?? 0)
                        };
                        DirectSalesOrderTransactionDAOs.Add(DirectSalesOrderTransactionDAO);
                    }
                    await context.DirectSalesOrderTransaction.BulkMergeAsync(DirectSalesOrderTransactionDAOs);
                }
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(DirectSalesOrderHandler));
            }
        }
    }
}
