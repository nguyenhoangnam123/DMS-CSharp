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
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;

namespace DMS.Repositories
{
    public interface IIndirectSalesOrderRepository
    {
        Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> List(List<long> Ids);

        Task<int> CountNew(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> ListNew(IndirectSalesOrderFilter IndirectSalesOrderFilter);

        Task<int> CountPending(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> ListPending(IndirectSalesOrderFilter IndirectSalesOrderFilter);

        Task<int> CountCompleted(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> ListCompleted(IndirectSalesOrderFilter IndirectSalesOrderFilter);

        Task<IndirectSalesOrder> Get(long Id);
        Task<bool> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> UpdateState(IndirectSalesOrder IndirectSalesOrder);
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

        private async Task<IQueryable<IndirectSalesOrderDAO>> DynamicFilter(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
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
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value && o.StatusId == 1).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value && o.StatusId == 1).FirstOrDefault();
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
            {
                if (filter.BuyerStoreId.In != null && filter.BuyerStoreId.In.Count > 0)
                {
                    if (filter.BuyerStoreId.NotIn == null || filter.BuyerStoreId.NotIn.Count == 0)
                    {
                        ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(filter.BuyerStoreId.In.Distinct().ToList());
                        query = query.Join(tempTableQuery.Query,
                                           c => c.BuyerStoreId,
                                           t => t.Column1,
                                           (c, t) => c);
                    }
                    else
                    {
                        ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                            .BulkInsertValuesIntoTempTableAsync<long>(filter.BuyerStoreId.In.Distinct().ToList());
                        query = query.Where(x => !filter.BuyerStoreId.NotIn.Contains(x.Id));
                        query = query.Join(tempTableQuery.Query,
                                           c => c.BuyerStoreId,
                                           t => t.Column1,
                                           (c, t) => c);
                    }
                }
                else
                {
                    query = query.Where(q => q.BuyerStoreId, filter.BuyerStoreId);
                }
            }    
            if (filter.PhoneNumber != null)
                query = query.Where(q => q.PhoneNumber, filter.PhoneNumber);
            if (filter.StoreAddress != null)
                query = query.Where(q => q.StoreAddress, filter.StoreAddress);
            if (filter.DeliveryAddress != null)
                query = query.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
            if (filter.SellerStoreId != null)
                query = query.Where(q => q.SellerStoreId, filter.SellerStoreId);
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
            if (filter.Total != null)
                query = query.Where(q => q.Total, filter.Total);
            if (filter.StoreStatusId != null)
                query = query.Where(q => q.BuyerStore.StoreStatusId, filter.StoreStatusId);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                List<string> Tokens = filter.Search.Split(" ").Select(x => x.ToLower()).ToList();
                var queryForCode = query;
                var queryForName = query;
                var queryForUnsignName = query;
                foreach (string Token in Tokens)
                {
                    if (string.IsNullOrWhiteSpace(Token))
                        continue;
                    queryForCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                    queryForName = queryForName.Where(x => x.BuyerStore.Name.ToLower().Contains(Token));
                    queryForUnsignName = queryForUnsignName.Where(x => x.BuyerStore.UnsignName.ToLower().Contains(Token));
                }
                query = queryForCode.Union(queryForName).Union(queryForUnsignName);
                query = query.Distinct();
            }
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
                if (IndirectSalesOrderFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, IndirectSalesOrderFilter.Id);
                if (IndirectSalesOrderFilter.OrganizationId != null)
                {
                    if (IndirectSalesOrderFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == IndirectSalesOrderFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (IndirectSalesOrderFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == IndirectSalesOrderFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (IndirectSalesOrderFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => IndirectSalesOrderFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (IndirectSalesOrderFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => IndirectSalesOrderFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                if (IndirectSalesOrderFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, IndirectSalesOrderFilter.Code);
                if (IndirectSalesOrderFilter.BuyerStoreId != null)
                    queryable = queryable.Where(q => q.BuyerStoreId, IndirectSalesOrderFilter.BuyerStoreId);
                if (IndirectSalesOrderFilter.PhoneNumber != null)
                    queryable = queryable.Where(q => q.PhoneNumber, IndirectSalesOrderFilter.PhoneNumber);
                if (IndirectSalesOrderFilter.StoreAddress != null)
                    queryable = queryable.Where(q => q.StoreAddress, IndirectSalesOrderFilter.StoreAddress);
                if (IndirectSalesOrderFilter.DeliveryAddress != null)
                    queryable = queryable.Where(q => q.DeliveryAddress, IndirectSalesOrderFilter.DeliveryAddress);
                if (IndirectSalesOrderFilter.SellerStoreId != null)
                    queryable = queryable.Where(q => q.SellerStoreId, IndirectSalesOrderFilter.SellerStoreId);
                if (IndirectSalesOrderFilter.AppUserId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, IndirectSalesOrderFilter.AppUserId);
                if (IndirectSalesOrderFilter.OrderDate != null)
                    queryable = queryable.Where(q => q.OrderDate, IndirectSalesOrderFilter.OrderDate);
                if (IndirectSalesOrderFilter.DeliveryDate != null)
                    queryable = queryable.Where(q => q.DeliveryDate, IndirectSalesOrderFilter.DeliveryDate);
                if (IndirectSalesOrderFilter.EditedPriceStatusId != null)
                    queryable = queryable.Where(q => q.EditedPriceStatusId, IndirectSalesOrderFilter.EditedPriceStatusId);
                if (IndirectSalesOrderFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, IndirectSalesOrderFilter.Note);
                if (IndirectSalesOrderFilter.SubTotal != null)
                    queryable = queryable.Where(q => q.SubTotal, IndirectSalesOrderFilter.SubTotal);
                if (IndirectSalesOrderFilter.GeneralDiscountPercentage != null)
                    queryable = queryable.Where(q => q.GeneralDiscountPercentage, IndirectSalesOrderFilter.GeneralDiscountPercentage);
                if (IndirectSalesOrderFilter.GeneralDiscountAmount != null)
                    queryable = queryable.Where(q => q.GeneralDiscountAmount, IndirectSalesOrderFilter.GeneralDiscountAmount);
                if (IndirectSalesOrderFilter.Total != null)
                    queryable = queryable.Where(q => q.Total, IndirectSalesOrderFilter.Total);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<IndirectSalesOrderDAO> DynamicOrder(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            query = query.Distinct();
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case IndirectSalesOrderOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case IndirectSalesOrderOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case IndirectSalesOrderOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
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
                        case IndirectSalesOrderOrder.EditedPriceStatus:
                            query = query.OrderBy(q => q.EditedPriceStatusId);
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
                        case IndirectSalesOrderOrder.Total:
                            query = query.OrderBy(q => q.Total);
                            break;
                        case IndirectSalesOrderOrder.CreatedAt:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case IndirectSalesOrderOrder.UpdatedAt:
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
                        case IndirectSalesOrderOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case IndirectSalesOrderOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case IndirectSalesOrderOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
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
                        case IndirectSalesOrderOrder.EditedPriceStatus:
                            query = query.OrderByDescending(q => q.EditedPriceStatusId);
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
                        case IndirectSalesOrderOrder.Total:
                            query = query.OrderByDescending(q => q.Total);
                            break;
                        case IndirectSalesOrderOrder.CreatedAt:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                        case IndirectSalesOrderOrder.UpdatedAt:
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

            return query;
        }

        private async Task<List<IndirectSalesOrder>> DynamicSelect(IQueryable<IndirectSalesOrderDAO> query, IndirectSalesOrderFilter filter)
        {
            List<IndirectSalesOrder> IndirectSalesOrders = await query.Select(q => new IndirectSalesOrder()
            {
                Id = filter.Selects.Contains(IndirectSalesOrderSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(IndirectSalesOrderSelect.Code) ? q.Code : default(string),
                OrganizationId = filter.Selects.Contains(IndirectSalesOrderSelect.Organization) ? q.OrganizationId : default(long),
                BuyerStoreId = filter.Selects.Contains(IndirectSalesOrderSelect.BuyerStore) ? q.BuyerStoreId : default(long),
                PhoneNumber = filter.Selects.Contains(IndirectSalesOrderSelect.PhoneNumber) ? q.PhoneNumber : default(string),
                StoreAddress = filter.Selects.Contains(IndirectSalesOrderSelect.StoreAddress) ? q.StoreAddress : default(string),
                DeliveryAddress = filter.Selects.Contains(IndirectSalesOrderSelect.DeliveryAddress) ? q.DeliveryAddress : default(string),
                SellerStoreId = filter.Selects.Contains(IndirectSalesOrderSelect.SellerStore) ? q.SellerStoreId : default(long),
                SaleEmployeeId = filter.Selects.Contains(IndirectSalesOrderSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                OrderDate = filter.Selects.Contains(IndirectSalesOrderSelect.OrderDate) ? q.OrderDate : default(DateTime),
                DeliveryDate = filter.Selects.Contains(IndirectSalesOrderSelect.DeliveryDate) ? q.DeliveryDate : default(DateTime?),
                EditedPriceStatusId = filter.Selects.Contains(IndirectSalesOrderSelect.EditedPriceStatus) ? q.EditedPriceStatusId : default(long),
                Note = filter.Selects.Contains(IndirectSalesOrderSelect.Note) ? q.Note : default(string),
                SubTotal = filter.Selects.Contains(IndirectSalesOrderSelect.SubTotal) ? q.SubTotal : default(long),
                GeneralDiscountPercentage = filter.Selects.Contains(IndirectSalesOrderSelect.GeneralDiscountPercentage) ? q.GeneralDiscountPercentage : default(long?),
                GeneralDiscountAmount = filter.Selects.Contains(IndirectSalesOrderSelect.GeneralDiscountAmount) ? q.GeneralDiscountAmount : default(long?),
                Total = filter.Selects.Contains(IndirectSalesOrderSelect.Total) ? q.Total : default(long),
                RequestStateId = filter.Selects.Contains(IndirectSalesOrderSelect.RequestState) ? q.RequestStateId : default(long),
                BuyerStore = filter.Selects.Contains(IndirectSalesOrderSelect.BuyerStore) && q.BuyerStore != null ? new Store
                {
                    Id = q.BuyerStore.Id,
                    Code = q.BuyerStore.Code,
                    CodeDraft = q.BuyerStore.CodeDraft,
                    Name = q.BuyerStore.Name,
                    Address = q.BuyerStore.Address,
                    StoreStatus = q.BuyerStore.StoreStatus == null ? null : new StoreStatus
                    {
                        Id = q.BuyerStore.StoreStatus.Id,
                        Code = q.BuyerStore.StoreStatus.Code,
                        Name = q.BuyerStore.StoreStatus.Name,
                    },
                    StoreGrouping = q.BuyerStore.StoreGrouping == null ? null : new StoreGrouping
                    {
                        Name = q.BuyerStore.StoreGrouping.Name
                    },
                    StoreType = q.BuyerStore.StoreType == null ? null : new StoreType
                    {
                        Name = q.BuyerStore.StoreType.Name
                    },
                } : null,
                EditedPriceStatus = filter.Selects.Contains(IndirectSalesOrderSelect.EditedPriceStatus) && q.EditedPriceStatus != null ? new EditedPriceStatus
                {
                    Id = q.EditedPriceStatus.Id,
                    Code = q.EditedPriceStatus.Code,
                    Name = q.EditedPriceStatus.Name,
                } : null,
                Organization = filter.Selects.Contains(IndirectSalesOrderSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    Path = q.Organization.Path,
                } : null,
                SaleEmployee = filter.Selects.Contains(IndirectSalesOrderSelect.SaleEmployee) && q.SaleEmployee != null ? new AppUser
                {
                    Id = q.SaleEmployee.Id,
                    Username = q.SaleEmployee.Username,
                    DisplayName = q.SaleEmployee.DisplayName,
                } : null,
                SellerStore = filter.Selects.Contains(IndirectSalesOrderSelect.SellerStore) && q.SellerStore != null ? new Store
                {
                    Id = q.SellerStore.Id,
                    Code = q.SellerStore.Code,
                    CodeDraft = q.SellerStore.CodeDraft,
                    Name = q.SellerStore.Name,
                } : null,
                RequestState = filter.Selects.Contains(IndirectSalesOrderSelect.RequestState) && q.RequestState != null ? new RequestState
                {
                    Id = q.RequestState.Id,
                    Code = q.RequestState.Code,
                    Name = q.RequestState.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).Skip(filter.Skip).Take(filter.Take).ToListAsync();
            return IndirectSalesOrders;
        }

        public async Task<int> Count(IndirectSalesOrderFilter filter)
        {
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = OrFilter(IndirectSalesOrderDAOs, filter);
            return await IndirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrder>();
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = OrFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = DynamicOrder(IndirectSalesOrderDAOs, filter);
            List<IndirectSalesOrder> IndirectSalesOrders = await DynamicSelect(IndirectSalesOrderDAOs, filter);
            return IndirectSalesOrders;
        }

        public async Task<List<IndirectSalesOrder>> List(List<long> Ids)
        {
            List<IndirectSalesOrder> IndirectSalesOrders = await DataContext.IndirectSalesOrder.AsNoTracking()
                .Where(x => Ids.Contains(x.Id))
                .Select(x => new IndirectSalesOrder()
                {
                    Id = x.Id,
                    Code = x.Code,
                    OrganizationId = x.OrganizationId,
                    BuyerStoreId = x.BuyerStoreId,
                    SellerStoreId = x.SellerStoreId,
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
                    Total = x.Total,
                    RowId = x.RowId,
                    StoreCheckingId = x.StoreCheckingId,
                    CreatorId = x.CreatorId,
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
                    SellerStore = x.SellerStore == null ? null : new Store
                    {
                        Id = x.SellerStore.Id,
                        Code = x.SellerStore.Code,
                        CodeDraft = x.SellerStore.CodeDraft,
                        Name = x.SellerStore.Name,
                        ParentStoreId = x.SellerStore.ParentStoreId,
                        OrganizationId = x.SellerStore.OrganizationId,
                        StoreTypeId = x.SellerStore.StoreTypeId,
                        StoreGroupingId = x.SellerStore.StoreGroupingId,
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
                        TaxCode = x.SellerStore.TaxCode,
                        LegalEntity = x.SellerStore.LegalEntity,
                        StatusId = x.SellerStore.StatusId,
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
                })
                .ToListAsync();

            List<Guid> RowIds = IndirectSalesOrders.Select(x => x.RowId).ToList();
            List<RequestWorkflowDefinitionMappingDAO> RequestWorkflowDefinitionMappingDAOs = await DataContext.RequestWorkflowDefinitionMapping
              .Where(x => RowIds.Contains(x.RequestId))
              .Include(x => x.RequestState)
              .AsNoTracking()
              .ToListAsync();
            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                RequestWorkflowDefinitionMappingDAO RequestWorkflowDefinitionMappingDAO = RequestWorkflowDefinitionMappingDAOs
                    .Where(x => x.RequestId == IndirectSalesOrder.RowId)
                    .FirstOrDefault();
                if (RequestWorkflowDefinitionMappingDAO != null)
                {
                    IndirectSalesOrder.RequestStateId = RequestWorkflowDefinitionMappingDAO.RequestStateId;
                    IndirectSalesOrder.RequestState = new RequestState
                    {
                        Id = RequestWorkflowDefinitionMappingDAO.RequestState.Id,
                        Code = RequestWorkflowDefinitionMappingDAO.RequestState.Code,
                        Name = RequestWorkflowDefinitionMappingDAO.RequestState.Name,
                    };
                }
            }

            List<IndirectSalesOrderContent> IndirectSalesOrderContents = await DataContext.IndirectSalesOrderContent.AsNoTracking()
                .Where(x => Ids.Contains(x.IndirectSalesOrderId))
                .Select(x => new IndirectSalesOrderContent
                {
                    Id = x.Id,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
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
            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                IndirectSalesOrder.IndirectSalesOrderContents = IndirectSalesOrderContents
                    .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                    .ToList();
            }


            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await DataContext.IndirectSalesOrderPromotion.AsNoTracking()
                .Where(x => Ids.Contains(x.IndirectSalesOrderId))
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

            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                IndirectSalesOrder.IndirectSalesOrderPromotions = IndirectSalesOrderPromotions
                    .Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id)
                    .ToList();
            }

            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                decimal GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount.HasValue ? IndirectSalesOrder.GeneralDiscountAmount.Value : 0;
                decimal DiscountAmount = IndirectSalesOrder.IndirectSalesOrderContents != null ?
                    IndirectSalesOrder.IndirectSalesOrderContents
                    .Select(x => x.DiscountAmount.GetValueOrDefault(0))
                    .Sum() : 0;
                IndirectSalesOrder.TotalDiscountAmount = GeneralDiscountAmount + DiscountAmount;
                IndirectSalesOrder.TotalRequestedQuantity = IndirectSalesOrder.IndirectSalesOrderContents != null ?
                    IndirectSalesOrder.IndirectSalesOrderContents
                    .Select(x => x.RequestedQuantity)
                    .Sum() : 0;
            }

            return IndirectSalesOrders;
        }

        public async Task<int> CountNew(IndirectSalesOrderFilter filter)
        {
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = from q in IndirectSalesOrderDAOs
                                     where (q.RequestStateId == RequestStateEnum.NEW.Id || q.RequestStateId == RequestStateEnum.REJECTED.Id) &&
                                     q.CreatorId == filter.ApproverId.Equal
                                     select q;

            return await IndirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<IndirectSalesOrder>> ListNew(IndirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrder>();
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            IndirectSalesOrderDAOs = from q in IndirectSalesOrderDAOs
                                     where (q.RequestStateId == RequestStateEnum.NEW.Id || q.RequestStateId == RequestStateEnum.REJECTED.Id) &&
                                     q.CreatorId == filter.ApproverId.Equal
                                     select q;

            IndirectSalesOrderDAOs = DynamicOrder(IndirectSalesOrderDAOs, filter);
            List<IndirectSalesOrder> IndirectSalesOrders = await DynamicSelect(IndirectSalesOrderDAOs, filter);
            return IndirectSalesOrders;
        }

        public async Task<int> CountPending(IndirectSalesOrderFilter filter)
        {
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                IndirectSalesOrderDAOs = from q in IndirectSalesOrderDAOs
                                         join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                         join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                         join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                         where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                         select q;
            }
            return await IndirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<IndirectSalesOrder>> ListPending(IndirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrder>();
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                IndirectSalesOrderDAOs = from q in IndirectSalesOrderDAOs
                                         join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                         join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                         join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                         where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                         select q;
            }
            IndirectSalesOrderDAOs = DynamicOrder(IndirectSalesOrderDAOs, filter);
            List<IndirectSalesOrder> IndirectSalesOrders = await DynamicSelect(IndirectSalesOrderDAOs, filter);
            return IndirectSalesOrders;
        }

        public async Task<int> CountCompleted(IndirectSalesOrderFilter filter)
        {
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in IndirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             q.RequestStateId != RequestStateEnum.NEW.Id &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal && rstep.RequestId == q.RowId
                             select q;
                var query2 = from q in IndirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                IndirectSalesOrderDAOs = query1.Union(query2);
            }
            return await IndirectSalesOrderDAOs.Distinct().CountAsync();
        }

        public async Task<List<IndirectSalesOrder>> ListCompleted(IndirectSalesOrderFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrder>();
            IQueryable<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = DataContext.IndirectSalesOrder.AsNoTracking();
            IndirectSalesOrderDAOs = await DynamicFilter(IndirectSalesOrderDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in IndirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             q.RequestStateId != RequestStateEnum.NEW.Id &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal && rstep.RequestId == q.RowId
                             select q;
                var query2 = from q in IndirectSalesOrderDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                IndirectSalesOrderDAOs = query1.Union(query2);
            }
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
                Code = x.Code,
                OrganizationId = x.OrganizationId,
                BuyerStoreId = x.BuyerStoreId,
                PhoneNumber = x.PhoneNumber,
                StoreAddress = x.StoreAddress,
                DeliveryAddress = x.DeliveryAddress,
                SellerStoreId = x.SellerStoreId,
                SaleEmployeeId = x.SaleEmployeeId,
                OrderDate = x.OrderDate,
                DeliveryDate = x.DeliveryDate,
                EditedPriceStatusId = x.EditedPriceStatusId,
                Note = x.Note,
                SubTotal = x.SubTotal,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                Total = x.Total,
                RowId = x.RowId,
                StoreCheckingId = x.StoreCheckingId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                RequestStateId = x.RequestStateId,
                RequestState = x.RequestState == null ? null : new RequestState
                {
                    Id = x.RequestState.Id,
                    Code = x.RequestState.Code,
                    Name = x.RequestState.Name,
                },
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
                SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                {
                    Id = x.SaleEmployee.Id,
                    Username = x.SaleEmployee.Username,
                    DisplayName = x.SaleEmployee.DisplayName,
                    Address = x.SaleEmployee.Address,
                    Email = x.SaleEmployee.Email,
                    Phone = x.SaleEmployee.Phone,
                },
                SellerStore = x.SellerStore == null ? null : new Store
                {
                    Id = x.SellerStore.Id,
                    Code = x.SellerStore.Code,
                    CodeDraft = x.SellerStore.CodeDraft,
                    Name = x.SellerStore.Name,
                    ParentStoreId = x.SellerStore.ParentStoreId,
                    OrganizationId = x.SellerStore.OrganizationId,
                    StoreTypeId = x.SellerStore.StoreTypeId,
                    StoreGroupingId = x.SellerStore.StoreGroupingId,
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
                    TaxCode = x.SellerStore.TaxCode,
                    LegalEntity = x.SellerStore.LegalEntity,
                    StatusId = x.SellerStore.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (IndirectSalesOrder == null)
                return null;
            RequestWorkflowDefinitionMappingDAO RequestWorkflowDefinitionMappingDAO = await DataContext.RequestWorkflowDefinitionMapping
               .Where(x => IndirectSalesOrder.RowId == x.RequestId)
               .Include(x => x.RequestState)
               .AsNoTracking()
               .FirstOrDefaultAsync();
            if (RequestWorkflowDefinitionMappingDAO != null)
            {
                IndirectSalesOrder.RequestStateId = RequestWorkflowDefinitionMappingDAO.RequestStateId;
                IndirectSalesOrder.RequestState = new RequestState
                {
                    Id = RequestWorkflowDefinitionMappingDAO.RequestState.Id,
                    Code = RequestWorkflowDefinitionMappingDAO.RequestState.Code,
                    Name = RequestWorkflowDefinitionMappingDAO.RequestState.Name,
                };
            }

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
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                Name = x.Item.Product.TaxType.Name,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
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
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
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
                    Factor = x.Factor,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                Name = x.Item.Product.TaxType.Name,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
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
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToListAsync();

            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                List<TaxType> TaxTypes = await DataContext.TaxType.Where(x => x.StatusId == StatusEnum.ACTIVE.Id)
                    .Select(x => new TaxType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Percentage = x.Percentage,
                        StatusId = x.StatusId,
                    }).ToListAsync();

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    TaxType TaxType = TaxTypes.Where(x => x.Percentage == IndirectSalesOrderContent.TaxPercentage).FirstOrDefault();
                    IndirectSalesOrderContent.TaxType = TaxType;
                }
            }

            decimal GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount.HasValue ? IndirectSalesOrder.GeneralDiscountAmount.Value : 0;
            decimal DiscountAmount = IndirectSalesOrder.IndirectSalesOrderContents != null ?
                IndirectSalesOrder.IndirectSalesOrderContents
                .Select(x => x.DiscountAmount.GetValueOrDefault(0))
                .Sum() : 0;
            IndirectSalesOrder.TotalDiscountAmount = GeneralDiscountAmount + DiscountAmount;
            IndirectSalesOrder.TotalRequestedQuantity = IndirectSalesOrder.IndirectSalesOrderContents != null ?
                IndirectSalesOrder.IndirectSalesOrderContents
                .Select(x => x.RequestedQuantity)
                .Sum() : 0;
            return IndirectSalesOrder;
        }
        public async Task<bool> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderDAO IndirectSalesOrderDAO = new IndirectSalesOrderDAO();
            IndirectSalesOrderDAO.Code = IndirectSalesOrder.Code;
            IndirectSalesOrderDAO.OrganizationId = IndirectSalesOrder.OrganizationId;
            IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
            IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
            IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            IndirectSalesOrderDAO.EditedPriceStatusId = IndirectSalesOrder.EditedPriceStatusId;
            IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
            IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
            IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
            IndirectSalesOrderDAO.RowId = Guid.NewGuid();
            IndirectSalesOrderDAO.RequestStateId = IndirectSalesOrder.RequestStateId;
            IndirectSalesOrderDAO.StoreCheckingId = IndirectSalesOrder.StoreCheckingId;
            IndirectSalesOrderDAO.CreatorId = IndirectSalesOrder.CreatorId;
            IndirectSalesOrderDAO.CreatedAt = StaticParams.DateTimeNow;
            IndirectSalesOrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.IndirectSalesOrder.Add(IndirectSalesOrderDAO);
            await DataContext.SaveChangesAsync();
            IndirectSalesOrder.Id = IndirectSalesOrderDAO.Id;
            IndirectSalesOrder.RowId = IndirectSalesOrderDAO.RowId;
            await SaveReference(IndirectSalesOrder);
            return true;
        }

        public async Task<bool> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderDAO IndirectSalesOrderDAO = DataContext.IndirectSalesOrder.Where(x => x.Id == IndirectSalesOrder.Id).FirstOrDefault();
            if (IndirectSalesOrderDAO == null)
                return false;
            IndirectSalesOrderDAO.Id = IndirectSalesOrder.Id;
            IndirectSalesOrderDAO.Code = IndirectSalesOrder.Code;
            IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
            IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
            IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            IndirectSalesOrderDAO.EditedPriceStatusId = IndirectSalesOrder.EditedPriceStatusId;
            IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
            IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
            IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
            IndirectSalesOrderDAO.StoreCheckingId = IndirectSalesOrder.StoreCheckingId;
            IndirectSalesOrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(IndirectSalesOrder);
            return true;
        }

        public async Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            await DataContext.IndirectSalesOrderTransaction.Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id).DeleteFromQueryAsync();
            await DataContext.IndirectSalesOrderContent.Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id).DeleteFromQueryAsync();
            await DataContext.IndirectSalesOrderPromotion.Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id).DeleteFromQueryAsync();
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
                IndirectSalesOrderDAO.Code = IndirectSalesOrder.Code;
                IndirectSalesOrderDAO.OrganizationId = IndirectSalesOrder.OrganizationId;
                IndirectSalesOrderDAO.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
                IndirectSalesOrderDAO.PhoneNumber = IndirectSalesOrder.PhoneNumber;
                IndirectSalesOrderDAO.StoreAddress = IndirectSalesOrder.StoreAddress;
                IndirectSalesOrderDAO.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
                IndirectSalesOrderDAO.SellerStoreId = IndirectSalesOrder.SellerStoreId;
                IndirectSalesOrderDAO.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
                IndirectSalesOrderDAO.OrderDate = IndirectSalesOrder.OrderDate;
                IndirectSalesOrderDAO.DeliveryDate = IndirectSalesOrder.DeliveryDate;
                IndirectSalesOrderDAO.EditedPriceStatusId = IndirectSalesOrder.EditedPriceStatusId;
                IndirectSalesOrderDAO.Note = IndirectSalesOrder.Note;
                IndirectSalesOrderDAO.SubTotal = IndirectSalesOrder.SubTotal;
                IndirectSalesOrderDAO.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
                IndirectSalesOrderDAO.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
                IndirectSalesOrderDAO.Total = IndirectSalesOrder.Total;
                IndirectSalesOrderDAO.StoreCheckingId = IndirectSalesOrder.StoreCheckingId;
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
                    IndirectSalesOrderContentDAO.PrimaryPrice = IndirectSalesOrderContent.PrimaryPrice;
                    IndirectSalesOrderContentDAO.SalePrice = IndirectSalesOrderContent.SalePrice;
                    IndirectSalesOrderContentDAO.EditedPriceStatusId = IndirectSalesOrderContent.EditedPriceStatusId;
                    IndirectSalesOrderContentDAO.DiscountPercentage = IndirectSalesOrderContent.DiscountPercentage;
                    IndirectSalesOrderContentDAO.DiscountAmount = IndirectSalesOrderContent.DiscountAmount;
                    IndirectSalesOrderContentDAO.GeneralDiscountPercentage = IndirectSalesOrderContent.GeneralDiscountPercentage;
                    IndirectSalesOrderContentDAO.GeneralDiscountAmount = IndirectSalesOrderContent.GeneralDiscountAmount;
                    IndirectSalesOrderContentDAO.Amount = IndirectSalesOrderContent.Amount;
                    IndirectSalesOrderContentDAO.TaxPercentage = IndirectSalesOrderContent.TaxPercentage;
                    IndirectSalesOrderContentDAO.TaxAmount = IndirectSalesOrderContent.TaxAmount;
                    IndirectSalesOrderContentDAO.Factor = IndirectSalesOrderContent.Factor;
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
                    IndirectSalesOrderPromotionDAO.Factor = IndirectSalesOrderPromotion.Factor;
                    IndirectSalesOrderPromotionDAOs.Add(IndirectSalesOrderPromotionDAO);
                }
                await DataContext.IndirectSalesOrderPromotion.BulkMergeAsync(IndirectSalesOrderPromotionDAOs);
            }

            await DataContext.IndirectSalesOrderTransaction.Where(x => x.IndirectSalesOrderId == IndirectSalesOrder.Id).DeleteFromQueryAsync();
            List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = new List<IndirectSalesOrderTransactionDAO>();
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    IndirectSalesOrderTransactionDAO IndirectSalesOrderTransactionDAO = new IndirectSalesOrderTransactionDAO
                    {
                        IndirectSalesOrderId = IndirectSalesOrder.Id,
                        ItemId = IndirectSalesOrderContent.ItemId,
                        OrganizationId = IndirectSalesOrder.OrganizationId,
                        BuyerStoreId = IndirectSalesOrder.BuyerStoreId,
                        SalesEmployeeId = IndirectSalesOrder.SaleEmployeeId,
                        OrderDate = IndirectSalesOrder.OrderDate,
                        TypeId = TransactionTypeEnum.SALES_CONTENT.Id,
                        UnitOfMeasureId = IndirectSalesOrderContent.PrimaryUnitOfMeasureId,
                        Quantity = IndirectSalesOrderContent.RequestedQuantity,
                        Revenue = IndirectSalesOrderContent.Amount - IndirectSalesOrderContent.GeneralDiscountAmount ?? 0,
                        Discount = (IndirectSalesOrderContent.DiscountAmount ?? 0) + (IndirectSalesOrderContent.GeneralDiscountAmount ?? 0)
                    };
                    IndirectSalesOrderTransactionDAOs.Add(IndirectSalesOrderTransactionDAO);
                }
            }

            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    IndirectSalesOrderTransactionDAO IndirectSalesOrderTransactionDAO = new IndirectSalesOrderTransactionDAO
                    {
                        IndirectSalesOrderId = IndirectSalesOrder.Id,
                        ItemId = IndirectSalesOrderPromotion.ItemId,
                        OrganizationId = IndirectSalesOrder.OrganizationId,
                        BuyerStoreId = IndirectSalesOrder.BuyerStoreId,
                        SalesEmployeeId = IndirectSalesOrder.SaleEmployeeId,
                        OrderDate = IndirectSalesOrder.OrderDate,
                        TypeId = TransactionTypeEnum.PROMOTION.Id,
                        UnitOfMeasureId = IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId,
                        Quantity = IndirectSalesOrderPromotion.RequestedQuantity,
                    };
                    IndirectSalesOrderTransactionDAOs.Add(IndirectSalesOrderTransactionDAO);
                }
            }
            await DataContext.IndirectSalesOrderTransaction.BulkMergeAsync(IndirectSalesOrderTransactionDAOs);
        }

        public async Task<bool> UpdateState(IndirectSalesOrder IndirectSalesOrder)
        {
            await DataContext.IndirectSalesOrder.Where(x => x.Id == IndirectSalesOrder.Id)
                .UpdateFromQueryAsync(x => new IndirectSalesOrderDAO
                {
                    RequestStateId = IndirectSalesOrder.RequestStateId,
                    UpdatedAt = StaticParams.DateTimeNow
                });
            return true;
        }
    }
}
