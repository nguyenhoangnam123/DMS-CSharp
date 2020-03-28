using Common;
using DMS.Entities;
using DMS.Services.MMenu;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public const string Update = Default + "/update";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(MenuFilter.Id), FieldType.ID },
            { nameof(MenuFilter.Code), FieldType.STRING },
            { nameof(MenuFilter.Name), FieldType.STRING },
            { nameof(MenuFilter.Path), FieldType.STRING },
        };
    }

    public class MenuController : RpcController
    {
        private IMenuService MenuService;
        private ICurrentContext CurrentContext;
        public MenuController(
            IMenuService MenuService,
            ICurrentContext CurrentContext
        )
        {
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
            Menu.Code = Menu_MenuDTO.Code;
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
                    MenuId = x.MenuId
                }).ToList();
            Menu.Pages = Menu_MenuDTO.Pages?
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    IsDeleted = x.IsDeleted,
                    MenuId = x.MenuId
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
            MenuFilter.Code = Menu_MenuFilterDTO.Code;
            MenuFilter.Name = Menu_MenuFilterDTO.Name;
            MenuFilter.Path = Menu_MenuFilterDTO.Path;
            return MenuFilter;
        }
    }
}

