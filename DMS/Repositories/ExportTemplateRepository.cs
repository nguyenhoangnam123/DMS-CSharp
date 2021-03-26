using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IExportTemplateRepository
    {
        Task<int> Count(ExportTemplateFilter ExportTemplateFilter);
        Task<List<ExportTemplate>> List(ExportTemplateFilter ExportTemplateFilter);
        Task<ExportTemplate> Get(long Id);
        Task<bool> Update(ExportTemplate ExportTemplate);
    }
    public class ExportTemplateRepository : IExportTemplateRepository
    {
        private DataContext DataContext;
        public ExportTemplateRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ExportTemplateDAO> DynamicFilter(IQueryable<ExportTemplateDAO> query, ExportTemplateFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Extension != null && filter.Extension.HasValue)
                query = query.Where(q => q.Extension, filter.Extension);
            if (filter.FileName != null && filter.FileName.HasValue)
                query = query.Where(q => q.FileName, filter.FileName);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ExportTemplateDAO> OrFilter(IQueryable<ExportTemplateDAO> query, ExportTemplateFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ExportTemplateDAO> initQuery = query.Where(q => false);
            foreach (ExportTemplateFilter ExportTemplateFilter in filter.OrFilter)
            {
                IQueryable<ExportTemplateDAO> queryable = query;
                if (ExportTemplateFilter.Id != null && ExportTemplateFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ExportTemplateFilter.Code != null && ExportTemplateFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (ExportTemplateFilter.Name != null && ExportTemplateFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (ExportTemplateFilter.Extension != null && ExportTemplateFilter.Extension.HasValue)
                    queryable = queryable.Where(q => q.Extension, filter.Extension);
                if (ExportTemplateFilter.FileName != null && ExportTemplateFilter.FileName.HasValue)
                    queryable = queryable.Where(q => q.Extension, filter.FileName);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ExportTemplateDAO> DynamicOrder(IQueryable<ExportTemplateDAO> query, ExportTemplateFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ExportTemplateOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ExportTemplateOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ExportTemplateOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ExportTemplateOrder.Extension:
                            query = query.OrderBy(q => q.Extension);
                            break;
                        case ExportTemplateOrder.FileName:
                            query = query.OrderBy(q => q.FileName);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ExportTemplateOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ExportTemplateOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ExportTemplateOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ExportTemplateOrder.Extension:
                            query = query.OrderByDescending(q => q.Extension);
                            break;
                        case ExportTemplateOrder.FileName:
                            query = query.OrderByDescending(q => q.FileName);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ExportTemplate>> DynamicSelect(IQueryable<ExportTemplateDAO> query, ExportTemplateFilter filter)
        {
            List<ExportTemplate> ExportTemplates = await query.Select(q => new ExportTemplate()
            {
                Id = filter.Selects.Contains(ExportTemplateSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ExportTemplateSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ExportTemplateSelect.Name) ? q.Name : default(string),
                Extension = filter.Selects.Contains(ExportTemplateSelect.Extension) ? q.Extension : default(string),
                FileName = filter.Selects.Contains(ExportTemplateSelect.FileName) ? q.FileName : default(string),
            }).ToListAsync();
            return ExportTemplates;
        }

        public async Task<int> Count(ExportTemplateFilter filter)
        {
            IQueryable<ExportTemplateDAO> ExportTemplates = DataContext.ExportTemplate.AsNoTracking();
            ExportTemplates = DynamicFilter(ExportTemplates, filter);
            return await ExportTemplates.CountAsync();
        }

        public async Task<List<ExportTemplate>> List(ExportTemplateFilter filter)
        {
            if (filter == null) return new List<ExportTemplate>();
            IQueryable<ExportTemplateDAO> ExportTemplateDAOs = DataContext.ExportTemplate.AsNoTracking();
            ExportTemplateDAOs = DynamicFilter(ExportTemplateDAOs, filter);
            ExportTemplateDAOs = DynamicOrder(ExportTemplateDAOs, filter);
            List<ExportTemplate> ExportTemplates = await DynamicSelect(ExportTemplateDAOs, filter);
            return ExportTemplates;
        }

        public async Task<ExportTemplate> Get(long Id)
        {
            ExportTemplate ExportTemplate = await DataContext.ExportTemplate.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new ExportTemplate()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Content = x.Content,
                Extension = x.Extension,
                FileName = x.FileName,
            }).FirstOrDefaultAsync();

            if (ExportTemplate == null)
                return null;

            return ExportTemplate;
        }

        public async Task<bool> Update(ExportTemplate ExportTemplate)
        {
            ExportTemplateDAO ExportTemplateDAO = DataContext.ExportTemplate.Where(x => x.Id == ExportTemplate.Id).FirstOrDefault();
            if (ExportTemplateDAO == null)
                return false;
            ExportTemplateDAO.Content = ExportTemplate.Content;
            ExportTemplateDAO.Extension = ExportTemplate.Extension;
            ExportTemplateDAO.FileName = ExportTemplate.FileName;
            await DataContext.SaveChangesAsync();
            return true;
        }

    }
}
