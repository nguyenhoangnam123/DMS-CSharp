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
    public interface IPageRepository
    {
        Task<int> Count(PageFilter PageFilter);
        Task<List<Page>> List(PageFilter PageFilter);
        Task<Page> Get(long Id);
        Task<bool> Create(Page Page);
        Task<bool> Update(Page Page);
        Task<bool> Delete(Page Page);
        Task<bool> BulkMerge(List<Page> Pages);
        Task<bool> BulkDelete(List<Page> Pages);
    }
    public class PageRepository : IPageRepository
    {
        private DataContext DataContext;
        public PageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PageDAO> DynamicFilter(IQueryable<PageDAO> query, PageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.MenuId != null)
                query = query.Where(q => q.MenuId, filter.MenuId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<PageDAO> OrFilter(IQueryable<PageDAO> query, PageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PageDAO> initQuery = query.Where(q => false);
            foreach (PageFilter PageFilter in filter.OrFilter)
            {
                IQueryable<PageDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                if (filter.MenuId != null)
                    queryable = queryable.Where(q => q.MenuId, filter.MenuId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PageDAO> DynamicOrder(IQueryable<PageDAO> query, PageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PageOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PageOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case PageOrder.Menu:
                            query = query.OrderBy(q => q.MenuId);
                            break;
                        case PageOrder.IsDeleted:
                            query = query.OrderBy(q => q.IsDeleted);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PageOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PageOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case PageOrder.Menu:
                            query = query.OrderByDescending(q => q.MenuId);
                            break;
                        case PageOrder.IsDeleted:
                            query = query.OrderByDescending(q => q.IsDeleted);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Page>> DynamicSelect(IQueryable<PageDAO> query, PageFilter filter)
        {
            List<Page> Pages = await query.Select(q => new Page()
            {
                Id = filter.Selects.Contains(PageSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(PageSelect.Name) ? q.Name : default(string),
                Path = filter.Selects.Contains(PageSelect.Path) ? q.Path : default(string),
                MenuId = filter.Selects.Contains(PageSelect.Menu) ? q.MenuId : default(long),
                IsDeleted = filter.Selects.Contains(PageSelect.IsDeleted) ? q.IsDeleted : default(bool),
                Menu = filter.Selects.Contains(PageSelect.Menu) && q.Menu != null ? new Menu
                {
                    Id = q.Menu.Id,
                    Name = q.Menu.Name,
                    Path = q.Menu.Path,
                    IsDeleted = q.Menu.IsDeleted,
                } : null,
            }).ToListAsync();
            return Pages;
        }

        public async Task<int> Count(PageFilter filter)
        {
            IQueryable<PageDAO> Pages = DataContext.Page;
            Pages = DynamicFilter(Pages, filter);
            return await Pages.CountAsync();
        }

        public async Task<List<Page>> List(PageFilter filter)
        {
            if (filter == null) return new List<Page>();
            IQueryable<PageDAO> PageDAOs = DataContext.Page;
            PageDAOs = DynamicFilter(PageDAOs, filter);
            PageDAOs = DynamicOrder(PageDAOs, filter);
            List<Page> Pages = await DynamicSelect(PageDAOs, filter);
            return Pages;
        }

        public async Task<Page> Get(long Id)
        {
            Page Page = await DataContext.Page.Where(x => x.Id == Id).Select(x => new Page()
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                MenuId = x.MenuId,
                IsDeleted = x.IsDeleted,
                Menu = x.Menu == null ? null : new Menu
                {
                    Id = x.Menu.Id,
                    Name = x.Menu.Name,
                    Path = x.Menu.Path,
                    IsDeleted = x.Menu.IsDeleted,
                },
            }).FirstOrDefaultAsync();

            if (Page == null)
                return null;

            return Page;
        }
        public async Task<bool> Create(Page Page)
        {
            PageDAO PageDAO = new PageDAO();
            PageDAO.Id = Page.Id;
            PageDAO.Name = Page.Name;
            PageDAO.Path = Page.Path;
            PageDAO.MenuId = Page.MenuId;
            PageDAO.IsDeleted = Page.IsDeleted;
            DataContext.Page.Add(PageDAO);
            await DataContext.SaveChangesAsync();
            Page.Id = PageDAO.Id;
            await SaveReference(Page);
            return true;
        }

        public async Task<bool> Update(Page Page)
        {
            PageDAO PageDAO = DataContext.Page.Where(x => x.Id == Page.Id).FirstOrDefault();
            if (PageDAO == null)
                return false;
            PageDAO.Id = Page.Id;
            PageDAO.Name = Page.Name;
            PageDAO.Path = Page.Path;
            PageDAO.MenuId = Page.MenuId;
            PageDAO.IsDeleted = Page.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Page);
            return true;
        }

        public async Task<bool> Delete(Page Page)
        {
            await DataContext.Page.Where(x => x.Id == Page.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Page> Pages)
        {
            List<PageDAO> PageDAOs = new List<PageDAO>();
            foreach (Page Page in Pages)
            {
                PageDAO PageDAO = new PageDAO();
                PageDAO.Id = Page.Id;
                PageDAO.Name = Page.Name;
                PageDAO.Path = Page.Path;
                PageDAO.MenuId = Page.MenuId;
                PageDAO.IsDeleted = Page.IsDeleted;
                PageDAOs.Add(PageDAO);
            }
            await DataContext.BulkMergeAsync(PageDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Page> Pages)
        {
            List<long> Ids = Pages.Select(x => x.Id).ToList();
            await DataContext.Page
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Page Page)
        {
        }
        
    }
}
