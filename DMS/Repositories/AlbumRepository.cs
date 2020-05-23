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
    public interface IAlbumRepository
    {
        Task<int> Count(AlbumFilter AlbumFilter);
        Task<List<Album>> List(AlbumFilter AlbumFilter);
        Task<Album> Get(long Id);
        Task<bool> Create(Album Album);
        Task<bool> Update(Album Album);
        Task<bool> Delete(Album Album);
        Task<bool> BulkMerge(List<Album> Albums);
        Task<bool> BulkDelete(List<Album> Albums);
    }
    public class AlbumRepository : IAlbumRepository
    {
        private DataContext DataContext;
        public AlbumRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AlbumDAO> DynamicFilter(IQueryable<AlbumDAO> query, AlbumFilter filter)
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
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<AlbumDAO> OrFilter(IQueryable<AlbumDAO> query, AlbumFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AlbumDAO> initQuery = query.Where(q => false);
            foreach (AlbumFilter AlbumFilter in filter.OrFilter)
            {
                IQueryable<AlbumDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<AlbumDAO> DynamicOrder(IQueryable<AlbumDAO> query, AlbumFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AlbumOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AlbumOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case AlbumOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AlbumOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AlbumOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case AlbumOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Album>> DynamicSelect(IQueryable<AlbumDAO> query, AlbumFilter filter)
        {
            List<Album> Albums = await query.Select(q => new Album()
            {
                Id = filter.Selects.Contains(AlbumSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(AlbumSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(AlbumSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(AlbumSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return Albums;
        }

        public async Task<int> Count(AlbumFilter filter)
        {
            IQueryable<AlbumDAO> Albums = DataContext.Album.AsNoTracking();
            Albums = DynamicFilter(Albums, filter);
            return await Albums.CountAsync();
        }

        public async Task<List<Album>> List(AlbumFilter filter)
        {
            if (filter == null) return new List<Album>();
            IQueryable<AlbumDAO> AlbumDAOs = DataContext.Album.AsNoTracking();
            AlbumDAOs = DynamicFilter(AlbumDAOs, filter);
            AlbumDAOs = DynamicOrder(AlbumDAOs, filter);
            List<Album> Albums = await DynamicSelect(AlbumDAOs, filter);
            return Albums;
        }

        public async Task<Album> Get(long Id)
        {
            Album Album = await DataContext.Album.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Album()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Name = x.Name,
                StatusId = x.StatusId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Album == null)
                return null;

            return Album;
        }
        public async Task<bool> Create(Album Album)
        {
            AlbumDAO AlbumDAO = new AlbumDAO();
            AlbumDAO.Id = Album.Id;
            AlbumDAO.Name = Album.Name;
            AlbumDAO.StatusId = Album.StatusId;
            AlbumDAO.CreatedAt = StaticParams.DateTimeNow;
            AlbumDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Album.Add(AlbumDAO);
            await DataContext.SaveChangesAsync();
            Album.Id = AlbumDAO.Id;
            await SaveReference(Album);
            return true;
        }

        public async Task<bool> Update(Album Album)
        {
            AlbumDAO AlbumDAO = DataContext.Album.Where(x => x.Id == Album.Id).FirstOrDefault();
            if (AlbumDAO == null)
                return false;
            AlbumDAO.Id = Album.Id;
            AlbumDAO.Name = Album.Name;
            AlbumDAO.StatusId = Album.StatusId;
            AlbumDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Album);
            return true;
        }

        public async Task<bool> Delete(Album Album)
        {
            await DataContext.Album.Where(x => x.Id == Album.Id).UpdateFromQueryAsync(x => new AlbumDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Album> Albums)
        {
            List<AlbumDAO> AlbumDAOs = new List<AlbumDAO>();
            foreach (Album Album in Albums)
            {
                AlbumDAO AlbumDAO = new AlbumDAO();
                AlbumDAO.Id = Album.Id;
                AlbumDAO.Name = Album.Name;
                AlbumDAO.StatusId = Album.StatusId;
                AlbumDAO.CreatedAt = StaticParams.DateTimeNow;
                AlbumDAO.UpdatedAt = StaticParams.DateTimeNow;
                AlbumDAOs.Add(AlbumDAO);
            }
            await DataContext.BulkMergeAsync(AlbumDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Album> Albums)
        {
            List<long> Ids = Albums.Select(x => x.Id).ToList();
            await DataContext.Album
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AlbumDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Album Album)
        {
        }
        
    }
}
