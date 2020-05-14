using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace DMS.Repositories
{
    public interface IDirectSalesOrderRepository
    {
        Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<DirectSalesOrder> Get(long Id);
        Task<bool> Create(DirectSalesOrder DirectSalesOrder);
        Task<bool> Update(DirectSalesOrder DirectSalesOrder);
        Task<bool> Delete(DirectSalesOrder DirectSalesOrder);
        Task<bool> BulkMerge(List<DirectSalesOrder> DirectSalesOrders);
        Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
    }
    public class DirectSalesOrderRepository : IDirectSalesOrderRepository
    {
        private DataContext DataContext;
        public DirectSalesOrderRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DirectSalesOrderDAO> DynamicFilter(IQueryable<DirectSalesOrderDAO> query, DirectSalesOrderFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.BuyerStoreId != null)
                query = query.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
            if (filter.StorePhone != null)
                query = query.Where(q => q.StorePhone, filter.StorePhone);
            if (filter.StoreAddress != null)
                query = query.Where(q => q.StoreAddress, filter.StoreAddress);
            if (filter.StoreDeliveryAddress != null)
                query = query.Where(q => q.StoreDeliveryAddress, filter.StoreDeliveryAddress);
            if (filter.TaxCode != null)
                query = query.Where(q => q.TaxCode, filter.TaxCode);
            if (filter.SaleEmployeeId != null)
                query = query.Where(q => q.SaleEmployeeId, filter.SaleEmployeeId);
            if (filter.OrderDate != null)
                query = query.Where(q => q.OrderDate, filter.OrderDate);
            if (filter.DeliveryDate != null)
                query = query.Where(q => q.DeliveryDate, filter.DeliveryDate);
            if (filter.EditedPriceStatusId != null)
                query = query.Where(q => q.EditedPriceStatusId, filter.EditedPriceStatusId);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.SubTotal != null)
                query = query.Where(q => q.SubTotal, filter.SubTotal);
            if (filter.GeneralDiscountPercentage != null)
                query = query.Where(q => q.GeneralDiscountPercentage, filter.GeneralDiscountPercentage);
            if (filter.GeneralDiscountAmount != null)
                query = query.Where(q => q.GeneralDiscountAmount, filter.GeneralDiscountAmount);
            if (filter.TotalTaxAmount != null)
                query = query.Where(q => q.TotalTaxAmount, filter.TotalTaxAmount);
            if (filter.Total != null)
                query = query.Where(q => q.Total, filter.Total);
            if (filter.RequestStateId != null)
                query = query.Where(q => q.RequestStateId, filter.RequestStateId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<DirectSalesOrderDAO> OrFilter(IQueryable<DirectSalesOrderDAO> query, DirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<DirectSalesOrderDAO> initQuery = query.Where(q => false);
            foreach (DirectSalesOrderFilter DirectSalesOrderFilter in filter.OrFilter)
            {
                IQueryable<DirectSalesOrderDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.BuyerStoreId != null)
                    queryable = queryable.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
                if (filter.StorePhone != null)
                    queryable = queryable.Where(q => q.StorePhone, filter.StorePhone);
                if (filter.StoreAddress != null)
                    queryable = queryable.Where(q => q.StoreAddress, filter.StoreAddress);
                if (filter.StoreDeliveryAddress != null)
                    queryable = queryable.Where(q => q.StoreDeliveryAddress, filter.StoreDeliveryAddress);
                if (filter.TaxCode != null)
                    queryable = queryable.Where(q => q.TaxCode, filter.TaxCode);
                if (filter.SaleEmployeeId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, filter.SaleEmployeeId);
                if (filter.OrderDate != null)
                    queryable = queryable.Where(q => q.OrderDate, filter.OrderDate);
                if (filter.DeliveryDate != null)
                    queryable = queryable.Where(q => q.DeliveryDate, filter.DeliveryDate);
                if (filter.EditedPriceStatusId != null)
                    queryable = queryable.Where(q => q.EditedPriceStatusId, filter.EditedPriceStatusId);
                if (filter.Note != null)
                    queryable = queryable.Where(q => q.Note, filter.Note);
                if (filter.SubTotal != null)
                    queryable = queryable.Where(q => q.SubTotal, filter.SubTotal);
                if (filter.GeneralDiscountPercentage != null)
                    queryable = queryable.Where(q => q.GeneralDiscountPercentage, filter.GeneralDiscountPercentage);
                if (filter.GeneralDiscountAmount != null)
                    queryable = queryable.Where(q => q.GeneralDiscountAmount, filter.GeneralDiscountAmount);
                if (filter.TotalTaxAmount != null)
                    queryable = queryable.Where(q => q.TotalTaxAmount, filter.TotalTaxAmount);
                if (filter.Total != null)
                    queryable = queryable.Where(q => q.Total, filter.Total);
                if (filter.RequestStateId != null)
                    queryable = queryable.Where(q => q.RequestStateId, filter.RequestStateId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<DirectSalesOrderDAO> DynamicOrder(IQueryable<DirectSalesOrderDAO> query, DirectSalesOrderFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case DirectSalesOrderOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case DirectSalesOrderOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case DirectSalesOrderOrder.BuyerStore:
                            query = query.OrderBy(q => q.BuyerStoreId);
                            break;
                        case DirectSalesOrderOrder.StorePhone:
                            query = query.OrderBy(q => q.StorePhone);
                            break;
                        case DirectSalesOrderOrder.StoreAddress:
                            query = query.OrderBy(q => q.StoreAddress);
                            break;
                        case DirectSalesOrderOrder.StoreDeliveryAddress:
                            query = query.OrderBy(q => q.StoreDeliveryAddress);
                            break;
                        case DirectSalesOrderOrder.TaxCode:
                            query = query.OrderBy(q => q.TaxCode);
                            break;
                        case DirectSalesOrderOrder.SaleEmployee:
                            query = query.OrderBy(q => q.SaleEmployeeId);
                            break;
                        case DirectSalesOrderOrder.OrderDate:
                            query = query.OrderBy(q => q.OrderDate);
                            break;
                        case DirectSalesOrderOrder.DeliveryDate:
                            query = query.OrderBy(q => q.DeliveryDate);
                            break;
                        case DirectSalesOrderOrder.EditedPriceStatus:
                            query = query.OrderBy(q => q.EditedPriceStatusId);
                            break;
                        case DirectSalesOrderOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case DirectSalesOrderOrder.SubTotal:
                            query = query.OrderBy(q => q.SubTotal);
                            break;
                        case DirectSalesOrderOrder.GeneralDiscountPercentage:
                            query = query.OrderBy(q => q.GeneralDiscountPercentage);
                            break;
                        case DirectSalesOrderOrder.GeneralDiscountAmount:
                            query = query.OrderBy(q => q.GeneralDiscountAmount);
                            break;
                        case DirectSalesOrderOrder.TotalTaxAmount:
                            query = query.OrderBy(q => q.TotalTaxAmount);
                            break;
                        case DirectSalesOrderOrder.Total:
                            query = query.OrderBy(q => q.Total);
                            break;
                        case DirectSalesOrderOrder.RequestState:
                            query = query.OrderBy(q => q.RequestStateId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case DirectSalesOrderOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case DirectSalesOrderOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case DirectSalesOrderOrder.BuyerStore:
                            query = query.OrderByDescending(q => q.BuyerStoreId);
                            break;
                        case DirectSalesOrderOrder.StorePhone:
                            query = query.OrderByDescending(q => q.StorePhone);
                            break;
                        case DirectSalesOrderOrder.StoreAddress:
                            query = query.OrderByDescending(q => q.StoreAddress);
                            break;
                        case DirectSalesOrderOrder.StoreDeliveryAddress:
                            query = query.OrderByDescending(q => q.StoreDeliveryAddress);
                            break;
                        case DirectSalesOrderOrder.TaxCode:
                            query = query.OrderByDescending(q => q.TaxCode);
                            break;
                        case DirectSalesOrderOrder.SaleEmployee:
                            query = query.OrderByDescending(q => q.SaleEmployeeId);
                            break;
                        case DirectSalesOrderOrder.OrderDate:
                            query = query.OrderByDescending(q => q.OrderDate);
                            break;
                        case DirectSalesOrderOrder.DeliveryDate:
                            query = query.OrderByDescending(q => q.DeliveryDate);
                            break;
                        case DirectSalesOrderOrder.EditedPriceStatus:
                            query = query.OrderByDescending(q => q.EditedPriceStatusId);
                            break;
                        case DirectSalesOrderOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case DirectSalesOrderOrder.SubTotal:
                            query = query.OrderByDescending(q => q.SubTotal);
                            break;
                        case DirectSalesOrderOrder.GeneralDiscountPercentage:
                            query = query.OrderByDescending(q => q.GeneralDiscountPercentage);
                            break;
                        case DirectSalesOrderOrder.GeneralDiscountAmount:
                            query = query.OrderByDescending(q => q.GeneralDiscountAmount);
                            break;
                        case DirectSalesOrderOrder.TotalTaxAmount:
                            query = query.OrderByDescending(q => q.TotalTaxAmount);
                            break;
                        case DirectSalesOrderOrder.Total:
                            query = query.OrderByDescending(q => q.Total);
                            break;
                        case DirectSalesOrderOrder.RequestState:
                            query = query.OrderByDescending(q => q.RequestStateId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<DirectSalesOrder>> DynamicSelect(IQueryable<DirectSalesOrderDAO> query, DirectSalesOrderFilter filter)
        {
            List<DirectSalesOrder> DirectSalesOrders = await query.Select(q => new DirectSalesOrder()
            {
                Id = filter.Selects.Contains(DirectSalesOrderSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(DirectSalesOrderSelect.Code) ? q.Code : default(string),
                BuyerStoreId = filter.Selects.Contains(DirectSalesOrderSelect.BuyerStore) ? q.BuyerStoreId : default(long),
                StorePhone = filter.Selects.Contains(DirectSalesOrderSelect.StorePhone) ? q.StorePhone : default(string),
                StoreAddress = filter.Selects.Contains(DirectSalesOrderSelect.StoreAddress) ? q.StoreAddress : default(string),
                StoreDeliveryAddress = filter.Selects.Contains(DirectSalesOrderSelect.StoreDeliveryAddress) ? q.StoreDeliveryAddress : default(string),
                TaxCode = filter.Selects.Contains(DirectSalesOrderSelect.TaxCode) ? q.TaxCode : default(string),
                SaleEmployeeId = filter.Selects.Contains(DirectSalesOrderSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                OrderDate = filter.Selects.Contains(DirectSalesOrderSelect.OrderDate) ? q.OrderDate : default(DateTime),
                DeliveryDate = filter.Selects.Contains(DirectSalesOrderSelect.DeliveryDate) ? q.DeliveryDate : default(DateTime?),
                EditedPriceStatusId = filter.Selects.Contains(DirectSalesOrderSelect.EditedPriceStatus) ? q.EditedPriceStatusId : default(long),
                Note = filter.Selects.Contains(DirectSalesOrderSelect.Note) ? q.Note : default(string),
                SubTotal = filter.Selects.Contains(DirectSalesOrderSelect.SubTotal) ? q.SubTotal : default(long),
                GeneralDiscountPercentage = filter.Selects.Contains(DirectSalesOrderSelect.GeneralDiscountPercentage) ? q.GeneralDiscountPercentage : default(decimal?),
                GeneralDiscountAmount = filter.Selects.Contains(DirectSalesOrderSelect.GeneralDiscountAmount) ? q.GeneralDiscountAmount : default(long?),
                TotalTaxAmount = filter.Selects.Contains(DirectSalesOrderSelect.TotalTaxAmount) ? q.TotalTaxAmount : default(long),
                Total = filter.Selects.Contains(DirectSalesOrderSelect.Total) ? q.Total : default(long),
                RequestStateId = filter.Selects.Contains(DirectSalesOrderSelect.RequestState) ? q.RequestStateId : default(long),
                BuyerStore = filter.Selects.Contains(DirectSalesOrderSelect.BuyerStore) && q.BuyerStore != null ? new Store
                {
                    Id = q.BuyerStore.Id,
                    Code = q.BuyerStore.Code,
                    Name = q.BuyerStore.Name,
                    ParentStoreId = q.BuyerStore.ParentStoreId,
                    OrganizationId = q.BuyerStore.OrganizationId,
                    StoreTypeId = q.BuyerStore.StoreTypeId,
                    StoreGroupingId = q.BuyerStore.StoreGroupingId,
                    ResellerId = q.BuyerStore.ResellerId,
                    Telephone = q.BuyerStore.Telephone,
                    ProvinceId = q.BuyerStore.ProvinceId,
                    DistrictId = q.BuyerStore.DistrictId,
                    WardId = q.BuyerStore.WardId,
                    Address = q.BuyerStore.Address,
                    DeliveryAddress = q.BuyerStore.DeliveryAddress,
                    Latitude = q.BuyerStore.Latitude,
                    Longitude = q.BuyerStore.Longitude,
                    DeliveryLatitude = q.BuyerStore.DeliveryLatitude,
                    DeliveryLongitude = q.BuyerStore.DeliveryLongitude,
                    OwnerName = q.BuyerStore.OwnerName,
                    OwnerPhone = q.BuyerStore.OwnerPhone,
                    OwnerEmail = q.BuyerStore.OwnerEmail,
                    StatusId = q.BuyerStore.StatusId,
                } : null,
                EditedPriceStatus = filter.Selects.Contains(DirectSalesOrderSelect.EditedPriceStatus) && q.EditedPriceStatus != null ? new EditedPriceStatus
                {
                    Id = q.EditedPriceStatus.Id,
                    Code = q.EditedPriceStatus.Code,
                    Name = q.EditedPriceStatus.Name,
                } : null,
                RequestState = filter.Selects.Contains(DirectSalesOrderSelect.RequestState) && q.RequestState != null ? new RequestState
                {
                    Id = q.RequestState.Id,
                    Code = q.RequestState.Code,
                    Name = q.RequestState.Name,
                } : null,
                SaleEmployee = filter.Selects.Contains(DirectSalesOrderSelect.SaleEmployee) && q.SaleEmployee != null ? new AppUser
                {
                    Id = q.SaleEmployee.Id,
                    Username = q.SaleEmployee.Username,
                    DisplayName = q.SaleEmployee.DisplayName,
                    Address = q.SaleEmployee.Address,
                    Email = q.SaleEmployee.Email,
                    Phone = q.SaleEmployee.Phone,
                } : null,
            }).ToListAsync();
            return DirectSalesOrders;
        }

        public async Task<int> Count(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrders = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrders = DynamicFilter(DirectSalesOrders, filter);
            return await DirectSalesOrders.CountAsync();
        }

        public async Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = DynamicOrder(DirectSalesOrderDAOs, filter);
            List<DirectSalesOrder> DirectSalesOrders = await DynamicSelect(DirectSalesOrderDAOs, filter);
            return DirectSalesOrders;
        }

        public async Task<DirectSalesOrder> Get(long Id)
        {
            DirectSalesOrder DirectSalesOrder = await DataContext.DirectSalesOrder.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new DirectSalesOrder()
            {
                Id = x.Id,
                Code = x.Code,
                BuyerStoreId = x.BuyerStoreId,
                StorePhone = x.StorePhone,
                StoreAddress = x.StoreAddress,
                StoreDeliveryAddress = x.StoreDeliveryAddress,
                TaxCode = x.TaxCode,
                SaleEmployeeId = x.SaleEmployeeId,
                OrderDate = x.OrderDate,
                DeliveryDate = x.DeliveryDate,
                EditedPriceStatusId = x.EditedPriceStatusId,
                Note = x.Note,
                SubTotal = x.SubTotal,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                TotalTaxAmount = x.TotalTaxAmount,
                Total = x.Total,
                RequestStateId = x.RequestStateId,
                BuyerStore = x.BuyerStore == null ? null : new Store
                {
                    Id = x.BuyerStore.Id,
                    Code = x.BuyerStore.Code,
                    Name = x.BuyerStore.Name,
                    ParentStoreId = x.BuyerStore.ParentStoreId,
                    OrganizationId = x.BuyerStore.OrganizationId,
                    StoreTypeId = x.BuyerStore.StoreTypeId,
                    StoreGroupingId = x.BuyerStore.StoreGroupingId,
                    ResellerId = x.BuyerStore.ResellerId,
                    Telephone = x.BuyerStore.Telephone,
                    ProvinceId = x.BuyerStore.ProvinceId,
                    DistrictId = x.BuyerStore.DistrictId,
                    WardId = x.BuyerStore.WardId,
                    Address = x.BuyerStore.Address,
                    DeliveryAddress = x.BuyerStore.DeliveryAddress,
                    Latitude = x.BuyerStore.Latitude,
                    Longitude = x.BuyerStore.Longitude,
                    DeliveryLatitude = x.BuyerStore.DeliveryLatitude,
                    DeliveryLongitude = x.BuyerStore.DeliveryLongitude,
                    OwnerName = x.BuyerStore.OwnerName,
                    OwnerPhone = x.BuyerStore.OwnerPhone,
                    OwnerEmail = x.BuyerStore.OwnerEmail,
                    StatusId = x.BuyerStore.StatusId,
                },
                EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                {
                    Id = x.EditedPriceStatus.Id,
                    Code = x.EditedPriceStatus.Code,
                    Name = x.EditedPriceStatus.Name,
                },
                RequestState = x.RequestState == null ? null : new RequestState
                {
                    Id = x.RequestState.Id,
                    Code = x.RequestState.Code,
                    Name = x.RequestState.Name,
                },
                SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                {
                    Id = x.SaleEmployee.Id,
                    Username = x.SaleEmployee.Username,
                    DisplayName = x.SaleEmployee.DisplayName,
                    Address = x.SaleEmployee.Address,
                    Email = x.SaleEmployee.Email,
                    Phone = x.SaleEmployee.Phone,
                },
            }).FirstOrDefaultAsync();

            if (DirectSalesOrder == null)
                return null;

            if (DirectSalesOrder == null)
                return null;
            DirectSalesOrder.DirectSalesOrderContents = await DataContext.DirectSalesOrderContent.AsNoTracking()
                .Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id)
                .Select(x => new DirectSalesOrderContent
                {
                    Id = x.Id,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    Price = x.Price,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    Factor = x.Factor,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                        Product = new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            TaxType = new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                Name = x.Item.Product.TaxType.Name,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                StatusId = x.Item.Product.UnitOfMeasureGrouping.StatusId,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId
                            }
                        }
                    },
                    PrimaryUnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToListAsync();
            DirectSalesOrder.DirectSalesOrderPromotions = await DataContext.DirectSalesOrderPromotion.AsNoTracking()
                .Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id)
                .Select(x => new DirectSalesOrderPromotion
                {
                    Id = x.Id,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    Note = x.Note,
                    Factor = x.Factor,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                        Product = new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            TaxType = new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                Name = x.Item.Product.TaxType.Name,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                StatusId = x.Item.Product.UnitOfMeasureGrouping.StatusId,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId
                            }
                        }
                    },
                    PrimaryUnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToListAsync();
            return DirectSalesOrder;
        }
        public async Task<bool> Create(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderDAO DirectSalesOrderDAO = new DirectSalesOrderDAO();
            DirectSalesOrderDAO.Id = DirectSalesOrder.Id;
            DirectSalesOrderDAO.Code = DirectSalesOrder.Code;
            DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
            DirectSalesOrderDAO.StorePhone = DirectSalesOrder.StorePhone;
            DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
            DirectSalesOrderDAO.StoreDeliveryAddress = DirectSalesOrder.StoreDeliveryAddress;
            DirectSalesOrderDAO.TaxCode = DirectSalesOrder.TaxCode;
            DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
            DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
            DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
            DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
            DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
            DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
            DataContext.DirectSalesOrder.Add(DirectSalesOrderDAO);
            await DataContext.SaveChangesAsync();
            DirectSalesOrder.Id = DirectSalesOrderDAO.Id;
            await SaveReference(DirectSalesOrder);
            return true;
        }

        public async Task<bool> Update(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderDAO DirectSalesOrderDAO = DataContext.DirectSalesOrder.Where(x => x.Id == DirectSalesOrder.Id).FirstOrDefault();
            if (DirectSalesOrderDAO == null)
                return false;
            DirectSalesOrderDAO.Id = DirectSalesOrder.Id;
            DirectSalesOrderDAO.Code = DirectSalesOrder.Code;
            DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
            DirectSalesOrderDAO.StorePhone = DirectSalesOrder.StorePhone;
            DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
            DirectSalesOrderDAO.StoreDeliveryAddress = DirectSalesOrder.StoreDeliveryAddress;
            DirectSalesOrderDAO.TaxCode = DirectSalesOrder.TaxCode;
            DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
            DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
            DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
            DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
            DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
            DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
            await DataContext.SaveChangesAsync();
            await SaveReference(DirectSalesOrder);
            return true;
        }

        public async Task<bool> Delete(DirectSalesOrder DirectSalesOrder)
        {
            await DataContext.DirectSalesOrder.Where(x => x.Id == DirectSalesOrder.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<DirectSalesOrder> DirectSalesOrders)
        {
            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = new List<DirectSalesOrderDAO>();
            foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
            {
                DirectSalesOrderDAO DirectSalesOrderDAO = new DirectSalesOrderDAO();
                DirectSalesOrderDAO.Id = DirectSalesOrder.Id;
                DirectSalesOrderDAO.Code = DirectSalesOrder.Code;
                DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
                DirectSalesOrderDAO.StorePhone = DirectSalesOrder.StorePhone;
                DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
                DirectSalesOrderDAO.StoreDeliveryAddress = DirectSalesOrder.StoreDeliveryAddress;
                DirectSalesOrderDAO.TaxCode = DirectSalesOrder.TaxCode;
                DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
                DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
                DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
                DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
                DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
                DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
                DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
                DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
                DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
                DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
                DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
                DirectSalesOrderDAOs.Add(DirectSalesOrderDAO);
            }
            await DataContext.BulkMergeAsync(DirectSalesOrderDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders)
        {
            List<long> Ids = DirectSalesOrders.Select(x => x.Id).ToList();
            await DataContext.DirectSalesOrder
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(DirectSalesOrder DirectSalesOrder)
        {
        }
        
    }
}
