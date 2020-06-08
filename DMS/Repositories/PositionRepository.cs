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
    public interface IPositionRepository
    {
        Task<int> Count(PositionFilter PositionFilter);
        Task<List<Position>> List(PositionFilter PositionFilter);
        Task<Position> Get(long Id);
    }
    public class PositionRepository : IPositionRepository
    {
        private DataContext DataContext;
        public PositionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PositionDAO> DynamicFilter(IQueryable<PositionDAO> query, PositionFilter filter)
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
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PositionDAO> OrFilter(IQueryable<PositionDAO> query, PositionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PositionDAO> initQuery = query.Where(q => false);
            foreach (PositionFilter PositionFilter in filter.OrFilter)
            {
                IQueryable<PositionDAO> queryable = query;
                if (PositionFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PositionFilter.Id);
                if (PositionFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, PositionFilter.Code);
                if (PositionFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PositionFilter.Name);
                if (PositionFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, PositionFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<PositionDAO> DynamicOrder(IQueryable<PositionDAO> query, PositionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PositionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PositionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PositionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PositionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PositionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PositionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PositionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PositionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Position>> DynamicSelect(IQueryable<PositionDAO> query, PositionFilter filter)
        {
            List<Position> Positions = await query.Select(q => new Position()
            {
                Id = filter.Selects.Contains(PositionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PositionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PositionSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(PositionSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(PositionSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return Positions;
        }

        public async Task<int> Count(PositionFilter filter)
        {
            IQueryable<PositionDAO> Positions = DataContext.Position.AsNoTracking();
            Positions = DynamicFilter(Positions, filter);
            return await Positions.CountAsync();
        }

        public async Task<List<Position>> List(PositionFilter filter)
        {
            if (filter == null) return new List<Position>();
            IQueryable<PositionDAO> PositionDAOs = DataContext.Position.AsNoTracking();
            PositionDAOs = DynamicFilter(PositionDAOs, filter);
            PositionDAOs = DynamicOrder(PositionDAOs, filter);
            List<Position> Positions = await DynamicSelect(PositionDAOs, filter);
            return Positions;
        }

        public async Task<Position> Get(long Id)
        {
            Position Position = await DataContext.Position.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Position()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Position == null)
                return null;

            return Position;
        }
    }
}
