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
    public interface IDistrictRepository
    {
        Task<int> Count(DistrictFilter DistrictFilter);
        Task<List<District>> List(DistrictFilter DistrictFilter);
        Task<District> Get(long Id);
    }
    public class DistrictRepository : IDistrictRepository
    {
        private DataContext DataContext;
        public DistrictRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DistrictDAO> DynamicFilter(IQueryable<DistrictDAO> query, DistrictFilter filter)
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
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<DistrictDAO> OrFilter(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<DistrictDAO> initQuery = query.Where(q => false);
            foreach (DistrictFilter DistrictFilter in filter.OrFilter)
            {
                IQueryable<DistrictDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    query = query.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, filter.Priority);
                if (filter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<DistrictDAO> DynamicOrder(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case DistrictOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case DistrictOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case DistrictOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case DistrictOrder.Priority:
                            query = query.OrderBy(q => q.Priority == null).ThenBy(x => x.Priority);
                            break;
                        case DistrictOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case DistrictOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case DistrictOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case DistrictOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case DistrictOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case DistrictOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority == null).ThenByDescending(x => x.Priority);
                            break;
                        case DistrictOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case DistrictOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<District>> DynamicSelect(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            List<District> Districts = await query.Select(q => new District()
            {
                Id = filter.Selects.Contains(DistrictSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(DistrictSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(DistrictSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(DistrictSelect.Priority) ? q.Priority : default(long?),
                ProvinceId = filter.Selects.Contains(DistrictSelect.Province) ? q.ProvinceId : default(long),
                StatusId = filter.Selects.Contains(DistrictSelect.Status) ? q.StatusId : default(long),
                Province = filter.Selects.Contains(DistrictSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Status = filter.Selects.Contains(DistrictSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                RowId = filter.Selects.Contains(DistrictSelect.RowId) ? q.RowId : default(Guid)
            }).ToListAsync();
            return Districts;
        }

        public async Task<int> Count(DistrictFilter filter)
        {
            IQueryable<DistrictDAO> Districts = DataContext.District;
            Districts = DynamicFilter(Districts, filter);
            return await Districts.CountAsync();
        }

        public async Task<List<District>> List(DistrictFilter filter)
        {
            if (filter == null) return new List<District>();
            IQueryable<DistrictDAO> DistrictDAOs = DataContext.District;
            DistrictDAOs = DynamicFilter(DistrictDAOs, filter);
            DistrictDAOs = DynamicOrder(DistrictDAOs, filter);
            List<District> Districts = await DynamicSelect(DistrictDAOs, filter);
            return Districts;
        }

        public async Task<District> Get(long Id)
        {
            District District = await DataContext.District.Where(x => x.Id == Id).AsNoTracking().Select(x => new District()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Priority = x.Priority,
                ProvinceId = x.ProvinceId,
                StatusId = x.StatusId,
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (District == null)
                return null;

            return District;
        }
    }
}
