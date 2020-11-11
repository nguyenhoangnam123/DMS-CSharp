using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;

namespace DMS.Repositories
{
    public interface ILuckyNumberRepository
    {
        Task<int> Count(LuckyNumberFilter LuckyNumberFilter);
        Task<List<LuckyNumber>> List(LuckyNumberFilter LuckyNumberFilter);
        Task<LuckyNumber> Get(long Id);
        Task<bool> Create(LuckyNumber LuckyNumber);
        Task<bool> Update(LuckyNumber LuckyNumber);
        Task<bool> Delete(LuckyNumber LuckyNumber);
        Task<bool> BulkMerge(List<LuckyNumber> LuckyNumbers);
        Task<bool> BulkDelete(List<LuckyNumber> LuckyNumbers);
    }
    public class LuckyNumberRepository : ILuckyNumberRepository
    {
        private DataContext DataContext;
        public LuckyNumberRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<LuckyNumberDAO> DynamicFilter(IQueryable<LuckyNumberDAO> query, LuckyNumberFilter filter)
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
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Value != null)
                query = query.Where(q => q.Value, filter.Value);
            if (filter.RewardStatusId != null)
                query = query.Where(q => q.RewardStatusId, filter.RewardStatusId);
            if (filter.RowId != null)
                query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<LuckyNumberDAO> OrFilter(IQueryable<LuckyNumberDAO> query, LuckyNumberFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<LuckyNumberDAO> initQuery = query.Where(q => false);
            foreach (LuckyNumberFilter LuckyNumberFilter in filter.OrFilter)
            {
                IQueryable<LuckyNumberDAO> queryable = query;
                if (LuckyNumberFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, LuckyNumberFilter.Id);
                if (LuckyNumberFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, LuckyNumberFilter.Code);
                if (LuckyNumberFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, LuckyNumberFilter.Name);
                if (LuckyNumberFilter.RewardStatusId != null)
                    queryable = queryable.Where(q => q.RewardStatusId, LuckyNumberFilter.RewardStatusId);
                if (LuckyNumberFilter.RowId != null)
                    queryable = queryable.Where(q => q.RowId, LuckyNumberFilter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<LuckyNumberDAO> DynamicOrder(IQueryable<LuckyNumberDAO> query, LuckyNumberFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case LuckyNumberOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case LuckyNumberOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case LuckyNumberOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case LuckyNumberOrder.Value:
                            query = query.OrderBy(q => q.Value);
                            break;
                        case LuckyNumberOrder.RewardStatus:
                            query = query.OrderBy(q => q.RewardStatusId);
                            break;
                        case LuckyNumberOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case LuckyNumberOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case LuckyNumberOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case LuckyNumberOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case LuckyNumberOrder.Value:
                            query = query.OrderByDescending(q => q.Value);
                            break;
                        case LuckyNumberOrder.RewardStatus:
                            query = query.OrderByDescending(q => q.RewardStatusId);
                            break;
                        case LuckyNumberOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<LuckyNumber>> DynamicSelect(IQueryable<LuckyNumberDAO> query, LuckyNumberFilter filter)
        {
            List<LuckyNumber> LuckyNumbers = await query.Select(q => new LuckyNumber()
            {
                Id = filter.Selects.Contains(LuckyNumberSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(LuckyNumberSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(LuckyNumberSelect.Name) ? q.Name : default(string),
                Value = filter.Selects.Contains(LuckyNumberSelect.Value) ? q.Value : default(string),
                RewardStatusId = filter.Selects.Contains(LuckyNumberSelect.RewardStatus) ? q.RewardStatusId : default(long),
                RowId = filter.Selects.Contains(LuckyNumberSelect.Row) ? q.RowId : default(Guid),
                RewardStatus = filter.Selects.Contains(LuckyNumberSelect.RewardStatus) && q.RewardStatus != null ? new RewardStatus
                {
                    Id = q.RewardStatus.Id,
                    Code = q.RewardStatus.Code,
                    Name = q.RewardStatus.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return LuckyNumbers;
        }

        public async Task<int> Count(LuckyNumberFilter filter)
        {
            IQueryable<LuckyNumberDAO> LuckyNumbers = DataContext.LuckyNumber.AsNoTracking();
            LuckyNumbers = DynamicFilter(LuckyNumbers, filter);
            return await LuckyNumbers.CountAsync();
        }

        public async Task<List<LuckyNumber>> List(LuckyNumberFilter filter)
        {
            if (filter == null) return new List<LuckyNumber>();
            IQueryable<LuckyNumberDAO> LuckyNumberDAOs = DataContext.LuckyNumber.AsNoTracking();
            LuckyNumberDAOs = DynamicFilter(LuckyNumberDAOs, filter);
            LuckyNumberDAOs = DynamicOrder(LuckyNumberDAOs, filter);
            List<LuckyNumber> LuckyNumbers = await DynamicSelect(LuckyNumberDAOs, filter);
            return LuckyNumbers;
        }

        public async Task<LuckyNumber> Get(long Id)
        {
            LuckyNumber LuckyNumber = await DataContext.LuckyNumber.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new LuckyNumber()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Value = x.Value,
                RewardStatusId = x.RewardStatusId,
                RowId = x.RowId,
                RewardStatus = x.RewardStatus == null ? null : new RewardStatus
                {
                    Id = x.RewardStatus.Id,
                    Code = x.RewardStatus.Code,
                    Name = x.RewardStatus.Name,
                },
            }).FirstOrDefaultAsync();

            if (LuckyNumber == null)
                return null;

            return LuckyNumber;
        }
        public async Task<bool> Create(LuckyNumber LuckyNumber)
        {
            LuckyNumberDAO LuckyNumberDAO = new LuckyNumberDAO();
            LuckyNumberDAO.Id = LuckyNumber.Id;
            LuckyNumberDAO.Code = LuckyNumber.Code;
            LuckyNumberDAO.Name = LuckyNumber.Name;
            LuckyNumberDAO.Value = LuckyNumber.Value;
            LuckyNumberDAO.RewardStatusId = LuckyNumber.RewardStatusId;
            LuckyNumberDAO.RowId = LuckyNumber.RowId;
            LuckyNumberDAO.CreatedAt = StaticParams.DateTimeNow;
            LuckyNumberDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.LuckyNumber.Add(LuckyNumberDAO);
            await DataContext.SaveChangesAsync();
            LuckyNumber.Id = LuckyNumberDAO.Id;
            await SaveReference(LuckyNumber);
            return true;
        }

        public async Task<bool> Update(LuckyNumber LuckyNumber)
        {
            LuckyNumberDAO LuckyNumberDAO = DataContext.LuckyNumber.Where(x => x.Id == LuckyNumber.Id).FirstOrDefault();
            if (LuckyNumberDAO == null)
                return false;
            LuckyNumberDAO.Id = LuckyNumber.Id;
            LuckyNumberDAO.Code = LuckyNumber.Code;
            LuckyNumberDAO.Name = LuckyNumber.Name;
            LuckyNumberDAO.Value = LuckyNumber.Value;
            LuckyNumberDAO.RewardStatusId = LuckyNumber.RewardStatusId;
            LuckyNumberDAO.RowId = LuckyNumber.RowId;
            LuckyNumberDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(LuckyNumber);
            return true;
        }

        public async Task<bool> Delete(LuckyNumber LuckyNumber)
        {
            await DataContext.LuckyNumber.Where(x => x.Id == LuckyNumber.Id).UpdateFromQueryAsync(x => new LuckyNumberDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<LuckyNumber> LuckyNumbers)
        {
            List<LuckyNumberDAO> LuckyNumberDAOs = new List<LuckyNumberDAO>();
            foreach (LuckyNumber LuckyNumber in LuckyNumbers)
            {
                LuckyNumberDAO LuckyNumberDAO = new LuckyNumberDAO();
                LuckyNumberDAO.Id = LuckyNumber.Id;
                LuckyNumberDAO.Code = LuckyNumber.Code;
                LuckyNumberDAO.Name = LuckyNumber.Name;
                LuckyNumberDAO.Value = LuckyNumber.Value;
                LuckyNumberDAO.RewardStatusId = LuckyNumber.RewardStatusId;
                LuckyNumberDAO.RowId = LuckyNumber.RowId;
                LuckyNumberDAO.CreatedAt = StaticParams.DateTimeNow;
                LuckyNumberDAO.UpdatedAt = StaticParams.DateTimeNow;
                LuckyNumberDAOs.Add(LuckyNumberDAO);
            }
            await DataContext.BulkMergeAsync(LuckyNumberDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<LuckyNumber> LuckyNumbers)
        {
            List<long> Ids = LuckyNumbers.Select(x => x.Id).ToList();
            await DataContext.LuckyNumber
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new LuckyNumberDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(LuckyNumber LuckyNumber)
        {
        }
        
    }
}
