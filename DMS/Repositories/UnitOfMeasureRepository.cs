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
    public interface IUnitOfMeasureRepository
    {
        Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<UnitOfMeasure> Get(long Id);
        Task<bool> Create(UnitOfMeasure UnitOfMeasure);
        Task<bool> Update(UnitOfMeasure UnitOfMeasure);
        Task<bool> Delete(UnitOfMeasure UnitOfMeasure);
        Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures);
        Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
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
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Description != null)
                    queryable = queryable.Where(q => q.Description, filter.Description);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
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

        public async Task<UnitOfMeasure> Get(long Id)
        {
            UnitOfMeasure UnitOfMeasure = await DataContext.UnitOfMeasure.Where(x => x.Id == Id).AsNoTracking().Select(x => new UnitOfMeasure()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StatusId = x.StatusId,
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
        public async Task<bool> Create(UnitOfMeasure UnitOfMeasure)
        {
            UnitOfMeasureDAO UnitOfMeasureDAO = new UnitOfMeasureDAO();
            UnitOfMeasureDAO.Id = UnitOfMeasure.Id;
            UnitOfMeasureDAO.Code = UnitOfMeasure.Code;
            UnitOfMeasureDAO.Name = UnitOfMeasure.Name;
            UnitOfMeasureDAO.Description = UnitOfMeasure.Description;
            UnitOfMeasureDAO.StatusId = UnitOfMeasure.StatusId;
            UnitOfMeasureDAO.CreatedAt = StaticParams.DateTimeNow;
            UnitOfMeasureDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.UnitOfMeasure.Add(UnitOfMeasureDAO);
            await DataContext.SaveChangesAsync();
            UnitOfMeasure.Id = UnitOfMeasureDAO.Id;
            await SaveReference(UnitOfMeasure);
            return true;
        }

        public async Task<bool> Update(UnitOfMeasure UnitOfMeasure)
        {
            UnitOfMeasureDAO UnitOfMeasureDAO = DataContext.UnitOfMeasure.Where(x => x.Id == UnitOfMeasure.Id).FirstOrDefault();
            if (UnitOfMeasureDAO == null)
                return false;
            UnitOfMeasureDAO.Id = UnitOfMeasure.Id;
            UnitOfMeasureDAO.Code = UnitOfMeasure.Code;
            UnitOfMeasureDAO.Name = UnitOfMeasure.Name;
            UnitOfMeasureDAO.Description = UnitOfMeasure.Description;
            UnitOfMeasureDAO.StatusId = UnitOfMeasure.StatusId;
            UnitOfMeasureDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(UnitOfMeasure);
            return true;
        }

        public async Task<bool> Delete(UnitOfMeasure UnitOfMeasure)
        {
            await DataContext.UnitOfMeasure.Where(x => x.Id == UnitOfMeasure.Id).UpdateFromQueryAsync(x => new UnitOfMeasureDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures)
        {
            List<UnitOfMeasureDAO> UnitOfMeasureDAOs = new List<UnitOfMeasureDAO>();
            foreach (UnitOfMeasure UnitOfMeasure in UnitOfMeasures)
            {
                UnitOfMeasureDAO UnitOfMeasureDAO = new UnitOfMeasureDAO();
                UnitOfMeasureDAO.Id = UnitOfMeasure.Id;
                UnitOfMeasureDAO.Code = UnitOfMeasure.Code;
                UnitOfMeasureDAO.Name = UnitOfMeasure.Name;
                UnitOfMeasureDAO.Description = UnitOfMeasure.Description;
                UnitOfMeasureDAO.StatusId = UnitOfMeasure.StatusId;
                UnitOfMeasureDAO.CreatedAt = StaticParams.DateTimeNow;
                UnitOfMeasureDAO.UpdatedAt = StaticParams.DateTimeNow;
                UnitOfMeasureDAOs.Add(UnitOfMeasureDAO);
            }
            await DataContext.BulkMergeAsync(UnitOfMeasureDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            List<long> Ids = UnitOfMeasures.Select(x => x.Id).ToList();
            await DataContext.UnitOfMeasure
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new UnitOfMeasureDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(UnitOfMeasure UnitOfMeasure)
        {
        }

    }
}
