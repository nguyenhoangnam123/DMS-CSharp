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
        private string SyncKey => Name + ".Sync";

        public override string Name => "DMSDirectSalesOrder";

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<DirectSalesOrder>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<DirectSalesOrder>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<DirectSalesOrder>> DirectSalesOrderEventMessages = await ListEventMessage<DirectSalesOrder>(context, SyncKey, RowIds);
            //List<DirectSalesOrder> DirectSalesOrders = DirectSalesOrderEventMessages.Select(x => x.Content).ToList();

            List<DirectSalesOrder> DirectSalesOrders = new List<DirectSalesOrder>();

            foreach (var RowId in RowIds)
            {
                EventMessage<DirectSalesOrder> EventMessage = DirectSalesOrderEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    DirectSalesOrders.Add(EventMessage.Content);
            } // loc message theo rowId

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
    }
}
