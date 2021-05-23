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
using DMS.Repositories;

namespace DMS.Handlers
{
    public class DirectSalesOrderHandler : Handler
    {
        private string CreateKey => $"{StaticParams.ModuleName}.{Name}.Create";

        private string UpdateKey => $"{StaticParams.ModuleName}.{Name}.Update";

        public override string Name => nameof(DirectSalesOrder);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{StaticParams.ModuleName}.{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == CreateKey)
                await Create(UOW, content);
            if (routingKey == UpdateKey)
                await Update(UOW, content);
        }

        private async Task Create(IUOW UOW, string json)
        {
            try
            {
                DirectSalesOrder DirectSalesOrder = JsonConvert.DeserializeObject<DirectSalesOrder>(json);
                DirectSalesOrder.RequestStateId = RequestStateEnum.APPROVED.Id; // don hang tao tu ams.abe mac dinh khong co wf -> wf requestState = approved
                DirectSalesOrder.StoreApprovalStateId = StoreApprovalStateEnum.PENDING.Id; // trang thai doi cua hang duyet
                DirectSalesOrder.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id; // don hang tao tu ams.abe mac dinh khong cho sua gia
                DirectSalesOrder.DirectSalesOrderSourceTypeId = DirectSalesOrderSourceTypeEnum.FROM_STORE.Id; // sourceType

                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder);

                DirectSalesOrder = (await UOW.DirectSalesOrderRepository.List(new List<long> { DirectSalesOrder.Id })).FirstOrDefault();

                await NotifyUsed(DirectSalesOrder);
                RabbitManager.PublishSingle(DirectSalesOrder, RoutingKeyEnum.DirectSalesOrderSync); // đồng bộ lên AMS 
                AuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderHandler)); // ghi log
            }
            catch (Exception ex)
            {
                Log(ex, nameof(DirectSalesOrderHandler));
            }
        }

        private async Task Update(IUOW UOW, string json)
        {
            try
            {
                DirectSalesOrder DirectSalesOrder = JsonConvert.DeserializeObject<DirectSalesOrder>(json);
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                DirectSalesOrder.OrganizationId = DirectSalesOrder.OrganizationId == 0 ? oldData.OrganizationId : DirectSalesOrder.OrganizationId;
                DirectSalesOrder.BuyerStoreId = DirectSalesOrder.BuyerStoreId == 0 ? oldData.BuyerStoreId : DirectSalesOrder.BuyerStoreId;
                DirectSalesOrder.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId == 0 ? oldData.SaleEmployeeId : DirectSalesOrder.SaleEmployeeId;
                DirectSalesOrder.RequestStateId = DirectSalesOrder.RequestStateId == 0 ? oldData.RequestStateId : DirectSalesOrder.RequestStateId;
                DirectSalesOrder.StoreUserCreatorId = DirectSalesOrder.StoreUserCreatorId == null ? oldData.StoreUserCreatorId : DirectSalesOrder.StoreUserCreatorId; // luu acc cua store tao don hang
                DirectSalesOrder.OrderDate = DirectSalesOrder.OrderDate == null ? oldData.OrderDate : DirectSalesOrder.OrderDate;

                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                DirectSalesOrder = (await UOW.DirectSalesOrderRepository.List(new List<long> { DirectSalesOrder.Id })).FirstOrDefault();

                await NotifyUsed(DirectSalesOrder);
                RabbitManager.PublishSingle(DirectSalesOrder, RoutingKeyEnum.DirectSalesOrderSync); // đồng bộ lên AMS 
                AuditLog(DirectSalesOrder, oldData, nameof(DirectSalesOrderHandler)); // ghi log
            }
            catch (Exception ex)
            {
                Log(ex, nameof(DirectSalesOrderHandler));
            }
        }

        private async Task NotifyUsed(DirectSalesOrder DirectSalesOrder)
        {
            List<Item> itemMessages = DirectSalesOrder.DirectSalesOrderContents.Select(x => new Item { Id = x.ItemId }).Distinct().ToList();
            RabbitManager.PublishList(itemMessages, RoutingKeyEnum.ItemUsed);
            List<UnitOfMeasure> UnitOfMeasureMessages = new List<UnitOfMeasure>();
            UnitOfMeasureMessages.AddRange(DirectSalesOrder.DirectSalesOrderContents.Select(x => new UnitOfMeasure { Id = x.PrimaryUnitOfMeasureId }));
            UnitOfMeasureMessages.AddRange(DirectSalesOrder.DirectSalesOrderContents.Select(x => new UnitOfMeasure { Id = x.UnitOfMeasureId }));
            UnitOfMeasureMessages = UnitOfMeasureMessages.Distinct().ToList();
            RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            RabbitManager.PublishSingle(new Store { Id = DirectSalesOrder.BuyerStoreId }, RoutingKeyEnum.StoreUsed);
            RabbitManager.PublishSingle(new StoreUser { Id = DirectSalesOrder.StoreUserCreatorId.Value }, RoutingKeyEnum.StoreUserUsed);
            RabbitManager.PublishSingle(new AppUser { Id = DirectSalesOrder.SaleEmployeeId }, RoutingKeyEnum.AppUserUsed);
        }
    }
}
