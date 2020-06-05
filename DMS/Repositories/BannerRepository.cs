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
    public interface IBannerRepository
    {
        Task<int> Count(BannerFilter BannerFilter);
        Task<List<Banner>> List(BannerFilter BannerFilter);
        Task<Banner> Get(long Id);
        Task<bool> Create(Banner Banner);
        Task<bool> Update(Banner Banner);
        Task<bool> Delete(Banner Banner);
        Task<bool> BulkMerge(List<Banner> Banners);
        Task<bool> BulkDelete(List<Banner> Banners);
    }
    public class BannerRepository : IBannerRepository
    {
        private DataContext DataContext;
        public BannerRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<BannerDAO> DynamicFilter(IQueryable<BannerDAO> query, BannerFilter filter)
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
            if (filter.Title != null)
                query = query.Where(q => q.Title, filter.Title);
            if (filter.Priority != null)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.Content != null)
                query = query.Where(q => q.Content, filter.Content);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.ImageId != null)
                query = query.Where(q => q.ImageId, filter.ImageId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<BannerDAO> OrFilter(IQueryable<BannerDAO> query, BannerFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<BannerDAO> initQuery = query.Where(q => false);
            foreach (BannerFilter BannerFilter in filter.OrFilter)
            {
                IQueryable<BannerDAO> queryable = query;
                if (BannerFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, BannerFilter.Id);
                if (BannerFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, BannerFilter.Code);
                if (BannerFilter.Title != null)
                    queryable = queryable.Where(q => q.Title, BannerFilter.Title);
                if (BannerFilter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, BannerFilter.Priority);
                if (BannerFilter.Content != null)
                    queryable = queryable.Where(q => q.Content, BannerFilter.Content);
                if (BannerFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, BannerFilter.CreatorId);
                if (BannerFilter.ImageId != null)
                    queryable = queryable.Where(q => q.ImageId, BannerFilter.ImageId);
                if (BannerFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, BannerFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<BannerDAO> DynamicOrder(IQueryable<BannerDAO> query, BannerFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case BannerOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case BannerOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case BannerOrder.Title:
                            query = query.OrderBy(q => q.Title);
                            break;
                        case BannerOrder.Priority:
                            query = query.OrderBy(q => q.Priority == null).ThenBy(x => x.Priority);
                            break;
                        case BannerOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case BannerOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case BannerOrder.Image:
                            query = query.OrderBy(q => q.ImageId);
                            break;
                        case BannerOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case BannerOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case BannerOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case BannerOrder.Title:
                            query = query.OrderByDescending(q => q.Title);
                            break;
                        case BannerOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority == null).ThenByDescending(x => x.Priority);
                            break;
                        case BannerOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case BannerOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case BannerOrder.Image:
                            query = query.OrderByDescending(q => q.ImageId);
                            break;
                        case BannerOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Banner>> DynamicSelect(IQueryable<BannerDAO> query, BannerFilter filter)
        {
            List<Banner> Banners = await query.Select(q => new Banner()
            {
                Id = filter.Selects.Contains(BannerSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(BannerSelect.Code) ? q.Code : default(string),
                Title = filter.Selects.Contains(BannerSelect.Title) ? q.Title : default(string),
                Priority = filter.Selects.Contains(BannerSelect.Priority) ? q.Priority : default(long?),
                Content = filter.Selects.Contains(BannerSelect.Content) ? q.Content : default(string),
                CreatorId = filter.Selects.Contains(BannerSelect.Creator) ? q.CreatorId : default(long),
                ImageId = filter.Selects.Contains(BannerSelect.Image) ? q.ImageId : default(long?),
                StatusId = filter.Selects.Contains(BannerSelect.Status) ? q.StatusId : default(long),
                Creator = filter.Selects.Contains(BannerSelect.Creator) && q.Creator != null ? new AppUser
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
                    SexId = q.Creator.SexId,
                    StatusId = q.Creator.StatusId,
                    Avatar = q.Creator.Avatar,
                    Birthday = q.Creator.Birthday,
                    ProvinceId = q.Creator.ProvinceId,
                } : null,
                Image = filter.Selects.Contains(BannerSelect.Image) && q.Image != null ? new Image
                {
                    Id = q.Image.Id,
                    Name = q.Image.Name,
                    Url = q.Image.Url,
                } : null,
                Status = filter.Selects.Contains(BannerSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return Banners;
        }

        public async Task<int> Count(BannerFilter filter)
        {
            IQueryable<BannerDAO> Banners = DataContext.Banner.AsNoTracking();
            Banners = DynamicFilter(Banners, filter);
            return await Banners.CountAsync();
        }

        public async Task<List<Banner>> List(BannerFilter filter)
        {
            if (filter == null) return new List<Banner>();
            IQueryable<BannerDAO> BannerDAOs = DataContext.Banner.AsNoTracking();
            BannerDAOs = DynamicFilter(BannerDAOs, filter);
            BannerDAOs = DynamicOrder(BannerDAOs, filter);
            List<Banner> Banners = await DynamicSelect(BannerDAOs, filter);
            return Banners;
        }

        public async Task<Banner> Get(long Id)
        {
            Banner Banner = await DataContext.Banner.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Banner()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Title = x.Title,
                Priority = x.Priority,
                Content = x.Content,
                CreatorId = x.CreatorId,
                ImageId = x.ImageId,
                StatusId = x.StatusId,
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
                    SexId = x.Creator.SexId,
                    StatusId = x.Creator.StatusId,
                    Avatar = x.Creator.Avatar,
                    Birthday = x.Creator.Birthday,
                    ProvinceId = x.Creator.ProvinceId,
                },
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Banner == null)
                return null;

            return Banner;
        }
        public async Task<bool> Create(Banner Banner)
        {
            BannerDAO BannerDAO = new BannerDAO();
            BannerDAO.Id = Banner.Id;
            BannerDAO.Code = Banner.Code;
            BannerDAO.Title = Banner.Title;
            BannerDAO.Priority = Banner.Priority;
            BannerDAO.Content = Banner.Content;
            BannerDAO.CreatorId = Banner.CreatorId;
            BannerDAO.ImageId = Banner.ImageId;
            BannerDAO.StatusId = Banner.StatusId;
            BannerDAO.CreatedAt = StaticParams.DateTimeNow;
            BannerDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Banner.Add(BannerDAO);
            await DataContext.SaveChangesAsync();
            Banner.Id = BannerDAO.Id;
            await SaveReference(Banner);
            return true;
        }

        public async Task<bool> Update(Banner Banner)
        {
            BannerDAO BannerDAO = DataContext.Banner.Where(x => x.Id == Banner.Id).FirstOrDefault();
            if (BannerDAO == null)
                return false;
            BannerDAO.Id = Banner.Id;
            BannerDAO.Code = Banner.Code;
            BannerDAO.Title = Banner.Title;
            BannerDAO.Priority = Banner.Priority;
            BannerDAO.Content = Banner.Content;
            BannerDAO.CreatorId = Banner.CreatorId;
            BannerDAO.ImageId = Banner.ImageId;
            BannerDAO.StatusId = Banner.StatusId;
            BannerDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Banner);
            return true;
        }

        public async Task<bool> Delete(Banner Banner)
        {
            await DataContext.Banner.Where(x => x.Id == Banner.Id).UpdateFromQueryAsync(x => new BannerDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Banner> Banners)
        {
            List<BannerDAO> BannerDAOs = new List<BannerDAO>();
            foreach (Banner Banner in Banners)
            {
                BannerDAO BannerDAO = new BannerDAO();
                BannerDAO.Id = Banner.Id;
                BannerDAO.Code = Banner.Code;
                BannerDAO.Title = Banner.Title;
                BannerDAO.Priority = Banner.Priority;
                BannerDAO.Content = Banner.Content;
                BannerDAO.CreatorId = Banner.CreatorId;
                BannerDAO.ImageId = Banner.ImageId;
                BannerDAO.StatusId = Banner.StatusId;
                BannerDAO.CreatedAt = StaticParams.DateTimeNow;
                BannerDAO.UpdatedAt = StaticParams.DateTimeNow;
                BannerDAOs.Add(BannerDAO);
            }
            await DataContext.BulkMergeAsync(BannerDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Banner> Banners)
        {
            List<long> Ids = Banners.Select(x => x.Id).ToList();
            await DataContext.Banner
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new BannerDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Banner Banner)
        {
        }
        
    }
}
