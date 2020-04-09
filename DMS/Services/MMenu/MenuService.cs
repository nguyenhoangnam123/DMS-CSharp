using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MMenu
{
    public interface IMenuService : IServiceScoped
    {
        Task<int> Count(MenuFilter MenuFilter);
        Task<List<Menu>> List(MenuFilter MenuFilter);
        Task<Menu> Get(long Id);
        Task<Menu> Create(Menu Menu);
        Task<Menu> Update(Menu Menu);
        Task<Menu> Delete(Menu Menu);
        Task<List<Menu>> BulkDelete(List<Menu> Menus);
        Task<List<Menu>> Import(DataFile DataFile);
        Task<DataFile> Export(MenuFilter MenuFilter);
        MenuFilter ToFilter(MenuFilter MenuFilter);
    }

    public class MenuService : BaseService, IMenuService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IMenuValidator MenuValidator;

        public MenuService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IMenuValidator MenuValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.MenuValidator = MenuValidator;
        }
        public async Task<int> Count(MenuFilter MenuFilter)
        {
            try
            {
                int result = await UOW.MenuRepository.Count(MenuFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Menu>> List(MenuFilter MenuFilter)
        {
            try
            {
                List<Menu> Menus = await UOW.MenuRepository.List(MenuFilter);
                return Menus;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Menu> Get(long Id)
        {
            Menu Menu = await UOW.MenuRepository.Get(Id);
            if (Menu == null)
                return null;
            return Menu;
        }

        public async Task<Menu> Create(Menu Menu)
        {
            if (!await MenuValidator.Create(Menu))
                return Menu;

            try
            {
                Menu.Id = 0;
                await UOW.Begin();
                await UOW.MenuRepository.Create(Menu);
                await UOW.Commit();

                await Logging.CreateAuditLog(Menu, new { }, nameof(MenuService));
                return await UOW.MenuRepository.Get(Menu.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Menu> Update(Menu Menu)
        {
            if (!await MenuValidator.Update(Menu))
                return Menu;
            try
            {
                var oldData = await UOW.MenuRepository.Get(Menu.Id);

                await UOW.Begin();
                await UOW.MenuRepository.Update(Menu);
                await UOW.Commit();

                var newData = await UOW.MenuRepository.Get(Menu.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(MenuService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Menu> Delete(Menu Menu)
        {
            if (!await MenuValidator.Delete(Menu))
                return Menu;

            try
            {
                await UOW.Begin();
                await UOW.MenuRepository.Delete(Menu);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Menu, nameof(MenuService));
                return Menu;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Menu>> BulkDelete(List<Menu> Menus)
        {
            if (!await MenuValidator.BulkDelete(Menus))
                return Menus;

            try
            {
                await UOW.Begin();
                await UOW.MenuRepository.BulkDelete(Menus);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Menus, nameof(MenuService));
                return Menus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Menu>> Import(DataFile DataFile)
        {
            List<Menu> Menus = new List<Menu>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Menus;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int IsDeletedColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string IsDeletedValue = worksheet.Cells[i + StartRow, IsDeletedColumn].Value?.ToString();
                    Menu Menu = new Menu();
                    Menu.Name = NameValue;
                    Menu.Path = PathValue;
                    Menus.Add(Menu);
                }
            }

            if (!await MenuValidator.Import(Menus))
                return Menus;

            try
            {
                Menus.ForEach(m => m.Id = 0);
                await UOW.Begin();
                await UOW.MenuRepository.BulkMerge(Menus);
                await UOW.Commit();

                await Logging.CreateAuditLog(Menus, new { }, nameof(MenuService));
                return Menus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(MenuService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(MenuFilter MenuFilter)
        {
            List<Menu> Menus = await UOW.MenuRepository.List(MenuFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Menu);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int IsDeletedColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Menu.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Menu.Name);
                worksheet.Cells[1, PathColumn].Value = nameof(Menu.Path);
                worksheet.Cells[1, IsDeletedColumn].Value = nameof(Menu.IsDeleted);

                for (int i = 0; i < Menus.Count; i++)
                {
                    Menu Menu = Menus[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Menu.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Menu.Name;
                    worksheet.Cells[i + StartRow, PathColumn].Value = Menu.Path;
                    worksheet.Cells[i + StartRow, IsDeletedColumn].Value = Menu.IsDeleted;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Menu),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public MenuFilter ToFilter(MenuFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MenuFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MenuFilter subFilter = new MenuFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Path))
                    subFilter.Path = Map(subFilter.Path, currentFilter.Value);
            }
            return filter;
        }
    }
}
