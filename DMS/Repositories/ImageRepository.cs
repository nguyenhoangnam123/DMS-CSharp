using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IImageRepository
    {
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<bool> Create(Image Image);
        Task<bool> Delete(Image Image);
    }
    public class ImageRepository : IImageRepository
    {
        private DataContext DataContext;
        public ImageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ImageDAO> DynamicFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Url != null)
                query = query.Where(q => q.Url, filter.Url);
            if (filter.ThumbnailUrl != null)
                query = query.Where(q => q.ThumbnailUrl, filter.ThumbnailUrl);
            
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ImageDAO> OrFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ImageDAO> initQuery = query.Where(q => false);
            foreach (ImageFilter ImageFilter in filter.OrFilter)
            {
                IQueryable<ImageDAO> queryable = query;
                if (ImageFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ImageFilter.Id);
                if (ImageFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ImageFilter.Name);
                if (ImageFilter.Url != null)
                    queryable = queryable.Where(q => q.Url, ImageFilter.Url);
                if (ImageFilter.ThumbnailUrl != null)
                    queryable = queryable.Where(q => q.ThumbnailUrl, ImageFilter.ThumbnailUrl);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ImageDAO> DynamicOrder(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderBy(q => q.Url);
                            break;
                        case ImageOrder.ThumbnailUrl:
                            query = query.OrderBy(q => q.ThumbnailUrl);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderByDescending(q => q.Url);
                            break;
                        case ImageOrder.ThumbnailUrl:
                            query = query.OrderByDescending(q => q.ThumbnailUrl);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Image>> DynamicSelect(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            List<Image> Images = await query.Select(q => new Image()
            {
                Id = filter.Selects.Contains(ImageSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ImageSelect.Name) ? q.Name : default(string),
                Url = filter.Selects.Contains(ImageSelect.Url) ? q.Url : default(string),
                ThumbnailUrl = filter.Selects.Contains(ImageSelect.ThumbnailUrl) ? q.ThumbnailUrl : default(string),
            }).ToListAsync();
            return Images;
        }

        public async Task<int> Count(ImageFilter filter)
        {
            IQueryable<ImageDAO> Images = DataContext.Image;
            Images = DynamicFilter(Images, filter);
            return await Images.CountAsync();
        }

        public async Task<List<Image>> List(ImageFilter filter)
        {
            if (filter == null) return new List<Image>();
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = DynamicFilter(ImageDAOs, filter);
            ImageDAOs = DynamicOrder(ImageDAOs, filter);
            List<Image> Images = await DynamicSelect(ImageDAOs, filter);
            return Images;
        }

        public async Task<Image> Get(long Id)
        {
            Image Image = await DataContext.Image.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Image()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url,
                    ThumbnailUrl = x.ThumbnailUrl,
                }).FirstOrDefaultAsync();

            if (Image == null)
                return null;

            return Image;
        }
        public async Task<bool> Create(Image Image)
        {
            ImageDAO ImageDAO = new ImageDAO();
            ImageDAO.Id = Image.Id;
            ImageDAO.Name = Image.Name;
            ImageDAO.Url = Image.Url;
            ImageDAO.ThumbnailUrl = Image.ThumbnailUrl;
            ImageDAO.RowId = Image.RowId;
            ImageDAO.CreatedAt = StaticParams.DateTimeNow;
            ImageDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.BulkMergeAsync<ImageDAO>(new List<ImageDAO> { ImageDAO });
            return true;
        }

        public async Task<bool> Delete(Image Image)
        {
            await DataContext.Image.Where(x => x.Id == Image.Id).UpdateFromQueryAsync(x => new ImageDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

    }
}
