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
    public interface IShowingWarehouseRepository
    {
        Task<int> Count(ShowingWarehouseFilter ShowingWarehouseFilter);
        Task<List<ShowingWarehouse>> List(ShowingWarehouseFilter ShowingWarehouseFilter);
        Task<List<ShowingWarehouse>> List(List<long> Ids);
        Task<ShowingWarehouse> Get(long Id);
        Task<bool> Create(ShowingWarehouse ShowingWarehouse);
        Task<bool> Update(ShowingWarehouse ShowingWarehouse);
        Task<bool> Delete(ShowingWarehouse ShowingWarehouse);
        Task<bool> BulkMerge(List<ShowingWarehouse> ShowingWarehouses);
        Task<bool> BulkDelete(List<ShowingWarehouse> ShowingWarehouses);
    }
    public class ShowingWarehouseRepository : IShowingWarehouseRepository
    {
        private DataContext DataContext;
        public ShowingWarehouseRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ShowingWarehouseDAO> DynamicFilter(IQueryable<ShowingWarehouseDAO> query, ShowingWarehouseFilter filter)
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
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Address != null && filter.Address.HasValue)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.OrganizationId != null && filter.OrganizationId.HasValue)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.ProvinceId != null && filter.ProvinceId.HasValue)
                query = query.Where(q => q.ProvinceId.HasValue).Where(q => q.ProvinceId.Value, filter.ProvinceId);
            if (filter.DistrictId != null && filter.DistrictId.HasValue)
                query = query.Where(q => q.DistrictId.HasValue).Where(q => q.DistrictId.Value, filter.DistrictId);
            if (filter.WardId != null && filter.WardId.HasValue)
                query = query.Where(q => q.WardId.HasValue).Where(q => q.WardId.Value, filter.WardId);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ShowingWarehouseDAO> OrFilter(IQueryable<ShowingWarehouseDAO> query, ShowingWarehouseFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ShowingWarehouseDAO> initQuery = query.Where(q => false);
            foreach (ShowingWarehouseFilter ShowingWarehouseFilter in filter.OrFilter)
            {
                IQueryable<ShowingWarehouseDAO> queryable = query;
                if (ShowingWarehouseFilter.Id != null && ShowingWarehouseFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ShowingWarehouseFilter.Code != null && ShowingWarehouseFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (ShowingWarehouseFilter.Name != null && ShowingWarehouseFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (ShowingWarehouseFilter.Address != null && ShowingWarehouseFilter.Address.HasValue)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (ShowingWarehouseFilter.OrganizationId != null && ShowingWarehouseFilter.OrganizationId.HasValue)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (ShowingWarehouseFilter.ProvinceId != null && ShowingWarehouseFilter.ProvinceId.HasValue)
                    queryable = queryable.Where(q => q.ProvinceId.HasValue).Where(q => q.ProvinceId.Value, filter.ProvinceId);
                if (ShowingWarehouseFilter.DistrictId != null && ShowingWarehouseFilter.DistrictId.HasValue)
                    queryable = queryable.Where(q => q.DistrictId.HasValue).Where(q => q.DistrictId.Value, filter.DistrictId);
                if (ShowingWarehouseFilter.WardId != null && ShowingWarehouseFilter.WardId.HasValue)
                    queryable = queryable.Where(q => q.WardId.HasValue).Where(q => q.WardId.Value, filter.WardId);
                if (ShowingWarehouseFilter.StatusId != null && ShowingWarehouseFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (ShowingWarehouseFilter.RowId != null && ShowingWarehouseFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ShowingWarehouseDAO> DynamicOrder(IQueryable<ShowingWarehouseDAO> query, ShowingWarehouseFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ShowingWarehouseOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ShowingWarehouseOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ShowingWarehouseOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ShowingWarehouseOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case ShowingWarehouseOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case ShowingWarehouseOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case ShowingWarehouseOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case ShowingWarehouseOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case ShowingWarehouseOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ShowingWarehouseOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ShowingWarehouseOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ShowingWarehouseOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ShowingWarehouseOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ShowingWarehouseOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case ShowingWarehouseOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case ShowingWarehouseOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case ShowingWarehouseOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case ShowingWarehouseOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case ShowingWarehouseOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ShowingWarehouseOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ShowingWarehouse>> DynamicSelect(IQueryable<ShowingWarehouseDAO> query, ShowingWarehouseFilter filter)
        {
            List<ShowingWarehouse> ShowingWarehouses = await query.Select(q => new ShowingWarehouse()
            {
                Id = filter.Selects.Contains(ShowingWarehouseSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ShowingWarehouseSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ShowingWarehouseSelect.Name) ? q.Name : default(string),
                Address = filter.Selects.Contains(ShowingWarehouseSelect.Address) ? q.Address : default(string),
                OrganizationId = filter.Selects.Contains(ShowingWarehouseSelect.Organization) ? q.OrganizationId : default(long),
                ProvinceId = filter.Selects.Contains(ShowingWarehouseSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(ShowingWarehouseSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(ShowingWarehouseSelect.Ward) ? q.WardId : default(long?),
                StatusId = filter.Selects.Contains(ShowingWarehouseSelect.Status) ? q.StatusId : default(long),
                RowId = filter.Selects.Contains(ShowingWarehouseSelect.Row) ? q.RowId : default(Guid),
                District = filter.Selects.Contains(ShowingWarehouseSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                    RowId = q.District.RowId,
                } : null,
                Organization = filter.Selects.Contains(ShowingWarehouseSelect.Organization) && q.Organization != null ? new Organization
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
                Province = filter.Selects.Contains(ShowingWarehouseSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                    RowId = q.Province.RowId,
                } : null,
                Status = filter.Selects.Contains(ShowingWarehouseSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Ward = filter.Selects.Contains(ShowingWarehouseSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                    RowId = q.Ward.RowId,
                } : null,
            }).ToListAsync();
            return ShowingWarehouses;
        }

        public async Task<int> Count(ShowingWarehouseFilter filter)
        {
            IQueryable<ShowingWarehouseDAO> ShowingWarehouses = DataContext.ShowingWarehouse.AsNoTracking();
            ShowingWarehouses = DynamicFilter(ShowingWarehouses, filter);
            return await ShowingWarehouses.CountAsync();
        }

        public async Task<List<ShowingWarehouse>> List(ShowingWarehouseFilter filter)
        {
            if (filter == null) return new List<ShowingWarehouse>();
            IQueryable<ShowingWarehouseDAO> ShowingWarehouseDAOs = DataContext.ShowingWarehouse.AsNoTracking();
            ShowingWarehouseDAOs = DynamicFilter(ShowingWarehouseDAOs, filter);
            ShowingWarehouseDAOs = DynamicOrder(ShowingWarehouseDAOs, filter);
            List<ShowingWarehouse> ShowingWarehouses = await DynamicSelect(ShowingWarehouseDAOs, filter);
            return ShowingWarehouses;
        }

        public async Task<List<ShowingWarehouse>> List(List<long> Ids)
        {
            List<ShowingWarehouse> ShowingWarehouses = await DataContext.ShowingWarehouse.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ShowingWarehouse()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                OrganizationId = x.OrganizationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
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
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                },
            }).ToListAsync();
            
            List<ShowingInventory> ShowingInventories = await DataContext.ShowingInventory.AsNoTracking()
                .Where(x => Ids.Contains(x.ShowingWarehouseId))
                .Where(x => x.DeletedAt == null)
                .Select(x => new ShowingInventory
                {
                    Id = x.Id,
                    ShowingWarehouseId = x.ShowingWarehouseId,
                    ShowingItemId = x.ShowingItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    AppUserId = x.AppUserId,
                    AppUser = new AppUser
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
                    ShowingItem = new ShowingItem
                    {
                        Id = x.ShowingItem.Id,
                        Code = x.ShowingItem.Code,
                        Name = x.ShowingItem.Name,
                        CategoryId = x.ShowingItem.CategoryId,
                        UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                        SalePrice = x.ShowingItem.SalePrice,
                        Desception = x.ShowingItem.Desception,
                        StatusId = x.ShowingItem.StatusId,
                        Used = x.ShowingItem.Used,
                        RowId = x.ShowingItem.RowId,
                    },
                }).ToListAsync();
            foreach(ShowingWarehouse ShowingWarehouse in ShowingWarehouses)
            {
                ShowingWarehouse.ShowingInventories = ShowingInventories
                    .Where(x => x.ShowingWarehouseId == ShowingWarehouse.Id)
                    .ToList();
            }

            return ShowingWarehouses;
        }

        public async Task<ShowingWarehouse> Get(long Id)
        {
            ShowingWarehouse ShowingWarehouse = await DataContext.ShowingWarehouse.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ShowingWarehouse()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                OrganizationId = x.OrganizationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
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
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                },
            }).FirstOrDefaultAsync();

            if (ShowingWarehouse == null)
                return null;
            ShowingWarehouse.ShowingInventories = await DataContext.ShowingInventory.AsNoTracking()
                .Where(x => x.ShowingWarehouseId == ShowingWarehouse.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new ShowingInventory
                {
                    Id = x.Id,
                    ShowingWarehouseId = x.ShowingWarehouseId,
                    ShowingItemId = x.ShowingItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    AppUserId = x.AppUserId,
                    AppUser = new AppUser
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
                    ShowingItem = new ShowingItem
                    {
                        Id = x.ShowingItem.Id,
                        Code = x.ShowingItem.Code,
                        Name = x.ShowingItem.Name,
                        CategoryId = x.ShowingItem.CategoryId,
                        UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                        SalePrice = x.ShowingItem.SalePrice,
                        Desception = x.ShowingItem.Desception,
                        StatusId = x.ShowingItem.StatusId,
                        Used = x.ShowingItem.Used,
                        RowId = x.ShowingItem.RowId,
                    },
                }).ToListAsync();

            return ShowingWarehouse;
        }
        public async Task<bool> Create(ShowingWarehouse ShowingWarehouse)
        {
            ShowingWarehouseDAO ShowingWarehouseDAO = new ShowingWarehouseDAO();
            ShowingWarehouseDAO.Id = ShowingWarehouse.Id;
            ShowingWarehouseDAO.Code = ShowingWarehouse.Code;
            ShowingWarehouseDAO.Name = ShowingWarehouse.Name;
            ShowingWarehouseDAO.Address = ShowingWarehouse.Address;
            ShowingWarehouseDAO.OrganizationId = ShowingWarehouse.OrganizationId;
            ShowingWarehouseDAO.ProvinceId = ShowingWarehouse.ProvinceId;
            ShowingWarehouseDAO.DistrictId = ShowingWarehouse.DistrictId;
            ShowingWarehouseDAO.WardId = ShowingWarehouse.WardId;
            ShowingWarehouseDAO.StatusId = ShowingWarehouse.StatusId;
            ShowingWarehouseDAO.RowId = ShowingWarehouse.RowId;
            ShowingWarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
            ShowingWarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ShowingWarehouse.Add(ShowingWarehouseDAO);
            await DataContext.SaveChangesAsync();
            ShowingWarehouse.Id = ShowingWarehouseDAO.Id;
            await SaveReference(ShowingWarehouse);
            return true;
        }

        public async Task<bool> Update(ShowingWarehouse ShowingWarehouse)
        {
            ShowingWarehouseDAO ShowingWarehouseDAO = DataContext.ShowingWarehouse.Where(x => x.Id == ShowingWarehouse.Id).FirstOrDefault();
            if (ShowingWarehouseDAO == null)
                return false;
            ShowingWarehouseDAO.Id = ShowingWarehouse.Id;
            ShowingWarehouseDAO.Code = ShowingWarehouse.Code;
            ShowingWarehouseDAO.Name = ShowingWarehouse.Name;
            ShowingWarehouseDAO.Address = ShowingWarehouse.Address;
            ShowingWarehouseDAO.OrganizationId = ShowingWarehouse.OrganizationId;
            ShowingWarehouseDAO.ProvinceId = ShowingWarehouse.ProvinceId;
            ShowingWarehouseDAO.DistrictId = ShowingWarehouse.DistrictId;
            ShowingWarehouseDAO.WardId = ShowingWarehouse.WardId;
            ShowingWarehouseDAO.StatusId = ShowingWarehouse.StatusId;
            ShowingWarehouseDAO.RowId = ShowingWarehouse.RowId;
            ShowingWarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ShowingWarehouse);
            return true;
        }

        public async Task<bool> Delete(ShowingWarehouse ShowingWarehouse)
        {
            await DataContext.ShowingWarehouse.Where(x => x.Id == ShowingWarehouse.Id).UpdateFromQueryAsync(x => new ShowingWarehouseDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ShowingWarehouse> ShowingWarehouses)
        {
            List<ShowingWarehouseDAO> ShowingWarehouseDAOs = new List<ShowingWarehouseDAO>();
            foreach (ShowingWarehouse ShowingWarehouse in ShowingWarehouses)
            {
                ShowingWarehouseDAO ShowingWarehouseDAO = new ShowingWarehouseDAO();
                ShowingWarehouseDAO.Id = ShowingWarehouse.Id;
                ShowingWarehouseDAO.Code = ShowingWarehouse.Code;
                ShowingWarehouseDAO.Name = ShowingWarehouse.Name;
                ShowingWarehouseDAO.Address = ShowingWarehouse.Address;
                ShowingWarehouseDAO.OrganizationId = ShowingWarehouse.OrganizationId;
                ShowingWarehouseDAO.ProvinceId = ShowingWarehouse.ProvinceId;
                ShowingWarehouseDAO.DistrictId = ShowingWarehouse.DistrictId;
                ShowingWarehouseDAO.WardId = ShowingWarehouse.WardId;
                ShowingWarehouseDAO.StatusId = ShowingWarehouse.StatusId;
                ShowingWarehouseDAO.RowId = ShowingWarehouse.RowId;
                ShowingWarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
                ShowingWarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
                ShowingWarehouseDAOs.Add(ShowingWarehouseDAO);
            }
            await DataContext.BulkMergeAsync(ShowingWarehouseDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ShowingWarehouse> ShowingWarehouses)
        {
            List<long> Ids = ShowingWarehouses.Select(x => x.Id).ToList();
            await DataContext.ShowingWarehouse
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ShowingWarehouseDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ShowingWarehouse ShowingWarehouse)
        {
            List<ShowingInventoryDAO> ShowingInventoryDAOs = await DataContext.ShowingInventory
                .Where(x => x.ShowingWarehouseId == ShowingWarehouse.Id).ToListAsync();
            ShowingInventoryDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (ShowingWarehouse.ShowingInventories != null)
            {
                foreach (ShowingInventory ShowingInventory in ShowingWarehouse.ShowingInventories)
                {
                    ShowingInventoryDAO ShowingInventoryDAO = ShowingInventoryDAOs
                        .Where(x => x.Id == ShowingInventory.Id && x.Id != 0).FirstOrDefault();
                    if (ShowingInventoryDAO == null)
                    {
                        ShowingInventoryDAO = new ShowingInventoryDAO();
                        ShowingInventoryDAO.Id = ShowingInventory.Id;
                        ShowingInventoryDAO.ShowingWarehouseId = ShowingWarehouse.Id;
                        ShowingInventoryDAO.ShowingItemId = ShowingInventory.ShowingItemId;
                        ShowingInventoryDAO.SaleStock = ShowingInventory.SaleStock;
                        ShowingInventoryDAO.AccountingStock = ShowingInventory.AccountingStock;
                        ShowingInventoryDAO.AppUserId = ShowingInventory.AppUserId;
                        ShowingInventoryDAOs.Add(ShowingInventoryDAO);
                        ShowingInventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                        ShowingInventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ShowingInventoryDAO.DeletedAt = null;
                    }
                    else
                    {
                        ShowingInventoryDAO.Id = ShowingInventory.Id;
                        ShowingInventoryDAO.ShowingWarehouseId = ShowingWarehouse.Id;
                        ShowingInventoryDAO.ShowingItemId = ShowingInventory.ShowingItemId;
                        ShowingInventoryDAO.SaleStock = ShowingInventory.SaleStock;
                        ShowingInventoryDAO.AccountingStock = ShowingInventory.AccountingStock;
                        ShowingInventoryDAO.AppUserId = ShowingInventory.AppUserId;
                        ShowingInventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ShowingInventoryDAO.DeletedAt = null;
                    }
                }
                await DataContext.ShowingInventory.BulkMergeAsync(ShowingInventoryDAOs);
            }
        }
        
    }
}
