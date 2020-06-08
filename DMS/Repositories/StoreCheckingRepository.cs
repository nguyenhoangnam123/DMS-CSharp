using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IStoreCheckingRepository
    {
        Task<int> Count(StoreCheckingFilter StoreCheckingFilter);
        Task<List<StoreChecking>> List(StoreCheckingFilter StoreCheckingFilter);
        Task<StoreChecking> Get(long Id);
        Task<bool> Create(StoreChecking StoreChecking);
        Task<bool> Update(StoreChecking StoreChecking);
    }
    public class StoreCheckingRepository : IStoreCheckingRepository
    {
        private DataContext DataContext;
        public StoreCheckingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreCheckingDAO> DynamicFilter(IQueryable<StoreCheckingDAO> query, StoreCheckingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.StoreId != null)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.SaleEmployeeId != null)
                query = query.Where(q => q.SaleEmployeeId, filter.SaleEmployeeId);
            if (filter.Longtitude != null)
                query = query.Where(q => q.Longtitude, filter.Longtitude);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.CheckInAt != null)
                query = query.Where(q => q.CheckInAt, filter.CheckInAt);
            if (filter.CheckOutAt != null)
                query = query.Where(q => q.CheckOutAt, filter.CheckOutAt);
            if (filter.CountIndirectSalesOrder != null)
                query = query.Where(q => q.IndirectSalesOrderCounter, filter.CountIndirectSalesOrder);
            if (filter.CountImage != null)
                query = query.Where(q => q.ImageCounter, filter.CountImage);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<StoreCheckingDAO> OrFilter(IQueryable<StoreCheckingDAO> query, StoreCheckingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreCheckingDAO> initQuery = query.Where(q => false);
            foreach (StoreCheckingFilter StoreCheckingFilter in filter.OrFilter)
            {
                IQueryable<StoreCheckingDAO> queryable = query;
                if (StoreCheckingFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, StoreCheckingFilter.Id);
                if (StoreCheckingFilter.StoreId != null)
                    queryable = queryable.Where(q => q.StoreId, StoreCheckingFilter.StoreId);
                if (StoreCheckingFilter.SaleEmployeeId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, StoreCheckingFilter.SaleEmployeeId);
                if (StoreCheckingFilter.Longtitude != null)
                    queryable = queryable.Where(q => q.Longtitude, StoreCheckingFilter.Longtitude);
                if (StoreCheckingFilter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, StoreCheckingFilter.Latitude);
                if (StoreCheckingFilter.CheckInAt != null)
                    queryable = queryable.Where(q => q.CheckInAt, StoreCheckingFilter.CheckInAt);
                if (StoreCheckingFilter.CheckOutAt != null)
                    queryable = queryable.Where(q => q.CheckOutAt, StoreCheckingFilter.CheckOutAt);
                if (StoreCheckingFilter.CountIndirectSalesOrder != null)
                    queryable = queryable.Where(q => q.IndirectSalesOrderCounter, StoreCheckingFilter.CountIndirectSalesOrder);
                if (StoreCheckingFilter.CountImage != null)
                    queryable = queryable.Where(q => q.ImageCounter, StoreCheckingFilter.CountImage);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<StoreCheckingDAO> DynamicOrder(IQueryable<StoreCheckingDAO> query, StoreCheckingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreCheckingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreCheckingOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case StoreCheckingOrder.SaleEmployee:
                            query = query.OrderBy(q => q.SaleEmployeeId);
                            break;
                        case StoreCheckingOrder.Longtitude:
                            query = query.OrderBy(q => q.Longtitude);
                            break;
                        case StoreCheckingOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case StoreCheckingOrder.CheckInAt:
                            query = query.OrderBy(q => q.CheckInAt);
                            break;
                        case StoreCheckingOrder.CheckOutAt:
                            query = query.OrderBy(q => q.CheckOutAt);
                            break;
                        case StoreCheckingOrder.CountIndirectSalesOrder:
                            query = query.OrderBy(q => q.IndirectSalesOrderCounter);
                            break;
                        case StoreCheckingOrder.CountImage:
                            query = query.OrderBy(q => q.ImageCounter);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreCheckingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreCheckingOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case StoreCheckingOrder.SaleEmployee:
                            query = query.OrderByDescending(q => q.SaleEmployeeId);
                            break;
                        case StoreCheckingOrder.Longtitude:
                            query = query.OrderByDescending(q => q.Longtitude);
                            break;
                        case StoreCheckingOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case StoreCheckingOrder.CheckInAt:
                            query = query.OrderByDescending(q => q.CheckInAt);
                            break;
                        case StoreCheckingOrder.CheckOutAt:
                            query = query.OrderByDescending(q => q.CheckOutAt);
                            break;
                        case StoreCheckingOrder.CountIndirectSalesOrder:
                            query = query.OrderByDescending(q => q.IndirectSalesOrderCounter);
                            break;
                        case StoreCheckingOrder.CountImage:
                            query = query.OrderByDescending(q => q.ImageCounter);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreChecking>> DynamicSelect(IQueryable<StoreCheckingDAO> query, StoreCheckingFilter filter)
        {
            List<StoreChecking> StoreCheckings = await query.Select(q => new StoreChecking()
            {
                Id = filter.Selects.Contains(StoreCheckingSelect.Id) ? q.Id : default(long),
                StoreId = filter.Selects.Contains(StoreCheckingSelect.Store) ? q.StoreId : default(long),
                SaleEmployeeId = filter.Selects.Contains(StoreCheckingSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                Longtitude = filter.Selects.Contains(StoreCheckingSelect.Longtitude) ? q.Longtitude : default(decimal?),
                Latitude = filter.Selects.Contains(StoreCheckingSelect.Latitude) ? q.Latitude : default(decimal?),
                CheckInAt = filter.Selects.Contains(StoreCheckingSelect.CheckInAt) ? q.CheckInAt : default(DateTime?),
                CheckOutAt = filter.Selects.Contains(StoreCheckingSelect.CheckOutAt) ? q.CheckOutAt : default(DateTime?),
                CountIndirectSalesOrder = filter.Selects.Contains(StoreCheckingSelect.CountIndirectSalesOrder) ? q.IndirectSalesOrderCounter : default(long?),
                CountImage = filter.Selects.Contains(StoreCheckingSelect.CountImage) ? q.ImageCounter : default(long?),
                SaleEmployee = filter.Selects.Contains(StoreCheckingSelect.SaleEmployee) && q.SaleEmployee == null ? null : new AppUser
                {
                    Id = q.SaleEmployee.Id,
                    Username = q.SaleEmployee.Username,
                    DisplayName = q.SaleEmployee.DisplayName,
                    Email = q.SaleEmployee.Email,
                    Phone = q.SaleEmployee.Phone,
                    Address = q.SaleEmployee.Address,
                    Department = q.SaleEmployee.Department,
                    PositionId = q.SaleEmployee.PositionId,
                    RowId = q.SaleEmployee.RowId,
                    SexId = q.SaleEmployee.SexId,
                    StatusId = q.SaleEmployee.StatusId,
                    OrganizationId = q.SaleEmployee.OrganizationId,
                    Organization = q.SaleEmployee.Organization == null ? null : new Organization
                    {
                        Id = q.SaleEmployee.Organization.Id,
                        Code = q.SaleEmployee.Organization.Code,
                        Name = q.SaleEmployee.Organization.Name,
                    },
                },
                Store = filter.Selects.Contains(StoreCheckingSelect.Store) && q.Store == null ? null : new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    Name = q.Store.Name,
                    ParentStoreId = q.Store.ParentStoreId,
                    OrganizationId = q.Store.OrganizationId,
                    StoreTypeId = q.Store.StoreTypeId,
                    StoreGroupingId = q.Store.StoreGroupingId,
                    Telephone = q.Store.Telephone,
                    ProvinceId = q.Store.ProvinceId,
                    DistrictId = q.Store.DistrictId,
                    WardId = q.Store.WardId,
                    Address = q.Store.Address,
                    DeliveryAddress = q.Store.DeliveryAddress,
                    Latitude = q.Store.Latitude,
                    Longitude = q.Store.Longitude,
                    OwnerName = q.Store.OwnerName,
                    OwnerPhone = q.Store.OwnerPhone,
                    OwnerEmail = q.Store.OwnerEmail,
                    TaxCode = q.Store.TaxCode,
                    LegalEntity = q.Store.LegalEntity,
                    StatusId = q.Store.StatusId,
                }
            }).ToListAsync();
            return StoreCheckings;
        }

        public async Task<int> Count(StoreCheckingFilter filter)
        {
            IQueryable<StoreCheckingDAO> StoreCheckings = DataContext.StoreChecking.AsNoTracking();
            StoreCheckings = DynamicFilter(StoreCheckings, filter);
            return await StoreCheckings.CountAsync();
        }

        public async Task<List<StoreChecking>> List(StoreCheckingFilter filter)
        {
            if (filter == null) return new List<StoreChecking>();
            IQueryable<StoreCheckingDAO> StoreCheckingDAOs = DataContext.StoreChecking.AsNoTracking();
            StoreCheckingDAOs = DynamicFilter(StoreCheckingDAOs, filter);
            StoreCheckingDAOs = DynamicOrder(StoreCheckingDAOs, filter);
            List<StoreChecking> StoreCheckings = await DynamicSelect(StoreCheckingDAOs, filter);
            return StoreCheckings;
        }

        public async Task<StoreChecking> Get(long Id)
        {
            StoreChecking StoreChecking = await DataContext.StoreChecking.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new StoreChecking()
            {
                Id = x.Id,
                StoreId = x.StoreId,
                SaleEmployeeId = x.SaleEmployeeId,
                Longtitude = x.Longtitude,
                Latitude = x.Latitude,
                CheckInAt = x.CheckInAt,
                CheckOutAt = x.CheckOutAt,
                CountIndirectSalesOrder = x.IndirectSalesOrderCounter,
                CountImage = x.ImageCounter,
                SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                {
                    Id = x.SaleEmployee.Id,
                    Username = x.SaleEmployee.Username,
                    DisplayName = x.SaleEmployee.DisplayName,
                    Email = x.SaleEmployee.Email,
                    Phone = x.SaleEmployee.Phone,
                    Address = x.SaleEmployee.Address,
                    Department = x.SaleEmployee.Department,
                    PositionId = x.SaleEmployee.PositionId,
                    RowId = x.SaleEmployee.RowId,
                    SexId = x.SaleEmployee.SexId,
                    StatusId = x.SaleEmployee.StatusId,
                    OrganizationId = x.SaleEmployee.OrganizationId,
                    Organization = x.SaleEmployee.Organization == null ? null : new Organization
                    {
                        Id = x.SaleEmployee.Organization.Id,
                        Code = x.SaleEmployee.Organization.Code,
                        Name = x.SaleEmployee.Organization.Name,
                    },
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
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    DeliveryAddress = x.Store.DeliveryAddress,
                    Latitude = x.Store.Latitude,
                    Longitude = x.Store.Longitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    TaxCode = x.Store.TaxCode,
                    LegalEntity = x.Store.LegalEntity,
                    StatusId = x.Store.StatusId,
                }
            }).FirstOrDefaultAsync();

            if (StoreChecking == null)
                return null;
            StoreChecking.StoreCheckingImageMappings = await DataContext.StoreCheckingImageMapping.AsNoTracking()
                .Where(x => x.StoreCheckingId == StoreChecking.Id)
                .Where(x => x.Image.DeletedAt == null)
                .Select(x => new StoreCheckingImageMapping
                {
                    ImageId = x.ImageId,
                    StoreCheckingId = x.StoreCheckingId,
                    AlbumId = x.AlbumId,
                    StoreId = x.StoreId,
                    SaleEmployeeId = x.SaleEmployeeId,
                    ShootingAt = x.ShootingAt,
                    Album = new Album
                    {
                        Id = x.Album.Id,
                        Name = x.Album.Name,
                    },
                    SaleEmployee = new AppUser
                    {
                        Id = x.SaleEmployee.Id,
                        Username = x.SaleEmployee.Username,
                        DisplayName = x.SaleEmployee.DisplayName,
                        Email = x.SaleEmployee.Email,
                        Phone = x.SaleEmployee.Phone,
                    },
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                    Store = new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                    },
                }).ToListAsync();

            return StoreChecking;
        }
        public async Task<bool> Create(StoreChecking StoreChecking)
        {
            StoreCheckingDAO StoreCheckingDAO = new StoreCheckingDAO();
            StoreCheckingDAO.Id = StoreChecking.Id;
            StoreCheckingDAO.StoreId = StoreChecking.StoreId;
            StoreCheckingDAO.SaleEmployeeId = StoreChecking.SaleEmployeeId;
            StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
            StoreCheckingDAO.Latitude = StoreChecking.Latitude;
            StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
            StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
            StoreCheckingDAO.IndirectSalesOrderCounter = StoreChecking.CountIndirectSalesOrder;
            StoreCheckingDAO.ImageCounter = StoreChecking.CountImage;
            DataContext.StoreChecking.Add(StoreCheckingDAO);
            await DataContext.SaveChangesAsync();
            StoreChecking.Id = StoreCheckingDAO.Id;
            await SaveReference(StoreChecking);
            return true;
        }

        public async Task<bool> Update(StoreChecking StoreChecking)
        {
            StoreCheckingDAO StoreCheckingDAO = DataContext.StoreChecking.Where(x => x.Id == StoreChecking.Id).FirstOrDefault();
            if (StoreCheckingDAO == null)
                return false;
            StoreCheckingDAO.Id = StoreChecking.Id;
            StoreCheckingDAO.StoreId = StoreChecking.StoreId;
            StoreCheckingDAO.SaleEmployeeId = StoreChecking.SaleEmployeeId;
            StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
            StoreCheckingDAO.Latitude = StoreChecking.Latitude;
            StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
            StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
            StoreCheckingDAO.IndirectSalesOrderCounter = StoreChecking.CountIndirectSalesOrder;
            StoreCheckingDAO.ImageCounter = StoreChecking.CountImage;
            await DataContext.SaveChangesAsync();
            await SaveReference(StoreChecking);
            return true;
        }

        public async Task<bool> BulkMerge(List<StoreChecking> StoreCheckings)
        {
            List<StoreCheckingDAO> StoreCheckingDAOs = new List<StoreCheckingDAO>();
            foreach (StoreChecking StoreChecking in StoreCheckings)
            {
                StoreCheckingDAO StoreCheckingDAO = new StoreCheckingDAO();
                StoreCheckingDAO.Id = StoreChecking.Id;
                StoreCheckingDAO.StoreId = StoreChecking.StoreId;
                StoreCheckingDAO.SaleEmployeeId = StoreChecking.SaleEmployeeId;
                StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
                StoreCheckingDAO.Latitude = StoreChecking.Latitude;
                StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
                StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
                StoreCheckingDAO.IndirectSalesOrderCounter = StoreChecking.CountIndirectSalesOrder;
                StoreCheckingDAO.ImageCounter = StoreChecking.CountImage;
                StoreCheckingDAOs.Add(StoreCheckingDAO);
            }
            await DataContext.BulkMergeAsync(StoreCheckingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<StoreChecking> StoreCheckings)
        {
            List<long> Ids = StoreCheckings.Select(x => x.Id).ToList();
            await DataContext.StoreChecking
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(StoreChecking StoreChecking)
        {
            await DataContext.StoreCheckingImageMapping
                .Where(x => x.StoreCheckingId == StoreChecking.Id)
                .DeleteFromQueryAsync();
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = new List<StoreCheckingImageMappingDAO>();
            if (StoreChecking.StoreCheckingImageMappings != null)
            {
                foreach (StoreCheckingImageMapping StoreCheckingImageMapping in StoreChecking.StoreCheckingImageMappings)
                {
                    StoreCheckingImageMappingDAO StoreCheckingImageMappingDAO = new StoreCheckingImageMappingDAO();
                    StoreCheckingImageMappingDAO.ImageId = StoreCheckingImageMapping.ImageId;
                    StoreCheckingImageMappingDAO.StoreCheckingId = StoreChecking.Id;
                    StoreCheckingImageMappingDAO.AlbumId = StoreCheckingImageMapping.AlbumId;
                    StoreCheckingImageMappingDAO.StoreId = StoreCheckingImageMapping.StoreId;
                    StoreCheckingImageMappingDAO.SaleEmployeeId = StoreCheckingImageMapping.SaleEmployeeId;
                    StoreCheckingImageMappingDAO.ShootingAt = StoreCheckingImageMapping.ShootingAt;
                    StoreCheckingImageMappingDAOs.Add(StoreCheckingImageMappingDAO);
                }
                await DataContext.StoreCheckingImageMapping.BulkMergeAsync(StoreCheckingImageMappingDAOs);
            }
        }
    }
}
