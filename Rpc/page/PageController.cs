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
using DMS.Services.MPage;
using DMS.Services.MMenu;

namespace DMS.Rpc.page
{
    public class PageRoute : Root
    {
        public const string Master = Module + "/page/page-master";
        public const string Detail = Module + "/page/page-detail";
        private const string Default = Rpc + Module + "/page";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListMenu = Default + "/single-list-menu";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PageFilter.Id), FieldType.ID },
            { nameof(PageFilter.Name), FieldType.STRING },
            { nameof(PageFilter.Path), FieldType.STRING },
            { nameof(PageFilter.MenuId), FieldType.ID },
        };
    }

    public class PageController : RpcController
    {
        private IMenuService MenuService;
        private IPageService PageService;
        private ICurrentContext CurrentContext;
        public PageController(
            IMenuService MenuService,
            IPageService PageService,
            ICurrentContext CurrentContext
        )
        {
            this.MenuService = MenuService;
            this.PageService = PageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            PageFilter = PageService.ToFilter(PageFilter);
            int count = await PageService.Count(PageFilter);
            return count;
        }

        [Route(PageRoute.List), HttpPost]
        public async Task<ActionResult<List<Page_PageDTO>>> List([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            PageFilter = PageService.ToFilter(PageFilter);
            List<Page> Pages = await PageService.List(PageFilter);
            List<Page_PageDTO> Page_PageDTOs = Pages
                .Select(c => new Page_PageDTO(c)).ToList();
            return Page_PageDTOs;
        }

        [Route(PageRoute.Get), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Get([FromBody]Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Page_PageDTO.Id))
                return Forbid();

            Page Page = await PageService.Get(Page_PageDTO.Id);
            return new Page_PageDTO(Page);
        }

        [Route(PageRoute.Create), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Create([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Page_PageDTO.Id))
                return Forbid();

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Create(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Update), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Update([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Page_PageDTO.Id))
                return Forbid();

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Update(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Delete), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Delete([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Page_PageDTO.Id))
                return Forbid();

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Delete(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Import), HttpPost]
        public async Task<ActionResult<List<Page_PageDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Page> Pages = await PageService.Import(DataFile);
            List<Page_PageDTO> Page_PageDTOs = Pages
                .Select(c => new Page_PageDTO(c)).ToList();
            return Page_PageDTOs;
        }

        [Route(PageRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            PageFilter = PageService.ToFilter(PageFilter);
            DataFile DataFile = await PageService.Export(PageFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(PageRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = new PageFilter();
            PageFilter.Id.In = Ids;
            PageFilter.Selects = PageSelect.Id;
            PageFilter.Skip = 0;
            PageFilter.Take = int.MaxValue;

            List<Page> Pages = await PageService.List(PageFilter);
            Pages = await PageService.BulkDelete(Pages);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            PageFilter PageFilter = new PageFilter();
            if (Id > 0)
                PageFilter.Id = new IdFilter { Equal = Id };
            PageFilter = PageService.ToFilter(PageFilter);
            int count = await PageService.Count(PageFilter);
            if (count == 0)
                return false;
            return true;
        }

        private Page ConvertDTOToEntity(Page_PageDTO Page_PageDTO)
        {
            Page Page = new Page();
            Page.Id = Page_PageDTO.Id;
            Page.Name = Page_PageDTO.Name;
            Page.Path = Page_PageDTO.Path;
            Page.MenuId = Page_PageDTO.MenuId;
            Page.IsDeleted = Page_PageDTO.IsDeleted;
            Page.Menu = Page_PageDTO.Menu == null ? null : new Menu
            {
                Id = Page_PageDTO.Menu.Id,
                Name = Page_PageDTO.Menu.Name,
                Path = Page_PageDTO.Menu.Path,
                IsDeleted = Page_PageDTO.Menu.IsDeleted,
            };
            Page.BaseLanguage = CurrentContext.Language;
            return Page;
        }

        private PageFilter ConvertFilterDTOToFilterEntity(Page_PageFilterDTO Page_PageFilterDTO)
        {
            PageFilter PageFilter = new PageFilter();
            PageFilter.Selects = PageSelect.ALL;
            PageFilter.Skip = Page_PageFilterDTO.Skip;
            PageFilter.Take = Page_PageFilterDTO.Take;
            PageFilter.OrderBy = Page_PageFilterDTO.OrderBy;
            PageFilter.OrderType = Page_PageFilterDTO.OrderType;

            PageFilter.Id = Page_PageFilterDTO.Id;
            PageFilter.Name = Page_PageFilterDTO.Name;
            PageFilter.Path = Page_PageFilterDTO.Path;
            PageFilter.MenuId = Page_PageFilterDTO.MenuId;
            return PageFilter;
        }

        [Route(PageRoute.SingleListMenu), HttpPost]
        public async Task<List<Page_MenuDTO>> SingleListMenu([FromBody] Page_MenuFilterDTO Page_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Skip = 0;
            MenuFilter.Take = 20;
            MenuFilter.OrderBy = MenuOrder.Id;
            MenuFilter.OrderType = OrderType.ASC;
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Id = Page_MenuFilterDTO.Id;
            MenuFilter.Name = Page_MenuFilterDTO.Name;
            MenuFilter.Path = Page_MenuFilterDTO.Path;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Page_MenuDTO> Page_MenuDTOs = Menus
                .Select(x => new Page_MenuDTO(x)).ToList();
            return Page_MenuDTOs;
        }

    }
}

