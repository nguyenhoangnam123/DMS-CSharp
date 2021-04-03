using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IWorkflowTypeRepository
    {
        Task<int> Count(WorkflowTypeFilter WorkflowTypeFilter);
        Task<List<WorkflowType>> List(WorkflowTypeFilter WorkflowTypeFilter);
        Task<WorkflowType> Get(long Id);
    }
    public class WorkflowTypeRepository : IWorkflowTypeRepository
    {
        private DataContext DataContext;
        public WorkflowTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowTypeDAO> DynamicFilter(IQueryable<WorkflowTypeDAO> query, WorkflowTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowTypeDAO> OrFilter(IQueryable<WorkflowTypeDAO> query, WorkflowTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowTypeDAO> initQuery = query.Where(q => false);
            foreach (WorkflowTypeFilter WorkflowTypeFilter in filter.OrFilter)
            {
                IQueryable<WorkflowTypeDAO> queryable = query;
                if (WorkflowTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowTypeFilter.Id);
                if (WorkflowTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowTypeFilter.Code);
                if (WorkflowTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowTypeDAO> DynamicOrder(IQueryable<WorkflowTypeDAO> query, WorkflowTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WorkflowTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WorkflowTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowType>> DynamicSelect(IQueryable<WorkflowTypeDAO> query, WorkflowTypeFilter filter)
        {
            List<WorkflowType> WorkflowTypes = await query.Select(q => new WorkflowType()
            {
                Id = filter.Selects.Contains(WorkflowTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(WorkflowTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WorkflowTypeSelect.Name) ? q.Name : default(string),
            }).AsNoTracking().ToListAsync();
            return WorkflowTypes;
        }

        public async Task<int> Count(WorkflowTypeFilter filter)
        {
            IQueryable<WorkflowTypeDAO> WorkflowTypes = DataContext.WorkflowType;
            WorkflowTypes = DynamicFilter(WorkflowTypes, filter);
            return await WorkflowTypes.CountAsync();
        }

        public async Task<List<WorkflowType>> List(WorkflowTypeFilter filter)
        {
            if (filter == null) return new List<WorkflowType>();
            IQueryable<WorkflowTypeDAO> WorkflowTypeDAOs = DataContext.WorkflowType;
            WorkflowTypeDAOs = DynamicFilter(WorkflowTypeDAOs, filter);
            WorkflowTypeDAOs = DynamicOrder(WorkflowTypeDAOs, filter);
            List<WorkflowType> WorkflowTypes = await DynamicSelect(WorkflowTypeDAOs, filter);
            return WorkflowTypes;
        }

        public async Task<WorkflowType> Get(long Id)
        {
            WorkflowType WorkflowType = await DataContext.WorkflowType.Where(x => x.Id == Id).Select(x => new WorkflowType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (WorkflowType == null)
                return null;

            return WorkflowType;
        }
    }
}
