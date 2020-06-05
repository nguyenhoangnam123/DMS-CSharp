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
    public interface IIndirectPriceListRepository
    {
        Task<int> Count(IndirectPriceListFilter IndirectPriceListFilter);
        Task<List<IndirectPriceList>> List(IndirectPriceListFilter IndirectPriceListFilter);
        Task<IndirectPriceList> Get(long Id);
        Task<bool> Create(IndirectPriceList IndirectPriceList);
        Task<bool> Update(IndirectPriceList IndirectPriceList);
        Task<bool> Delete(IndirectPriceList IndirectPriceList);
        Task<bool> BulkMerge(List<IndirectPriceList> IndirectPriceLists);
        Task<bool> BulkDelete(List<IndirectPriceList> IndirectPriceLists);
    }
    public class IndirectPriceListRepository : IIndirectPriceListRepository
    {
        private DataContext DataContext;
        public IndirectPriceListRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<IndirectPriceListDAO> DynamicFilter(IQueryable<IndirectPriceListDAO> query, IndirectPriceListFilter filter)
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
            if (filter.IndirectPriceListTypeId != null)
                query = query.Where(q => q.IndirectPriceListTypeId, filter.IndirectPriceListTypeId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<IndirectPriceListDAO> OrFilter(IQueryable<IndirectPriceListDAO> query, IndirectPriceListFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<IndirectPriceListDAO> initQuery = query.Where(q => false);
            foreach (IndirectPriceListFilter IndirectPriceListFilter in filter.OrFilter)
            {
                IQueryable<IndirectPriceListDAO> queryable = query;
                if (IndirectPriceListFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, IndirectPriceListFilter.Id);
                if (IndirectPriceListFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, IndirectPriceListFilter.Code);
                if (IndirectPriceListFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, IndirectPriceListFilter.Name);
                if (IndirectPriceListFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, IndirectPriceListFilter.StartDate);
                if (IndirectPriceListFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, IndirectPriceListFilter.EndDate);
                if (IndirectPriceListFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, IndirectPriceListFilter.StatusId);
                if (IndirectPriceListFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, IndirectPriceListFilter.OrganizationId);
                if (IndirectPriceListFilter.IndirectPriceListTypeId != null)
                    queryable = queryable.Where(q => q.IndirectPriceListTypeId, IndirectPriceListFilter.IndirectPriceListTypeId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<IndirectPriceListDAO> DynamicOrder(IQueryable<IndirectPriceListDAO> query, IndirectPriceListFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case IndirectPriceListOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case IndirectPriceListOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case IndirectPriceListOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case IndirectPriceListOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case IndirectPriceListOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case IndirectPriceListOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case IndirectPriceListOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case IndirectPriceListOrder.IndirectPriceListType:
                            query = query.OrderBy(q => q.IndirectPriceListTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case IndirectPriceListOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case IndirectPriceListOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case IndirectPriceListOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case IndirectPriceListOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case IndirectPriceListOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case IndirectPriceListOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case IndirectPriceListOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case IndirectPriceListOrder.IndirectPriceListType:
                            query = query.OrderByDescending(q => q.IndirectPriceListTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<IndirectPriceList>> DynamicSelect(IQueryable<IndirectPriceListDAO> query, IndirectPriceListFilter filter)
        {
            List<IndirectPriceList> IndirectPriceLists = await query.Select(q => new IndirectPriceList()
            {
                Id = filter.Selects.Contains(IndirectPriceListSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(IndirectPriceListSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(IndirectPriceListSelect.Name) ? q.Name : default(string),
                StartDate = filter.Selects.Contains(IndirectPriceListSelect.StartDate) ? q.StartDate : default(DateTime),
                EndDate = filter.Selects.Contains(IndirectPriceListSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(IndirectPriceListSelect.Status) ? q.StatusId : default(long),
                OrganizationId = filter.Selects.Contains(IndirectPriceListSelect.Organization) ? q.OrganizationId : default(long),
                IndirectPriceListTypeId = filter.Selects.Contains(IndirectPriceListSelect.IndirectPriceListType) ? q.IndirectPriceListTypeId : default(long),
                IndirectPriceListType = filter.Selects.Contains(IndirectPriceListSelect.IndirectPriceListType) && q.IndirectPriceListType != null ? new IndirectPriceListType
                {
                    Id = q.IndirectPriceListType.Id,
                    Code = q.IndirectPriceListType.Code,
                    Name = q.IndirectPriceListType.Name,
                } : null,
                Organization = filter.Selects.Contains(IndirectPriceListSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(IndirectPriceListSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return IndirectPriceLists;
        }

        public async Task<int> Count(IndirectPriceListFilter filter)
        {
            IQueryable<IndirectPriceListDAO> IndirectPriceLists = DataContext.IndirectPriceList.AsNoTracking();
            IndirectPriceLists = DynamicFilter(IndirectPriceLists, filter);
            return await IndirectPriceLists.CountAsync();
        }

        public async Task<List<IndirectPriceList>> List(IndirectPriceListFilter filter)
        {
            if (filter == null) return new List<IndirectPriceList>();
            IQueryable<IndirectPriceListDAO> IndirectPriceListDAOs = DataContext.IndirectPriceList.AsNoTracking();
            IndirectPriceListDAOs = DynamicFilter(IndirectPriceListDAOs, filter);
            IndirectPriceListDAOs = DynamicOrder(IndirectPriceListDAOs, filter);
            List<IndirectPriceList> IndirectPriceLists = await DynamicSelect(IndirectPriceListDAOs, filter);
            return IndirectPriceLists;
        }

        public async Task<IndirectPriceList> Get(long Id)
        {
            IndirectPriceList IndirectPriceList = await DataContext.IndirectPriceList.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new IndirectPriceList()
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
                IndirectPriceListTypeId = x.IndirectPriceListTypeId,
                IndirectPriceListType = x.IndirectPriceListType == null ? null : new IndirectPriceListType
                {
                    Id = x.IndirectPriceListType.Id,
                    Code = x.IndirectPriceListType.Code,
                    Name = x.IndirectPriceListType.Name,
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

            if (IndirectPriceList == null)
                return null;
            IndirectPriceList.IndirectPriceListItemMappings = await DataContext.IndirectPriceListItemMapping.AsNoTracking()
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new IndirectPriceListItemMapping
                {
                    IndirectPriceListId = x.IndirectPriceListId,
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
            IndirectPriceList.IndirectPriceListStoreGroupingMappings = await DataContext.IndirectPriceListStoreGroupingMapping.AsNoTracking()
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .Where(x => x.StoreGrouping.DeletedAt == null)
                .Select(x => new IndirectPriceListStoreGroupingMapping
                {
                    IndirectPriceListId = x.IndirectPriceListId,
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
            IndirectPriceList.IndirectPriceListStoreMappings = await DataContext.IndirectPriceListStoreMapping.AsNoTracking()
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .Where(x => x.Store.DeletedAt == null)
                .Select(x => new IndirectPriceListStoreMapping
                {
                    IndirectPriceListId = x.IndirectPriceListId,
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
                        TaxCode = x.Store.TaxCode,
                        LegalEntity = x.Store.LegalEntity,
                        StatusId = x.Store.StatusId,
                    },
                }).ToListAsync();
            IndirectPriceList.IndirectPriceListStoreTypeMappings = await DataContext.IndirectPriceListStoreTypeMapping.AsNoTracking()
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .Where(x => x.StoreType.DeletedAt == null)
                .Select(x => new IndirectPriceListStoreTypeMapping
                {
                    IndirectPriceListId = x.IndirectPriceListId,
                    StoreTypeId = x.StoreTypeId,
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                }).ToListAsync();

            return IndirectPriceList;
        }
        public async Task<bool> Create(IndirectPriceList IndirectPriceList)
        {
            IndirectPriceListDAO IndirectPriceListDAO = new IndirectPriceListDAO();
            IndirectPriceListDAO.Id = IndirectPriceList.Id;
            IndirectPriceListDAO.Code = IndirectPriceList.Code;
            IndirectPriceListDAO.Name = IndirectPriceList.Name;
            IndirectPriceListDAO.StartDate = IndirectPriceList.StartDate;
            IndirectPriceListDAO.EndDate = IndirectPriceList.EndDate;
            IndirectPriceListDAO.StatusId = IndirectPriceList.StatusId;
            IndirectPriceListDAO.OrganizationId = IndirectPriceList.OrganizationId;
            IndirectPriceListDAO.IndirectPriceListTypeId = IndirectPriceList.IndirectPriceListTypeId;
            IndirectPriceListDAO.CreatedAt = StaticParams.DateTimeNow;
            IndirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.IndirectPriceList.Add(IndirectPriceListDAO);
            await DataContext.SaveChangesAsync();
            IndirectPriceList.Id = IndirectPriceListDAO.Id;
            await SaveReference(IndirectPriceList);
            return true;
        }

        public async Task<bool> Update(IndirectPriceList IndirectPriceList)
        {
            IndirectPriceListDAO IndirectPriceListDAO = DataContext.IndirectPriceList.Where(x => x.Id == IndirectPriceList.Id).FirstOrDefault();
            if (IndirectPriceListDAO == null)
                return false;
            IndirectPriceListDAO.Id = IndirectPriceList.Id;
            IndirectPriceListDAO.Code = IndirectPriceList.Code;
            IndirectPriceListDAO.Name = IndirectPriceList.Name;
            IndirectPriceListDAO.StartDate = IndirectPriceList.StartDate;
            IndirectPriceListDAO.EndDate = IndirectPriceList.EndDate;
            IndirectPriceListDAO.StatusId = IndirectPriceList.StatusId;
            IndirectPriceListDAO.OrganizationId = IndirectPriceList.OrganizationId;
            IndirectPriceListDAO.IndirectPriceListTypeId = IndirectPriceList.IndirectPriceListTypeId;
            IndirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(IndirectPriceList);
            return true;
        }

        public async Task<bool> Delete(IndirectPriceList IndirectPriceList)
        {
            await DataContext.IndirectPriceList.Where(x => x.Id == IndirectPriceList.Id).UpdateFromQueryAsync(x => new IndirectPriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<IndirectPriceList> IndirectPriceLists)
        {
            List<IndirectPriceListDAO> IndirectPriceListDAOs = new List<IndirectPriceListDAO>();
            foreach (IndirectPriceList IndirectPriceList in IndirectPriceLists)
            {
                IndirectPriceListDAO IndirectPriceListDAO = new IndirectPriceListDAO();
                IndirectPriceListDAO.Id = IndirectPriceList.Id;
                IndirectPriceListDAO.Code = IndirectPriceList.Code;
                IndirectPriceListDAO.Name = IndirectPriceList.Name;
                IndirectPriceListDAO.StartDate = IndirectPriceList.StartDate;
                IndirectPriceListDAO.EndDate = IndirectPriceList.EndDate;
                IndirectPriceListDAO.StatusId = IndirectPriceList.StatusId;
                IndirectPriceListDAO.OrganizationId = IndirectPriceList.OrganizationId;
                IndirectPriceListDAO.IndirectPriceListTypeId = IndirectPriceList.IndirectPriceListTypeId;
                IndirectPriceListDAO.CreatedAt = StaticParams.DateTimeNow;
                IndirectPriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
                IndirectPriceListDAOs.Add(IndirectPriceListDAO);
            }
            await DataContext.BulkMergeAsync(IndirectPriceListDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<IndirectPriceList> IndirectPriceLists)
        {
            List<long> Ids = IndirectPriceLists.Select(x => x.Id).ToList();
            await DataContext.IndirectPriceList
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new IndirectPriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(IndirectPriceList IndirectPriceList)
        {
            await DataContext.IndirectPriceListItemMapping
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<IndirectPriceListItemMappingDAO> IndirectPriceListItemMappingDAOs = new List<IndirectPriceListItemMappingDAO>();
            if (IndirectPriceList.IndirectPriceListItemMappings != null)
            {
                foreach (IndirectPriceListItemMapping IndirectPriceListItemMapping in IndirectPriceList.IndirectPriceListItemMappings)
                {
                    IndirectPriceListItemMappingDAO IndirectPriceListItemMappingDAO = new IndirectPriceListItemMappingDAO();
                    IndirectPriceListItemMappingDAO.IndirectPriceListId = IndirectPriceList.Id;
                    IndirectPriceListItemMappingDAO.ItemId = IndirectPriceListItemMapping.ItemId;
                    IndirectPriceListItemMappingDAO.Price = IndirectPriceListItemMapping.Price;
                    IndirectPriceListItemMappingDAOs.Add(IndirectPriceListItemMappingDAO);
                }
                await DataContext.IndirectPriceListItemMapping.BulkMergeAsync(IndirectPriceListItemMappingDAOs);
            }
            await DataContext.IndirectPriceListStoreGroupingMapping
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<IndirectPriceListStoreGroupingMappingDAO> IndirectPriceListStoreGroupingMappingDAOs = new List<IndirectPriceListStoreGroupingMappingDAO>();
            if (IndirectPriceList.IndirectPriceListStoreGroupingMappings != null)
            {
                foreach (IndirectPriceListStoreGroupingMapping IndirectPriceListStoreGroupingMapping in IndirectPriceList.IndirectPriceListStoreGroupingMappings)
                {
                    IndirectPriceListStoreGroupingMappingDAO IndirectPriceListStoreGroupingMappingDAO = new IndirectPriceListStoreGroupingMappingDAO();
                    IndirectPriceListStoreGroupingMappingDAO.IndirectPriceListId = IndirectPriceList.Id;
                    IndirectPriceListStoreGroupingMappingDAO.StoreGroupingId = IndirectPriceListStoreGroupingMapping.StoreGroupingId;
                    IndirectPriceListStoreGroupingMappingDAOs.Add(IndirectPriceListStoreGroupingMappingDAO);
                }
                await DataContext.IndirectPriceListStoreGroupingMapping.BulkMergeAsync(IndirectPriceListStoreGroupingMappingDAOs);
            }
            await DataContext.IndirectPriceListStoreMapping
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<IndirectPriceListStoreMappingDAO> IndirectPriceListStoreMappingDAOs = new List<IndirectPriceListStoreMappingDAO>();
            if (IndirectPriceList.IndirectPriceListStoreMappings != null)
            {
                foreach (IndirectPriceListStoreMapping IndirectPriceListStoreMapping in IndirectPriceList.IndirectPriceListStoreMappings)
                {
                    IndirectPriceListStoreMappingDAO IndirectPriceListStoreMappingDAO = new IndirectPriceListStoreMappingDAO();
                    IndirectPriceListStoreMappingDAO.IndirectPriceListId = IndirectPriceList.Id;
                    IndirectPriceListStoreMappingDAO.StoreId = IndirectPriceListStoreMapping.StoreId;
                    IndirectPriceListStoreMappingDAOs.Add(IndirectPriceListStoreMappingDAO);
                }
                await DataContext.IndirectPriceListStoreMapping.BulkMergeAsync(IndirectPriceListStoreMappingDAOs);
            }
            await DataContext.IndirectPriceListStoreTypeMapping
                .Where(x => x.IndirectPriceListId == IndirectPriceList.Id)
                .DeleteFromQueryAsync();
            List<IndirectPriceListStoreTypeMappingDAO> IndirectPriceListStoreTypeMappingDAOs = new List<IndirectPriceListStoreTypeMappingDAO>();
            if (IndirectPriceList.IndirectPriceListStoreTypeMappings != null)
            {
                foreach (IndirectPriceListStoreTypeMapping IndirectPriceListStoreTypeMapping in IndirectPriceList.IndirectPriceListStoreTypeMappings)
                {
                    IndirectPriceListStoreTypeMappingDAO IndirectPriceListStoreTypeMappingDAO = new IndirectPriceListStoreTypeMappingDAO();
                    IndirectPriceListStoreTypeMappingDAO.IndirectPriceListId = IndirectPriceList.Id;
                    IndirectPriceListStoreTypeMappingDAO.StoreTypeId = IndirectPriceListStoreTypeMapping.StoreTypeId;
                    IndirectPriceListStoreTypeMappingDAOs.Add(IndirectPriceListStoreTypeMappingDAO);
                }
                await DataContext.IndirectPriceListStoreTypeMapping.BulkMergeAsync(IndirectPriceListStoreTypeMappingDAOs);
            }
        }
        
    }
}
