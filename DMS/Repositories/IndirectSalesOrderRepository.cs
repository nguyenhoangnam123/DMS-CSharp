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
    public interface IIndirectSalesOrderRepository
    {
        Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<IndirectSalesOrder> Get(long Id);
        Task<bool> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> BulkMerge(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
    }
    public class IndirectSalesOrderRepository : IIndirectSalesOrderRepository
    {
        private DataContext DataContext;
        public IndirectSalesOrderRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<IndirectSalesOrderDAO> DynamicFilter(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.BuyerStoreId != null)
                query = query.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
            if (filter.PhoneNumber != null)
                query = query.Where(q => q.PhoneNumber, filter.PhoneNumber);
            if (filter.StoreAddress != null)
                query = query.Where(q => q.StoreAddress, filter.StoreAddress);
            if (filter.DeliveryAddress != null)
                query = query.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
            if (filter.SellerStoreId != null)
                query = query.Where(q => q.SellerStoreId, filter.SellerStoreId);
            if (filter.SaleEmployeeId != null)
                query = query.Where(q => q.SaleEmployeeId, filter.SaleEmployeeId);
            if (filter.OrderDate != null)
                query = query.Where(q => q.OrderDate, filter.OrderDate);
            if (filter.DeliveryDate != null)
                query = query.Where(q => q.DeliveryDate, filter.DeliveryDate);
            if (filter.IndirectSalesOrderStatusId != null)
                query = query.Where(q => q.IndirectSalesOrderStatusId, filter.IndirectSalesOrderStatusId);
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
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<IndirectSalesOrderDAO> OrFilter(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<IndirectSalesOrderDAO> initQuery = query.Where(q => false);
            foreach (IndirectSalesOrderFilter IndirectSalesOrderFilter in filter.OrFilter)
            {
                IQueryable<IndirectSalesOrderDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.BuyerStoreId != null)
                    queryable = queryable.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
                if (filter.PhoneNumber != null)
                    queryable = queryable.Where(q => q.PhoneNumber, filter.PhoneNumber);
                if (filter.StoreAddress != null)
                    queryable = queryable.Where(q => q.StoreAddress, filter.StoreAddress);
                if (filter.DeliveryAddress != null)
                    queryable = queryable.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
                if (filter.SellerStoreId != null)
                    queryable = queryable.Where(q => q.SellerStoreId, filter.SellerStoreId);
                if (filter.SaleEmployeeId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, filter.SaleEmployeeId);
                if (filter.OrderDate != null)
                    queryable = queryable.Where(q => q.OrderDate, filter.OrderDate);
                if (filter.DeliveryDate != null)
                    queryable = queryable.Where(q => q.DeliveryDate, filter.DeliveryDate);
                if (filter.IndirectSalesOrderStatusId != null)
                    queryable = queryable.Where(q => q.IndirectSalesOrderStatusId, filter.IndirectSalesOrderStatusId);
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
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<IndirectSalesOrderDAO> DynamicOrder(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case IndirectSalesOrderOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case IndirectSalesOrderOrder.BuyerStore:
                            query = query.OrderBy(q => q.BuyerStoreId);
                            break;
                        case IndirectSalesOrderOrder.PhoneNumber:
                            query = query.OrderBy(q => q.PhoneNumber);
                            break;
                        case IndirectSalesOrderOrder.StoreAddress:
                            query = query.OrderBy(q => q.StoreAddress);
                            break;
                        case IndirectSalesOrderOrder.DeliveryAddress:
                            query = query.OrderBy(q => q.DeliveryAddress);
                            break;
                        case IndirectSalesOrderOrder.SellerStore:
                            query = query.OrderBy(q => q.SellerStoreId);
                            break;
                        case IndirectSalesOrderOrder.SaleEmployee:
                            query = query.OrderBy(q => q.SaleEmployeeId);
                            break;
                        case IndirectSalesOrderOrder.OrderDate:
                            query = query.OrderBy(q => q.OrderDate);
                            break;
                        case IndirectSalesOrderOrder.DeliveryDate:
                            query = query.OrderBy(q => q.DeliveryDate);
                            break;
                        case IndirectSalesOrderOrder.IndirectSalesOrderStatus:
                            query = query.OrderBy(q => q.IndirectSalesOrderStatusId);
                            break;
                        case IndirectSalesOrderOrder.IsEditedPrice:
                            query = query.OrderBy(q => q.IsEditedPrice);
                            break;
                        case IndirectSalesOrderOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case IndirectSalesOrderOrder.SubTotal:
                            query = query.OrderBy(q => q.SubTotal);
                            break;
                        case IndirectSalesOrderOrder.GeneralDiscountPercentage:
                            query = query.OrderBy(q => q.GeneralDiscountPercentage);
                            break;
                        case IndirectSalesOrderOrder.GeneralDiscountAmount:
                            query = query.OrderBy(q => q.GeneralDiscountAmount);
                            break;
                        case IndirectSalesOrderOrder.TotalTaxAmount:
                            query = query.OrderBy(q => q.TotalTaxAmount);
                            break;
                        case IndirectSalesOrderOrder.Total:
                            query = query.OrderBy(q => q.Total);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case IndirectSalesOrderOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case IndirectSalesOrderOrder.BuyerStore:
                            query = query.OrderByDescending(q => q.BuyerStoreId);
                            break;
                        case IndirectSalesOrderOrder.PhoneNumber:
                            query = query.OrderByDescending(q => q.PhoneNumber);
                            break;
                        case IndirectSalesOrderOrder.StoreAddress:
                            query = query.OrderByDescending(q => q.StoreAddress);
                            break;
                        case IndirectSalesOrderOrder.DeliveryAddress:
                            query = query.OrderByDescending(q => q.DeliveryAddress);
                            break;
                        case IndirectSalesOrderOrder.SellerStore:
                            query = query.OrderByDescending(q => q.SellerStoreId);
                            break;
                        case IndirectSalesOrderOrder.SaleEmployee:
                            query = query.OrderByDescending(q => q.SaleEmployeeId);
                            break;
                        case IndirectSalesOrderOrder.OrderDate:
                            query = query.OrderByDescending(q => q.OrderDate);
                            break;
                        case IndirectSalesOrderOrder.DeliveryDate:
                            query = query.OrderByDescending(q => q.DeliveryDate);
                            break;
                        case IndirectSalesOrderOrder.IndirectSalesOrderStatus:
                            query = query.OrderByDescending(q => q.IndirectSalesOrderStatusId);
                            break;
                        case IndirectSalesOrderOrder.IsEditedPrice:
                            query = query.OrderByDescending(q => q.IsEditedPrice);
                            break;
                        case IndirectSalesOrderOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case IndirectSalesOrderOrder.SubTotal:
                            query = query.OrderByDescending(q => q.SubTotal);
                            break;
                        case IndirectSalesOrderOrder.GeneralDiscountPercentage:
                            query = query.OrderByDescending(q => q.GeneralDiscountPercentage);
                            break;
                        case IndirectSalesOrderOrder.GeneralDiscountAmount:
                            query = query.OrderByDescending(q => q.GeneralDiscountAmount);
                            break;
                        case IndirectSalesOrderOrder.TotalTaxAmount:
                            query = query.OrderByDescending(q => q.TotalTaxAmount);
                            break;
                        case IndirectSalesOrderOrder.Total:
                            query = query.OrderByDescending(q => q.Total);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<IndirectSalesOrder>> DynamicSelect(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            List<IndirectSalesOrder> IndirectSalesOrders = await query.Select(q => new IndirectSalesOrder()
            {
                Id = filter.Selects.Contains(IndirectSalesOrderSelect.Id) ? q.Id : default(long),
                BuyerStoreId = filter.Selects.Contains(IndirectSalesOrderSelect.BuyerStore) ? q.BuyerStoreId : default(long),
                PhoneNumber = filter.Selects.Contains(IndirectSalesOrderSelect.PhoneNumber) ? q.PhoneNumber : default(string),
                StoreAddress = filter.Selects.Contains(IndirectSalesOrderSelect.StoreAddress) ? q.StoreAddress : default(string),
                DeliveryAddress = filter.Selects.Contains(IndirectSalesOrderSelect.DeliveryAddress) ? q.DeliveryAddress : default(string),
                SellerStoreId = filter.Selects.Contains(IndirectSalesOrderSelect.SellerStore) ? q.SellerStoreId : default(long),
                SaleEmployeeId = filter.Selects.Contains(IndirectSalesOrderSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                OrderDate = filter.Selects.Contains(IndirectSalesOrderSelect.OrderDate) ? q.OrderDate : default(DateTime),
                DeliveryDate = filter.Selects.Contains(IndirectSalesOrderSelect.DeliveryDate) ? q.DeliveryDate : default(DateTime?),
                IndirectSalesOrderStatusId = filter.Selects.Contains(IndirectSalesOrderSelect.IndirectSalesOrderStatus) ? q.IndirectSalesOrderStatusId : default(long),
                IsEditedPrice = filter.Selects.Contains(IndirectSalesOrderSelect.IsEditedPrice) ? q.IsEditedPrice : default(bool),
                Note = filter.Selects.Contains(IndirectSalesOrderSelect.Note) ? q.Note : default(string),
                SubTotal = filter.Selects.Contains(IndirectSalesOrderSelect.SubTotal) ? q.SubTotal : default(long),
                GeneralDiscountPercentage = filter.Selects.Contains(IndirectSalesOrderSelect.GeneralDiscountPercentage) ? q.GeneralDiscountPercentage : default(long?),
                GeneralDiscountAmount = filter.Selects.Contains(IndirectSalesOrderSelect.GeneralDiscountAmount) ? q.GeneralDiscountAmount : default(long?),
                TotalTaxAmount = filter.Selects.Contains(IndirectSalesOrderSelect.TotalTaxAmount) ? q.TotalTaxAmount : default(long),
                Total = filter.Selects.Contains(IndirectSalesOrderSelect.Total) ? q.Total : default(long),
                BuyerStore = filter.Selects.Contains(IndirectSalesOrderSelect.BuyerStore) && q.BuyerStore != null ? new Store
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
                    StoreStatusId = q.BuyerStore.StoreStatusId,
                    StatusId = q.BuyerStore.StatusId,
                    WorkflowDefinitionId = q.BuyerStore.WorkflowDefinitionId,
                    RequestStateId = q.BuyerStore.RequestStateId,
                } : null,
                IndirectSalesOrderStatus = filter.Selects.Contains(IndirectSalesOrderSelect.IndirectSalesOrderStatus) && q.IndirectSalesOrderStatus != null ? new IndirectSalesOrderStatus
                {
                    Id = q.IndirectSalesOrderStatus.Id,
                    Code = q.IndirectSalesOrderStatus.Code,
                    Name = q.IndirectSalesOrderStatus.Name,
                } : null,
                SaleEmployee = filter.Selects.Contains(IndirectSalesOrderSelect.SaleEmployee) && q.SaleEmployee != null ? new AppUser
                {
                    Id = q.SaleEmployee.Id,
                    Username = q.SaleEmployee.Username,
                    Password = q.SaleEmployee.Password,
                    DisplayName = q.SaleEmployee.DisplayName,
                    Address = q.SaleEmployee.Address,
                    Email = q.SaleEmployee.Email,
                    Phone = q.SaleEmployee.Phone,
                    Position = q.SaleEmployee.Position,
                    Department = q.SaleEmployee.Department,
                    OrganizationId = q.SaleEmployee.OrganizationId,
                    SexId = q.SaleEmployee.SexId,
                    StatusId = q.SaleEmployee.StatusId,
                    Avatar = q.SaleEmployee.Avatar,
                    Birthday = q.SaleEmployee.Birthday,
                    RowId = q.SaleEmployee.RowId,
                    //ProvinceId = q.SaleEmployee.ProvinceId,
                } : null,
                SellerStore = filter.Selects.Contains(IndirectSalesOrderSelect.SellerStore) && q.SellerStore != null ? new Store
                {
                    Id = q.SellerStore.Id,
                    Code = q.SellerStore.Code,
                    Name = q.SellerStore.Name,
                    ParentStoreId = q.SellerStore.ParentStoreId,
                    OrganizationId = q.SellerStore.OrganizationId,
                    StoreTypeId = q.SellerStore.StoreTypeId,
                    StoreGroupingId = q.SellerStore.StoreGroupingId,
                    ResellerId = q.SellerStore.ResellerId,
                    Telephone = q.SellerStore.Telephone,
                    ProvinceId = q.SellerStore.ProvinceId,
                    DistrictId = q.SellerStore.DistrictId,
                    WardId = q.SellerStore.WardId,
                    Address = q.SellerStore.Address,
                    DeliveryAddress = q.SellerStore.DeliveryAddress,
                    Latitude = q.SellerStore.Latitude,
                    Longitude = q.SellerStore.Longitude,
                    DeliveryLatitude = q.SellerStore.DeliveryLatitude,
                    DeliveryLongitude = q.SellerStore.DeliveryLongitude,
                    OwnerName = q.SellerStore.OwnerName,
                    OwnerPhone = q.SellerStore.OwnerPhone,
                    OwnerEmail = q.SellerStore.OwnerEmail,
                    StoreStatusId = q.SellerStore.StoreStatusId,
                    StatusId = q.SellerStore.StatusId,
                    WorkflowDefinitionId = q.SellerStore.WorkflowDefinitionId,
                    RequestStateId = q.SellerStore.RequestStateId,
                } : null,
            }).ToListAsync();
            return IndirectSalesOrders;
        }

        public async Task<int> Count(IndirectSalesOrderFilter filter)
        {
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrders = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrders = DynamicFilter(IndirectSalesOrders, filter);
            return await IndirectSalesOrders.CountAsync();
        }

        public async Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrder>();
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = DynamicFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = DynamicOrder(IndirectSalesOrderDAOs, filter);
            List<IndirectSalesOrder> IndirectSalesOrders = await DynamicSelect(IndirectSalesOrderDAOs, filter);
            return IndirectSalesOrders;
        }

        public async Task<IndirectSalesOrder> Get(long Id)
        {
            IndirectSalesOrder IndirectSalesOrder = await DataContext.IndirectSalesOrder.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new IndirectSalesOrder()
            {
                Id = x.Id,
                BuyerStoreId = x.BuyerStoreId,
                PhoneNumber = x.PhoneNumber,
                StoreAddress = x.StoreAddress,
                DeliveryAddress = x.DeliveryAddress,
                SellerStoreId = x.SellerStoreId,
                SaleEmployeeId = x.SaleEmployeeId,
                OrderDate = x.OrderDate,
                DeliveryDate = x.DeliveryDate,
                IndirectSalesOrderStatusId = x.IndirectSalesOrderStatusId,
                IsEditedPrice = x.IsEditedPrice,
                Note = x.Note,
                SubTotal = x.SubTotal,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                TotalTaxAmount = x.TotalTaxAmount,
                Total = x.Total,
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
                    StoreStatusId = x.BuyerStore.StoreStatusId,
                    StatusId = x.BuyerStore.StatusId,
                    WorkflowDefinitionId = x.BuyerStore.WorkflowDefinitionId,
                    RequestStateId = x.BuyerStore.RequestStateId,
                },
                IndirectSalesOrderStatus = x.IndirectSalesOrderStatus == null ? null : new IndirectSalesOrderStatus
                {
                    Id = x.IndirectSalesOrderStatus.Id,
                    Code = x.IndirectSalesOrderStatus.Code,
                    Name = x.IndirectSalesOrderStatus.Name,
                },
                SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                {
                    Id = x.SaleEmployee.Id,
                    Username = x.SaleEmployee.Username,
                    Password = x.SaleEmployee.Password,
                    DisplayName = x.SaleEmployee.DisplayName,
                    Address = x.SaleEmployee.Address,
                    Email = x.SaleEmployee.Email,
                    Phone = x.SaleEmployee.Phone,
                    Position = x.SaleEmployee.Position,
                    Department = x.SaleEmployee.Department,
                    OrganizationId = x.SaleEmployee.OrganizationId,
                    SexId = x.SaleEmployee.SexId,
                    StatusId = x.SaleEmployee.StatusId,
                    Avatar = x.SaleEmployee.Avatar,
                    Birthday = x.SaleEmployee.Birthday,
                    RowId = x.SaleEmployee.RowId,
                    //ProvinceId = x.SaleEmployee.ProvinceId,
                },
                SellerStore = x.SellerStore == null ? null : new Store
                {
                    Id = x.SellerStore.Id,
                    Code = x.SellerStore.Code,
                    Name = x.SellerStore.Name,
                    ParentStoreId = x.SellerStore.ParentStoreId,
                    OrganizationId = x.SellerStore.OrganizationId,
                    StoreTypeId = x.SellerStore.StoreTypeId,
                    StoreGroupingId = x.SellerStore.StoreGroupingId,
                    ResellerId = x.SellerStore.ResellerId,
                    Telephone = x.SellerStore.Telephone,
                    ProvinceId = x.SellerStore.ProvinceId,
                    DistrictId = x.SellerStore.DistrictId,
                    WardId = x.SellerStore.WardId,
                    Address = x.SellerStore.Address,
                    DeliveryAddress = x.SellerStore.DeliveryAddress,
                    Latitude = x.SellerStore.Latitude,
                    Longitude = x.SellerStore.Longitude,
                    DeliveryLatitude = x.SellerStore.DeliveryLatitude,
                    DeliveryLongitude = x.SellerStore.DeliveryLongitude,
                    OwnerName = x.SellerStore.OwnerName,
                    OwnerPhone = x.SellerStore.OwnerPhone,
                    OwnerEmail = x.SellerStore.OwnerEmail,
                    StoreStatusId = x.SellerStore.StoreStatusId,
                    StatusId = x.SellerStore.StatusId,
                    WorkflowDefinitionId = x.SellerStore.WorkflowDefinitionId,
                    RequestStateId = x.SellerStore.RequestStateId,
                },
            }).FirstOrDefaultAsync();

            if (IndirectSalesOrder == null)
                return null;
            IndirectSalesOrder.IndirectSalesOrderContents = await DataContext.IndirectSalesOrderContent.AsNoTracking()
                .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                .Select(x => new IndirectSalesOrderContent
                {
                    Id = x.Id,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    SalePrice = x.SalePrice,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    IndirectSalesOrderNavigation = new Item
                    {
                        Id = x.IndirectSalesOrderNavigation.Id,
                        ProductId = x.IndirectSalesOrderNavigation.ProductId,
                        Code = x.IndirectSalesOrderNavigation.Code,
                        Name = x.IndirectSalesOrderNavigation.Name,
                        ScanCode = x.IndirectSalesOrderNavigation.ScanCode,
                        SalePrice = x.IndirectSalesOrderNavigation.SalePrice,
                        RetailPrice = x.IndirectSalesOrderNavigation.RetailPrice,
                        StatusId = x.IndirectSalesOrderNavigation.StatusId,
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
            IndirectSalesOrder.IndirectSalesOrderPromotions = await DataContext.IndirectSalesOrderPromotion.AsNoTracking()
                .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                .Select(x => new IndirectSalesOrderPromotion
                {
                    Id = x.Id,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    Note = x.Note,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
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

            return IndirectSalesOrder;
        }
        public async Task<bool> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderDAO IndirectSalesOrderDAO = new IndirectSalesOrderDAO();
            IndirectSalesOrderDAO.Id = IndirectSalesOrder.Id;
            IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
            IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
            IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            IndirectSalesOrderDAO.IndirectSalesOrderStatusId = IndirectSalesOrder.IndirectSalesOrderStatusId;
            IndirectSalesOrderDAO.IsEditedPrice = IndirectSalesOrder.IsEditedPrice;
            IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
            IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
            IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            IndirectSalesOrderDAO.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
            IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
            DataContext.IndirectSalesOrder.Add(IndirectSalesOrderDAO);
            await DataContext.SaveChangesAsync();
            IndirectSalesOrder.Id = IndirectSalesOrderDAO.Id;
            await SaveReference(IndirectSalesOrder);
            return true;
        }

        public async Task<bool> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderDAO IndirectSalesOrderDAO = DataContext.IndirectSalesOrder.Where(x => x.Id == IndirectSalesOrder.Id).FirstOrDefault();
            if (IndirectSalesOrderDAO == null)
                return false;
            IndirectSalesOrderDAO.Id = IndirectSalesOrder.Id;
            IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
            IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
            IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            IndirectSalesOrderDAO.IndirectSalesOrderStatusId = IndirectSalesOrder.IndirectSalesOrderStatusId;
            IndirectSalesOrderDAO.IsEditedPrice = IndirectSalesOrder.IsEditedPrice;
            IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
            IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
            IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            IndirectSalesOrderDAO.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
            IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
            await DataContext.SaveChangesAsync();
            await SaveReference(IndirectSalesOrder);
            return true;
        }

        public async Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            await DataContext.IndirectSalesOrder.Where(x => x.Id == IndirectSalesOrder.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = new List<IndirectSalesOrderDAO>();
            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                IndirectSalesOrderDAO IndirectSalesOrderDAO = new IndirectSalesOrderDAO();
                IndirectSalesOrderDAO.Id = IndirectSalesOrder.Id;
                IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
                IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
                IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
                IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
                IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
                IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
                IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
                IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
                IndirectSalesOrderDAO.IndirectSalesOrderStatusId = IndirectSalesOrder.IndirectSalesOrderStatusId;
                IndirectSalesOrderDAO.IsEditedPrice = IndirectSalesOrder.IsEditedPrice;
                IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
                IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
                IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
                IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
                IndirectSalesOrderDAO.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
                IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
                IndirectSalesOrderDAOs.Add(IndirectSalesOrderDAO);
            }
            await DataContext.BulkMergeAsync(IndirectSalesOrderDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            List<long> Ids = IndirectSalesOrders.Select(x => x.Id).ToList();
            await DataContext.IndirectSalesOrder
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(IndirectSalesOrder IndirectSalesOrder)
        {
            await DataContext.IndirectSalesOrderContent
                .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                .DeleteFromQueryAsync();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = new List<IndirectSalesOrderContentDAO>();
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (IndirectSalesOrderContent IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO = new IndirectSalesOrderContentDAO();
                    IndirectSalesOrderContentDAO.Id = IndirectSalesOrderContent.Id;
                    IndirectSalesOrderContentDAO.IndirectSalesOrderId = IndirectSalesOrder.Id;
                    IndirectSalesOrderContentDAO.ItemId = IndirectSalesOrderContent.ItemId;
                    IndirectSalesOrderContentDAO.UnitOfMeasureId = IndirectSalesOrderContent.UnitOfMeasureId;
                    IndirectSalesOrderContentDAO.Quantity = IndirectSalesOrderContent.Quantity;
                    IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId = IndirectSalesOrderContent.PrimaryUnitOfMeasureId;
                    IndirectSalesOrderContentDAO.RequestedQuantity = IndirectSalesOrderContent.RequestedQuantity;
                    IndirectSalesOrderContentDAO.SalePrice = IndirectSalesOrderContent.SalePrice;
                    IndirectSalesOrderContentDAO.DiscountPercentage = IndirectSalesOrderContent.DiscountPercentage;
                    IndirectSalesOrderContentDAO.DiscountAmount = IndirectSalesOrderContent.DiscountAmount;
                    IndirectSalesOrderContentDAO.GeneralDiscountPercentage = IndirectSalesOrderContent.GeneralDiscountPercentage;
                    IndirectSalesOrderContentDAO.GeneralDiscountAmount = IndirectSalesOrderContent.GeneralDiscountAmount;
                    IndirectSalesOrderContentDAO.Amount = IndirectSalesOrderContent.Amount;
                    IndirectSalesOrderContentDAO.TaxPercentage = IndirectSalesOrderContent.TaxPercentage;
                    IndirectSalesOrderContentDAO.TaxAmount = IndirectSalesOrderContent.TaxAmount;
                    IndirectSalesOrderContentDAOs.Add(IndirectSalesOrderContentDAO);
                }
                await DataContext.IndirectSalesOrderContent.BulkMergeAsync(IndirectSalesOrderContentDAOs);
            }
            await DataContext.IndirectSalesOrderPromotion
                .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                .DeleteFromQueryAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = new List<IndirectSalesOrderPromotionDAO>();
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                foreach (IndirectSalesOrderPromotion IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO = new IndirectSalesOrderPromotionDAO();
                    IndirectSalesOrderPromotionDAO.Id = IndirectSalesOrderPromotion.Id;
                    IndirectSalesOrderPromotionDAO.IndirectSalesOrderId = IndirectSalesOrder.Id;
                    IndirectSalesOrderPromotionDAO.ItemId = IndirectSalesOrderPromotion.ItemId;
                    IndirectSalesOrderPromotionDAO.UnitOfMeasureId = IndirectSalesOrderPromotion.UnitOfMeasureId;
                    IndirectSalesOrderPromotionDAO.Quantity = IndirectSalesOrderPromotion.Quantity;
                    IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId = IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId;
                    IndirectSalesOrderPromotionDAO.RequestedQuantity = IndirectSalesOrderPromotion.RequestedQuantity;
                    IndirectSalesOrderPromotionDAO.Note = IndirectSalesOrderPromotion.Note;
                    IndirectSalesOrderPromotionDAOs.Add(IndirectSalesOrderPromotionDAO);
                }
                await DataContext.IndirectSalesOrderPromotion.BulkMergeAsync(IndirectSalesOrderPromotionDAOs);
            }
        }
        
    }
}
