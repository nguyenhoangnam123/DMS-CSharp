using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Enums;
using DMS.ABE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderValidator : IServiceScoped
    {
        Task<bool> Create(DirectSalesOrder DirectSalesOrder);
        Task<bool> Update(DirectSalesOrder DirectSalesOrder);
        Task<bool> Approve(DirectSalesOrder DirectSalesOrder);
    }

    public class DirectSalesOrderValidator : IDirectSalesOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            BuyerStoreNotExisted,
            StoreUserCreatorEmpty,
            CreatorNotExisted,
            SellerStoreNotExisted,
            BuyerStoreEmpty,
            SellerStoreEmpty,
            SaleEmployeeNotExisted,
            SaleEmployeeEmpty,
            OrderDateEmpty,
            EditedPriceStatusNotExisted,
            PriceOutOfRange,
            UnitOfMeasureEmpty,
            UnitOfMeasureNotExisted,
            QuantityEmpty,
            ItemNotExisted,
            QuantityInvalid,
            SellerStoreEqualBuyerStore,
            ContentEmpty,
            DeliveryDateInvalid,
            BuyerStoreNotInERouteScope,
            SaleEmployeeNotInOrg,
            PromotionCodeNotExisted,
            PromotionCodeHasUsed,
            OrganizationInvalid,
            StoreInvalid,
            ItemInvalid,
            RequestStateInvalid,
        }
        private IUOW UOW;
        private ICurrentContext CurrentContext;


        public DirectSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrder.Id },
                Selects = DirectSalesOrderSelect.Id
            };

            int count = await UOW.DirectSalesOrderRepository.Count(DirectSalesOrderFilter);
            if (count == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }


        private async Task<bool> ValidateStore(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.BuyerStoreId == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreEmpty);
            else
            {
                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { In = new List<long> { DirectSalesOrder.BuyerStoreId } },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = StoreSelect.Id
                };

                int count = await UOW.StoreRepository.Count(StoreFilter);
                if (count == 0)
                    DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreNotExisted);
            }

            return DirectSalesOrder.IsValidated;
        } // validate cua hang mua co ton tai khong

        private async Task<bool> ValidateEmployee(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.SaleEmployeeId == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeEmpty);

            return DirectSalesOrder.IsValidated;
        } // validate nhân viên phụ trách

        private async Task<bool> ValidateOrderDate(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.OrderDate == default(DateTime))
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateDeliveryDate(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.DeliveryDate.HasValue)
            {
                if (DirectSalesOrder.DeliveryDate.Value < DirectSalesOrder.OrderDate)
                {
                    DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.DeliveryDate), ErrorCode.DeliveryDateInvalid);
                }
            }
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEditedPrice(DirectSalesOrder DirectSalesOrder)
        {
            if (EditedPriceStatusEnum.ACTIVE.Id != DirectSalesOrder.EditedPriceStatusId && EditedPriceStatusEnum.INACTIVE.Id != DirectSalesOrder.EditedPriceStatusId)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.EditedPriceStatus), ErrorCode.EditedPriceStatusNotExisted);
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateContent(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.DirectSalesOrderContents == null)
            {
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.Id), ErrorCode.ContentEmpty);
                return DirectSalesOrder.IsValidated;
            }

            var ItemIds = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList();
            List<Item> Items = await UOW.ItemRepository.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.Id | ItemSelect.SalePrice | ItemSelect.ProductId
            });

            var ProductIds = Items.Select(x => x.ProductId).ToList();
            var Products = await UOW.ProductRepository.List(new ProductFilter
            {
                Id = new IdFilter { In = ProductIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.UnitOfMeasure | ProductSelect.UnitOfMeasureGrouping | ProductSelect.Id
            });

            var UOMGs = await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.Id | UnitOfMeasureGroupingSelect.UnitOfMeasure | UnitOfMeasureGroupingSelect.UnitOfMeasureGroupingContents
            });

            foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
            {
                var Item = Items.Where(x => x.Id == DirectSalesOrderContent.ItemId).FirstOrDefault();
                if (Item == null)
                {
                    DirectSalesOrderContent.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Item), ErrorCode.ItemNotExisted);
                } // validate Item

                var Product = Products.Where(x => Item.ProductId == x.Id).FirstOrDefault();

                UnitOfMeasure UOM = new UnitOfMeasure
                {
                    Id = Product.UnitOfMeasure.Id,
                    Code = Product.UnitOfMeasure.Code,
                    Name = Product.UnitOfMeasure.Name,
                    Description = Product.UnitOfMeasure.Description,
                    StatusId = Product.UnitOfMeasure.StatusId,
                    Factor = 1
                };

                if (UOM.Id == 0)
                    DirectSalesOrderContent.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);

                if (DirectSalesOrderContent.Quantity <= 0)
                    DirectSalesOrderContent.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Quantity), ErrorCode.QuantityEmpty);
            }

            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateRequestState(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.RequestStateId != RequestStateEnum.APPROVED.Id)
            {
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.RequestStateId), ErrorCode.RequestStateInvalid);
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Create(DirectSalesOrder DirectSalesOrder)
        {
            await ValidateStore(DirectSalesOrder);
            await ValidateEmployee(DirectSalesOrder);
            await ValidateDeliveryDate(DirectSalesOrder);
            await ValidateContent(DirectSalesOrder);
            await ValidateEditedPrice(DirectSalesOrder);
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
                await ValidateRequestState(DirectSalesOrder);// đơn hàng phải ở trạng thái phê duyệt wf
                await ValidateStore(DirectSalesOrder);
                await ValidateEmployee(DirectSalesOrder);
                await ValidateOrderDate(DirectSalesOrder);
                await ValidateDeliveryDate(DirectSalesOrder);
                //await ValidateContent(DirectSalesOrder);
                await ValidateEditedPrice(DirectSalesOrder);
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Approve(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
                await ValidateRequestState(DirectSalesOrder);// đơn hàng phải ở trạng thái phê duyệt wf
            }
            return DirectSalesOrder.IsValidated;
        }
    }
}
