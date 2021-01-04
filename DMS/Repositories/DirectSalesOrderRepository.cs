using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IDirectSalesOrderRepository
    {
        Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter);

        Task<int> CountNew(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListNew(DirectSalesOrderFilter DirectSalesOrderFilter);

        Task<int> CountPending(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListPending(DirectSalesOrderFilter DirectSalesOrderFilter);

        Task<int> CountCompleted(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListCompleted(DirectSalesOrderFilter DirectSalesOrderFilter);

        Task<DirectSalesOrder> Get(long Id);
        Task<bool> Create(DirectSalesOrder DirectSalesOrder);
        Task<bool> Update(DirectSalesOrder DirectSalesOrder);
        Task<bool> Delete(DirectSalesOrder DirectSalesOrder);
        Task<bool> BulkMerge(List<DirectSalesOrder> DirectSalesOrders);
        Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
        Task<bool> UpdateState(DirectSalesOrder DirectSalesOrder);
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
            if (filter.OrganizationId != null)
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => Ids.Contains(q.OrganizationId));
                }
                if (filter.OrganizationId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => !Ids.Contains(q.OrganizationId));
                }
            }
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.BuyerStoreId != null)
                query = query.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
            if (filter.PhoneNumber != null)
                query = query.Where(q => q.PhoneNumber, filter.PhoneNumber);
            if (filter.StoreAddress != null)
                query = query.Where(q => q.StoreAddress, filter.StoreAddress);
            if (filter.DeliveryAddress != null)
                query = query.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
            if (filter.AppUserId != null)
                query = query.Where(q => q.SaleEmployeeId, filter.AppUserId);
            if (filter.OrderDate != null)
                query = query.Where(q => q.OrderDate, filter.OrderDate);
            if (filter.DeliveryDate != null)
                query = query.Where(q => q.DeliveryDate, filter.DeliveryDate);
            if (filter.EditedPriceStatusId != null)
                query = query.Where(q => q.EditedPriceStatusId, filter.EditedPriceStatusId);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.RequestStateId != null)
                query = query.Where(q => q.RequestStateId, filter.RequestStateId);
            if (filter.SubTotal != null)
                query = query.Where(q => q.SubTotal, filter.SubTotal);
            if (filter.GeneralDiscountPercentage != null)
                query = query.Where(q => q.GeneralDiscountPercentage, filter.GeneralDiscountPercentage);
            if (filter.GeneralDiscountAmount != null)
                query = query.Where(q => q.GeneralDiscountAmount, filter.GeneralDiscountAmount);
            if (filter.PromotionCode != null && filter.PromotionCode.HasValue)
                query = query.Where(q => q.PromotionCode, filter.PromotionCode);
            if (filter.TotalTaxAmount != null)
                query = query.Where(q => q.TotalTaxAmount, filter.TotalTaxAmount);
            if (filter.Total != null)
                query = query.Where(q => q.Total, filter.Total);
            if (filter.StoreStatusId != null)
                query = query.Where(q => q.BuyerStore.StoreStatusId, filter.StoreStatusId);
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
                if (DirectSalesOrderFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, DirectSalesOrderFilter.Id);
                if (DirectSalesOrderFilter.OrganizationId != null)
                {
                    if (DirectSalesOrderFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == DirectSalesOrderFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (DirectSalesOrderFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == DirectSalesOrderFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (DirectSalesOrderFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => DirectSalesOrderFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (DirectSalesOrderFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => DirectSalesOrderFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                if (DirectSalesOrderFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, DirectSalesOrderFilter.Code);
                if (DirectSalesOrderFilter.BuyerStoreId != null)
                    queryable = queryable.Where(q => q.BuyerStoreId, DirectSalesOrderFilter.BuyerStoreId);
                if (DirectSalesOrderFilter.PhoneNumber != null)
                    queryable = queryable.Where(q => q.PhoneNumber, DirectSalesOrderFilter.PhoneNumber);
                if (DirectSalesOrderFilter.StoreAddress != null)
                    queryable = queryable.Where(q => q.StoreAddress, DirectSalesOrderFilter.StoreAddress);
                if (DirectSalesOrderFilter.DeliveryAddress != null)
                    queryable = queryable.Where(q => q.DeliveryAddress, DirectSalesOrderFilter.DeliveryAddress);
                if (DirectSalesOrderFilter.AppUserId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, DirectSalesOrderFilter.AppUserId);
                if (DirectSalesOrderFilter.OrderDate != null)
                    queryable = queryable.Where(q => q.OrderDate, DirectSalesOrderFilter.OrderDate);
                if (DirectSalesOrderFilter.DeliveryDate != null)
                    queryable = queryable.Where(q => q.DeliveryDate, DirectSalesOrderFilter.DeliveryDate);
                if (DirectSalesOrderFilter.EditedPriceStatusId != null)
                    queryable = queryable.Where(q => q.EditedPriceStatusId, DirectSalesOrderFilter.EditedPriceStatusId);
                if (DirectSalesOrderFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, DirectSalesOrderFilter.Note);
                if (DirectSalesOrderFilter.RequestStateId != null)
                    queryable = queryable.Where(q => q.RequestStateId, DirectSalesOrderFilter.RequestStateId);
                if (DirectSalesOrderFilter.SubTotal != null)
                    queryable = queryable.Where(q => q.SubTotal, DirectSalesOrderFilter.SubTotal);
                if (DirectSalesOrderFilter.GeneralDiscountPercentage != null)
                    queryable = queryable.Where(q => q.GeneralDiscountPercentage, DirectSalesOrderFilter.GeneralDiscountPercentage);
                if (DirectSalesOrderFilter.GeneralDiscountAmount != null)
                    queryable = queryable.Where(q => q.GeneralDiscountAmount, DirectSalesOrderFilter.GeneralDiscountAmount);
                if (DirectSalesOrderFilter.TotalTaxAmount != null)
                    queryable = queryable.Where(q => q.TotalTaxAmount, DirectSalesOrderFilter.TotalTaxAmount);
                if (DirectSalesOrderFilter.Total != null)
                    queryable = queryable.Where(q => q.Total, DirectSalesOrderFilter.Total);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<DirectSalesOrderDAO> DynamicOrder(IQueryable<DirectSalesOrderDAO> query, DirectSalesOrderFilter filter)
        {
            query = query.Distinct();
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
                        case DirectSalesOrderOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case DirectSalesOrderOrder.BuyerStore:
                            query = query.OrderBy(q => q.BuyerStoreId);
                            break;
                        case DirectSalesOrderOrder.PhoneNumber:
                            query = query.OrderBy(q => q.PhoneNumber);
                            break;
                        case DirectSalesOrderOrder.StoreAddress:
                            query = query.OrderBy(q => q.StoreAddress);
                            break;
                        case DirectSalesOrderOrder.DeliveryAddress:
                            query = query.OrderBy(q => q.DeliveryAddress);
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
                        case DirectSalesOrderOrder.RequestState:
                            query = query.OrderBy(q => q.RequestStateId);
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
                        case DirectSalesOrderOrder.CreatedAt:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case DirectSalesOrderOrder.UpdatedAt:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                        default:
                            query = query.OrderBy(q => q.UpdatedAt);
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
                        case DirectSalesOrderOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case DirectSalesOrderOrder.BuyerStore:
                            query = query.OrderByDescending(q => q.BuyerStoreId);
                            break;
                        case DirectSalesOrderOrder.PhoneNumber:
                            query = query.OrderByDescending(q => q.PhoneNumber);
                            break;
                        case DirectSalesOrderOrder.StoreAddress:
                            query = query.OrderByDescending(q => q.StoreAddress);
                            break;
                        case DirectSalesOrderOrder.DeliveryAddress:
                            query = query.OrderByDescending(q => q.DeliveryAddress);
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
                        case DirectSalesOrderOrder.RequestState:
                            query = query.OrderByDescending(q => q.RequestStateId);
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
                        case DirectSalesOrderOrder.CreatedAt:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                        case DirectSalesOrderOrder.UpdatedAt:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                        default:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                    }
                    break;
                default:
                    query = query.OrderByDescending(q => q.UpdatedAt);
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
                OrganizationId = filter.Selects.Contains(DirectSalesOrderSelect.Organization) ? q.OrganizationId : default(long),
                BuyerStoreId = filter.Selects.Contains(DirectSalesOrderSelect.BuyerStore) ? q.BuyerStoreId : default(long),
                PhoneNumber = filter.Selects.Contains(DirectSalesOrderSelect.PhoneNumber) ? q.PhoneNumber : default(string),
                StoreAddress = filter.Selects.Contains(DirectSalesOrderSelect.StoreAddress) ? q.StoreAddress : default(string),
                DeliveryAddress = filter.Selects.Contains(DirectSalesOrderSelect.DeliveryAddress) ? q.DeliveryAddress : default(string),
                SaleEmployeeId = filter.Selects.Contains(DirectSalesOrderSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                OrderDate = filter.Selects.Contains(DirectSalesOrderSelect.OrderDate) ? q.OrderDate : default(DateTime),
                DeliveryDate = filter.Selects.Contains(DirectSalesOrderSelect.DeliveryDate) ? q.DeliveryDate : default(DateTime?),
                EditedPriceStatusId = filter.Selects.Contains(DirectSalesOrderSelect.EditedPriceStatus) ? q.EditedPriceStatusId : default(long),
                Note = filter.Selects.Contains(DirectSalesOrderSelect.Note) ? q.Note : default(string),
                RequestStateId = filter.Selects.Contains(DirectSalesOrderSelect.RequestState) ? q.RequestStateId : default(long),
                SubTotal = filter.Selects.Contains(DirectSalesOrderSelect.SubTotal) ? q.SubTotal : default(decimal),
                GeneralDiscountPercentage = filter.Selects.Contains(DirectSalesOrderSelect.GeneralDiscountPercentage) ? q.GeneralDiscountPercentage : default(decimal?),
                GeneralDiscountAmount = filter.Selects.Contains(DirectSalesOrderSelect.GeneralDiscountAmount) ? q.GeneralDiscountAmount : default(decimal?),
                PromotionCode = filter.Selects.Contains(DirectSalesOrderSelect.PromotionCode) ? q.PromotionCode : default(string),
                PromotionValue = filter.Selects.Contains(DirectSalesOrderSelect.PromotionValue) ? q.PromotionValue : default(decimal?),
                TotalTaxAmount = filter.Selects.Contains(DirectSalesOrderSelect.TotalTaxAmount) ? q.TotalTaxAmount : default(decimal),
                TotalAfterTax = filter.Selects.Contains(DirectSalesOrderSelect.TotalAfterTax) ? q.TotalAfterTax : default(decimal),
                Total = filter.Selects.Contains(DirectSalesOrderSelect.Total) ? q.Total : default(decimal),
                BuyerStore = filter.Selects.Contains(DirectSalesOrderSelect.BuyerStore) && q.BuyerStore != null ? new Store
                {
                    Id = q.BuyerStore.Id,
                    Code = q.BuyerStore.Code,
                    CodeDraft = q.BuyerStore.CodeDraft,
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
                    TaxCode = q.BuyerStore.TaxCode,
                    LegalEntity = q.BuyerStore.LegalEntity,
                    StatusId = q.BuyerStore.StatusId,
                    StoreStatus = q.BuyerStore.StoreStatus == null ? null : new StoreStatus
                    {
                        Id = q.BuyerStore.StoreStatus.Id,
                        Code = q.BuyerStore.StoreStatus.Code,
                        Name = q.BuyerStore.StoreStatus.Name,
                    }
                } : null,
                EditedPriceStatus = filter.Selects.Contains(DirectSalesOrderSelect.EditedPriceStatus) && q.EditedPriceStatus != null ? new EditedPriceStatus
                {
                    Id = q.EditedPriceStatus.Id,
                    Code = q.EditedPriceStatus.Code,
                    Name = q.EditedPriceStatus.Name,
                } : null,
                Organization = filter.Selects.Contains(DirectSalesOrderSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    Address = q.Organization.Address,
                    Phone = q.Organization.Phone,
                    Path = q.Organization.Path,
                    ParentId = q.Organization.ParentId,
                    Email = q.Organization.Email,
                    StatusId = q.Organization.StatusId,
                    Level = q.Organization.Level
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
                RowId = q.RowId
            }).ToListAsync();
            return DirectSalesOrders;
        }

        public async Task<int> Count(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = OrFilter(DirectSalesOrderDAOs, filter);
            return await DirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = OrFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = DynamicOrder(DirectSalesOrderDAOs, filter);
            List<DirectSalesOrder> DirectSalesOrders = await DynamicSelect(DirectSalesOrderDAOs, filter);
            return DirectSalesOrders;
        }

        public async Task<int> CountAll(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            var query1 = from q in DirectSalesOrderDAOs
                         where q.RequestStateId == RequestStateEnum.NEW.Id &&
                         q.SaleEmployeeId == filter.ApproverId.Equal
                         select q;
            var query2 = from q in DirectSalesOrderDAOs
                         join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                         join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                         where rstep.AppUserId == filter.ApproverId.Equal
                         select q;
            DirectSalesOrderDAOs = query1.Union(query2);
            int count = await DirectSalesOrderDAOs.Distinct().CountAsync();
            return count;
        }

        public async Task<List<DirectSalesOrder>> ListAll(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            var query1 = from q in DirectSalesOrderDAOs
                         where q.RequestStateId == RequestStateEnum.NEW.Id &&
                         q.SaleEmployeeId == filter.ApproverId.Equal
                         select q;
            var query2 = from q in DirectSalesOrderDAOs
                         join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                         join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                         where rstep.AppUserId == filter.ApproverId.Equal
                         select q;
            DirectSalesOrderDAOs = query1.Union(query2);
            DirectSalesOrderDAOs = DynamicOrder(DirectSalesOrderDAOs, filter);
            List<DirectSalesOrder> DirectSalesOrders = await DynamicSelect(DirectSalesOrderDAOs, filter);
            return DirectSalesOrders;
        }

        public async Task<int> CountNew(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = from q in DirectSalesOrderDAOs
                                   where (q.RequestStateId == RequestStateEnum.NEW.Id || q.RequestStateId == RequestStateEnum.REJECTED.Id) &&
                                   q.CreatorId == filter.ApproverId.Equal
                                   select q;

            return await DirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<DirectSalesOrder>> ListNew(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            DirectSalesOrderDAOs = from q in DirectSalesOrderDAOs
                                   where (q.RequestStateId == RequestStateEnum.NEW.Id || q.RequestStateId == RequestStateEnum.REJECTED.Id) &&
                                   q.CreatorId == filter.ApproverId.Equal
                                   select q;

            DirectSalesOrderDAOs = DynamicOrder(DirectSalesOrderDAOs, filter);
            List<DirectSalesOrder> DirectSalesOrders = await DynamicSelect(DirectSalesOrderDAOs, filter);
            return DirectSalesOrders;
        }

        public async Task<int> CountPending(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                DirectSalesOrderDAOs = from q in DirectSalesOrderDAOs
                                       join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                       join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                       join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                       join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                       where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                       select q;
            }
            return await DirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<DirectSalesOrder>> ListPending(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                DirectSalesOrderDAOs = from q in DirectSalesOrderDAOs
                                       join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                       join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                       join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                       join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                       where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                       select q;
            }
            DirectSalesOrderDAOs = DynamicOrder(DirectSalesOrderDAOs, filter);
            List<DirectSalesOrder> DirectSalesOrders = await DynamicSelect(DirectSalesOrderDAOs, filter);
            return DirectSalesOrders;
        }

        public async Task<int> CountCompleted(DirectSalesOrderFilter filter)
        {
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in DirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             q.RequestStateId != RequestStateEnum.NEW.Id &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal && rstep.RequestId == q.RowId
                             select q;
                var query2 = from q in DirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                DirectSalesOrderDAOs = query1.Union(query2);
            }
            return await DirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<DirectSalesOrder>> ListCompleted(DirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<DirectSalesOrder>();
            IQueryable<DirectSalesOrderDAO> DirectSalesOrderDAOs = DataContext.DirectSalesOrder.AsNoTracking();
            DirectSalesOrderDAOs = DynamicFilter(DirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in DirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             q.RequestStateId != RequestStateEnum.NEW.Id &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal && rstep.RequestId == q.RowId
                             select q;
                var query2 = from q in DirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                DirectSalesOrderDAOs = query1.Union(query2);
            }
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
                OrganizationId = x.OrganizationId,
                BuyerStoreId = x.BuyerStoreId,
                PhoneNumber = x.PhoneNumber,
                StoreAddress = x.StoreAddress,
                DeliveryAddress = x.DeliveryAddress,
                SaleEmployeeId = x.SaleEmployeeId,
                OrderDate = x.OrderDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeliveryDate = x.DeliveryDate,
                EditedPriceStatusId = x.EditedPriceStatusId,
                Note = x.Note,
                RequestStateId = x.RequestStateId,
                SubTotal = x.SubTotal,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                PromotionCode = x.PromotionCode,
                PromotionValue = x.PromotionValue,
                TotalTaxAmount = x.TotalTaxAmount,
                TotalAfterTax = x.TotalAfterTax,
                Total = x.Total,
                RowId = x.RowId,
                StoreCheckingId = x.StoreCheckingId,
                BuyerStore = x.BuyerStore == null ? null : new Store
                {
                    Id = x.BuyerStore.Id,
                    Code = x.BuyerStore.Code,
                    CodeDraft = x.BuyerStore.CodeDraft,
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
                    TaxCode = x.BuyerStore.TaxCode,
                    LegalEntity = x.BuyerStore.LegalEntity,
                    StatusId = x.BuyerStore.StatusId,
                },
                EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                {
                    Id = x.EditedPriceStatus.Id,
                    Code = x.EditedPriceStatus.Code,
                    Name = x.EditedPriceStatus.Name,
                },
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    Address = x.Organization.Address,
                    Phone = x.Organization.Phone,
                    Path = x.Organization.Path,
                    ParentId = x.Organization.ParentId,
                    Email = x.Organization.Email,
                    StatusId = x.Organization.StatusId,
                    Level = x.Organization.Level
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

            RequestWorkflowDefinitionMappingDAO RequestWorkflowDefinitionMappingDAO = await DataContext.RequestWorkflowDefinitionMapping
              .Where(x => DirectSalesOrder.RowId == x.RequestId)
              .Include(x => x.RequestState)
              .AsNoTracking()
              .FirstOrDefaultAsync();
            if (RequestWorkflowDefinitionMappingDAO != null)
            {
                DirectSalesOrder.RequestStateId = RequestWorkflowDefinitionMappingDAO.RequestStateId;
                DirectSalesOrder.RequestState = new RequestState
                {
                    Id = RequestWorkflowDefinitionMappingDAO.RequestState.Id,
                    Code = RequestWorkflowDefinitionMappingDAO.RequestState.Code,
                    Name = RequestWorkflowDefinitionMappingDAO.RequestState.Name,
                };
            }

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
                    PrimaryPrice = x.PrimaryPrice,
                    SalePrice = x.SalePrice,
                    EditedPriceStatusId = x.EditedPriceStatusId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    Factor = x.Factor,
                    EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                    {
                        Id = x.EditedPriceStatus.Id,
                        Code = x.EditedPriceStatus.Code,
                        Name = x.EditedPriceStatus.Name,
                    },
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

            decimal GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount.HasValue ? DirectSalesOrder.GeneralDiscountAmount.Value : 0;
            decimal DiscountAmount = DirectSalesOrder.DirectSalesOrderContents != null ?
                DirectSalesOrder.DirectSalesOrderContents
                .Select(x => x.DiscountAmount.GetValueOrDefault(0))
                .Sum() : 0;
            DirectSalesOrder.TotalDiscountAmount = GeneralDiscountAmount + DiscountAmount;
            DirectSalesOrder.TotalRequestedQuantity = DirectSalesOrder.DirectSalesOrderContents != null ?
                DirectSalesOrder.DirectSalesOrderContents
                .Select(x => x.RequestedQuantity)
                .Sum() : 0;
            return DirectSalesOrder;
        }
        public async Task<bool> Create(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderDAO DirectSalesOrderDAO = new DirectSalesOrderDAO();
            DirectSalesOrderDAO.Id = DirectSalesOrder.Id;
            DirectSalesOrderDAO.Code = DirectSalesOrder.Code;
            DirectSalesOrderDAO.OrganizationId = DirectSalesOrder.OrganizationId;
            DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
            DirectSalesOrderDAO.PhoneNumber = DirectSalesOrder.PhoneNumber;
            DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
            DirectSalesOrderDAO.DeliveryAddress = DirectSalesOrder.DeliveryAddress;
            DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
            DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
            DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
            DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
            DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
            DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            DirectSalesOrderDAO.PromotionCode = DirectSalesOrder.PromotionCode;
            DirectSalesOrderDAO.PromotionValue = DirectSalesOrder.PromotionValue;
            DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            DirectSalesOrderDAO.TotalAfterTax = DirectSalesOrder.TotalAfterTax;
            DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
            DirectSalesOrderDAO.RowId = Guid.NewGuid();
            DirectSalesOrderDAO.StoreCheckingId = DirectSalesOrder.StoreCheckingId;
            DirectSalesOrderDAO.CreatorId = DirectSalesOrder.CreatorId;
            DirectSalesOrderDAO.CreatedAt = StaticParams.DateTimeNow;
            DirectSalesOrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.DirectSalesOrder.Add(DirectSalesOrderDAO);
            await DataContext.SaveChangesAsync();
            DirectSalesOrder.Id = DirectSalesOrderDAO.Id;
            DirectSalesOrder.RowId = DirectSalesOrderDAO.RowId;
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
            DirectSalesOrderDAO.PhoneNumber = DirectSalesOrder.PhoneNumber;
            DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
            DirectSalesOrderDAO.DeliveryAddress = DirectSalesOrder.DeliveryAddress;
            DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
            DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
            DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
            DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
            DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
            DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            DirectSalesOrderDAO.PromotionCode = DirectSalesOrder.PromotionCode;
            DirectSalesOrderDAO.PromotionValue = DirectSalesOrder.PromotionValue;
            DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            DirectSalesOrderDAO.TotalAfterTax = DirectSalesOrder.TotalAfterTax;
            DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
            DirectSalesOrderDAO.StoreCheckingId = DirectSalesOrder.StoreCheckingId;
            DirectSalesOrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(DirectSalesOrder);
            return true;
        }

        public async Task<bool> Delete(DirectSalesOrder DirectSalesOrder)
        {
            await DataContext.DirectSalesOrderTransaction.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id).DeleteFromQueryAsync();
            await DataContext.DirectSalesOrderContent.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id).DeleteFromQueryAsync();
            await DataContext.DirectSalesOrderPromotion.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id).DeleteFromQueryAsync();
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
                DirectSalesOrderDAO.OrganizationId = DirectSalesOrder.OrganizationId;
                DirectSalesOrderDAO.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
                DirectSalesOrderDAO.PhoneNumber = DirectSalesOrder.PhoneNumber;
                DirectSalesOrderDAO.StoreAddress = DirectSalesOrder.StoreAddress;
                DirectSalesOrderDAO.DeliveryAddress = DirectSalesOrder.DeliveryAddress;
                DirectSalesOrderDAO.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
                DirectSalesOrderDAO.OrderDate = DirectSalesOrder.OrderDate;
                DirectSalesOrderDAO.DeliveryDate = DirectSalesOrder.DeliveryDate;
                DirectSalesOrderDAO.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
                DirectSalesOrderDAO.Note = DirectSalesOrder.Note;
                DirectSalesOrderDAO.RequestStateId = DirectSalesOrder.RequestStateId;
                DirectSalesOrderDAO.SubTotal = DirectSalesOrder.SubTotal;
                DirectSalesOrderDAO.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
                DirectSalesOrderDAO.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
                DirectSalesOrderDAO.PromotionCode = DirectSalesOrder.PromotionCode;
                DirectSalesOrderDAO.PromotionValue = DirectSalesOrder.PromotionValue;
                DirectSalesOrderDAO.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
                DirectSalesOrderDAO.TotalAfterTax = DirectSalesOrder.TotalAfterTax;
                DirectSalesOrderDAO.Total = DirectSalesOrder.Total;
                DirectSalesOrderDAO.StoreCheckingId = DirectSalesOrder.StoreCheckingId;
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
            await DataContext.DirectSalesOrderContent
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
                await DataContext.DirectSalesOrderContent.BulkMergeAsync(DirectSalesOrderContentDAOs);
            }
            await DataContext.DirectSalesOrderPromotion
                .Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id)
                .DeleteFromQueryAsync();
            List<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotionDAOs = new List<DirectSalesOrderPromotionDAO>();
            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                foreach (DirectSalesOrderPromotion DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    DirectSalesOrderPromotionDAO DirectSalesOrderPromotionDAO = new DirectSalesOrderPromotionDAO();
                    DirectSalesOrderPromotionDAO.Id = DirectSalesOrderPromotion.Id;
                    DirectSalesOrderPromotionDAO.DirectSalesOrderId = DirectSalesOrder.Id;
                    DirectSalesOrderPromotionDAO.ItemId = DirectSalesOrderPromotion.ItemId;
                    DirectSalesOrderPromotionDAO.UnitOfMeasureId = DirectSalesOrderPromotion.UnitOfMeasureId;
                    DirectSalesOrderPromotionDAO.Quantity = DirectSalesOrderPromotion.Quantity;
                    DirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId = DirectSalesOrderPromotion.PrimaryUnitOfMeasureId;
                    DirectSalesOrderPromotionDAO.RequestedQuantity = DirectSalesOrderPromotion.RequestedQuantity;
                    DirectSalesOrderPromotionDAO.Note = DirectSalesOrderPromotion.Note;
                    DirectSalesOrderPromotionDAO.Factor = DirectSalesOrderPromotion.Factor;
                    DirectSalesOrderPromotionDAOs.Add(DirectSalesOrderPromotionDAO);
                }
                await DataContext.DirectSalesOrderPromotion.BulkMergeAsync(DirectSalesOrderPromotionDAOs);
            }

            await DataContext.DirectSalesOrderTransaction.Where(x => x.DirectSalesOrderId == DirectSalesOrder.Id).DeleteFromQueryAsync();
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
            }

            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    DirectSalesOrderTransactionDAO DirectSalesOrderTransactionDAO = new DirectSalesOrderTransactionDAO
                    {
                        DirectSalesOrderId = DirectSalesOrder.Id,
                        ItemId = DirectSalesOrderPromotion.ItemId,
                        OrganizationId = DirectSalesOrder.OrganizationId,
                        BuyerStoreId = DirectSalesOrder.BuyerStoreId,
                        SalesEmployeeId = DirectSalesOrder.SaleEmployeeId,
                        OrderDate = DirectSalesOrder.OrderDate,
                        TypeId = TransactionTypeEnum.PROMOTION.Id,
                        UnitOfMeasureId = DirectSalesOrderPromotion.PrimaryUnitOfMeasureId,
                        Quantity = DirectSalesOrderPromotion.RequestedQuantity,
                    };
                    DirectSalesOrderTransactionDAOs.Add(DirectSalesOrderTransactionDAO);
                }
            }
            await DataContext.DirectSalesOrderTransaction.BulkMergeAsync(DirectSalesOrderTransactionDAOs);
        }

        public async Task<bool> UpdateState(DirectSalesOrder DirectSalesOrder)
        {
            await DataContext.DirectSalesOrder.Where(x => x.Id == DirectSalesOrder.Id)
                .UpdateFromQueryAsync(x => new DirectSalesOrderDAO
                {
                    RequestStateId = DirectSalesOrder.RequestStateId,
                    UpdatedAt = StaticParams.DateTimeNow
                });
            return true;
        }
    }
}
