using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IShowingOrderWithDrawRepository
    {
        Task<int> Count(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter);
        Task<List<ShowingOrderWithDraw>> List(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter);
        Task<List<ShowingOrderWithDraw>> List(List<long> Ids);
        Task<ShowingOrderWithDraw> Get(long Id);
        Task<bool> Create(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<bool> Update(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<bool> Delete(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<bool> BulkMerge(List<ShowingOrderWithDraw> ShowingOrderWithDraws);
        Task<bool> BulkDelete(List<ShowingOrderWithDraw> ShowingOrderWithDraws);
    }
    public class ShowingOrderWithDrawRepository : IShowingOrderWithDrawRepository
    {
        private DataContext DataContext;
        public ShowingOrderWithDrawRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ShowingOrderWithDrawDAO> DynamicFilter(IQueryable<ShowingOrderWithDrawDAO> query, ShowingOrderWithDrawFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.OrganizationId != null && filter.OrganizationId.HasValue)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.StoreId != null && filter.StoreId.HasValue)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.ShowingItemId != null && filter.ShowingItemId.HasValue)
            {
                if (filter.ShowingItemId.Equal.HasValue)
                {
                    query = from q in query
                            join c in DataContext.ShowingOrderContentWithDraw on q.Id equals c.ShowingOrderWithDrawId
                            where c.ShowingItemId == filter.ShowingItemId.Equal.Value
                            select q;
                }

                if (filter.ShowingItemId.NotEqual.HasValue)
                {
                    query = from q in query
                            join c in DataContext.ShowingOrderContentWithDraw on q.Id equals c.ShowingOrderWithDrawId
                            where c.ShowingItemId != filter.ShowingItemId.Equal.Value
                            select q;
                }

                if (filter.ShowingItemId.In != null)
                {
                    query = from q in query
                            join c in DataContext.ShowingOrderContentWithDraw on q.Id equals c.ShowingOrderWithDrawId
                            where filter.ShowingItemId.In.Contains(c.ShowingItemId)
                            select q;
                }

                if (filter.ShowingItemId.NotIn != null)
                {
                    query = from q in query
                            join c in DataContext.ShowingOrderContentWithDraw on q.Id equals c.ShowingOrderWithDrawId
                            where !filter.ShowingItemId.In.Contains(c.ShowingItemId)
                            select q;
                }
                query = query.Distinct();
            }
            if (filter.Date != null && filter.Date.HasValue)
                query = query.Where(q => q.Date, filter.Date);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.Total != null && filter.Total.HasValue)
                query = query.Where(q => q.Total, filter.Total);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ShowingOrderWithDrawDAO> OrFilter(IQueryable<ShowingOrderWithDrawDAO> query, ShowingOrderWithDrawFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ShowingOrderWithDrawDAO> initQuery = query.Where(q => false);
            foreach (ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter in filter.OrFilter)
            {
                IQueryable<ShowingOrderWithDrawDAO> queryable = query;
                if (ShowingOrderWithDrawFilter.Id != null && ShowingOrderWithDrawFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ShowingOrderWithDrawFilter.Code != null && ShowingOrderWithDrawFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (ShowingOrderWithDrawFilter.AppUserId != null && ShowingOrderWithDrawFilter.AppUserId.HasValue)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                if (ShowingOrderWithDrawFilter.OrganizationId != null && ShowingOrderWithDrawFilter.OrganizationId.HasValue)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (ShowingOrderWithDrawFilter.StoreId != null && ShowingOrderWithDrawFilter.StoreId.HasValue)
                    queryable = queryable.Where(q => q.StoreId, filter.StoreId);
                if (ShowingOrderWithDrawFilter.Date != null && ShowingOrderWithDrawFilter.Date.HasValue)
                    queryable = queryable.Where(q => q.Date, filter.Date);
                if (ShowingOrderWithDrawFilter.StatusId != null && ShowingOrderWithDrawFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (ShowingOrderWithDrawFilter.Total != null && ShowingOrderWithDrawFilter.Total.HasValue)
                    queryable = queryable.Where(q => q.Total, filter.Total);
                if (ShowingOrderWithDrawFilter.RowId != null && ShowingOrderWithDrawFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ShowingOrderWithDrawDAO> DynamicOrder(IQueryable<ShowingOrderWithDrawDAO> query, ShowingOrderWithDrawFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ShowingOrderWithDrawOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ShowingOrderWithDrawOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ShowingOrderWithDrawOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case ShowingOrderWithDrawOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case ShowingOrderWithDrawOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case ShowingOrderWithDrawOrder.Date:
                            query = query.OrderBy(q => q.Date);
                            break;
                        case ShowingOrderWithDrawOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ShowingOrderWithDrawOrder.Total:
                            query = query.OrderBy(q => q.Total);
                            break;
                        case ShowingOrderWithDrawOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ShowingOrderWithDrawOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ShowingOrderWithDrawOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ShowingOrderWithDrawOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case ShowingOrderWithDrawOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case ShowingOrderWithDrawOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case ShowingOrderWithDrawOrder.Date:
                            query = query.OrderByDescending(q => q.Date);
                            break;
                        case ShowingOrderWithDrawOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ShowingOrderWithDrawOrder.Total:
                            query = query.OrderByDescending(q => q.Total);
                            break;
                        case ShowingOrderWithDrawOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ShowingOrderWithDraw>> DynamicSelect(IQueryable<ShowingOrderWithDrawDAO> query, ShowingOrderWithDrawFilter filter)
        {
            List<ShowingOrderWithDraw> ShowingOrderWithDraws = await query.Select(q => new ShowingOrderWithDraw()
            {
                Id = filter.Selects.Contains(ShowingOrderWithDrawSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ShowingOrderWithDrawSelect.Code) ? q.Code : default(string),
                AppUserId = filter.Selects.Contains(ShowingOrderWithDrawSelect.AppUser) ? q.AppUserId : default(long),
                OrganizationId = filter.Selects.Contains(ShowingOrderWithDrawSelect.Organization) ? q.OrganizationId : default(long),
                StoreId = filter.Selects.Contains(ShowingOrderWithDrawSelect.Store) ? q.StoreId : default(long),
                Date = filter.Selects.Contains(ShowingOrderWithDrawSelect.Date) ? q.Date : default(DateTime),
                StatusId = filter.Selects.Contains(ShowingOrderWithDrawSelect.Status) ? q.StatusId : default(long),
                Total = filter.Selects.Contains(ShowingOrderWithDrawSelect.Total) ? q.Total : default(decimal),
                RowId = filter.Selects.Contains(ShowingOrderWithDrawSelect.Row) ? q.RowId : default(Guid),
                AppUser = filter.Selects.Contains(ShowingOrderWithDrawSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                    Address = q.AppUser.Address,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                    SexId = q.AppUser.SexId,
                    Birthday = q.AppUser.Birthday,
                    Avatar = q.AppUser.Avatar,
                    PositionId = q.AppUser.PositionId,
                    Department = q.AppUser.Department,
                    OrganizationId = q.AppUser.OrganizationId,
                    ProvinceId = q.AppUser.ProvinceId,
                    Longitude = q.AppUser.Longitude,
                    Latitude = q.AppUser.Latitude,
                    StatusId = q.AppUser.StatusId,
                    GPSUpdatedAt = q.AppUser.GPSUpdatedAt,
                    RowId = q.AppUser.RowId,
                } : null,
                Organization = filter.Selects.Contains(ShowingOrderWithDrawSelect.Organization) && q.Organization != null ? new Organization
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
                    RowId = q.Organization.RowId,
                } : null,
                Status = filter.Selects.Contains(ShowingOrderWithDrawSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Store = filter.Selects.Contains(ShowingOrderWithDrawSelect.Store) && q.Store != null ? new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    CodeDraft = q.Store.CodeDraft,
                    Name = q.Store.Name,
                    UnsignName = q.Store.UnsignName,
                    ParentStoreId = q.Store.ParentStoreId,
                    OrganizationId = q.Store.OrganizationId,
                    StoreTypeId = q.Store.StoreTypeId,
                    StoreGroupingId = q.Store.StoreGroupingId,
                    Telephone = q.Store.Telephone,
                    ProvinceId = q.Store.ProvinceId,
                    DistrictId = q.Store.DistrictId,
                    WardId = q.Store.WardId,
                    Address = q.Store.Address,
                    UnsignAddress = q.Store.UnsignAddress,
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
                    CreatorId = q.Store.CreatorId,
                    AppUserId = q.Store.AppUserId,
                    StatusId = q.Store.StatusId,
                    RowId = q.Store.RowId,
                    Used = q.Store.Used,
                    StoreScoutingId = q.Store.StoreScoutingId,
                    StoreStatusId = q.Store.StoreStatusId,
                    StoreStatus = q.Store.StoreStatus == null ? null : new StoreStatus
                    {
                        Id = q.Store.StoreStatus.Id,
                        Code = q.Store.StoreStatus.Code,
                        Name = q.Store.StoreStatus.Name,
                    }
                } : null,
            }).ToListAsync();
            return ShowingOrderWithDraws;
        }

        public async Task<int> Count(ShowingOrderWithDrawFilter filter)
        {
            IQueryable<ShowingOrderWithDrawDAO> ShowingOrderWithDraws = DataContext.ShowingOrderWithDraw.AsNoTracking();
            ShowingOrderWithDraws = DynamicFilter(ShowingOrderWithDraws, filter);
            return await ShowingOrderWithDraws.CountAsync();
        }

        public async Task<List<ShowingOrderWithDraw>> List(ShowingOrderWithDrawFilter filter)
        {
            if (filter == null) return new List<ShowingOrderWithDraw>();
            IQueryable<ShowingOrderWithDrawDAO> ShowingOrderWithDrawDAOs = DataContext.ShowingOrderWithDraw.AsNoTracking();
            ShowingOrderWithDrawDAOs = DynamicFilter(ShowingOrderWithDrawDAOs, filter);
            ShowingOrderWithDrawDAOs = DynamicOrder(ShowingOrderWithDrawDAOs, filter);
            List<ShowingOrderWithDraw> ShowingOrderWithDraws = await DynamicSelect(ShowingOrderWithDrawDAOs, filter);
            return ShowingOrderWithDraws;
        }

        public async Task<List<ShowingOrderWithDraw>> List(List<long> Ids)
        {
            List<ShowingOrderWithDraw> ShowingOrderWithDraws = await DataContext.ShowingOrderWithDraw.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ShowingOrderWithDraw()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                AppUserId = x.AppUserId,
                OrganizationId = x.OrganizationId,
                StoreId = x.StoreId,
                Date = x.Date,
                StatusId = x.StatusId,
                Total = x.Total,
                RowId = x.RowId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    PositionId = x.AppUser.PositionId,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    ProvinceId = x.AppUser.ProvinceId,
                    Longitude = x.AppUser.Longitude,
                    Latitude = x.AppUser.Latitude,
                    StatusId = x.AppUser.StatusId,
                    GPSUpdatedAt = x.AppUser.GPSUpdatedAt,
                    RowId = x.AppUser.RowId,
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
                    RowId = x.Organization.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Store = x.Store == null ? null : new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    CodeDraft = x.Store.CodeDraft,
                    Name = x.Store.Name,
                    UnsignName = x.Store.UnsignName,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    UnsignAddress = x.Store.UnsignAddress,
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
                    CreatorId = x.Store.CreatorId,
                    AppUserId = x.Store.AppUserId,
                    StatusId = x.Store.StatusId,
                    RowId = x.Store.RowId,
                    Used = x.Store.Used,
                    StoreScoutingId = x.Store.StoreScoutingId,
                    StoreStatusId = x.Store.StoreStatusId,
                },
            }).ToListAsync();
            
            List<ShowingOrderContentWithDraw> ShowingOrderContentWithDraws = await DataContext.ShowingOrderContentWithDraw.AsNoTracking()
                .Where(x => Ids.Contains(x.ShowingOrderWithDrawId))
                .Select(x => new ShowingOrderContentWithDraw
                {
                    Id = x.Id,
                    ShowingOrderWithDrawId = x.ShowingOrderWithDrawId,
                    ShowingItemId = x.ShowingItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    SalePrice = x.SalePrice,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    ShowingItem = new ShowingItem
                    {
                        Id = x.ShowingItem.Id,
                        Code = x.ShowingItem.Code,
                        Name = x.ShowingItem.Name,
                        ShowingCategoryId = x.ShowingItem.ShowingCategoryId,
                        UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                        SalePrice = x.ShowingItem.SalePrice,
                        Description = x.ShowingItem.Description,
                        StatusId = x.ShowingItem.StatusId,
                        Used = x.ShowingItem.Used,
                        RowId = x.ShowingItem.RowId,
                        ShowingCategory = x.ShowingItem.ShowingCategory == null ? null : new ShowingCategory
                        {
                            Id = x.ShowingItem.ShowingCategory.Id,
                            Code = x.ShowingItem.ShowingCategory.Code,
                            Name = x.ShowingItem.ShowingCategory.Name,
                        }
                    },
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Used = x.UnitOfMeasure.Used,
                        RowId = x.UnitOfMeasure.RowId,
                    },
                }).ToListAsync();
            foreach(ShowingOrderWithDraw ShowingOrderWithDraw in ShowingOrderWithDraws)
            {
                ShowingOrderWithDraw.ShowingOrderContentWithDraws = ShowingOrderContentWithDraws
                    .Where(x => x.ShowingOrderWithDrawId == ShowingOrderWithDraw.Id)
                    .ToList();
            }

            return ShowingOrderWithDraws;
        }

        public async Task<ShowingOrderWithDraw> Get(long Id)
        {
            ShowingOrderWithDraw ShowingOrderWithDraw = await DataContext.ShowingOrderWithDraw.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ShowingOrderWithDraw()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                AppUserId = x.AppUserId,
                OrganizationId = x.OrganizationId,
                StoreId = x.StoreId,
                Date = x.Date,
                StatusId = x.StatusId,
                Total = x.Total,
                RowId = x.RowId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    PositionId = x.AppUser.PositionId,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    ProvinceId = x.AppUser.ProvinceId,
                    Longitude = x.AppUser.Longitude,
                    Latitude = x.AppUser.Latitude,
                    StatusId = x.AppUser.StatusId,
                    GPSUpdatedAt = x.AppUser.GPSUpdatedAt,
                    RowId = x.AppUser.RowId,
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
                    RowId = x.Organization.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Store = x.Store == null ? null : new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    CodeDraft = x.Store.CodeDraft,
                    Name = x.Store.Name,
                    UnsignName = x.Store.UnsignName,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    UnsignAddress = x.Store.UnsignAddress,
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
                    CreatorId = x.Store.CreatorId,
                    AppUserId = x.Store.AppUserId,
                    StatusId = x.Store.StatusId,
                    RowId = x.Store.RowId,
                    Used = x.Store.Used,
                    StoreScoutingId = x.Store.StoreScoutingId,
                    StoreStatusId = x.Store.StoreStatusId,
                },
            }).FirstOrDefaultAsync();

            if (ShowingOrderWithDraw == null)
                return null;
            ShowingOrderWithDraw.ShowingOrderContentWithDraws = await DataContext.ShowingOrderContentWithDraw.AsNoTracking()
                .Where(x => x.ShowingOrderWithDrawId == ShowingOrderWithDraw.Id)
                .Select(x => new ShowingOrderContentWithDraw
                {
                    Id = x.Id,
                    ShowingOrderWithDrawId = x.ShowingOrderWithDrawId,
                    ShowingItemId = x.ShowingItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    SalePrice = x.SalePrice,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    ShowingItem = new ShowingItem
                    {
                        Id = x.ShowingItem.Id,
                        Code = x.ShowingItem.Code,
                        Name = x.ShowingItem.Name,
                        ShowingCategoryId = x.ShowingItem.ShowingCategoryId,
                        UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                        SalePrice = x.ShowingItem.SalePrice,
                        Description = x.ShowingItem.Description,
                        StatusId = x.ShowingItem.StatusId,
                        Used = x.ShowingItem.Used,
                        RowId = x.ShowingItem.RowId,
                        ShowingCategory = x.ShowingItem.ShowingCategory == null ? null : new ShowingCategory
                        {
                            Id = x.ShowingItem.ShowingCategory.Id,
                            Code = x.ShowingItem.ShowingCategory.Code,
                            Name = x.ShowingItem.ShowingCategory.Name,
                        }
                    },
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Used = x.UnitOfMeasure.Used,
                        RowId = x.UnitOfMeasure.RowId,
                    },
                }).ToListAsync();

            return ShowingOrderWithDraw;
        }
        public async Task<bool> Create(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            ShowingOrderWithDrawDAO ShowingOrderWithDrawDAO = new ShowingOrderWithDrawDAO();
            ShowingOrderWithDrawDAO.Id = ShowingOrderWithDraw.Id;
            ShowingOrderWithDrawDAO.Code = ShowingOrderWithDraw.Code;
            ShowingOrderWithDrawDAO.AppUserId = ShowingOrderWithDraw.AppUserId;
            ShowingOrderWithDrawDAO.OrganizationId = ShowingOrderWithDraw.OrganizationId;
            ShowingOrderWithDrawDAO.StoreId = ShowingOrderWithDraw.StoreId;
            ShowingOrderWithDrawDAO.Date = ShowingOrderWithDraw.Date;
            ShowingOrderWithDrawDAO.StatusId = ShowingOrderWithDraw.StatusId;
            ShowingOrderWithDrawDAO.Total = ShowingOrderWithDraw.Total;
            ShowingOrderWithDrawDAO.RowId = ShowingOrderWithDraw.RowId;
            ShowingOrderWithDrawDAO.CreatedAt = StaticParams.DateTimeNow;
            ShowingOrderWithDrawDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ShowingOrderWithDraw.Add(ShowingOrderWithDrawDAO);
            await DataContext.SaveChangesAsync();
            ShowingOrderWithDraw.Id = ShowingOrderWithDrawDAO.Id;
            await SaveReference(ShowingOrderWithDraw);
            return true;
        }

        public async Task<bool> Update(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            ShowingOrderWithDrawDAO ShowingOrderWithDrawDAO = DataContext.ShowingOrderWithDraw.Where(x => x.Id == ShowingOrderWithDraw.Id).FirstOrDefault();
            if (ShowingOrderWithDrawDAO == null)
                return false;
            ShowingOrderWithDrawDAO.Id = ShowingOrderWithDraw.Id;
            ShowingOrderWithDrawDAO.Code = ShowingOrderWithDraw.Code;
            ShowingOrderWithDrawDAO.AppUserId = ShowingOrderWithDraw.AppUserId;
            ShowingOrderWithDrawDAO.OrganizationId = ShowingOrderWithDraw.OrganizationId;
            ShowingOrderWithDrawDAO.StoreId = ShowingOrderWithDraw.StoreId;
            ShowingOrderWithDrawDAO.Date = ShowingOrderWithDraw.Date;
            ShowingOrderWithDrawDAO.StatusId = ShowingOrderWithDraw.StatusId;
            ShowingOrderWithDrawDAO.Total = ShowingOrderWithDraw.Total;
            ShowingOrderWithDrawDAO.RowId = ShowingOrderWithDraw.RowId;
            ShowingOrderWithDrawDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ShowingOrderWithDraw);
            return true;
        }

        public async Task<bool> Delete(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            await DataContext.ShowingOrderWithDraw.Where(x => x.Id == ShowingOrderWithDraw.Id).UpdateFromQueryAsync(x => new ShowingOrderWithDrawDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ShowingOrderWithDraw> ShowingOrderWithDraws)
        {
            List<ShowingOrderWithDrawDAO> ShowingOrderWithDrawDAOs = new List<ShowingOrderWithDrawDAO>();
            foreach (ShowingOrderWithDraw ShowingOrderWithDraw in ShowingOrderWithDraws)
            {
                ShowingOrderWithDrawDAO ShowingOrderWithDrawDAO = new ShowingOrderWithDrawDAO();
                ShowingOrderWithDrawDAO.Id = ShowingOrderWithDraw.Id;
                ShowingOrderWithDrawDAO.Code = ShowingOrderWithDraw.Code;
                ShowingOrderWithDrawDAO.AppUserId = ShowingOrderWithDraw.AppUserId;
                ShowingOrderWithDrawDAO.OrganizationId = ShowingOrderWithDraw.OrganizationId;
                ShowingOrderWithDrawDAO.StoreId = ShowingOrderWithDraw.StoreId;
                ShowingOrderWithDrawDAO.Date = ShowingOrderWithDraw.Date;
                ShowingOrderWithDrawDAO.StatusId = ShowingOrderWithDraw.StatusId;
                ShowingOrderWithDrawDAO.Total = ShowingOrderWithDraw.Total;
                ShowingOrderWithDrawDAO.RowId = ShowingOrderWithDraw.RowId;
                ShowingOrderWithDrawDAO.CreatedAt = StaticParams.DateTimeNow;
                ShowingOrderWithDrawDAO.UpdatedAt = StaticParams.DateTimeNow;
                ShowingOrderWithDrawDAOs.Add(ShowingOrderWithDrawDAO);
            }
            await DataContext.BulkMergeAsync(ShowingOrderWithDrawDAOs);

            foreach (var ShowingOrderWithDrawDAO in ShowingOrderWithDrawDAOs)
            {
                if(ShowingOrderWithDrawDAO.Id < 1000000)
                {
                    var Code = (ShowingOrderWithDrawDAO.Id + 1000000).ToString();
                    ShowingOrderWithDrawDAO.Code = Code.Substring(Code.Length - 6);
                }
                else
                {
                    ShowingOrderWithDrawDAO.Code = ShowingOrderWithDrawDAO.Id.ToString();
                }
            }
            await DataContext.BulkMergeAsync(ShowingOrderWithDrawDAOs);

            var Ids = ShowingOrderWithDrawDAOs.Select(x => x.Id).ToList();
            await DataContext.ShowingOrderContentWithDraw
                .Where(x => Ids.Contains(x.ShowingOrderWithDrawId))
                .DeleteFromQueryAsync();
            List<ShowingOrderContentWithDrawDAO> ShowingOrderContentWithDrawDAOs = new List<ShowingOrderContentWithDrawDAO>();
            foreach (var ShowingOrderWithDraw in ShowingOrderWithDraws)
            {
                ShowingOrderWithDraw.Id = ShowingOrderWithDrawDAOs.Where(x => x.RowId == ShowingOrderWithDraw.RowId).Select(x => x.Id).FirstOrDefault();
                if (ShowingOrderWithDraw.ShowingOrderContentWithDraws != null)
                {
                    var listContent = ShowingOrderWithDraw.ShowingOrderContentWithDraws?.Select(x => new ShowingOrderContentWithDrawDAO
                    {
                        Amount = x.Amount,
                        Quantity = x.Quantity,
                        SalePrice = x.SalePrice,
                        ShowingItemId = x.ShowingItemId,
                        ShowingOrderWithDrawId = ShowingOrderWithDraw.Id,
                        UnitOfMeasureId = x.UnitOfMeasureId,
                    }).ToList();
                    ShowingOrderContentWithDrawDAOs.AddRange(listContent);
                }
            }
            await DataContext.BulkMergeAsync(ShowingOrderContentWithDrawDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ShowingOrderWithDraw> ShowingOrderWithDraws)
        {
            List<long> Ids = ShowingOrderWithDraws.Select(x => x.Id).ToList();
            await DataContext.ShowingOrderWithDraw
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ShowingOrderWithDrawDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            await DataContext.ShowingOrderContentWithDraw
                .Where(x => x.ShowingOrderWithDrawId == ShowingOrderWithDraw.Id)
                .DeleteFromQueryAsync();
            List<ShowingOrderContentWithDrawDAO> ShowingOrderContentWithDrawDAOs = new List<ShowingOrderContentWithDrawDAO>();
            if (ShowingOrderWithDraw.ShowingOrderContentWithDraws != null)
            {
                foreach (ShowingOrderContentWithDraw ShowingOrderContentWithDraw in ShowingOrderWithDraw.ShowingOrderContentWithDraws)
                {
                    ShowingOrderContentWithDrawDAO ShowingOrderContentWithDrawDAO = new ShowingOrderContentWithDrawDAO();
                    ShowingOrderContentWithDrawDAO.Id = ShowingOrderContentWithDraw.Id;
                    ShowingOrderContentWithDrawDAO.ShowingOrderWithDrawId = ShowingOrderWithDraw.Id;
                    ShowingOrderContentWithDrawDAO.ShowingItemId = ShowingOrderContentWithDraw.ShowingItemId;
                    ShowingOrderContentWithDrawDAO.UnitOfMeasureId = ShowingOrderContentWithDraw.UnitOfMeasureId;
                    ShowingOrderContentWithDrawDAO.SalePrice = ShowingOrderContentWithDraw.SalePrice;
                    ShowingOrderContentWithDrawDAO.Quantity = ShowingOrderContentWithDraw.Quantity;
                    ShowingOrderContentWithDrawDAO.Amount = ShowingOrderContentWithDraw.Amount;
                    ShowingOrderContentWithDrawDAOs.Add(ShowingOrderContentWithDrawDAO);
                }
                await DataContext.ShowingOrderContentWithDraw.BulkMergeAsync(ShowingOrderContentWithDrawDAOs);
            }
        }
        
    }
}
