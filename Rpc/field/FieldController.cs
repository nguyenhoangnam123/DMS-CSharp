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
using DMS.Services.MField;
using DMS.Services.MMenu;

namespace DMS.Rpc.field
{
    public class FieldRoute : Root
    {
        public const string Master = Module + "/field/field-master";
        public const string Detail = Module + "/field/field-detail";
        private const string Default = Rpc + Module + "/field";
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
            { nameof(FieldFilter.Id), FieldType.ID },
            { nameof(FieldFilter.Name), FieldType.STRING },
            { nameof(FieldFilter.Type), FieldType.STRING },
            { nameof(FieldFilter.MenuId), FieldType.ID },
        };
    }

    public class FieldController : RpcController
    {
        private IMenuService MenuService;
        private IFieldService FieldService;
        private ICurrentContext CurrentContext;
        public FieldController(
            IMenuService MenuService,
            IFieldService FieldService,
            ICurrentContext CurrentContext
        )
        {
            this.MenuService = MenuService;
            this.FieldService = FieldService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FieldRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Field_FieldFilterDTO Field_FieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FieldFilter FieldFilter = ConvertFilterDTOToFilterEntity(Field_FieldFilterDTO);
            FieldFilter = FieldService.ToFilter(FieldFilter);
            int count = await FieldService.Count(FieldFilter);
            return count;
        }

        [Route(FieldRoute.List), HttpPost]
        public async Task<ActionResult<List<Field_FieldDTO>>> List([FromBody] Field_FieldFilterDTO Field_FieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FieldFilter FieldFilter = ConvertFilterDTOToFilterEntity(Field_FieldFilterDTO);
            FieldFilter = FieldService.ToFilter(FieldFilter);
            List<Field> Fields = await FieldService.List(FieldFilter);
            List<Field_FieldDTO> Field_FieldDTOs = Fields
                .Select(c => new Field_FieldDTO(c)).ToList();
            return Field_FieldDTOs;
        }

        [Route(FieldRoute.Get), HttpPost]
        public async Task<ActionResult<Field_FieldDTO>> Get([FromBody]Field_FieldDTO Field_FieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Field_FieldDTO.Id))
                return Forbid();

            Field Field = await FieldService.Get(Field_FieldDTO.Id);
            return new Field_FieldDTO(Field);
        }

        [Route(FieldRoute.Create), HttpPost]
        public async Task<ActionResult<Field_FieldDTO>> Create([FromBody] Field_FieldDTO Field_FieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Field_FieldDTO.Id))
                return Forbid();

            Field Field = ConvertDTOToEntity(Field_FieldDTO);
            Field = await FieldService.Create(Field);
            Field_FieldDTO = new Field_FieldDTO(Field);
            if (Field.IsValidated)
                return Field_FieldDTO;
            else
                return BadRequest(Field_FieldDTO);
        }

        [Route(FieldRoute.Update), HttpPost]
        public async Task<ActionResult<Field_FieldDTO>> Update([FromBody] Field_FieldDTO Field_FieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Field_FieldDTO.Id))
                return Forbid();

            Field Field = ConvertDTOToEntity(Field_FieldDTO);
            Field = await FieldService.Update(Field);
            Field_FieldDTO = new Field_FieldDTO(Field);
            if (Field.IsValidated)
                return Field_FieldDTO;
            else
                return BadRequest(Field_FieldDTO);
        }

        [Route(FieldRoute.Delete), HttpPost]
        public async Task<ActionResult<Field_FieldDTO>> Delete([FromBody] Field_FieldDTO Field_FieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Field_FieldDTO.Id))
                return Forbid();

            Field Field = ConvertDTOToEntity(Field_FieldDTO);
            Field = await FieldService.Delete(Field);
            Field_FieldDTO = new Field_FieldDTO(Field);
            if (Field.IsValidated)
                return Field_FieldDTO;
            else
                return BadRequest(Field_FieldDTO);
        }

        [Route(FieldRoute.Import), HttpPost]
        public async Task<ActionResult<List<Field_FieldDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Field> Fields = await FieldService.Import(DataFile);
            List<Field_FieldDTO> Field_FieldDTOs = Fields
                .Select(c => new Field_FieldDTO(c)).ToList();
            return Field_FieldDTOs;
        }

        [Route(FieldRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Field_FieldFilterDTO Field_FieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FieldFilter FieldFilter = ConvertFilterDTOToFilterEntity(Field_FieldFilterDTO);
            FieldFilter = FieldService.ToFilter(FieldFilter);
            DataFile DataFile = await FieldService.Export(FieldFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(FieldRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FieldFilter FieldFilter = new FieldFilter();
            FieldFilter.Id.In = Ids;
            FieldFilter.Selects = FieldSelect.Id;
            FieldFilter.Skip = 0;
            FieldFilter.Take = int.MaxValue;

            List<Field> Fields = await FieldService.List(FieldFilter);
            Fields = await FieldService.BulkDelete(Fields);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            FieldFilter FieldFilter = new FieldFilter();
            if (Id > 0)
                FieldFilter.Id = new IdFilter { Equal = Id };
            FieldFilter = FieldService.ToFilter(FieldFilter);
            int count = await FieldService.Count(FieldFilter);
            if (count == 0)
                return false;
            return true;
        }

        private Field ConvertDTOToEntity(Field_FieldDTO Field_FieldDTO)
        {
            Field Field = new Field();
            Field.Id = Field_FieldDTO.Id;
            Field.Name = Field_FieldDTO.Name;
            Field.Type = Field_FieldDTO.Type;
            Field.MenuId = Field_FieldDTO.MenuId;
            Field.IsDeleted = Field_FieldDTO.IsDeleted;
            Field.Menu = Field_FieldDTO.Menu == null ? null : new Menu
            {
                Id = Field_FieldDTO.Menu.Id,
                Name = Field_FieldDTO.Menu.Name,
                Path = Field_FieldDTO.Menu.Path,
                IsDeleted = Field_FieldDTO.Menu.IsDeleted,
            };
            Field.BaseLanguage = CurrentContext.Language;
            return Field;
        }

        private FieldFilter ConvertFilterDTOToFilterEntity(Field_FieldFilterDTO Field_FieldFilterDTO)
        {
            FieldFilter FieldFilter = new FieldFilter();
            FieldFilter.Selects = FieldSelect.ALL;
            FieldFilter.Skip = Field_FieldFilterDTO.Skip;
            FieldFilter.Take = Field_FieldFilterDTO.Take;
            FieldFilter.OrderBy = Field_FieldFilterDTO.OrderBy;
            FieldFilter.OrderType = Field_FieldFilterDTO.OrderType;

            FieldFilter.Id = Field_FieldFilterDTO.Id;
            FieldFilter.Name = Field_FieldFilterDTO.Name;
            FieldFilter.Type = Field_FieldFilterDTO.Type;
            FieldFilter.MenuId = Field_FieldFilterDTO.MenuId;
            return FieldFilter;
        }

        [Route(FieldRoute.SingleListMenu), HttpPost]
        public async Task<List<Field_MenuDTO>> SingleListMenu([FromBody] Field_MenuFilterDTO Field_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Skip = 0;
            MenuFilter.Take = 20;
            MenuFilter.OrderBy = MenuOrder.Id;
            MenuFilter.OrderType = OrderType.ASC;
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Id = Field_MenuFilterDTO.Id;
            MenuFilter.Name = Field_MenuFilterDTO.Name;
            MenuFilter.Path = Field_MenuFilterDTO.Path;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Field_MenuDTO> Field_MenuDTOs = Menus
                .Select(x => new Field_MenuDTO(x)).ToList();
            return Field_MenuDTOs;
        }

    }
}

