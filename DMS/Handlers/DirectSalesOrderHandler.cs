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
            List<DirectSalesOrder> DirectSalesOrders = await GetDataFromMessage(context, json);
            try
            {
                List<long> DirectSalesOrderIds = DirectSalesOrders.Select(x => x.Id).ToList(); // lẩy ra Ids để thêm mới hoặc udpate
                List<DirectSalesOrderDAO> DirectSalesOrdersDAOs = await context.DirectSalesOrder.Where(x => DirectSalesOrderIds.Contains(x.Id)).ToListAsync();
                foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
                {
                    DirectSalesOrderDAO DirectSalesOrderDAO = DirectSalesOrdersDAOs.Where(x => x.Id == DirectSalesOrder.Id).FirstOrDefault();
                    if (DirectSalesOrderDAO == null)
                    {
                        DirectSalesOrderDAO = new DirectSalesOrderDAO();
                    } // tạo mới
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
                    DirectSalesOrderDAO.CreatorId = DirectSalesOrder.CreatorId;
                    DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate; // ams.abe tao ra neu client ko gui ve
                    DirectSalesOrderDAO.CreatedAt = DirectSalesOrder.CreatedAt; // lay tu ams.abe ra neu client ko gui ve
                }
                await context.BulkMergeAsync(DirectSalesOrdersDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(DirectSalesOrderHandler));
            }
        }

        private async Task Update(DataContext context, string json)
        {
            List<DirectSalesOrder> DirectSalesOrders = await GetDataFromMessage(context, json);
            try
            {
                List<long> DirectSalesOrderIds = DirectSalesOrders.Select(x => x.Id).ToList(); // lẩy ra Ids để thêm mới hoặc udpate
                List<DirectSalesOrderDAO> DirectSalesOrdersDAOs = await context.DirectSalesOrder.Where(x => DirectSalesOrderIds.Contains(x.Id)).ToListAsync();
                foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
                {
                    DirectSalesOrderDAO DirectSalesOrderDAO = DirectSalesOrdersDAOs.Where(x => x.Id == DirectSalesOrder.Id).FirstOrDefault();
                    if (DirectSalesOrderDAO == null)
                    {
                        DirectSalesOrderDAO = new DirectSalesOrderDAO();
                    } // tạo mới
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
                    DirectSalesOrderDAO.CreatorId = DirectSalesOrder.CreatorId;
                    DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate; // ams.abe tao ra neu client ko gui ve
                    DirectSalesOrderDAO.CreatedAt = DirectSalesOrder.CreatedAt; // lay tu ams.abe ra neu client ko gui ve
                }
                await context.BulkMergeAsync(DirectSalesOrdersDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(DirectSalesOrderHandler));
            }
        }

        private async Task<List<DirectSalesOrder>> GetDataFromMessage(DataContext context, string json)
        {
            List<DirectSalesOrder> DirectSalesOrders = new List<DirectSalesOrder>();
            List<EventMessage<DirectSalesOrder>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<DirectSalesOrder>>>(json);
            await SaveEventMessage(context, CreateKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<DirectSalesOrder>> DirectSalesOrderEventMessages = await ListEventMessage<DirectSalesOrder>(context, CreateKey, RowIds);

            foreach (var RowId in RowIds)
            {
                EventMessage<DirectSalesOrder> EventMessage = DirectSalesOrderEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    DirectSalesOrders.Add(EventMessage.Content);
            } // loc message theo rowId

            return DirectSalesOrders;
        }

        private async Task Sync(DirectSalesOrder DirectSalesOrder)
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
        } // thông báo item, UOM, Store đã sử dụng
    }
}
