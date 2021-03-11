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
                                                                                                         //await Logging.CreateAuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderHandler)); // ghi log
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

    }
}
