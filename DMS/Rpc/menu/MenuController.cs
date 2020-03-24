using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MMenu;
using DMS.Services.MField;
using DMS.Services.MPage;

namespace DMS.Rpc.menu
{
    public class MenuRoute : Root
    {
        public const string Master = Module + "/menu/menu-master";
        public const string Detail = Module + "/menu/menu-detail";
        private const string Default = Rpc + Module + "/menu";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListField = Default + "/single-list-field";
        public const string SingleListPage = Default + "/single-list-page";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(MenuFilter.Id), FieldType.ID },
            { nameof(MenuFilter.Name), FieldType.STRING },
            { nameof(MenuFilter.Path), FieldType.STRING },
        };
    }

    public class MenuController : RpcController
    {
        private IFieldService FieldService;
        private IPageService PageService;
        private IMenuService MenuService;
        private ICurrentContext CurrentContext;
        public MenuController(
            IFieldService FieldService,
            IPageService PageService,
            IMenuService MenuService,
            ICurrentContext CurrentContext
        )
        {
            this.FieldService = FieldService;
            this.PageService = PageService;
            this.MenuService = MenuService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MenuRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Menu_MenuFilterDTO Menu_MenuFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MenuFilter MenuFilter = ConvertFilterDTOToFilterEntity(Menu_MenuFilterDTO);
            MenuFilter = MenuService.ToFilter(MenuFilter);
            int count = await MenuService.Count(MenuFilter);
            return count;
        }

        [Route(MenuRoute.List), HttpPost]
        public async Task<ActionResult<List<Menu_MenuDTO>>> List([FromBody] Menu_MenuFilterDTO Menu_MenuFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MenuFilter MenuFilter = ConvertFilterDTOToFilterEntity(Menu_MenuFilterDTO);
            MenuFilter = MenuService.ToFilter(MenuFilter);
            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Menu_MenuDTO> Menu_MenuDTOs = Menus
                .Select(c => new Menu_MenuDTO(c)).ToList();
            return Menu_MenuDTOs;
        }

        [Route(MenuRoute.Get), HttpPost]
        public async Task<ActionResult<Menu_MenuDTO>> Get([FromBody]Menu_MenuDTO Menu_MenuDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Menu_MenuDTO.Id))
                return Forbid();

            Menu Menu = await MenuService.Get(Menu_MenuDTO.Id);
            return new Menu_MenuDTO(Menu);
        }

        [Route(MenuRoute.Create), HttpPost]
        public async Task<ActionResult<Menu_MenuDTO>> Create([FromBody] Menu_MenuDTO Menu_MenuDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Menu_MenuDTO.Id))
                return Forbid();

            Menu Menu = ConvertDTOToEntity(Menu_MenuDTO);
            Menu = await MenuService.Create(Menu);
            Menu_MenuDTO = new Menu_MenuDTO(Menu);
            if (Menu.IsValidated)
                return Menu_MenuDTO;
            else
                return BadRequest(Menu_MenuDTO);
        }

        [Route(MenuRoute.Update), HttpPost]
        public async Task<ActionResult<Menu_MenuDTO>> Update([FromBody] Menu_MenuDTO Menu_MenuDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Menu_MenuDTO.Id))
                return Forbid();

            Menu Menu = ConvertDTOToEntity(Menu_MenuDTO);
            Menu = await MenuService.Update(Menu);
            Menu_MenuDTO = new Menu_MenuDTO(Menu);
            if (Menu.IsValidated)
                return Menu_MenuDTO;
            else
                return BadRequest(Menu_MenuDTO);
        }

        [Route(MenuRoute.Delete), HttpPost]
        public async Task<ActionResult<Menu_MenuDTO>> Delete([FromBody] Menu_MenuDTO Menu_MenuDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Menu_MenuDTO.Id))
                return Forbid();

            Menu Menu = ConvertDTOToEntity(Menu_MenuDTO);
            Menu = await MenuService.Delete(Menu);
            Menu_MenuDTO = new Menu_MenuDTO(Menu);
            if (Menu.IsValidated)
                return Menu_MenuDTO;
            else
                return BadRequest(Menu_MenuDTO);
        }

        [Route(MenuRoute.Import), HttpPost]
        public async Task<ActionResult<List<Menu_MenuDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Menu> Menus = await MenuService.Import(DataFile);
            List<Menu_MenuDTO> Menu_MenuDTOs = Menus
                .Select(c => new Menu_MenuDTO(c)).ToList();
            return Menu_MenuDTOs;
        }

        [Route(MenuRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Menu_MenuFilterDTO Menu_MenuFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MenuFilter MenuFilter = ConvertFilterDTOToFilterEntity(Menu_MenuFilterDTO);
            MenuFilter = MenuService.ToFilter(MenuFilter);
            DataFile DataFile = await MenuService.Export(MenuFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(MenuRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Id = new IdFilter { In = Ids };
            MenuFilter.Selects = MenuSelect.Id;
            MenuFilter.Skip = 0;
            MenuFilter.Take = int.MaxValue;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            Menus = await MenuService.BulkDelete(Menus);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter = MenuService.ToFilter(MenuFilter);
            if (Id == 0)
            {

            }
            else
            {
                MenuFilter.Id = new IdFilter { Equal = Id };
                int count = await MenuService.Count(MenuFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Menu ConvertDTOToEntity(Menu_MenuDTO Menu_MenuDTO)
        {
            Menu Menu = new Menu();
            Menu.Id = Menu_MenuDTO.Id;
            Menu.Name = Menu_MenuDTO.Name;
            Menu.Path = Menu_MenuDTO.Path;
            Menu.IsDeleted = Menu_MenuDTO.IsDeleted;
            Menu.Fields = Menu_MenuDTO.Fields?
                .Select(x => new Field
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    IsDeleted = x.IsDeleted,
                }).ToList();
            Menu.Pages = Menu_MenuDTO.Pages?
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    IsDeleted = x.IsDeleted,
                }).ToList();
            Menu.BaseLanguage = CurrentContext.Language;
            return Menu;
        }

        private MenuFilter ConvertFilterDTOToFilterEntity(Menu_MenuFilterDTO Menu_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Skip = Menu_MenuFilterDTO.Skip;
            MenuFilter.Take = Menu_MenuFilterDTO.Take;
            MenuFilter.OrderBy = Menu_MenuFilterDTO.OrderBy;
            MenuFilter.OrderType = Menu_MenuFilterDTO.OrderType;

            MenuFilter.Id = Menu_MenuFilterDTO.Id;
            MenuFilter.Name = Menu_MenuFilterDTO.Name;
            MenuFilter.Path = Menu_MenuFilterDTO.Path;
            return MenuFilter;
        }

        [Route(MenuRoute.SingleListField), HttpPost]
        public async Task<List<Menu_FieldDTO>> SingleListField([FromBody] Menu_FieldFilterDTO Menu_FieldFilterDTO)
        {
            FieldFilter FieldFilter = new FieldFilter();
            FieldFilter.Skip = 0;
            FieldFilter.Take = 20;
            FieldFilter.OrderBy = FieldOrder.Id;
            FieldFilter.OrderType = OrderType.ASC;
            FieldFilter.Selects = FieldSelect.ALL;
            FieldFilter.Id = Menu_FieldFilterDTO.Id;
            FieldFilter.Name = Menu_FieldFilterDTO.Name;
            FieldFilter.Type = Menu_FieldFilterDTO.Type;
            FieldFilter.MenuId = Menu_FieldFilterDTO.MenuId;

            List<Field> Fields = await FieldService.List(FieldFilter);
            List<Menu_FieldDTO> Menu_FieldDTOs = Fields
                .Select(x => new Menu_FieldDTO(x)).ToList();
            return Menu_FieldDTOs;
        }
        [Route(MenuRoute.SingleListPage), HttpPost]
        public async Task<List<Menu_PageDTO>> SingleListPage([FromBody] Menu_PageFilterDTO Menu_PageFilterDTO)
        {
            PageFilter PageFilter = new PageFilter();
            PageFilter.Skip = 0;
            PageFilter.Take = 20;
            PageFilter.OrderBy = PageOrder.Id;
            PageFilter.OrderType = OrderType.ASC;
            PageFilter.Selects = PageSelect.ALL;
            PageFilter.Id = Menu_PageFilterDTO.Id;
            PageFilter.Name = Menu_PageFilterDTO.Name;
            PageFilter.Path = Menu_PageFilterDTO.Path;
            PageFilter.MenuId = Menu_PageFilterDTO.MenuId;

            List<Page> Pages = await PageService.List(PageFilter);
            List<Menu_PageDTO> Menu_PageDTOs = Pages
                .Select(x => new Menu_PageDTO(x)).ToList();
            return Menu_PageDTOs;
        }

    }
}

