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
    public interface IDirectPriceListRepository
    {
        Task<int> Count(DirectPriceListFilter DirectPriceListFilter);
        Task<List<DirectPriceList>> List(DirectPriceListFilter DirectPriceListFilter);
        Task<DirectPriceList> Get(long Id);
        Task<bool> Create(DirectPriceList DirectPriceList);
        Task<bool> Update(DirectPriceList DirectPriceList);
        Task<bool> Delete(DirectPriceList DirectPriceList);
        Task<bool> BulkMerge(List<DirectPriceList> DirectPriceLists);
        Task<bool> BulkDelete(List<DirectPriceList> DirectPriceLists);
    }
    public class DirectPriceListRepository : IDirectPriceListRepository
    {
        private DataContext DataContext;
        public DirectPriceListRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DirectPriceListDAO> DynamicFilter(IQueryable<DirectPriceListDAO> query, DirectPriceListFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate, filter.EndDate);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.DirectPriceListTypeId != null)
                query = query.Where(q => q.DirectPriceListTypeId, filter.DirectPriceListTypeId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<DirectPriceListDAO> OrFilter(IQueryable<DirectPriceListDAO> query, DirectPriceListFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<DirectPriceListDAO> initQuery = query.Where(q => false);
            foreach (DirectPriceListFilter DirectPriceListFilter in filter.OrFilter)
            {
                IQueryable<DirectPriceListDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, filter.StartDate);
                if (filter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, filter.EndDate);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.DirectPriceListTypeId != null)
                    queryable = queryable.Where(q => q.DirectPriceListTypeId, filter.DirectPriceListTypeId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<DirectPriceListDAO> DynamicOrder(IQueryable<DirectPriceListDAO> query, DirectPriceListFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case DirectPriceListOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case DirectPriceListOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case DirectPriceListOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case DirectPriceListOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case DirectPriceListOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case DirectPriceListOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case DirectPriceListOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case DirectPriceListOrder.DirectPriceListType:
                            query = query.OrderBy(q => q.DirectPriceListTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case DirectPriceListOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case DirectPriceListOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case DirectPriceListOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case DirectPriceListOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case DirectPriceListOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case DirectPriceListOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case DirectPriceListOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case DirectPriceListOrder.DirectPriceListType:
                            query = query.OrderByDescending(q => q.DirectPriceListTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<DirectPriceList>> DynamicSelect(IQueryable<DirectPriceListDAO> query, DirectPriceListFilter filter)
        {
            List<DirectPriceList> DirectPriceLists = await query.Select(q => new DirectPriceList()
            {
                Id = filter.Selects.Contains(DirectPriceListSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(DirectPriceListSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(DirectPriceListSelect.Name) ? q.Name : default(string),
                StartDate = filter.Selects.Contains(DirectPriceListSelect.StartDate) ? q.StartDate : default(DateTime),
                EndDate = filter.Selects.Contains(DirectPriceListSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(DirectPriceListSelect.Status) ? q.StatusId : default(long),
                OrganizationId = filter.Selects.Contains(DirectPriceListSelect.Organization) ? q.OrganizationId : default(long),
                DirectPriceListTypeId = filter.Selects.Contains(DirectPriceListSelect.DirectPriceListType) ? q.DirectPriceListTypeId : default(long),
                DirectPriceListType = filter.Selects.Contains(DirectPriceListSelect.DirectPriceListType) && q.DirectPriceListType != null ? new DirectPriceListType
                {
                    Id = q.DirectPriceListType.Id,
                    Code = q.DirectPriceListType.Code,
                    Name = q.DirectPriceListType.Name,
                } : null,
                Organization = filter.Selects.Contains(DirectPriceListSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Email = q.Organization.Email,
                    Address = q.Organization.Address,
                } : null,
                Status = filter.Selects.Contains(DirectPriceListSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return DirectPriceLists;
        }

        public async Task<int> Count(DirectPriceListFilter filter)
        {
            IQueryable<DirectPriceListDAO> DirectPriceLists = DataContext.DirectPriceList.AsNoTracking();
            DirectPriceLists = DynamicFilter(DirectPriceLists, filter);
            return await DirectPriceLists.CountAsync();
        }

        public async Task<List<DirectPriceList>> List(DirectPriceListFilter filter)
        {
            if (filter == null) return new List<DirectPriceList>();
            IQueryable<DirectPriceListDAO> DirectPriceListDAOs = DataContext.DirectPriceList.AsNoTracking();
            DirectPriceListDAOs = DynamicFilter(DirectPriceListDAOs, filter);
            DirectPriceListDAOs = DynamicOrder(DirectPriceListDAOs, filter);
            List<DirectPriceList> DirectPriceLists = await DynamicSelect(DirectPriceListDAOs, filter);
            return DirectPriceLists;
        }

        public async Task<DirectPriceList> Get(long Id)
        {
            DirectPriceList DirectPriceList = await DataContext.DirectPriceList.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new DirectPriceList()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StatusId = x.StatusId,
                OrganizationId = x.OrganizationId,
                DirectPriceListTypeId = x.DirectPriceListTypeId,
                DirectPriceListType = x.DirectPriceListType == null ? null : new DirectPriceListType
                {
                    Id = x.DirectPriceListType.Id,
                    Code = x.DirectPriceListType.Code,
                    Name = x.DirectPriceListType.Name,
                },
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                    Phone = x.Organization.Phone,
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (DirectPriceList == null)
                return null;
            DirectPriceList.DirectPriceListItemMappings = await DataContext.DirectPriceListItemMapping.AsNoTracking()
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new DirectPriceListItemMapping
                {
                    DirectPriceListId = x.DirectPriceListId,
                    ItemId = x.ItemId,
                    Price = x.Price,
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
                }).ToListAsync();
            DirectPriceList.DirectPriceListStoreGroupingMappings = await DataContext.DirectPriceListStoreGroupingMapping.AsNoTracking()
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .Where(x => x.StoreGrouping.DeletedAt == null)
                .Select(x => new DirectPriceListStoreGroupingMapping
                {
                    DirectPriceListId = x.DirectPriceListId,
                    StoreGroupingId = x.StoreGroupingId,
                    StoreGrouping = new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                        StatusId = x.StoreGrouping.StatusId,
                    },
                }).ToListAsync();
            DirectPriceList.DirectPriceListStoreMappings = await DataContext.DirectPriceListStoreMapping.AsNoTracking()
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .Where(x => x.Store.DeletedAt == null)
                .Select(x => new DirectPriceListStoreMapping
                {
                    DirectPriceListId = x.DirectPriceListId,
                    StoreId = x.StoreId,
                    Store = new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        ParentStoreId = x.Store.ParentStoreId,
                        OrganizationId = x.Store.OrganizationId,
                        StoreTypeId = x.Store.StoreTypeId,
                        StoreGroupingId = x.Store.StoreGroupingId,
                        ResellerId = x.Store.ResellerId,
                        Telephone = x.Store.Telephone,
                        ProvinceId = x.Store.ProvinceId,
                        DistrictId = x.Store.DistrictId,
                        WardId = x.Store.WardId,
                        Address = x.Store.Address,
                        DeliveryAddress = x.Store.DeliveryAddress,
                        Latitude = x.Store.Latitude,
                        Longitude = x.Store.Longitude,
                        DeliveryLatitude = x.Store.DeliveryLatitude,
                        DeliveryLongitude = x.Store.DeliveryLongitude,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                        StatusId = x.Store.StatusId,
                        WorkflowDefinitionId = x.Store.WorkflowDefinitionId,
                        RequestStateId = x.Store.RequestStateId,
                    },
                }).ToListAsync();
            DirectPriceList.DirectPriceListStoreTypeMappings = await DataContext.DirectPriceListStoreTypeMapping.AsNoTracking()
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .Where(x => x.StoreType.DeletedAt == null)
                .Select(x => new DirectPriceListStoreTypeMapping
                {
                    DirectPriceListId = x.DirectPriceListId,
                    StoreTypeId = x.StoreTypeId,
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                }).ToListAsync();

            return DirectPriceList;
        }
        public async Task<bool> Create(DirectPriceList DirectPriceList)
        {
            DirectPriceListDAO DirectPriceListDAO = new DirectPriceListDAO();
            DirectPriceListDAO.Id = DirectPriceList.Id;
            DirectPriceListDAO.Code = DirectPriceList.Code;
            DirectPriceListDAO.Name = DirectPriceList.Name;
            DirectPriceListDAO.StartDate = DirectPriceList.StartDate;
            DirectPriceListDAO.EndDate = DirectPriceList.EndDate;
            DirectPriceListDAO.StatusId = DirectPriceList.StatusId;
            DirectPriceListDAO.OrganizationId = DirectPriceList.OrganizationId;
            DirectPriceListDAO.DirectPriceListTypeId = DirectPriceList.DirectPriceListTypeId;
            DirectPriceListDAO.CreatedAt = StaticParams.DateTimeNow;
            DirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.DirectPriceList.Add(DirectPriceListDAO);
            await DataContext.SaveChangesAsync();
            DirectPriceList.Id = DirectPriceListDAO.Id;
            await SaveReference(DirectPriceList);
            return true;
        }

        public async Task<bool> Update(DirectPriceList DirectPriceList)
        {
            DirectPriceListDAO DirectPriceListDAO = DataContext.DirectPriceList.Where(x => x.Id == DirectPriceList.Id).FirstOrDefault();
            if (DirectPriceListDAO == null)
                return false;
            DirectPriceListDAO.Id = DirectPriceList.Id;
            DirectPriceListDAO.Code = DirectPriceList.Code;
            DirectPriceListDAO.Name = DirectPriceList.Name;
            DirectPriceListDAO.StartDate = DirectPriceList.StartDate;
            DirectPriceListDAO.EndDate = DirectPriceList.EndDate;
            DirectPriceListDAO.StatusId = DirectPriceList.StatusId;
            DirectPriceListDAO.OrganizationId = DirectPriceList.OrganizationId;
            DirectPriceListDAO.DirectPriceListTypeId = DirectPriceList.DirectPriceListTypeId;
            DirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(DirectPriceList);
            return true;
        }

        public async Task<bool> Delete(DirectPriceList DirectPriceList)
        {
            await DataContext.DirectPriceList.Where(x => x.Id == DirectPriceList.Id).UpdateFromQueryAsync(x => new DirectPriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<DirectPriceList> DirectPriceLists)
        {
            List<DirectPriceListDAO> DirectPriceListDAOs = new List<DirectPriceListDAO>();
            foreach (DirectPriceList DirectPriceList in DirectPriceLists)
            {
                DirectPriceListDAO DirectPriceListDAO = new DirectPriceListDAO();
                DirectPriceListDAO.Id = DirectPriceList.Id;
                DirectPriceListDAO.Code = DirectPriceList.Code;
                DirectPriceListDAO.Name = DirectPriceList.Name;
                DirectPriceListDAO.StartDate = DirectPriceList.StartDate;
                DirectPriceListDAO.EndDate = DirectPriceList.EndDate;
                DirectPriceListDAO.StatusId = DirectPriceList.StatusId;
                DirectPriceListDAO.OrganizationId = DirectPriceList.OrganizationId;
                DirectPriceListDAO.DirectPriceListTypeId = DirectPriceList.DirectPriceListTypeId;
                DirectPriceListDAO.CreatedAt = StaticParams.DateTimeNow;
                DirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
                DirectPriceListDAOs.Add(DirectPriceListDAO);
            }
            await DataContext.BulkMergeAsync(DirectPriceListDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<DirectPriceList> DirectPriceLists)
        {
            List<long> Ids = DirectPriceLists.Select(x => x.Id).ToList();
            await DataContext.DirectPriceList
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new DirectPriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(DirectPriceList DirectPriceList)
        {
            await DataContext.DirectPriceListItemMapping
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<DirectPriceListItemMappingDAO> DirectPriceListItemMappingDAOs = new List<DirectPriceListItemMappingDAO>();
            if (DirectPriceList.DirectPriceListItemMappings != null)
            {
                foreach (DirectPriceListItemMapping DirectPriceListItemMapping in DirectPriceList.DirectPriceListItemMappings)
                {
                    DirectPriceListItemMappingDAO DirectPriceListItemMappingDAO = new DirectPriceListItemMappingDAO();
                    DirectPriceListItemMappingDAO.DirectPriceListId = DirectPriceList.Id;
                    DirectPriceListItemMappingDAO.ItemId = DirectPriceListItemMapping.ItemId;
                    DirectPriceListItemMappingDAO.Price = DirectPriceListItemMapping.Price;
                    DirectPriceListItemMappingDAOs.Add(DirectPriceListItemMappingDAO);
                }
                await DataContext.DirectPriceListItemMapping.BulkMergeAsync(DirectPriceListItemMappingDAOs);
            }
            await DataContext.DirectPriceListStoreGroupingMapping
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<DirectPriceListStoreGroupingMappingDAO> DirectPriceListStoreGroupingMappingDAOs = new List<DirectPriceListStoreGroupingMappingDAO>();
            if (DirectPriceList.DirectPriceListStoreGroupingMappings != null)
            {
                foreach (DirectPriceListStoreGroupingMapping DirectPriceListStoreGroupingMapping in DirectPriceList.DirectPriceListStoreGroupingMappings)
                {
                    DirectPriceListStoreGroupingMappingDAO DirectPriceListStoreGroupingMappingDAO = new DirectPriceListStoreGroupingMappingDAO();
                    DirectPriceListStoreGroupingMappingDAO.DirectPriceListId = DirectPriceList.Id;
                    DirectPriceListStoreGroupingMappingDAO.StoreGroupingId = DirectPriceListStoreGroupingMapping.StoreGroupingId;
                    DirectPriceListStoreGroupingMappingDAOs.Add(DirectPriceListStoreGroupingMappingDAO);
                }
                await DataContext.DirectPriceListStoreGroupingMapping.BulkMergeAsync(DirectPriceListStoreGroupingMappingDAOs);
            }
            await DataContext.DirectPriceListStoreMapping
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<DirectPriceListStoreMappingDAO> DirectPriceListStoreMappingDAOs = new List<DirectPriceListStoreMappingDAO>();
            if (DirectPriceList.DirectPriceListStoreMappings != null)
            {
                foreach (DirectPriceListStoreMapping DirectPriceListStoreMapping in DirectPriceList.DirectPriceListStoreMappings)
                {
                    DirectPriceListStoreMappingDAO DirectPriceListStoreMappingDAO = new DirectPriceListStoreMappingDAO();
                    DirectPriceListStoreMappingDAO.DirectPriceListId = DirectPriceList.Id;
                    DirectPriceListStoreMappingDAO.StoreId = DirectPriceListStoreMapping.StoreId;
                    DirectPriceListStoreMappingDAOs.Add(DirectPriceListStoreMappingDAO);
                }
                await DataContext.DirectPriceListStoreMapping.BulkMergeAsync(DirectPriceListStoreMappingDAOs);
            }
            await DataContext.DirectPriceListStoreTypeMapping
                .Where(x => x.DirectPriceListId == DirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<DirectPriceListStoreTypeMappingDAO> DirectPriceListStoreTypeMappingDAOs = new List<DirectPriceListStoreTypeMappingDAO>();
            if (DirectPriceList.DirectPriceListStoreTypeMappings != null)
            {
                foreach (DirectPriceListStoreTypeMapping DirectPriceListStoreTypeMapping in DirectPriceList.DirectPriceListStoreTypeMappings)
                {
                    DirectPriceListStoreTypeMappingDAO DirectPriceListStoreTypeMappingDAO = new DirectPriceListStoreTypeMappingDAO();
                    DirectPriceListStoreTypeMappingDAO.DirectPriceListId = DirectPriceList.Id;
                    DirectPriceListStoreTypeMappingDAO.StoreTypeId = DirectPriceListStoreTypeMapping.StoreTypeId;
                    DirectPriceListStoreTypeMappingDAOs.Add(DirectPriceListStoreTypeMappingDAO);
                }
                await DataContext.DirectPriceListStoreTypeMapping.BulkMergeAsync(DirectPriceListStoreTypeMappingDAOs);
            }
        }
        
    }
}
