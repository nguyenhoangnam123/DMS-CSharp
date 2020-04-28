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
    public interface IStoreRepository
    {
        Task<int> Count(StoreFilter StoreFilter);
        Task<List<Store>> List(StoreFilter StoreFilter);
        Task<Store> Get(long Id);
        Task<bool> Create(Store Store);
        Task<bool> Update(Store Store);
        Task<bool> Delete(Store Store);
        Task<bool> BulkMerge(List<Store> Stores);
        Task<bool> BulkDelete(List<Store> Stores);
    }
    public class StoreRepository : IStoreRepository
    {
        private DataContext DataContext;
        public StoreRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreDAO> DynamicFilter(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ParentStoreId != null)
                query = query.Where(q => q.ParentStoreId, filter.ParentStoreId);
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.StoreTypeId != null)
                query = query.Where(q => q.StoreTypeId, filter.StoreTypeId);
            if (filter.StoreGroupingId != null)
                query = query.Where(q => q.StoreGroupingId, filter.StoreGroupingId);
            if (filter.ResellerId != null)
                query = query.Where(q => q.ResellerId, filter.ResellerId);
            if (filter.Telephone != null)
                query = query.Where(q => q.Telephone, filter.Telephone);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.WardId != null)
                query = query.Where(q => q.WardId, filter.WardId);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.DeliveryAddress != null)
                query = query.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longitude != null)
                query = query.Where(q => q.Longitude, filter.Longitude);
            if (filter.DeliveryLatitude != null)
                query = query.Where(q => q.DeliveryLatitude, filter.DeliveryLatitude);
            if (filter.DeliveryLongitude != null)
                query = query.Where(q => q.DeliveryLongitude, filter.DeliveryLongitude);
            if (filter.OwnerName != null)
                query = query.Where(q => q.OwnerName, filter.OwnerName);
            if (filter.OwnerPhone != null)
                query = query.Where(q => q.OwnerPhone, filter.OwnerPhone);
            if (filter.OwnerEmail != null)
                query = query.Where(q => q.OwnerEmail, filter.OwnerEmail);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.StoreStatusId != null)
                query = query.Where(q => q.StoreStatusId, filter.StoreStatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<StoreDAO> OrFilter(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreDAO> initQuery = query.Where(q => false);
            foreach (StoreFilter StoreFilter in filter.OrFilter)
            {
                IQueryable<StoreDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ParentStoreId != null)
                    queryable = queryable.Where(q => q.ParentStoreId, filter.ParentStoreId);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.StoreTypeId != null)
                    queryable = queryable.Where(q => q.StoreTypeId, filter.StoreTypeId);
                if (filter.StoreGroupingId != null)
                    queryable = queryable.Where(q => q.StoreGroupingId, filter.StoreGroupingId);
                if (filter.ResellerId != null)
                    queryable = queryable.Where(q => q.ResellerId, filter.ResellerId);
                if (filter.Telephone != null)
                    queryable = queryable.Where(q => q.Telephone, filter.Telephone);
                if (filter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                if (filter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                if (filter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, filter.WardId);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.DeliveryAddress != null)
                    queryable = queryable.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
                if (filter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, filter.Latitude);
                if (filter.Longitude != null)
                    queryable = queryable.Where(q => q.Longitude, filter.Longitude);
                if (filter.DeliveryLatitude != null)
                    queryable = queryable.Where(q => q.DeliveryLatitude, filter.DeliveryLatitude);
                if (filter.DeliveryLongitude != null)
                    queryable = queryable.Where(q => q.DeliveryLongitude, filter.DeliveryLongitude);
                if (filter.OwnerName != null)
                    queryable = queryable.Where(q => q.OwnerName, filter.OwnerName);
                if (filter.OwnerPhone != null)
                    queryable = queryable.Where(q => q.OwnerPhone, filter.OwnerPhone);
                if (filter.OwnerEmail != null)
                    queryable = queryable.Where(q => q.OwnerEmail, filter.OwnerEmail);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.StoreStatusId != null)
                    queryable = queryable.Where(q => q.StoreStatusId, filter.StoreStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<StoreDAO> DynamicOrder(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreOrder.ParentStore:
                            query = query.OrderBy(q => q.ParentStoreId);
                            break;
                        case StoreOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case StoreOrder.StoreType:
                            query = query.OrderBy(q => q.StoreTypeId);
                            break;
                        case StoreOrder.StoreGrouping:
                            query = query.OrderBy(q => q.StoreGroupingId);
                            break;
                        case StoreOrder.Reseller:
                            query = query.OrderBy(q => q.ResellerId);
                            break;
                        case StoreOrder.Telephone:
                            query = query.OrderBy(q => q.Telephone);
                            break;
                        case StoreOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case StoreOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case StoreOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case StoreOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case StoreOrder.DeliveryAddress:
                            query = query.OrderBy(q => q.DeliveryAddress);
                            break;
                        case StoreOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case StoreOrder.Longitude:
                            query = query.OrderBy(q => q.Longitude);
                            break;
                        case StoreOrder.DeliveryLatitude:
                            query = query.OrderBy(q => q.DeliveryLatitude);
                            break;
                        case StoreOrder.DeliveryLongitude:
                            query = query.OrderBy(q => q.DeliveryLongitude);
                            break;
                        case StoreOrder.OwnerName:
                            query = query.OrderBy(q => q.OwnerName);
                            break;
                        case StoreOrder.OwnerPhone:
                            query = query.OrderBy(q => q.OwnerPhone);
                            break;
                        case StoreOrder.OwnerEmail:
                            query = query.OrderBy(q => q.OwnerEmail);
                            break;
                        case StoreOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreOrder.ParentStore:
                            query = query.OrderByDescending(q => q.ParentStoreId);
                            break;
                        case StoreOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case StoreOrder.StoreType:
                            query = query.OrderByDescending(q => q.StoreTypeId);
                            break;
                        case StoreOrder.StoreGrouping:
                            query = query.OrderByDescending(q => q.StoreGroupingId);
                            break;
                        case StoreOrder.Reseller:
                            query = query.OrderByDescending(q => q.ResellerId);
                            break;
                        case StoreOrder.Telephone:
                            query = query.OrderByDescending(q => q.Telephone);
                            break;
                        case StoreOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case StoreOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case StoreOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case StoreOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case StoreOrder.DeliveryAddress:
                            query = query.OrderByDescending(q => q.DeliveryAddress);
                            break;
                        case StoreOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case StoreOrder.Longitude:
                            query = query.OrderByDescending(q => q.Longitude);
                            break;
                        case StoreOrder.DeliveryLatitude:
                            query = query.OrderByDescending(q => q.DeliveryLatitude);
                            break;
                        case StoreOrder.DeliveryLongitude:
                            query = query.OrderByDescending(q => q.DeliveryLongitude);
                            break;
                        case StoreOrder.OwnerName:
                            query = query.OrderByDescending(q => q.OwnerName);
                            break;
                        case StoreOrder.OwnerPhone:
                            query = query.OrderByDescending(q => q.OwnerPhone);
                            break;
                        case StoreOrder.OwnerEmail:
                            query = query.OrderByDescending(q => q.OwnerEmail);
                            break;
                        case StoreOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Store>> DynamicSelect(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            List<Store> Stores = await query.Select(q => new Store()
            {
                Id = filter.Selects.Contains(StoreSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StoreSelect.Name) ? q.Name : default(string),
                ParentStoreId = filter.Selects.Contains(StoreSelect.ParentStore) ? q.ParentStoreId : default(long?),
                OrganizationId = filter.Selects.Contains(StoreSelect.Organization) ? q.OrganizationId : default(long),
                StoreTypeId = filter.Selects.Contains(StoreSelect.StoreType) ? q.StoreTypeId : default(long),
                StoreGroupingId = filter.Selects.Contains(StoreSelect.StoreGrouping) ? q.StoreGroupingId : default(long?),
                ResellerId = filter.Selects.Contains(StoreSelect.Reseller) ? q.ResellerId : default(long?),
                Telephone = filter.Selects.Contains(StoreSelect.Telephone) ? q.Telephone : default(string),
                ProvinceId = filter.Selects.Contains(StoreSelect.Province) ? q.ProvinceId : default(long),
                DistrictId = filter.Selects.Contains(StoreSelect.District) ? q.DistrictId : default(long),
                WardId = filter.Selects.Contains(StoreSelect.Ward) ? q.WardId : default(long),
                Address = filter.Selects.Contains(StoreSelect.Address) ? q.Address : default(string),
                DeliveryAddress = filter.Selects.Contains(StoreSelect.DeliveryAddress) ? q.DeliveryAddress : default(string),
                Latitude = filter.Selects.Contains(StoreSelect.Latitude) ? q.Latitude : default(decimal?),
                Longitude = filter.Selects.Contains(StoreSelect.Longitude) ? q.Longitude : default(decimal?),
                DeliveryLatitude = filter.Selects.Contains(StoreSelect.DeliveryLatitude) ? q.DeliveryLatitude : default(decimal?),
                DeliveryLongitude = filter.Selects.Contains(StoreSelect.DeliveryLongitude) ? q.DeliveryLongitude : default(decimal?),
                OwnerName = filter.Selects.Contains(StoreSelect.OwnerName) ? q.OwnerName : default(string),
                OwnerPhone = filter.Selects.Contains(StoreSelect.OwnerPhone) ? q.OwnerPhone : default(string),
                OwnerEmail = filter.Selects.Contains(StoreSelect.OwnerEmail) ? q.OwnerEmail : default(string),
                StatusId = filter.Selects.Contains(StoreSelect.Status) ? q.StatusId : default(long),
                StoreStatusId = filter.Selects.Contains(StoreSelect.StoreStatus) ? q.StoreStatusId : default(long),
                District = filter.Selects.Contains(StoreSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                } : null,
                Organization = filter.Selects.Contains(StoreSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Address = q.Organization.Address,
                    Latitude = q.Organization.Latitude,
                    Longitude = q.Organization.Longitude,
                } : null,
                ParentStore = filter.Selects.Contains(StoreSelect.ParentStore) && q.ParentStore != null ? new Store
                {
                    Id = q.ParentStore.Id,
                    Code = q.ParentStore.Code,
                    Name = q.ParentStore.Name,
                    ParentStoreId = q.ParentStore.ParentStoreId,
                    OrganizationId = q.ParentStore.OrganizationId,
                    StoreTypeId = q.ParentStore.StoreTypeId,
                    StoreGroupingId = q.ParentStore.StoreGroupingId,
                    Telephone = q.ParentStore.Telephone,
                    ResellerId = q.ParentStore.ResellerId,
                    ProvinceId = q.ParentStore.ProvinceId,
                    DistrictId = q.ParentStore.DistrictId,
                    WardId = q.ParentStore.WardId,
                    Address = q.ParentStore.Address,
                    DeliveryAddress = q.ParentStore.DeliveryAddress,
                    Latitude = q.ParentStore.Latitude,
                    Longitude = q.ParentStore.Longitude,
                    DeliveryLatitude = q.ParentStore.DeliveryLatitude,
                    DeliveryLongitude = q.ParentStore.DeliveryLongitude,
                    OwnerName = q.ParentStore.OwnerName,
                    OwnerPhone = q.ParentStore.OwnerPhone,
                    OwnerEmail = q.ParentStore.OwnerEmail,
                    StatusId = q.ParentStore.StatusId,
                    StoreStatusId = q.ParentStore.StoreStatusId,
                } : null,
                Reseller = filter.Selects.Contains(StoreSelect.Reseller) && q.Reseller != null ? new Reseller
                {
                    Id = q.Reseller.Id,
                    Name = q.Reseller.Name,
                    Code = q.Reseller.Code,
                    Email = q.Reseller.Email,
                    Phone = q.Reseller.Phone,
                    CompanyName = q.Reseller.CompanyName,
                    DeputyName = q.Reseller.DeputyName,
                    Address = q.Reseller.Address,
                    Description = q.Reseller.Description,
                    OrganizationId = q.Reseller.OrganizationId,
                    ResellerStatusId = q.Reseller.ResellerStatusId,
                    ResellerTypeId = q.Reseller.ResellerTypeId,
                    StaffId = q.Reseller.StaffId,
                    TaxCode = q.Reseller.TaxCode,
                    StatusId = q.Reseller.StatusId,
                } : null,
                Province = filter.Selects.Contains(StoreSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Status = filter.Selects.Contains(StoreSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                StoreGrouping = filter.Selects.Contains(StoreSelect.StoreGrouping) && q.StoreGrouping != null ? new StoreGrouping
                {
                    Id = q.StoreGrouping.Id,
                    Code = q.StoreGrouping.Code,
                    Name = q.StoreGrouping.Name,
                    ParentId = q.StoreGrouping.ParentId,
                    Path = q.StoreGrouping.Path,
                    Level = q.StoreGrouping.Level,
                } : null,
                StoreType = filter.Selects.Contains(StoreSelect.StoreType) && q.StoreType != null ? new StoreType
                {
                    Id = q.StoreType.Id,
                    Code = q.StoreType.Code,
                    Name = q.StoreType.Name,
                    StatusId = q.StoreType.StatusId,
                } : null,
                Ward = filter.Selects.Contains(StoreSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                } : null,
                StoreImageMappings = filter.Selects.Contains(StoreSelect.StoreImageMappings) &&  q.StoreImageMappings != null ? q.StoreImageMappings.Skip(0).Take(1).Select(x => new StoreImageMapping
                {
                    StoreId = x.StoreId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url
                    }
                }).ToList() : null,
            }).ToListAsync();
            return Stores;
        }

        public async Task<int> Count(StoreFilter filter)
        {
            IQueryable<StoreDAO> Stores = DataContext.Store;
            Stores = DynamicFilter(Stores, filter);
            return await Stores.CountAsync();
        }

        public async Task<List<Store>> List(StoreFilter filter)
        {
            if (filter == null) return new List<Store>();
            IQueryable<StoreDAO> StoreDAOs = DataContext.Store.AsNoTracking();
            StoreDAOs = DynamicFilter(StoreDAOs, filter);
            StoreDAOs = DynamicOrder(StoreDAOs, filter);
            List<Store> Stores = await DynamicSelect(StoreDAOs, filter);
            return Stores;
        }

        public async Task<Store> Get(long Id)
        {
            Store Store = await DataContext.Store.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Store()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ParentStoreId = x.ParentStoreId,
                OrganizationId = x.OrganizationId,
                StoreTypeId = x.StoreTypeId,
                StoreGroupingId = x.StoreGroupingId,
                Telephone = x.Telephone,
                ResellerId = x.ResellerId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                Address = x.Address,
                DeliveryAddress = x.DeliveryAddress,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                DeliveryLatitude = x.DeliveryLatitude,
                DeliveryLongitude = x.DeliveryLongitude,
                OwnerName = x.OwnerName,
                OwnerPhone = x.OwnerPhone,
                OwnerEmail = x.OwnerEmail,
                StatusId = x.StatusId,
                StoreStatusId = x.StoreStatusId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
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
                    Address = x.Organization.Address,
                    Latitude = x.Organization.Latitude,
                    Longitude = x.Organization.Longitude,
                },
                ParentStore = x.ParentStore == null ? null : new Store
                {
                    Id = x.ParentStore.Id,
                    Code = x.ParentStore.Code,
                    Name = x.ParentStore.Name,
                    ParentStoreId = x.ParentStore.ParentStoreId,
                    OrganizationId = x.ParentStore.OrganizationId,
                    StoreTypeId = x.ParentStore.StoreTypeId,
                    StoreGroupingId = x.ParentStore.StoreGroupingId,
                    Telephone = x.ParentStore.Telephone,
                    ProvinceId = x.ParentStore.ProvinceId,
                    DistrictId = x.ParentStore.DistrictId,
                    WardId = x.ParentStore.WardId,
                    Address = x.ParentStore.Address,
                    DeliveryAddress = x.ParentStore.DeliveryAddress,
                    Latitude = x.ParentStore.Latitude,
                    Longitude = x.ParentStore.Longitude,
                    OwnerName = x.ParentStore.OwnerName,
                    OwnerPhone = x.ParentStore.OwnerPhone,
                    OwnerEmail = x.ParentStore.OwnerEmail,
                    StatusId = x.ParentStore.StatusId,
                    StoreStatusId = x.ParentStore.StoreStatusId,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                },
                Reseller = x.Reseller == null ? null : new Reseller
                {
                    Id = x.Reseller.Id,
                    Name = x.Reseller.Name,
                    Code = x.Reseller.Code,
                    Email = x.Reseller.Email,
                    Phone = x.Reseller.Phone,
                    CompanyName = x.Reseller.CompanyName,
                    DeputyName = x.Reseller.DeputyName,
                    Address = x.Reseller.Address,
                    Description = x.Reseller.Description,
                    OrganizationId = x.Reseller.OrganizationId,
                    ResellerStatusId = x.Reseller.ResellerStatusId,
                    ResellerTypeId = x.Reseller.ResellerTypeId,
                    StaffId = x.Reseller.StaffId,
                    TaxCode = x.Reseller.TaxCode,
                    StatusId = x.Reseller.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                StoreGrouping = x.StoreGrouping == null ? null : new StoreGrouping
                {
                    Id = x.StoreGrouping.Id,
                    Code = x.StoreGrouping.Code,
                    Name = x.StoreGrouping.Name,
                    ParentId = x.StoreGrouping.ParentId,
                    Path = x.StoreGrouping.Path,
                    Level = x.StoreGrouping.Level,
                },
                StoreType = x.StoreType == null ? null : new StoreType
                {
                    Id = x.StoreType.Id,
                    Code = x.StoreType.Code,
                    Name = x.StoreType.Name,
                    StatusId = x.StoreType.StatusId,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (Store == null)
                return null;

            Store.StoreImageMappings = await DataContext.StoreImageMapping
                .Where(x => x.StoreId == Id).Select(x => new StoreImageMapping
                {
                    StoreId = x.StoreId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url
                    }
                }).ToListAsync();
            return Store;
        }
        public async Task<bool> Create(Store Store)
        {
            StoreDAO StoreDAO = new StoreDAO();
            StoreDAO.Id = Store.Id;
            StoreDAO.Code = Store.Code;
            StoreDAO.Name = Store.Name;
            StoreDAO.ParentStoreId = Store.ParentStoreId;
            StoreDAO.OrganizationId = Store.OrganizationId;
            StoreDAO.StoreTypeId = Store.StoreTypeId;
            StoreDAO.StoreGroupingId = Store.StoreGroupingId;
            StoreDAO.ResellerId = Store.ResellerId;
            StoreDAO.Telephone = Store.Telephone;
            StoreDAO.ProvinceId = Store.ProvinceId;
            StoreDAO.DistrictId = Store.DistrictId;
            StoreDAO.WardId = Store.WardId;
            StoreDAO.Address = Store.Address;
            StoreDAO.DeliveryAddress = Store.DeliveryAddress;
            StoreDAO.Latitude = Store.Latitude;
            StoreDAO.Longitude = Store.Longitude;
            StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
            StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
            StoreDAO.OwnerName = Store.OwnerName;
            StoreDAO.OwnerPhone = Store.OwnerPhone;
            StoreDAO.OwnerEmail = Store.OwnerEmail;
            StoreDAO.StatusId = Store.StatusId;
            StoreDAO.StoreStatusId = Store.StoreStatusId;
            StoreDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Store.Add(StoreDAO);
            await DataContext.SaveChangesAsync();
            Store.Id = StoreDAO.Id;
            await SaveReference(Store);
            return true;
        }

        public async Task<bool> Update(Store Store)
        {
            StoreDAO StoreDAO = DataContext.Store.Where(x => x.Id == Store.Id).FirstOrDefault();
            if (StoreDAO == null)
                return false;
            StoreDAO.Id = Store.Id;
            StoreDAO.Code = Store.Code;
            StoreDAO.Name = Store.Name;
            StoreDAO.ParentStoreId = Store.ParentStoreId;
            StoreDAO.OrganizationId = Store.OrganizationId;
            StoreDAO.StoreTypeId = Store.StoreTypeId;
            StoreDAO.StoreGroupingId = Store.StoreGroupingId;
            StoreDAO.Telephone = Store.Telephone;
            StoreDAO.ResellerId = Store.ResellerId;
            StoreDAO.ProvinceId = Store.ProvinceId;
            StoreDAO.DistrictId = Store.DistrictId;
            StoreDAO.WardId = Store.WardId;
            StoreDAO.Address = Store.Address;
            StoreDAO.DeliveryAddress = Store.DeliveryAddress;
            StoreDAO.Latitude = Store.Latitude;
            StoreDAO.Longitude = Store.Longitude;
            StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
            StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
            StoreDAO.OwnerName = Store.OwnerName;
            StoreDAO.OwnerPhone = Store.OwnerPhone;
            StoreDAO.OwnerEmail = Store.OwnerEmail;
            StoreDAO.StatusId = Store.StatusId;
            StoreDAO.StoreStatusId = Store.StoreStatusId;
            StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Store);
            return true;
        }

        public async Task<bool> Delete(Store Store)
        {
            await DataContext.Store.Where(x => x.ParentStoreId == Store.Id).UpdateFromQueryAsync(x => new StoreDAO { ParentStoreId = null });
            await DataContext.Store.Where(x => x.Id == Store.Id).UpdateFromQueryAsync(x => new StoreDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Store> Stores)
        {
            List<StoreDAO> StoreDAOs = new List<StoreDAO>();
            foreach (Store Store in Stores)
            {
                StoreDAO StoreDAO = new StoreDAO();
                StoreDAO.Id = Store.Id;
                StoreDAO.Code = Store.Code;
                StoreDAO.Name = Store.Name;
                StoreDAO.ParentStoreId = Store.ParentStoreId;
                StoreDAO.OrganizationId = Store.OrganizationId;
                StoreDAO.StoreTypeId = Store.StoreTypeId;
                StoreDAO.StoreGroupingId = Store.StoreGroupingId;
                StoreDAO.Telephone = Store.Telephone;
                StoreDAO.ResellerId = Store.ResellerId;
                StoreDAO.ProvinceId = Store.ProvinceId;
                StoreDAO.DistrictId = Store.DistrictId;
                StoreDAO.WardId = Store.WardId;
                StoreDAO.Address = Store.Address;
                StoreDAO.DeliveryAddress = Store.DeliveryAddress;
                StoreDAO.Latitude = Store.Latitude;
                StoreDAO.Longitude = Store.Longitude;
                StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
                StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
                StoreDAO.OwnerName = Store.OwnerName;
                StoreDAO.OwnerPhone = Store.OwnerPhone;
                StoreDAO.OwnerEmail = Store.OwnerEmail;
                StoreDAO.StatusId = Store.StatusId;
                StoreDAO.StoreStatusId = Store.StoreStatusId;
                StoreDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreDAOs.Add(StoreDAO);
            }
            await DataContext.BulkMergeAsync(StoreDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Store> Stores)
        {
            List<long> Ids = Stores.Select(x => x.Id).ToList();
            await DataContext.Store.Where(x => Ids.Contains(x.ParentStoreId.Value)).UpdateFromQueryAsync(x => new StoreDAO { ParentStoreId = null });
            await DataContext.Store
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Store Store)
        {
            await DataContext.StoreImageMapping.Where(x => x.StoreId == Store.Id).DeleteFromQueryAsync();
            List<StoreImageMappingDAO> StoreImageMappingDAOs = new List<StoreImageMappingDAO>();
            if(Store.StoreImageMappings != null)
            {
                foreach (StoreImageMapping StoreImageMapping in Store.StoreImageMappings)
                {
                    StoreImageMappingDAO StoreImageMappingDAO = new StoreImageMappingDAO();
                    StoreImageMappingDAO.StoreId = StoreImageMapping.StoreId;
                    StoreImageMappingDAO.ImageId = StoreImageMapping.ImageId;
                    StoreImageMappingDAOs.Add(StoreImageMappingDAO);
                }
                await DataContext.StoreImageMapping.BulkMergeAsync(StoreImageMappingDAOs);
            }
        }

    }
}
