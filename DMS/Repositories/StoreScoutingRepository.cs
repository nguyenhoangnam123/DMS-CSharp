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
    public interface IStoreScoutingRepository
    {
        Task<int> Count(StoreScoutingFilter StoreScoutingFilter);
        Task<List<StoreScouting>> List(StoreScoutingFilter StoreScoutingFilter);
        Task<StoreScouting> Get(long Id);
        Task<bool> Create(StoreScouting StoreScouting);
        Task<bool> Update(StoreScouting StoreScouting);
        Task<bool> Delete(StoreScouting StoreScouting);
        Task<bool> BulkMerge(List<StoreScouting> StoreScoutings);
        Task<bool> BulkDelete(List<StoreScouting> StoreScoutings);
    }
    public class StoreScoutingRepository : IStoreScoutingRepository
    {
        private DataContext DataContext;
        public StoreScoutingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreScoutingDAO> DynamicFilter(IQueryable<StoreScoutingDAO> query, StoreScoutingFilter filter)
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
            if (filter.OwnerPhone != null)
                query = query.Where(q => q.OwnerPhone, filter.OwnerPhone);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.WardId != null)
                query = query.Where(q => q.WardId, filter.WardId);
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longitude != null)
                query = query.Where(q => q.Longitude, filter.Longitude);
            if (filter.StoreId != null)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.StoreScoutingStatusId != null)
                query = query.Where(q => q.StoreScoutingStatusId, filter.StoreScoutingStatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<StoreScoutingDAO> OrFilter(IQueryable<StoreScoutingDAO> query, StoreScoutingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreScoutingDAO> initQuery = query.Where(q => false);
            foreach (StoreScoutingFilter StoreScoutingFilter in filter.OrFilter)
            {
                IQueryable<StoreScoutingDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.OwnerPhone != null)
                    queryable = queryable.Where(q => q.OwnerPhone, filter.OwnerPhone);
                if (filter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                if (filter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                if (filter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, filter.WardId);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, filter.Latitude);
                if (filter.Longitude != null)
                    queryable = queryable.Where(q => q.Longitude, filter.Longitude);
                if (filter.StoreId != null)
                    queryable = queryable.Where(q => q.StoreId, filter.StoreId);
                if (filter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                if (filter.StoreScoutingStatusId != null)
                    queryable = queryable.Where(q => q.StoreScoutingStatusId, filter.StoreScoutingStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<StoreScoutingDAO> DynamicOrder(IQueryable<StoreScoutingDAO> query, StoreScoutingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreScoutingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreScoutingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreScoutingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreScoutingOrder.OwnerPhone:
                            query = query.OrderBy(q => q.OwnerPhone);
                            break;
                        case StoreScoutingOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case StoreScoutingOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case StoreScoutingOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case StoreScoutingOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case StoreScoutingOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case StoreScoutingOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case StoreScoutingOrder.Longitude:
                            query = query.OrderBy(q => q.Longitude);
                            break;
                        case StoreScoutingOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case StoreScoutingOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case StoreScoutingOrder.StoreScoutingStatus:
                            query = query.OrderBy(q => q.StoreScoutingStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreScoutingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreScoutingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreScoutingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreScoutingOrder.OwnerPhone:
                            query = query.OrderByDescending(q => q.OwnerPhone);
                            break;
                        case StoreScoutingOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case StoreScoutingOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case StoreScoutingOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case StoreScoutingOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case StoreScoutingOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case StoreScoutingOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case StoreScoutingOrder.Longitude:
                            query = query.OrderByDescending(q => q.Longitude);
                            break;
                        case StoreScoutingOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case StoreScoutingOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case StoreScoutingOrder.StoreScoutingStatus:
                            query = query.OrderByDescending(q => q.StoreScoutingStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreScouting>> DynamicSelect(IQueryable<StoreScoutingDAO> query, StoreScoutingFilter filter)
        {
            List<StoreScouting> StoreScoutings = await query.Select(q => new StoreScouting()
            {
                Id = filter.Selects.Contains(StoreScoutingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreScoutingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StoreScoutingSelect.Name) ? q.Name : default(string),
                OwnerPhone = filter.Selects.Contains(StoreScoutingSelect.OwnerPhone) ? q.OwnerPhone : default(string),
                ProvinceId = filter.Selects.Contains(StoreScoutingSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(StoreScoutingSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(StoreScoutingSelect.Ward) ? q.WardId : default(long?),
                OrganizationId = filter.Selects.Contains(StoreScoutingSelect.Organization) ? q.OrganizationId : default(long?),
                Address = filter.Selects.Contains(StoreScoutingSelect.Address) ? q.Address : default(string),
                Latitude = filter.Selects.Contains(StoreScoutingSelect.Latitude) ? q.Latitude : default(decimal?),
                Longitude = filter.Selects.Contains(StoreScoutingSelect.Longitude) ? q.Longitude : default(decimal?),
                StoreId = filter.Selects.Contains(StoreScoutingSelect.Store) ? q.StoreId : default(long?),
                CreatorId = filter.Selects.Contains(StoreScoutingSelect.Creator) ? q.CreatorId : default(long),
                StoreScoutingStatusId = filter.Selects.Contains(StoreScoutingSelect.StoreScoutingStatus) ? q.StoreScoutingStatusId : default(long),
                Creator = filter.Selects.Contains(StoreScoutingSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    PositionId = q.Creator.PositionId,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    StatusId = q.Creator.StatusId,
                    Avatar = q.Creator.Avatar,
                    ProvinceId = q.Creator.ProvinceId,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                } : null,
                District = filter.Selects.Contains(StoreScoutingSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                } : null,
                Organization = filter.Selects.Contains(StoreScoutingSelect.Organization) && q.Organization != null ? new Organization
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
                Province = filter.Selects.Contains(StoreScoutingSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Store = filter.Selects.Contains(StoreScoutingSelect.Store) && q.Store != null ? new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    Name = q.Store.Name,
                    ParentStoreId = q.Store.ParentStoreId,
                    OrganizationId = q.Store.OrganizationId,
                    StoreTypeId = q.Store.StoreTypeId,
                    StoreGroupingId = q.Store.StoreGroupingId,
                    ResellerId = q.Store.ResellerId,
                    Telephone = q.Store.Telephone,
                    ProvinceId = q.Store.ProvinceId,
                    DistrictId = q.Store.DistrictId,
                    WardId = q.Store.WardId,
                    Address = q.Store.Address,
                    DeliveryAddress = q.Store.DeliveryAddress,
                    Latitude = q.Store.Latitude,
                    Longitude = q.Store.Longitude,
                    DeliveryLatitude = q.Store.DeliveryLatitude,
                    DeliveryLongitude = q.Store.DeliveryLongitude,
                    OwnerName = q.Store.OwnerName,
                    OwnerPhone = q.Store.OwnerPhone,
                    OwnerEmail = q.Store.OwnerEmail,
                    TaxCode = q.Store.TaxCode,
                    LegalEntity = q.Store.LegalEntity,
                    StatusId = q.Store.StatusId,
                    Used = q.Store.Used,
                } : null,
                StoreScoutingStatus = filter.Selects.Contains(StoreScoutingSelect.StoreScoutingStatus) && q.StoreScoutingStatus != null ? new StoreScoutingStatus
                {
                    Id = q.StoreScoutingStatus.Id,
                    Code = q.StoreScoutingStatus.Code,
                    Name = q.StoreScoutingStatus.Name,
                } : null,
                Ward = filter.Selects.Contains(StoreScoutingSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return StoreScoutings;
        }

        public async Task<int> Count(StoreScoutingFilter filter)
        {
            IQueryable<StoreScoutingDAO> StoreScoutings = DataContext.StoreScouting.AsNoTracking();
            StoreScoutings = DynamicFilter(StoreScoutings, filter);
            return await StoreScoutings.CountAsync();
        }

        public async Task<List<StoreScouting>> List(StoreScoutingFilter filter)
        {
            if (filter == null) return new List<StoreScouting>();
            IQueryable<StoreScoutingDAO> StoreScoutingDAOs = DataContext.StoreScouting.AsNoTracking();
            StoreScoutingDAOs = DynamicFilter(StoreScoutingDAOs, filter);
            StoreScoutingDAOs = DynamicOrder(StoreScoutingDAOs, filter);
            List<StoreScouting> StoreScoutings = await DynamicSelect(StoreScoutingDAOs, filter);
            return StoreScoutings;
        }

        public async Task<StoreScouting> Get(long Id)
        {
            StoreScouting StoreScouting = await DataContext.StoreScouting.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new StoreScouting()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                OwnerPhone = x.OwnerPhone,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                OrganizationId = x.OrganizationId,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                StoreId = x.StoreId,
                CreatorId = x.CreatorId,
                StoreScoutingStatusId = x.StoreScoutingStatusId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    PositionId = x.Creator.PositionId,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    StatusId = x.Creator.StatusId,
                    Avatar = x.Creator.Avatar,
                    ProvinceId = x.Creator.ProvinceId,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                },
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
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
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                },
                Store = x.Store == null ? null : new Store
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
                    Used = x.Store.Used,
                },
                StoreScoutingStatus = x.StoreScoutingStatus == null ? null : new StoreScoutingStatus
                {
                    Id = x.StoreScoutingStatus.Id,
                    Code = x.StoreScoutingStatus.Code,
                    Name = x.StoreScoutingStatus.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (StoreScouting == null)
                return null;

            return StoreScouting;
        }
        public async Task<bool> Create(StoreScouting StoreScouting)
        {
            StoreScoutingDAO StoreScoutingDAO = new StoreScoutingDAO();
            StoreScoutingDAO.Id = StoreScouting.Id;
            StoreScoutingDAO.Code = StoreScouting.Code;
            StoreScoutingDAO.Name = StoreScouting.Name;
            StoreScoutingDAO.OwnerPhone = StoreScouting.OwnerPhone;
            StoreScoutingDAO.ProvinceId = StoreScouting.ProvinceId;
            StoreScoutingDAO.DistrictId = StoreScouting.DistrictId;
            StoreScoutingDAO.WardId = StoreScouting.WardId;
            StoreScoutingDAO.OrganizationId = StoreScouting.OrganizationId;
            StoreScoutingDAO.Address = StoreScouting.Address;
            StoreScoutingDAO.Latitude = StoreScouting.Latitude;
            StoreScoutingDAO.Longitude = StoreScouting.Longitude;
            StoreScoutingDAO.StoreId = StoreScouting.StoreId;
            StoreScoutingDAO.CreatorId = StoreScouting.CreatorId;
            StoreScoutingDAO.StoreScoutingStatusId = StoreScouting.StoreScoutingStatusId;
            StoreScoutingDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreScoutingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.StoreScouting.Add(StoreScoutingDAO);
            await DataContext.SaveChangesAsync();
            StoreScouting.Id = StoreScoutingDAO.Id;
            await SaveReference(StoreScouting);
            return true;
        }

        public async Task<bool> Update(StoreScouting StoreScouting)
        {
            StoreScoutingDAO StoreScoutingDAO = DataContext.StoreScouting.Where(x => x.Id == StoreScouting.Id).FirstOrDefault();
            if (StoreScoutingDAO == null)
                return false;
            StoreScoutingDAO.Id = StoreScouting.Id;
            StoreScoutingDAO.Code = StoreScouting.Code;
            StoreScoutingDAO.Name = StoreScouting.Name;
            StoreScoutingDAO.OwnerPhone = StoreScouting.OwnerPhone;
            StoreScoutingDAO.ProvinceId = StoreScouting.ProvinceId;
            StoreScoutingDAO.DistrictId = StoreScouting.DistrictId;
            StoreScoutingDAO.WardId = StoreScouting.WardId;
            StoreScoutingDAO.OrganizationId = StoreScouting.OrganizationId;
            StoreScoutingDAO.Address = StoreScouting.Address;
            StoreScoutingDAO.Latitude = StoreScouting.Latitude;
            StoreScoutingDAO.Longitude = StoreScouting.Longitude;
            StoreScoutingDAO.StoreId = StoreScouting.StoreId;
            StoreScoutingDAO.CreatorId = StoreScouting.CreatorId;
            StoreScoutingDAO.StoreScoutingStatusId = StoreScouting.StoreScoutingStatusId;
            StoreScoutingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(StoreScouting);
            return true;
        }

        public async Task<bool> Delete(StoreScouting StoreScouting)
        {
            await DataContext.StoreScouting.Where(x => x.Id == StoreScouting.Id).UpdateFromQueryAsync(x => new StoreScoutingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<StoreScouting> StoreScoutings)
        {
            List<StoreScoutingDAO> StoreScoutingDAOs = new List<StoreScoutingDAO>();
            foreach (StoreScouting StoreScouting in StoreScoutings)
            {
                StoreScoutingDAO StoreScoutingDAO = new StoreScoutingDAO();
                StoreScoutingDAO.Id = StoreScouting.Id;
                StoreScoutingDAO.Code = StoreScouting.Code;
                StoreScoutingDAO.Name = StoreScouting.Name;
                StoreScoutingDAO.OwnerPhone = StoreScouting.OwnerPhone;
                StoreScoutingDAO.ProvinceId = StoreScouting.ProvinceId;
                StoreScoutingDAO.DistrictId = StoreScouting.DistrictId;
                StoreScoutingDAO.WardId = StoreScouting.WardId;
                StoreScoutingDAO.OrganizationId = StoreScouting.OrganizationId;
                StoreScoutingDAO.Address = StoreScouting.Address;
                StoreScoutingDAO.Latitude = StoreScouting.Latitude;
                StoreScoutingDAO.Longitude = StoreScouting.Longitude;
                StoreScoutingDAO.StoreId = StoreScouting.StoreId;
                StoreScoutingDAO.CreatorId = StoreScouting.CreatorId;
                StoreScoutingDAO.StoreScoutingStatusId = StoreScouting.StoreScoutingStatusId;
                StoreScoutingDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreScoutingDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreScoutingDAOs.Add(StoreScoutingDAO);
            }
            await DataContext.BulkMergeAsync(StoreScoutingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<StoreScouting> StoreScoutings)
        {
            List<long> Ids = StoreScoutings.Select(x => x.Id).ToList();
            await DataContext.StoreScouting
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreScoutingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(StoreScouting StoreScouting)
        {
        }
        
    }
}
