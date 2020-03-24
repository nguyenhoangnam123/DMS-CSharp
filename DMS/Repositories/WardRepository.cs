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
    public interface IWardRepository
    {
        Task<int> Count(WardFilter WardFilter);
        Task<List<Ward>> List(WardFilter WardFilter);
        Task<Ward> Get(long Id);
        Task<bool> Create(Ward Ward);
        Task<bool> Update(Ward Ward);
        Task<bool> Delete(Ward Ward);
        Task<bool> BulkMerge(List<Ward> Wards);
        Task<bool> BulkDelete(List<Ward> Wards);
    }
    public class WardRepository : IWardRepository
    {
        private DataContext DataContext;
        public WardRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WardDAO> DynamicFilter(IQueryable<WardDAO> query, WardFilter filter)
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
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<WardDAO> OrFilter(IQueryable<WardDAO> query, WardFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WardDAO> initQuery = query.Where(q => false);
            foreach (WardFilter WardFilter in filter.OrFilter)
            {
                IQueryable<WardDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, filter.Priority);
                if (filter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<WardDAO> DynamicOrder(IQueryable<WardDAO> query, WardFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WardOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WardOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WardOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case WardOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case WardOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WardOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WardOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WardOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case WardOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case WardOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Ward>> DynamicSelect(IQueryable<WardDAO> query, WardFilter filter)
        {
            List<Ward> Wards = await query.Select(q => new Ward()
            {
                Id = filter.Selects.Contains(WardSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(WardSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(WardSelect.Priority) ? q.Priority : default(long?),
                DistrictId = filter.Selects.Contains(WardSelect.District) ? q.DistrictId : default(long),
                StatusId = filter.Selects.Contains(WardSelect.Status) ? q.StatusId : default(long),
                District = filter.Selects.Contains(WardSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                } : null,
                Status = filter.Selects.Contains(WardSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return Wards;
        }

        public async Task<int> Count(WardFilter filter)
        {
            IQueryable<WardDAO> Wards = DataContext.Ward;
            Wards = DynamicFilter(Wards, filter);
            return await Wards.CountAsync();
        }

        public async Task<List<Ward>> List(WardFilter filter)
        {
            if (filter == null) return new List<Ward>();
            IQueryable<WardDAO> WardDAOs = DataContext.Ward;
            WardDAOs = DynamicFilter(WardDAOs, filter);
            WardDAOs = DynamicOrder(WardDAOs, filter);
            List<Ward> Wards = await DynamicSelect(WardDAOs, filter);
            return Wards;
        }

        public async Task<Ward> Get(long Id)
        {
            Ward Ward = await DataContext.Ward.Where(x => x.Id == Id).Select(x => new Ward()
            {
                Id = x.Id,
                Name = x.Name,
                Priority = x.Priority,
                DistrictId = x.DistrictId,
                StatusId = x.StatusId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Ward == null)
                return null;

            return Ward;
        }
        public async Task<bool> Create(Ward Ward)
        {
            WardDAO WardDAO = new WardDAO();
            WardDAO.Id = Ward.Id;
            WardDAO.Name = Ward.Name;
            WardDAO.Priority = Ward.Priority;
            WardDAO.DistrictId = Ward.DistrictId;
            WardDAO.StatusId = Ward.StatusId;
            WardDAO.CreatedAt = StaticParams.DateTimeNow;
            WardDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Ward.Add(WardDAO);
            await DataContext.SaveChangesAsync();
            Ward.Id = WardDAO.Id;
            await SaveReference(Ward);
            return true;
        }

        public async Task<bool> Update(Ward Ward)
        {
            WardDAO WardDAO = DataContext.Ward.Where(x => x.Id == Ward.Id).FirstOrDefault();
            if (WardDAO == null)
                return false;
            WardDAO.Id = Ward.Id;
            WardDAO.Name = Ward.Name;
            WardDAO.Priority = Ward.Priority;
            WardDAO.DistrictId = Ward.DistrictId;
            WardDAO.StatusId = Ward.StatusId;
            WardDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Ward);
            return true;
        }

        public async Task<bool> Delete(Ward Ward)
        {
            await DataContext.Ward.Where(x => x.Id == Ward.Id).UpdateFromQueryAsync(x => new WardDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Ward> Wards)
        {
            List<WardDAO> WardDAOs = new List<WardDAO>();
            foreach (Ward Ward in Wards)
            {
                WardDAO WardDAO = new WardDAO();
                WardDAO.Id = Ward.Id;
                WardDAO.Name = Ward.Name;
                WardDAO.Priority = Ward.Priority;
                WardDAO.DistrictId = Ward.DistrictId;
                WardDAO.StatusId = Ward.StatusId;
                WardDAO.CreatedAt = StaticParams.DateTimeNow;
                WardDAO.UpdatedAt = StaticParams.DateTimeNow;
                WardDAOs.Add(WardDAO);
            }
            await DataContext.BulkMergeAsync(WardDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Ward> Wards)
        {
            List<long> Ids = Wards.Select(x => x.Id).ToList();
            await DataContext.Ward
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new WardDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Ward Ward)
        {
        }
        
    }
}
