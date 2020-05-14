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
            if (filter.AppUserId != null)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.Longtitude != null)
                query = query.Where(q => q.Longtitude, filter.Longtitude);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.CheckInAt != null)
                query = query.Where(q => q.CheckInAt, filter.CheckInAt);
            if (filter.CheckOutAt != null)
                query = query.Where(q => q.CheckOutAt, filter.CheckOutAt);
            if (filter.CountIndirectSalesOrder != null)
                query = query.Where(q => q.CountIndirectSalesOrder, filter.CountIndirectSalesOrder);
            if (filter.CountImage != null)
                query = query.Where(q => q.CountImage, filter.CountImage);
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
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.StoreId != null)
                    queryable = queryable.Where(q => q.StoreId, filter.StoreId);
                if (filter.AppUserId != null)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                if (filter.Longtitude != null)
                    queryable = queryable.Where(q => q.Longtitude, filter.Longtitude);
                if (filter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, filter.Latitude);
                if (filter.CheckInAt != null)
                    queryable = queryable.Where(q => q.CheckInAt, filter.CheckInAt);
                if (filter.CheckOutAt != null)
                    queryable = queryable.Where(q => q.CheckOutAt, filter.CheckOutAt);
                if (filter.CountIndirectSalesOrder != null)
                    queryable = queryable.Where(q => q.CountIndirectSalesOrder, filter.CountIndirectSalesOrder);
                if (filter.CountImage != null)
                    queryable = queryable.Where(q => q.CountImage, filter.CountImage);
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
                        case StoreCheckingOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
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
                            query = query.OrderBy(q => q.CountIndirectSalesOrder);
                            break;
                        case StoreCheckingOrder.CountImage:
                            query = query.OrderBy(q => q.CountImage);
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
                        case StoreCheckingOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
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
                            query = query.OrderByDescending(q => q.CountIndirectSalesOrder);
                            break;
                        case StoreCheckingOrder.CountImage:
                            query = query.OrderByDescending(q => q.CountImage);
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
                AppUserId = filter.Selects.Contains(StoreCheckingSelect.AppUser) ? q.AppUserId : default(long),
                Longtitude = filter.Selects.Contains(StoreCheckingSelect.Longtitude) ? q.Longtitude : default(decimal?),
                Latitude = filter.Selects.Contains(StoreCheckingSelect.Latitude) ? q.Latitude : default(decimal?),
                CheckInAt = filter.Selects.Contains(StoreCheckingSelect.CheckInAt) ? q.CheckInAt : default(DateTime?),
                CheckOutAt = filter.Selects.Contains(StoreCheckingSelect.CheckOutAt) ? q.CheckOutAt : default(DateTime?),
                CountIndirectSalesOrder = filter.Selects.Contains(StoreCheckingSelect.CountIndirectSalesOrder) ? q.CountIndirectSalesOrder : default(long?),
                CountImage = filter.Selects.Contains(StoreCheckingSelect.CountImage) ? q.CountImage : default(long?),
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
                AppUserId = x.AppUserId,
                Longtitude = x.Longtitude,
                Latitude = x.Latitude,
                CheckInAt = x.CheckInAt,
                CheckOutAt = x.CheckOutAt,
                CountIndirectSalesOrder = x.CountIndirectSalesOrder,
                CountImage = x.CountImage,
            }).FirstOrDefaultAsync();

            if (StoreChecking == null)
                return null;
            StoreChecking.ImageStoreCheckingMappings = await DataContext.ImageStoreCheckingMapping.AsNoTracking()
                .Where(x => x.StoreCheckingId == StoreChecking.Id)
                .Where(x => x.Image.DeletedAt == null)
                .Select(x => new ImageStoreCheckingMapping
                {
                    ImageId = x.ImageId,
                    StoreCheckingId = x.StoreCheckingId,
                    AlbumId = x.AlbumId,
                    StoreId = x.StoreId,
                    AppUserId = x.AppUserId,
                    ShootingAt = x.ShootingAt,
                    Album = new Album
                    {
                        Id = x.Album.Id,
                        Name = x.Album.Name,
                    },
                    AppUser = new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
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
            StoreCheckingDAO.AppUserId = StoreChecking.AppUserId;
            StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
            StoreCheckingDAO.Latitude = StoreChecking.Latitude;
            StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
            StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
            StoreCheckingDAO.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            StoreCheckingDAO.CountImage = StoreChecking.CountImage;
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
            StoreCheckingDAO.AppUserId = StoreChecking.AppUserId;
            StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
            StoreCheckingDAO.Latitude = StoreChecking.Latitude;
            StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
            StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
            StoreCheckingDAO.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            StoreCheckingDAO.CountImage = StoreChecking.CountImage;
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
                StoreCheckingDAO.AppUserId = StoreChecking.AppUserId;
                StoreCheckingDAO.Longtitude = StoreChecking.Longtitude;
                StoreCheckingDAO.Latitude = StoreChecking.Latitude;
                StoreCheckingDAO.CheckInAt = StoreChecking.CheckInAt;
                StoreCheckingDAO.CheckOutAt = StoreChecking.CheckOutAt;
                StoreCheckingDAO.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
                StoreCheckingDAO.CountImage = StoreChecking.CountImage;
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
            await DataContext.ImageStoreCheckingMapping
                .Where(x => x.StoreCheckingId == StoreChecking.Id)
                .DeleteFromQueryAsync();
            List<ImageStoreCheckingMappingDAO> ImageStoreCheckingMappingDAOs = new List<ImageStoreCheckingMappingDAO>();
            if (StoreChecking.ImageStoreCheckingMappings != null)
            {
                foreach (ImageStoreCheckingMapping ImageStoreCheckingMapping in StoreChecking.ImageStoreCheckingMappings)
                {
                    ImageStoreCheckingMappingDAO ImageStoreCheckingMappingDAO = new ImageStoreCheckingMappingDAO();
                    ImageStoreCheckingMappingDAO.ImageId = ImageStoreCheckingMapping.ImageId;
                    ImageStoreCheckingMappingDAO.StoreCheckingId = StoreChecking.Id;
                    ImageStoreCheckingMappingDAO.AlbumId = ImageStoreCheckingMapping.AlbumId;
                    ImageStoreCheckingMappingDAO.StoreId = ImageStoreCheckingMapping.StoreId;
                    ImageStoreCheckingMappingDAO.AppUserId = ImageStoreCheckingMapping.AppUserId;
                    ImageStoreCheckingMappingDAO.ShootingAt = ImageStoreCheckingMapping.ShootingAt;
                    ImageStoreCheckingMappingDAOs.Add(ImageStoreCheckingMappingDAO);
                }
                await DataContext.ImageStoreCheckingMapping.BulkMergeAsync(ImageStoreCheckingMappingDAOs);
            }
        }
    }
}
