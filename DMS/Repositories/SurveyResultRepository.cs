using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface ISurveyResultRepository
    {
        Task<int> Count(SurveyResultFilter SurveyResultFilter);
        Task<List<SurveyResult>> List(SurveyResultFilter SurveyResultFilter);
        Task<SurveyResult> Get(long Id);
        Task<bool> Create(SurveyResult SurveyResult);
        Task<bool> Delete(SurveyResult SurveyResult);
    }
    public class SurveyResultRepository : ISurveyResultRepository
    {
        private DataContext DataContext;
        public SurveyResultRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SurveyResultDAO> DynamicFilter(IQueryable<SurveyResultDAO> query, SurveyResultFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.AppUserId != null)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.SurveyId != null)
                query = query.Where(q => q.SurveyId, filter.SurveyId);
            if (filter.StoreId != null)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.Time != null)
                query = query.Where(q => q.Time, filter.Time);
            return query;
        }

        private IQueryable<SurveyResultDAO> DynamicOrder(IQueryable<SurveyResultDAO> query, SurveyResultFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SurveyResultOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SurveyResultOrder.AppUser:
                            query = query.OrderBy(q => q.AppUser.DisplayName);
                            break;
                        case SurveyResultOrder.Store:
                            query = query.OrderBy(q => q.Store.Name);
                            break;
                        case SurveyResultOrder.Survey:
                            query = query.OrderBy(q => q.Survey.Title);
                            break;
                        case SurveyResultOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SurveyResultOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SurveyResultOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUser.DisplayName);
                            break;
                        case SurveyResultOrder.Store:
                            query = query.OrderByDescending(q => q.Store.Name);
                            break;
                        case SurveyResultOrder.Survey:
                            query = query.OrderByDescending(q => q.Survey.Title);
                            break;
                        case SurveyResultOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<SurveyResult>> DynamicSelect(IQueryable<SurveyResultDAO> query, SurveyResultFilter filter)
        {
            List<SurveyResult> SurveyResults = await query.Select(q => new SurveyResult()
            {
                Id = filter.Selects.Contains(SurveyResultSelect.Id) ? q.Id : default(long),
                AppUserId = filter.Selects.Contains(SurveyResultSelect.AppUser) ? q.AppUserId : default(long),
                StoreId = filter.Selects.Contains(SurveyResultSelect.Store) ? q.StoreId : default(long),
                SurveyId = filter.Selects.Contains(SurveyResultSelect.Survey) ? q.SurveyId : default(long),
                Time = filter.Selects.Contains(SurveyResultSelect.Time) ? q.Time : default(DateTime),
                AppUser = filter.Selects.Contains(SurveyResultSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                } : null,
                Store = filter.Selects.Contains(SurveyResultSelect.Store) && q.Store != null ? new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    Name = q.Store.Name,
                } : null,
            }).ToListAsync();
            return SurveyResults;
        }

        public async Task<int> Count(SurveyResultFilter filter)
        {
            IQueryable<SurveyResultDAO> SurveyResults = DataContext.SurveyResult.AsNoTracking();
            SurveyResults = DynamicFilter(SurveyResults, filter);
            return await SurveyResults.CountAsync();
        }

        public async Task<List<SurveyResult>> List(SurveyResultFilter filter)
        {
            if (filter == null) return new List<SurveyResult>();
            IQueryable<SurveyResultDAO> SurveyResultDAOs = DataContext.SurveyResult.AsNoTracking();
            SurveyResultDAOs = DynamicFilter(SurveyResultDAOs, filter);
            SurveyResultDAOs = DynamicOrder(SurveyResultDAOs, filter);
            List<SurveyResult> SurveyResults = await DynamicSelect(SurveyResultDAOs, filter);
            return SurveyResults;
        }

        public async Task<SurveyResult> Get(long Id)
        {
            SurveyResult SurveyResult = await DataContext.SurveyResult.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new SurveyResult()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                StoreId = x.StoreId,
                SurveyId = x.SurveyId,
                Time = x.Time,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                },
                Store = x.Store == null ? null : new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    Name = x.Store.Name,
                },
            }).FirstOrDefaultAsync();

            if (SurveyResult == null)
                return null;
            SurveyResult.SurveyResultCells = await DataContext.SurveyResultCell.AsNoTracking()
                .Where(x => x.SurveyResultId == SurveyResult.Id)
                .Select(x => new SurveyResultCell
                {
                    SurveyResultId = x.SurveyResultId,
                    SurveyQuestionId = x.SurveyQuestionId,
                    ColumnOptionId = x.ColumnOptionId,
                    RowOptionId = x.RowOptionId,
                }).ToListAsync();
            SurveyResult.SurveyResultSingles = await DataContext.SurveyResultSingle.AsNoTracking()
                .Where(x => x.SurveyResultId == SurveyResult.Id)
                .Select(x => new SurveyResultSingle
                {
                    SurveyResultId = x.SurveyResultId,
                    SurveyQuestionId = x.SurveyQuestionId,
                    SurveyOptionId = x.SurveyOptionId,
                }).ToListAsync();
            return SurveyResult;
        }

        public async Task<bool> Create(SurveyResult SurveyResult)
        {
            SurveyResult.RowId = Guid.NewGuid();
            SurveyResultDAO SurveyResultDAO = new SurveyResultDAO();
            SurveyResultDAO.AppUserId = SurveyResult.AppUserId;
            SurveyResultDAO.StoreId = SurveyResult.StoreId;
            SurveyResultDAO.SurveyId = SurveyResult.SurveyId;
            SurveyResultDAO.Time = SurveyResult.Time;
            SurveyResultDAO.RowId = SurveyResult.RowId;
            DataContext.SurveyResult.Add(SurveyResultDAO);
            await DataContext.SaveChangesAsync();
            SurveyResult.Id = SurveyResultDAO.Id;
            await SaveReference(SurveyResult);
            return true;
        }


        public async Task<bool> Delete(SurveyResult SurveyResult)
        {
            await DataContext.SurveyResult.Where(x => x.Id == SurveyResult.Id).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(SurveyResult SurveyResult)
        {

            List<SurveyResultSingleDAO> SurveyResultSingleDAOs = SurveyResult.SurveyResultSingles.Select(s => new SurveyResultSingleDAO
            {
                SurveyResultId = SurveyResult.Id,
                SurveyOptionId = s.SurveyOptionId,
                SurveyQuestionId = s.SurveyQuestionId,
            }).ToList();
            List<SurveyResultCellDAO> SurveyResultCellDAOs = SurveyResult.SurveyResultCells.Select(s => new SurveyResultCellDAO
            {
                SurveyResultId = SurveyResult.Id,
                SurveyQuestionId = s.SurveyQuestionId,
                ColumnOptionId = s.ColumnOptionId,
                RowOptionId = s.RowOptionId,
            }).ToList();


            await DataContext.SurveyResultSingle.BulkInsertAsync(SurveyResultSingleDAOs);
            await DataContext.SurveyResultCell.BulkInsertAsync(SurveyResultCellDAOs);
        }
    }
}
