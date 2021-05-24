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
    public interface IUnitOfMeasureRepository
    {
        Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<List<UnitOfMeasure>> List(List<long> Ids);
        Task<UnitOfMeasure> Get(long Id);
        Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures);
    }
    public class UnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        private DataContext DataContext;
        public UnitOfMeasureRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UnitOfMeasureDAO> DynamicFilter(IQueryable<UnitOfMeasureDAO> query, UnitOfMeasureFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<UnitOfMeasureDAO> OrFilter(IQueryable<UnitOfMeasureDAO> query, UnitOfMeasureFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<UnitOfMeasureDAO> initQuery = query.Where(q => false);
            foreach (UnitOfMeasureFilter UnitOfMeasureFilter in filter.OrFilter)
            {
                IQueryable<UnitOfMeasureDAO> queryable = query;
                if (UnitOfMeasureFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, UnitOfMeasureFilter.Id);
                if (UnitOfMeasureFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, UnitOfMeasureFilter.Code);
                if (UnitOfMeasureFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, UnitOfMeasureFilter.Name);
                if (UnitOfMeasureFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, UnitOfMeasureFilter.Description);
                if (UnitOfMeasureFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, UnitOfMeasureFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<UnitOfMeasureDAO> DynamicOrder(IQueryable<UnitOfMeasureDAO> query, UnitOfMeasureFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UnitOfMeasureOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case UnitOfMeasureOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case UnitOfMeasureOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case UnitOfMeasureOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UnitOfMeasureOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case UnitOfMeasureOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case UnitOfMeasureOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case UnitOfMeasureOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UnitOfMeasure>> DynamicSelect(IQueryable<UnitOfMeasureDAO> query, UnitOfMeasureFilter filter)
        {
            List<UnitOfMeasure> UnitOfMeasures = await query.Select(q => new UnitOfMeasure()
            {
                Id = filter.Selects.Contains(UnitOfMeasureSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(UnitOfMeasureSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(UnitOfMeasureSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(UnitOfMeasureSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(UnitOfMeasureSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(UnitOfMeasureSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Used = q.Used,
            }).ToListAsync();
            return UnitOfMeasures;
        }

        public async Task<int> Count(UnitOfMeasureFilter filter)
        {
            IQueryable<UnitOfMeasureDAO> UnitOfMeasures = DataContext.UnitOfMeasure;
            UnitOfMeasures = DynamicFilter(UnitOfMeasures, filter);
            return await UnitOfMeasures.CountAsync();
        }

        public async Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter filter)
        {
            if (filter == null) return new List<UnitOfMeasure>();
            IQueryable<UnitOfMeasureDAO> UnitOfMeasureDAOs = DataContext.UnitOfMeasure.AsNoTracking();
            UnitOfMeasureDAOs = DynamicFilter(UnitOfMeasureDAOs, filter);
            UnitOfMeasureDAOs = DynamicOrder(UnitOfMeasureDAOs, filter);
            List<UnitOfMeasure> UnitOfMeasures = await DynamicSelect(UnitOfMeasureDAOs, filter);
            return UnitOfMeasures;
        }

        public async Task<List<UnitOfMeasure>> List(List<long> Ids)
        {
            IQueryable<UnitOfMeasureDAO> UnitOfMeasureDAOs = DataContext.UnitOfMeasure.AsNoTracking();
            List<UnitOfMeasure> UnitOfMeasures = UnitOfMeasureDAOs.Where(q => q.Id, new IdFilter { In = Ids }).Select(x => new UnitOfMeasure()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StatusId = x.StatusId,
                Used = x.Used,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToList();
            return UnitOfMeasures;
        }

        public async Task<UnitOfMeasure> Get(long Id)
        {
            UnitOfMeasure UnitOfMeasure = await DataContext.UnitOfMeasure.Where(x => x.Id == Id).AsNoTracking().Select(x => new UnitOfMeasure()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StatusId = x.StatusId,
                Used = x.Used,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (UnitOfMeasure == null)
                return null;

            return UnitOfMeasure;
        }
        public async Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures)
        {
            List<UnitOfMeasureDAO> UnitOfMeasureDAOs = UnitOfMeasures.Select(x => new UnitOfMeasureDAO
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Used = x.Used,
                RowId = x.RowId,
                StatusId = x.StatusId
            }).ToList();
            await DataContext.BulkMergeAsync(UnitOfMeasureDAOs);
            return true;
        }
    }
}
