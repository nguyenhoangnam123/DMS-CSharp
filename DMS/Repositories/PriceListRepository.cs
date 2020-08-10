using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IPriceListRepository
    {
        Task<int> Count(PriceListFilter PriceListFilter);
        Task<List<PriceList>> List(PriceListFilter PriceListFilter);
        Task<PriceList> Get(long Id);
        Task<bool> Create(PriceList PriceList);
        Task<bool> Update(PriceList PriceList);
        Task<bool> Delete(PriceList PriceList);
        Task<bool> BulkMerge(List<PriceList> PriceLists);
        Task<bool> BulkDelete(List<PriceList> PriceLists);
    }
    public class PriceListRepository : IPriceListRepository
    {
        private DataContext DataContext;
        public PriceListRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PriceListDAO> DynamicFilter(IQueryable<PriceListDAO> query, PriceListFilter filter)
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
            if (filter.PriceListTypeId != null)
                query = query.Where(q => q.PriceListTypeId, filter.PriceListTypeId);
            if (filter.SalesOrderTypeId != null)
                query = query.Where(q => q.SalesOrderTypeId, filter.SalesOrderTypeId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PriceListDAO> OrFilter(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PriceListDAO> initQuery = query.Where(q => false);
            foreach (PriceListFilter PriceListFilter in filter.OrFilter)
            {
                IQueryable<PriceListDAO> queryable = query;
                if (PriceListFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PriceListFilter.Id);
                if (PriceListFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, PriceListFilter.Code);
                if (PriceListFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PriceListFilter.Name);
                if (PriceListFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, PriceListFilter.StartDate);
                if (PriceListFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, PriceListFilter.EndDate);
                if (PriceListFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, PriceListFilter.StatusId);
                if (PriceListFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, PriceListFilter.OrganizationId);
                if (PriceListFilter.PriceListTypeId != null)
                    queryable = queryable.Where(q => q.PriceListTypeId, PriceListFilter.PriceListTypeId);
                if (PriceListFilter.SalesOrderTypeId != null)
                    queryable = queryable.Where(q => q.SalesOrderTypeId, PriceListFilter.SalesOrderTypeId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<PriceListDAO> DynamicOrder(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PriceListOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PriceListOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PriceListOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PriceListOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case PriceListOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case PriceListOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case PriceListOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case PriceListOrder.PriceListType:
                            query = query.OrderBy(q => q.PriceListTypeId);
                            break;
                        case PriceListOrder.SalesOrderType:
                            query = query.OrderBy(q => q.SalesOrderTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PriceListOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PriceListOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PriceListOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PriceListOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case PriceListOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case PriceListOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case PriceListOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case PriceListOrder.PriceListType:
                            query = query.OrderByDescending(q => q.PriceListTypeId);
                            break;
                        case PriceListOrder.SalesOrderType:
                            query = query.OrderByDescending(q => q.SalesOrderTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PriceList>> DynamicSelect(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            List<PriceList> PriceLists = await query.Select(q => new PriceList()
            {
                Id = filter.Selects.Contains(PriceListSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PriceListSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PriceListSelect.Name) ? q.Name : default(string),
                StartDate = filter.Selects.Contains(PriceListSelect.StartDate) ? q.StartDate : default(DateTime?),
                EndDate = filter.Selects.Contains(PriceListSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(PriceListSelect.Status) ? q.StatusId : default(long),
                OrganizationId = filter.Selects.Contains(PriceListSelect.Organization) ? q.OrganizationId : default(long),
                PriceListTypeId = filter.Selects.Contains(PriceListSelect.PriceListType) ? q.PriceListTypeId : default(long),
                SalesOrderTypeId = filter.Selects.Contains(PriceListSelect.SalesOrderType) ? q.SalesOrderTypeId : default(long),
                PriceListType = filter.Selects.Contains(PriceListSelect.PriceListType) && q.PriceListType != null ? new PriceListType
                {
                    Id = q.PriceListType.Id,
                    Code = q.PriceListType.Code,
                    Name = q.PriceListType.Name,
                } : null,
                SalesOrderType = filter.Selects.Contains(PriceListSelect.SalesOrderType) && q.SalesOrderType != null ? new SalesOrderType
                {
                    Id = q.SalesOrderType.Id,
                    Code = q.SalesOrderType.Code,
                    Name = q.SalesOrderType.Name,
                } : null,
                Organization = filter.Selects.Contains(PriceListSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(PriceListSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return PriceLists;
        }

        public async Task<int> Count(PriceListFilter filter)
        {
            IQueryable<PriceListDAO> PriceLists = DataContext.PriceList.AsNoTracking();
            PriceLists = DynamicFilter(PriceLists, filter);
            return await PriceLists.CountAsync();
        }

        public async Task<List<PriceList>> List(PriceListFilter filter)
        {
            if (filter == null) return new List<PriceList>();
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            PriceListDAOs = DynamicOrder(PriceListDAOs, filter);
            List<PriceList> PriceLists = await DynamicSelect(PriceListDAOs, filter);
            return PriceLists;
        }

        public async Task<PriceList> Get(long Id)
        {
            PriceList PriceList = await DataContext.PriceList.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PriceList()
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
                PriceListTypeId = x.PriceListTypeId,
                SalesOrderTypeId = x.SalesOrderTypeId,
                PriceListType = x.PriceListType == null ? null : new PriceListType
                {
                    Id = x.PriceListType.Id,
                    Code = x.PriceListType.Code,
                    Name = x.PriceListType.Name,
                },
                SalesOrderType = x.SalesOrderType == null ? null : new SalesOrderType
                {
                    Id = x.SalesOrderType.Id,
                    Code = x.SalesOrderType.Code,
                    Name = x.SalesOrderType.Name,
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

            if (PriceList == null)
                return null;
            PriceList.PriceListItemMappings = await DataContext.PriceListItemMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new PriceListItemMapping
                {
                    PriceListId = x.PriceListId,
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
            PriceList.PriceListStoreGroupingMappings = await DataContext.PriceListStoreGroupingMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.StoreGrouping.DeletedAt == null)
                .Select(x => new PriceListStoreGroupingMapping
                {
                    PriceListId = x.PriceListId,
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
            PriceList.PriceListStoreMappings = await DataContext.PriceListStoreMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.Store.DeletedAt == null)
                .Select(x => new PriceListStoreMapping
                {
                    PriceListId = x.PriceListId,
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
            PriceList.PriceListStoreTypeMappings = await DataContext.PriceListStoreTypeMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.StoreType.DeletedAt == null)
                .Select(x => new PriceListStoreTypeMapping
                {
                    PriceListId = x.PriceListId,
                    StoreTypeId = x.StoreTypeId,
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                }).ToListAsync();

            return PriceList;
        }
        public async Task<bool> Create(PriceList PriceList)
        {
            PriceListDAO PriceListDAO = new PriceListDAO();
            PriceListDAO.Id = PriceList.Id;
            PriceListDAO.Code = PriceList.Code;
            PriceListDAO.Name = PriceList.Name;
            PriceListDAO.StartDate = PriceList.StartDate;
            PriceListDAO.EndDate = PriceList.EndDate;
            PriceListDAO.StatusId = PriceList.StatusId;
            PriceListDAO.OrganizationId = PriceList.OrganizationId;
            PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
            PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
            PriceListDAO.CreatedAt = StaticParams.DateTimeNow;
            PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.PriceList.Add(PriceListDAO);
            await DataContext.SaveChangesAsync();
            PriceList.Id = PriceListDAO.Id;
            await SaveReference(PriceList);
            return true;
        }

        public async Task<bool> Update(PriceList PriceList)
        {
            PriceListDAO PriceListDAO = DataContext.PriceList.Where(x => x.Id == PriceList.Id).FirstOrDefault();
            if (PriceListDAO == null)
                return false;
            PriceListDAO.Id = PriceList.Id;
            PriceListDAO.Code = PriceList.Code;
            PriceListDAO.Name = PriceList.Name;
            PriceListDAO.StartDate = PriceList.StartDate;
            PriceListDAO.EndDate = PriceList.EndDate;
            PriceListDAO.StatusId = PriceList.StatusId;
            PriceListDAO.OrganizationId = PriceList.OrganizationId;
            PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
            PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
            PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(PriceList);
            return true;
        }

        public async Task<bool> Delete(PriceList PriceList)
        {
            await DataContext.PriceList.Where(x => x.Id == PriceList.Id).UpdateFromQueryAsync(x => new PriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<PriceList> PriceLists)
        {
            List<PriceListDAO> PriceListDAOs = new List<PriceListDAO>();
            foreach (PriceList PriceList in PriceLists)
            {
                PriceListDAO PriceListDAO = new PriceListDAO();
                PriceListDAO.Id = PriceList.Id;
                PriceListDAO.Code = PriceList.Code;
                PriceListDAO.Name = PriceList.Name;
                PriceListDAO.StartDate = PriceList.StartDate;
                PriceListDAO.EndDate = PriceList.EndDate;
                PriceListDAO.StatusId = PriceList.StatusId;
                PriceListDAO.OrganizationId = PriceList.OrganizationId;
                PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
                PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
                PriceListDAO.CreatedAt = StaticParams.DateTimeNow;
                PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
                PriceListDAOs.Add(PriceListDAO);
            }
            await DataContext.BulkMergeAsync(PriceListDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PriceList> PriceLists)
        {
            List<long> Ids = PriceLists.Select(x => x.Id).ToList();
            await DataContext.PriceList
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(PriceList PriceList)
        {
            await DataContext.PriceListItemMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListItemMappingDAO> PriceListItemMappingDAOs = new List<PriceListItemMappingDAO>();
            if (PriceList.PriceListItemMappings != null)
            {
                foreach (PriceListItemMapping PriceListItemMapping in PriceList.PriceListItemMappings)
                {
                    PriceListItemMappingDAO PriceListItemMappingDAO = new PriceListItemMappingDAO();
                    PriceListItemMappingDAO.PriceListId = PriceList.Id;
                    PriceListItemMappingDAO.ItemId = PriceListItemMapping.ItemId;
                    PriceListItemMappingDAO.Price = PriceListItemMapping.Price;
                    PriceListItemMappingDAOs.Add(PriceListItemMappingDAO);
                }
                await DataContext.PriceListItemMapping.BulkMergeAsync(PriceListItemMappingDAOs);
            }
            await DataContext.PriceListStoreGroupingMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreGroupingMappingDAO> PriceListStoreGroupingMappingDAOs = new List<PriceListStoreGroupingMappingDAO>();
            if (PriceList.PriceListStoreGroupingMappings != null)
            {
                foreach (PriceListStoreGroupingMapping PriceListStoreGroupingMapping in PriceList.PriceListStoreGroupingMappings)
                {
                    PriceListStoreGroupingMappingDAO PriceListStoreGroupingMappingDAO = new PriceListStoreGroupingMappingDAO();
                    PriceListStoreGroupingMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreGroupingMappingDAO.StoreGroupingId = PriceListStoreGroupingMapping.StoreGroupingId;
                    PriceListStoreGroupingMappingDAOs.Add(PriceListStoreGroupingMappingDAO);
                }
                await DataContext.PriceListStoreGroupingMapping.BulkMergeAsync(PriceListStoreGroupingMappingDAOs);
            }
            await DataContext.PriceListStoreMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreMappingDAO> PriceListStoreMappingDAOs = new List<PriceListStoreMappingDAO>();
            if (PriceList.PriceListStoreMappings != null)
            {
                foreach (PriceListStoreMapping PriceListStoreMapping in PriceList.PriceListStoreMappings)
                {
                    PriceListStoreMappingDAO PriceListStoreMappingDAO = new PriceListStoreMappingDAO();
                    PriceListStoreMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreMappingDAO.StoreId = PriceListStoreMapping.StoreId;
                    PriceListStoreMappingDAOs.Add(PriceListStoreMappingDAO);
                }
                await DataContext.PriceListStoreMapping.BulkMergeAsync(PriceListStoreMappingDAOs);
            }
            await DataContext.PriceListStoreTypeMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreTypeMappingDAO> PriceListStoreTypeMappingDAOs = new List<PriceListStoreTypeMappingDAO>();
            if (PriceList.PriceListStoreTypeMappings != null)
            {
                foreach (PriceListStoreTypeMapping PriceListStoreTypeMapping in PriceList.PriceListStoreTypeMappings)
                {
                    PriceListStoreTypeMappingDAO PriceListStoreTypeMappingDAO = new PriceListStoreTypeMappingDAO();
                    PriceListStoreTypeMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreTypeMappingDAO.StoreTypeId = PriceListStoreTypeMapping.StoreTypeId;
                    PriceListStoreTypeMappingDAOs.Add(PriceListStoreTypeMappingDAO);
                }
                await DataContext.PriceListStoreTypeMapping.BulkMergeAsync(PriceListStoreTypeMappingDAOs);
            }
        }

    }
}
