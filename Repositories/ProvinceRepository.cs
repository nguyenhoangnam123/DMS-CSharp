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
    public interface IProvinceRepository
    {
        Task<int> Count(ProvinceFilter ProvinceFilter);
        Task<List<Province>> List(ProvinceFilter ProvinceFilter);
        Task<Province> Get(long Id);
        Task<bool> Create(Province Province);
        Task<bool> Update(Province Province);
        Task<bool> Delete(Province Province);
        Task<bool> BulkMerge(List<Province> Provinces);
        Task<bool> BulkDelete(List<Province> Provinces);
    }
    public class ProvinceRepository : IProvinceRepository
    {
        private DataContext DataContext;
        public ProvinceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProvinceDAO> DynamicFilter(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Priority != null)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ProvinceDAO> OrFilter(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProvinceDAO> initQuery = query.Where(q => false);
            foreach (ProvinceFilter ProvinceFilter in filter.OrFilter)
            {
                IQueryable<ProvinceDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, filter.Priority);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ProvinceDAO> DynamicOrder(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProvinceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProvinceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProvinceOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case ProvinceOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProvinceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProvinceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProvinceOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case ProvinceOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Province>> DynamicSelect(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            List<Province> Provinces = await query.Select(q => new Province()
            {
                Id = filter.Selects.Contains(ProvinceSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ProvinceSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(ProvinceSelect.Priority) ? q.Priority : default(long?),
                StatusId = filter.Selects.Contains(ProvinceSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(ProvinceSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return Provinces;
        }

        public async Task<int> Count(ProvinceFilter filter)
        {
            IQueryable<ProvinceDAO> Provinces = DataContext.Province;
            Provinces = DynamicFilter(Provinces, filter);
            return await Provinces.CountAsync();
        }

        public async Task<List<Province>> List(ProvinceFilter filter)
        {
            if (filter == null) return new List<Province>();
            IQueryable<ProvinceDAO> ProvinceDAOs = DataContext.Province;
            ProvinceDAOs = DynamicFilter(ProvinceDAOs, filter);
            ProvinceDAOs = DynamicOrder(ProvinceDAOs, filter);
            List<Province> Provinces = await DynamicSelect(ProvinceDAOs, filter);
            return Provinces;
        }

        public async Task<Province> Get(long Id)
        {
            Province Province = await DataContext.Province.Where(x => x.Id == Id).Select(x => new Province()
            {
                Id = x.Id,
                Name = x.Name,
                Priority = x.Priority,
                StatusId = x.StatusId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Province == null)
                return null;

            return Province;
        }
        public async Task<bool> Create(Province Province)
        {
            ProvinceDAO ProvinceDAO = new ProvinceDAO();
            ProvinceDAO.Id = Province.Id;
            ProvinceDAO.Name = Province.Name;
            ProvinceDAO.Priority = Province.Priority;
            ProvinceDAO.StatusId = Province.StatusId;
            ProvinceDAO.CreatedAt = StaticParams.DateTimeNow;
            ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Province.Add(ProvinceDAO);
            await DataContext.SaveChangesAsync();
            Province.Id = ProvinceDAO.Id;
            await SaveReference(Province);
            return true;
        }

        public async Task<bool> Update(Province Province)
        {
            ProvinceDAO ProvinceDAO = DataContext.Province.Where(x => x.Id == Province.Id).FirstOrDefault();
            if (ProvinceDAO == null)
                return false;
            ProvinceDAO.Id = Province.Id;
            ProvinceDAO.Name = Province.Name;
            ProvinceDAO.Priority = Province.Priority;
            ProvinceDAO.StatusId = Province.StatusId;
            ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Province);
            return true;
        }

        public async Task<bool> Delete(Province Province)
        {
            await DataContext.Province.Where(x => x.Id == Province.Id).UpdateFromQueryAsync(x => new ProvinceDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Province> Provinces)
        {
            List<ProvinceDAO> ProvinceDAOs = new List<ProvinceDAO>();
            foreach (Province Province in Provinces)
            {
                ProvinceDAO ProvinceDAO = new ProvinceDAO();
                ProvinceDAO.Id = Province.Id;
                ProvinceDAO.Name = Province.Name;
                ProvinceDAO.Priority = Province.Priority;
                ProvinceDAO.StatusId = Province.StatusId;
                ProvinceDAO.CreatedAt = StaticParams.DateTimeNow;
                ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProvinceDAOs.Add(ProvinceDAO);
            }
            await DataContext.BulkMergeAsync(ProvinceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Province> Provinces)
        {
            List<long> Ids = Provinces.Select(x => x.Id).ToList();
            await DataContext.Province
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProvinceDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Province Province)
        {
        }
        
    }
}
