using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IWorkflowOperatorRepository
    {
        Task<int> Count(WorkflowOperatorFilter WorkflowOperatorFilter);
        Task<List<WorkflowOperator>> List(WorkflowOperatorFilter WorkflowOperatorFilter);
        Task<WorkflowOperator> Get(long Id);
    }
    public class WorkflowOperatorRepository : IWorkflowOperatorRepository
    {
        private DataContext DataContext;
        public WorkflowOperatorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowOperatorDAO> DynamicFilter(IQueryable<WorkflowOperatorDAO> query, WorkflowOperatorFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.WorkflowParameterTypeId != null)
                query = query.Where(q => q.WorkflowParameterTypeId, filter.WorkflowParameterTypeId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowOperatorDAO> OrFilter(IQueryable<WorkflowOperatorDAO> query, WorkflowOperatorFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowOperatorDAO> initQuery = query.Where(q => false);
            foreach (WorkflowOperatorFilter WorkflowOperatorFilter in filter.OrFilter)
            {
                IQueryable<WorkflowOperatorDAO> queryable = query;
                if (WorkflowOperatorFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowOperatorFilter.Id);
                if (WorkflowOperatorFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowOperatorFilter.Code);
                if (WorkflowOperatorFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowOperatorFilter.Name);
                if (WorkflowOperatorFilter.WorkflowParameterTypeId != null)
                    queryable = queryable.Where(q => q.WorkflowParameterTypeId, WorkflowOperatorFilter.WorkflowParameterTypeId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowOperatorDAO> DynamicOrder(IQueryable<WorkflowOperatorDAO> query, WorkflowOperatorFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowOperatorOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowOperatorOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WorkflowOperatorOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowOperatorOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowOperatorOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WorkflowOperatorOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowOperator>> DynamicSelect(IQueryable<WorkflowOperatorDAO> query, WorkflowOperatorFilter filter)
        {
            List<WorkflowOperator> WorkflowOperators = await query.Select(q => new WorkflowOperator()
            {
                Id = q.Id,
                Code = q.Code,
                Name = q.Name,
                WorkflowParameterTypeId = q.WorkflowParameterTypeId,
            }).AsNoTracking().ToListAsync();
            return WorkflowOperators;
        }

        public async Task<int> Count(WorkflowOperatorFilter filter)
        {
            IQueryable<WorkflowOperatorDAO> WorkflowOperators = DataContext.WorkflowOperator;
            WorkflowOperators = DynamicFilter(WorkflowOperators, filter);
            return await WorkflowOperators.CountAsync();
        }

        public async Task<List<WorkflowOperator>> List(WorkflowOperatorFilter filter)
        {
            if (filter == null) return new List<WorkflowOperator>();
            IQueryable<WorkflowOperatorDAO> WorkflowOperatorDAOs = DataContext.WorkflowOperator;
            WorkflowOperatorDAOs = DynamicFilter(WorkflowOperatorDAOs, filter);
            WorkflowOperatorDAOs = DynamicOrder(WorkflowOperatorDAOs, filter);
            List<WorkflowOperator> WorkflowOperators = await DynamicSelect(WorkflowOperatorDAOs, filter);
            return WorkflowOperators;
        }

        public async Task<WorkflowOperator> Get(long Id)
        {
            WorkflowOperator WorkflowOperator = await DataContext.WorkflowOperator.Where(x => x.Id == Id).Select(x => new WorkflowOperator()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                WorkflowParameterTypeId = x.WorkflowParameterTypeId,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (WorkflowOperator == null)
                return null;

            return WorkflowOperator;
        }
    }
}
