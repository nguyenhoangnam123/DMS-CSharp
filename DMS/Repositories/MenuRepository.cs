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
    public interface IMenuRepository
    {
        Task<int> Count(MenuFilter MenuFilter);
        Task<List<Menu>> List(MenuFilter MenuFilter);
        Task<Menu> Get(long Id);
        Task<bool> Create(Menu Menu);
        Task<bool> Update(Menu Menu);
        Task<bool> Delete(Menu Menu);
        Task<bool> BulkMerge(List<Menu> Menus);
        Task<bool> BulkDelete(List<Menu> Menus);
    }
    public class MenuRepository : IMenuRepository
    {
        private DataContext DataContext;
        public MenuRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<MenuDAO> DynamicFilter(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<MenuDAO> OrFilter(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MenuDAO> initQuery = query.Where(q => false);
            foreach (MenuFilter MenuFilter in filter.OrFilter)
            {
                IQueryable<MenuDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MenuDAO> DynamicOrder(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MenuOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MenuOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case MenuOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case MenuOrder.IsDeleted:
                            query = query.OrderBy(q => q.IsDeleted);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MenuOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MenuOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case MenuOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case MenuOrder.IsDeleted:
                            query = query.OrderByDescending(q => q.IsDeleted);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Menu>> DynamicSelect(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            List<Menu> Menus = await query.Select(q => new Menu()
            {
                Id = filter.Selects.Contains(MenuSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(MenuSelect.Name) ? q.Name : default(string),
                Path = filter.Selects.Contains(MenuSelect.Path) ? q.Path : default(string),
                IsDeleted = filter.Selects.Contains(MenuSelect.IsDeleted) ? q.IsDeleted : default(bool),
            }).ToListAsync();
            return Menus;
        }

        public async Task<int> Count(MenuFilter filter)
        {
            IQueryable<MenuDAO> Menus = DataContext.Menu;
            Menus = DynamicFilter(Menus, filter);
            return await Menus.CountAsync();
        }

        public async Task<List<Menu>> List(MenuFilter filter)
        {
            if (filter == null) return new List<Menu>();
            IQueryable<MenuDAO> MenuDAOs = DataContext.Menu;
            MenuDAOs = DynamicFilter(MenuDAOs, filter);
            MenuDAOs = DynamicOrder(MenuDAOs, filter);
            List<Menu> Menus = await DynamicSelect(MenuDAOs, filter);
            return Menus;
        }

        public async Task<Menu> Get(long Id)
        {
            Menu Menu = await DataContext.Menu.Where(x => x.Id == Id).Select(x => new Menu()
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                IsDeleted = x.IsDeleted,
            }).FirstOrDefaultAsync();

            if (Menu == null)
                return null;
            Menu.Fields = await DataContext.Field
                .Where(x => x.MenuId == Menu.Id)
                .Select(x => new Field
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    MenuId = x.MenuId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();
            Menu.Pages = await DataContext.Page
                .Where(x => x.MenuId == Menu.Id)
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    MenuId = x.MenuId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();

            return Menu;
        }
        public async Task<bool> Create(Menu Menu)
        {
            MenuDAO MenuDAO = new MenuDAO();
            MenuDAO.Id = Menu.Id;
            MenuDAO.Name = Menu.Name;
            MenuDAO.Path = Menu.Path;
            MenuDAO.IsDeleted = Menu.IsDeleted;
            DataContext.Menu.Add(MenuDAO);
            await DataContext.SaveChangesAsync();
            Menu.Id = MenuDAO.Id;
            await SaveReference(Menu);
            return true;
        }

        public async Task<bool> Update(Menu Menu)
        {
            MenuDAO MenuDAO = DataContext.Menu.Where(x => x.Id == Menu.Id).FirstOrDefault();
            if (MenuDAO == null)
                return false;
            MenuDAO.Id = Menu.Id;
            MenuDAO.Name = Menu.Name;
            MenuDAO.Path = Menu.Path;
            MenuDAO.IsDeleted = Menu.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Menu);
            return true;
        }

        public async Task<bool> Delete(Menu Menu)
        {
            await DataContext.Menu.Where(x => x.Id == Menu.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Menu> Menus)
        {
            List<MenuDAO> MenuDAOs = new List<MenuDAO>();
            foreach (Menu Menu in Menus)
            {
                MenuDAO MenuDAO = new MenuDAO();
                MenuDAO.Id = Menu.Id;
                MenuDAO.Name = Menu.Name;
                MenuDAO.Path = Menu.Path;
                MenuDAO.IsDeleted = Menu.IsDeleted;
                MenuDAOs.Add(MenuDAO);
            }
            await DataContext.BulkMergeAsync(MenuDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Menu> Menus)
        {
            List<long> Ids = Menus.Select(x => x.Id).ToList();
            await DataContext.Menu
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Menu Menu)
        {
            await DataContext.Field
                .Where(x => x.MenuId == Menu.Id)
                .DeleteFromQueryAsync();
            List<FieldDAO> FieldDAOs = new List<FieldDAO>();
            if (Menu.Fields != null)
            {
                foreach (Field Field in Menu.Fields)
                {
                    FieldDAO FieldDAO = new FieldDAO();
                    FieldDAO.Id = Field.Id;
                    FieldDAO.Name = Field.Name;
                    FieldDAO.Type = Field.Type;
                    FieldDAO.MenuId = Menu.Id;
                    FieldDAO.IsDeleted = Field.IsDeleted;
                    FieldDAOs.Add(FieldDAO);
                }
                await DataContext.Field.BulkMergeAsync(FieldDAOs);
            }
            await DataContext.Page
                .Where(x => x.MenuId == Menu.Id)
                .DeleteFromQueryAsync();
            List<PageDAO> PageDAOs = new List<PageDAO>();
            if (Menu.Pages != null)
            {
                foreach (Page Page in Menu.Pages)
                {
                    PageDAO PageDAO = new PageDAO();
                    PageDAO.Id = Page.Id;
                    PageDAO.Name = Page.Name;
                    PageDAO.Path = Page.Path;
                    PageDAO.MenuId = Menu.Id;
                    PageDAO.IsDeleted = Page.IsDeleted;
                    PageDAOs.Add(PageDAO);
                }
                await DataContext.Page.BulkMergeAsync(PageDAOs);
            }
        }
        
    }
}
