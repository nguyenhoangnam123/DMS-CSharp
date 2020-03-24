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
using DMS.Services.MVariation;
using DMS.Services.MVariationGrouping;

namespace DMS.Rpc.variation
{
    public class VariationRoute : Root
    {
        public const string Master = Module + "/variation/variation-master";
        public const string Detail = Module + "/variation/variation-detail";
        private const string Default = Rpc + Module + "/variation";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListVariationGrouping = Default + "/single-list-variation-grouping";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(VariationFilter.Id), FieldType.ID },
            { nameof(VariationFilter.Code), FieldType.STRING },
            { nameof(VariationFilter.Name), FieldType.STRING },
            { nameof(VariationFilter.VariationGroupingId), FieldType.ID },
        };
    }

    public class VariationController : RpcController
    {
        private IVariationGroupingService VariationGroupingService;
        private IVariationService VariationService;
        private ICurrentContext CurrentContext;
        public VariationController(
            IVariationGroupingService VariationGroupingService,
            IVariationService VariationService,
            ICurrentContext CurrentContext
        )
        {
            this.VariationGroupingService = VariationGroupingService;
            this.VariationService = VariationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(VariationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Variation_VariationFilterDTO Variation_VariationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationFilter VariationFilter = ConvertFilterDTOToFilterEntity(Variation_VariationFilterDTO);
            VariationFilter = VariationService.ToFilter(VariationFilter);
            int count = await VariationService.Count(VariationFilter);
            return count;
        }

        [Route(VariationRoute.List), HttpPost]
        public async Task<ActionResult<List<Variation_VariationDTO>>> List([FromBody] Variation_VariationFilterDTO Variation_VariationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationFilter VariationFilter = ConvertFilterDTOToFilterEntity(Variation_VariationFilterDTO);
            VariationFilter = VariationService.ToFilter(VariationFilter);
            List<Variation> Variations = await VariationService.List(VariationFilter);
            List<Variation_VariationDTO> Variation_VariationDTOs = Variations
                .Select(c => new Variation_VariationDTO(c)).ToList();
            return Variation_VariationDTOs;
        }

        [Route(VariationRoute.Get), HttpPost]
        public async Task<ActionResult<Variation_VariationDTO>> Get([FromBody]Variation_VariationDTO Variation_VariationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Variation_VariationDTO.Id))
                return Forbid();

            Variation Variation = await VariationService.Get(Variation_VariationDTO.Id);
            return new Variation_VariationDTO(Variation);
        }

        [Route(VariationRoute.Create), HttpPost]
        public async Task<ActionResult<Variation_VariationDTO>> Create([FromBody] Variation_VariationDTO Variation_VariationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Variation_VariationDTO.Id))
                return Forbid();

            Variation Variation = ConvertDTOToEntity(Variation_VariationDTO);
            Variation = await VariationService.Create(Variation);
            Variation_VariationDTO = new Variation_VariationDTO(Variation);
            if (Variation.IsValidated)
                return Variation_VariationDTO;
            else
                return BadRequest(Variation_VariationDTO);
        }

        [Route(VariationRoute.Update), HttpPost]
        public async Task<ActionResult<Variation_VariationDTO>> Update([FromBody] Variation_VariationDTO Variation_VariationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Variation_VariationDTO.Id))
                return Forbid();

            Variation Variation = ConvertDTOToEntity(Variation_VariationDTO);
            Variation = await VariationService.Update(Variation);
            Variation_VariationDTO = new Variation_VariationDTO(Variation);
            if (Variation.IsValidated)
                return Variation_VariationDTO;
            else
                return BadRequest(Variation_VariationDTO);
        }

        [Route(VariationRoute.Delete), HttpPost]
        public async Task<ActionResult<Variation_VariationDTO>> Delete([FromBody] Variation_VariationDTO Variation_VariationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Variation_VariationDTO.Id))
                return Forbid();

            Variation Variation = ConvertDTOToEntity(Variation_VariationDTO);
            Variation = await VariationService.Delete(Variation);
            Variation_VariationDTO = new Variation_VariationDTO(Variation);
            if (Variation.IsValidated)
                return Variation_VariationDTO;
            else
                return BadRequest(Variation_VariationDTO);
        }

        [Route(VariationRoute.Import), HttpPost]
        public async Task<ActionResult<List<Variation_VariationDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Variation> Variations = await VariationService.Import(DataFile);
            List<Variation_VariationDTO> Variation_VariationDTOs = Variations
                .Select(c => new Variation_VariationDTO(c)).ToList();
            return Variation_VariationDTOs;
        }

        [Route(VariationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Variation_VariationFilterDTO Variation_VariationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationFilter VariationFilter = ConvertFilterDTOToFilterEntity(Variation_VariationFilterDTO);
            VariationFilter = VariationService.ToFilter(VariationFilter);
            DataFile DataFile = await VariationService.Export(VariationFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(VariationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationFilter VariationFilter = new VariationFilter();
            VariationFilter.Id = new IdFilter { In = Ids };
            VariationFilter.Selects = VariationSelect.Id;
            VariationFilter.Skip = 0;
            VariationFilter.Take = int.MaxValue;

            List<Variation> Variations = await VariationService.List(VariationFilter);
            Variations = await VariationService.BulkDelete(Variations);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            VariationFilter VariationFilter = new VariationFilter();
            VariationFilter = VariationService.ToFilter(VariationFilter);
            if (Id == 0)
            {

            }
            else
            {
                VariationFilter.Id = new IdFilter { Equal = Id };
                int count = await VariationService.Count(VariationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Variation ConvertDTOToEntity(Variation_VariationDTO Variation_VariationDTO)
        {
            Variation Variation = new Variation();
            Variation.Id = Variation_VariationDTO.Id;
            Variation.Code = Variation_VariationDTO.Code;
            Variation.Name = Variation_VariationDTO.Name;
            Variation.VariationGroupingId = Variation_VariationDTO.VariationGroupingId;
            Variation.VariationGrouping = Variation_VariationDTO.VariationGrouping == null ? null : new VariationGrouping
            {
                Id = Variation_VariationDTO.VariationGrouping.Id,
                Name = Variation_VariationDTO.VariationGrouping.Name,
                ProductId = Variation_VariationDTO.VariationGrouping.ProductId,
            };
            Variation.BaseLanguage = CurrentContext.Language;
            return Variation;
        }

        private VariationFilter ConvertFilterDTOToFilterEntity(Variation_VariationFilterDTO Variation_VariationFilterDTO)
        {
            VariationFilter VariationFilter = new VariationFilter();
            VariationFilter.Selects = VariationSelect.ALL;
            VariationFilter.Skip = Variation_VariationFilterDTO.Skip;
            VariationFilter.Take = Variation_VariationFilterDTO.Take;
            VariationFilter.OrderBy = Variation_VariationFilterDTO.OrderBy;
            VariationFilter.OrderType = Variation_VariationFilterDTO.OrderType;

            VariationFilter.Id = Variation_VariationFilterDTO.Id;
            VariationFilter.Code = Variation_VariationFilterDTO.Code;
            VariationFilter.Name = Variation_VariationFilterDTO.Name;
            VariationFilter.VariationGroupingId = Variation_VariationFilterDTO.VariationGroupingId;
            return VariationFilter;
        }

        [Route(VariationRoute.SingleListVariationGrouping), HttpPost]
        public async Task<List<Variation_VariationGroupingDTO>> SingleListVariationGrouping([FromBody] Variation_VariationGroupingFilterDTO Variation_VariationGroupingFilterDTO)
        {
            VariationGroupingFilter VariationGroupingFilter = new VariationGroupingFilter();
            VariationGroupingFilter.Skip = 0;
            VariationGroupingFilter.Take = 20;
            VariationGroupingFilter.OrderBy = VariationGroupingOrder.Id;
            VariationGroupingFilter.OrderType = OrderType.ASC;
            VariationGroupingFilter.Selects = VariationGroupingSelect.ALL;
            VariationGroupingFilter.Id = Variation_VariationGroupingFilterDTO.Id;
            VariationGroupingFilter.Name = Variation_VariationGroupingFilterDTO.Name;
            VariationGroupingFilter.ProductId = Variation_VariationGroupingFilterDTO.ProductId;

            List<VariationGrouping> VariationGroupings = await VariationGroupingService.List(VariationGroupingFilter);
            List<Variation_VariationGroupingDTO> Variation_VariationGroupingDTOs = VariationGroupings
                .Select(x => new Variation_VariationGroupingDTO(x)).ToList();
            return Variation_VariationGroupingDTOs;
        }

    }
}

