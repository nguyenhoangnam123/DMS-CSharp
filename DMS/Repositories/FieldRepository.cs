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
    public interface IFieldRepository
    {
        Task<int> Count(FieldFilter FieldFilter);
        Task<List<Field>> List(FieldFilter FieldFilter);
        Task<Field> Get(long Id);
        Task<bool> Create(Field Field);
        Task<bool> Update(Field Field);
        Task<bool> Delete(Field Field);
        Task<bool> BulkMerge(List<Field> Fields);
        Task<bool> BulkDelete(List<Field> Fields);
    }
    public class FieldRepository : IFieldRepository
    {
        private DataContext DataContext;
        public FieldRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<FieldDAO> DynamicFilter(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Type != null)
                query = query.Where(q => q.Type, filter.Type);
            if (filter.MenuId != null)
                query = query.Where(q => q.MenuId, filter.MenuId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<FieldDAO> OrFilter(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FieldDAO> initQuery = query.Where(q => false);
            foreach (FieldFilter FieldFilter in filter.OrFilter)
            {
                IQueryable<FieldDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Type != null)
                    queryable = queryable.Where(q => q.Type, filter.Type);
                if (filter.MenuId != null)
                    queryable = queryable.Where(q => q.MenuId, filter.MenuId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<FieldDAO> DynamicOrder(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FieldOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FieldOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case FieldOrder.Type:
                            query = query.OrderBy(q => q.Type);
                            break;
                        case FieldOrder.Menu:
                            query = query.OrderBy(q => q.MenuId);
                            break;
                        case FieldOrder.IsDeleted:
                            query = query.OrderBy(q => q.IsDeleted);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FieldOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FieldOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case FieldOrder.Type:
                            query = query.OrderByDescending(q => q.Type);
                            break;
                        case FieldOrder.Menu:
                            query = query.OrderByDescending(q => q.MenuId);
                            break;
                        case FieldOrder.IsDeleted:
                            query = query.OrderByDescending(q => q.IsDeleted);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Field>> DynamicSelect(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            List<Field> Fields = await query.Select(q => new Field()
            {
                Id = filter.Selects.Contains(FieldSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(FieldSelect.Name) ? q.Name : default(string),
                Type = filter.Selects.Contains(FieldSelect.Type) ? q.Type : default(string),
                MenuId = filter.Selects.Contains(FieldSelect.Menu) ? q.MenuId : default(long),
                IsDeleted = filter.Selects.Contains(FieldSelect.IsDeleted) ? q.IsDeleted : default(bool),
                Menu = filter.Selects.Contains(FieldSelect.Menu) && q.Menu != null ? new Menu
                {
                    Id = q.Menu.Id,
                    Name = q.Menu.Name,
                    Path = q.Menu.Path,
                    IsDeleted = q.Menu.IsDeleted,
                } : null,
            }).ToListAsync();
            return Fields;
        }

        public async Task<int> Count(FieldFilter filter)
        {
            IQueryable<FieldDAO> Fields = DataContext.Field;
            Fields = DynamicFilter(Fields, filter);
            return await Fields.CountAsync();
        }

        public async Task<List<Field>> List(FieldFilter filter)
        {
            if (filter == null) return new List<Field>();
            IQueryable<FieldDAO> FieldDAOs = DataContext.Field;
            FieldDAOs = DynamicFilter(FieldDAOs, filter);
            FieldDAOs = DynamicOrder(FieldDAOs, filter);
            List<Field> Fields = await DynamicSelect(FieldDAOs, filter);
            return Fields;
        }

        public async Task<Field> Get(long Id)
        {
            Field Field = await DataContext.Field.Where(x => x.Id == Id).Select(x => new Field()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
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

            if (Field == null)
                return null;

            return Field;
        }
        public async Task<bool> Create(Field Field)
        {
            FieldDAO FieldDAO = new FieldDAO();
            FieldDAO.Id = Field.Id;
            FieldDAO.Name = Field.Name;
            FieldDAO.Type = Field.Type;
            FieldDAO.MenuId = Field.MenuId;
            FieldDAO.IsDeleted = Field.IsDeleted;
            DataContext.Field.Add(FieldDAO);
            await DataContext.SaveChangesAsync();
            Field.Id = FieldDAO.Id;
            await SaveReference(Field);
            return true;
        }

        public async Task<bool> Update(Field Field)
        {
            FieldDAO FieldDAO = DataContext.Field.Where(x => x.Id == Field.Id).FirstOrDefault();
            if (FieldDAO == null)
                return false;
            FieldDAO.Id = Field.Id;
            FieldDAO.Name = Field.Name;
            FieldDAO.Type = Field.Type;
            FieldDAO.MenuId = Field.MenuId;
            FieldDAO.IsDeleted = Field.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Field);
            return true;
        }

        public async Task<bool> Delete(Field Field)
        {
            await DataContext.Field.Where(x => x.Id == Field.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Field> Fields)
        {
            List<FieldDAO> FieldDAOs = new List<FieldDAO>();
            foreach (Field Field in Fields)
            {
                FieldDAO FieldDAO = new FieldDAO();
                FieldDAO.Id = Field.Id;
                FieldDAO.Name = Field.Name;
                FieldDAO.Type = Field.Type;
                FieldDAO.MenuId = Field.MenuId;
                FieldDAO.IsDeleted = Field.IsDeleted;
                FieldDAOs.Add(FieldDAO);
            }
            await DataContext.BulkMergeAsync(FieldDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Field> Fields)
        {
            List<long> Ids = Fields.Select(x => x.Id).ToList();
            await DataContext.Field
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Field Field)
        {
        }

    }
}
