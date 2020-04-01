﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MStatus;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGroupingContent;
using DMS.Enums;
using OfficeOpenXml;
using System.IO;

namespace DMS.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGroupingRoute : Root
    {
        public const string Master = Module + "/unit-of-measure-grouping/unit-of-measure-grouping-master";
        public const string Detail = Module + "/unit-of-measure-grouping/unit-of-measure-grouping-detail";
        private const string Default = Rpc + Module + "/unit-of-measure-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(UnitOfMeasureGroupingFilter.Id), FieldType.ID },
            { nameof(UnitOfMeasureGroupingFilter.Code), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.Name), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.Description), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(UnitOfMeasureGroupingFilter.StatusId), FieldType.ID },
        };
    }

    public class UnitOfMeasureGroupingController : RpcController
    {
        private IStatusService StatusService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingContentService UnitOfMeasureGroupingContentService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private ICurrentContext CurrentContext;
        public UnitOfMeasureGroupingController(
            IStatusService StatusService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingContentService UnitOfMeasureGroupingContentService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingContentService = UnitOfMeasureGroupingContentService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(UnitOfMeasureGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO);
            UnitOfMeasureGroupingFilter = UnitOfMeasureGroupingService.ToFilter(UnitOfMeasureGroupingFilter);
            int count = await UnitOfMeasureGroupingService.Count(UnitOfMeasureGroupingFilter);
            return count;
        }

        [Route(UnitOfMeasureGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>>> List([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO);
            UnitOfMeasureGroupingFilter = UnitOfMeasureGroupingService.ToFilter(UnitOfMeasureGroupingFilter);
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            List<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO> UnitOfMeasureGrouping_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(c => new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(c)).ToList();
            return UnitOfMeasureGrouping_UnitOfMeasureGroupingDTOs;
        }

        [Route(UnitOfMeasureGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>> Get([FromBody]UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id))
                return Forbid();

            UnitOfMeasureGrouping UnitOfMeasureGrouping = await UnitOfMeasureGroupingService.Get(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id);
            return new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping);
        }

        [Route(UnitOfMeasureGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>> Create([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id))
                return Forbid();

            UnitOfMeasureGrouping UnitOfMeasureGrouping = ConvertDTOToEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
            UnitOfMeasureGrouping = await UnitOfMeasureGroupingService.Create(UnitOfMeasureGrouping);
            UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO = new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping);
            if (UnitOfMeasureGrouping.IsValidated)
                return UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO;
            else
                return BadRequest(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
        }

        [Route(UnitOfMeasureGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>> Update([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id))
                return Forbid();

            UnitOfMeasureGrouping UnitOfMeasureGrouping = ConvertDTOToEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
            UnitOfMeasureGrouping = await UnitOfMeasureGroupingService.Update(UnitOfMeasureGrouping);
            UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO = new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping);
            if (UnitOfMeasureGrouping.IsValidated)
                return UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO;
            else
                return BadRequest(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
        }

        [Route(UnitOfMeasureGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>> Delete([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id))
                return Forbid();

            UnitOfMeasureGrouping UnitOfMeasureGrouping = ConvertDTOToEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
            UnitOfMeasureGrouping = await UnitOfMeasureGroupingService.Delete(UnitOfMeasureGrouping);
            UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO = new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping);
            if (UnitOfMeasureGrouping.IsValidated)
                return UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO;
            else
                return BadRequest(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO);
        }

        [Route(UnitOfMeasureGroupingRoute.Import), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.Import(DataFile);
            List<UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO> UnitOfMeasureGrouping_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(c => new UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(c)).ToList();
            return UnitOfMeasureGrouping_UnitOfMeasureGroupingDTOs;
        }

        [Route(UnitOfMeasureGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO);
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = int.MaxValue;
            UnitOfMeasureGroupingFilter = UnitOfMeasureGroupingService.ToFilter(UnitOfMeasureGroupingFilter);

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var UnitOfMeasureGroupingHeaders = new List<string[]>()
                {
                    new string[] {"Mã nhóm đơn vị tính","Tên nhóm đơn vị tính","Mô tả"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < UnitOfMeasureGroupings.Count; i++)
                {
                    var UnitOfMeasureGrouping = UnitOfMeasureGroupings[i];

                    data.Add(new Object[]
                    {
                         UnitOfMeasureGrouping.Code,
                         UnitOfMeasureGrouping.Name,
                         UnitOfMeasureGrouping.Description
                    });
                    excel.GenerateWorksheet("UnitOfMeasureGrouping", UnitOfMeasureGroupingHeaders, data);
                }
                excel.Save();
            }

            return File(memoryStream.ToArray(), "application/octet-stream", "UnitOfMeasure.xlsx");

        }

        [Route(UnitOfMeasureGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter = UnitOfMeasureGroupingService.ToFilter(UnitOfMeasureGroupingFilter);
            UnitOfMeasureGroupingFilter.Id = new IdFilter { In = Ids };
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.Id;
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = int.MaxValue;

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.BulkDelete(UnitOfMeasureGroupings);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter = UnitOfMeasureGroupingService.ToFilter(UnitOfMeasureGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                UnitOfMeasureGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await UnitOfMeasureGroupingService.Count(UnitOfMeasureGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private UnitOfMeasureGrouping ConvertDTOToEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO)
        {
            UnitOfMeasureGrouping UnitOfMeasureGrouping = new UnitOfMeasureGrouping();
            UnitOfMeasureGrouping.Id = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Id;
            UnitOfMeasureGrouping.Code = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Code;
            UnitOfMeasureGrouping.Name = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Name;
            UnitOfMeasureGrouping.Description = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Description;
            UnitOfMeasureGrouping.UnitOfMeasureId = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasureId;
            UnitOfMeasureGrouping.StatusId = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.StatusId;
            UnitOfMeasureGrouping.Status = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Status == null ? null : new Status
            {
                Id = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Status.Id,
                Code = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Status.Code,
                Name = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.Status.Name,
            };
            UnitOfMeasureGrouping.UnitOfMeasure = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure.Id,
                Code = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure.Code,
                Name = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure.Name,
                Description = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure.Description,
                StatusId = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasure.StatusId,
            };
            UnitOfMeasureGrouping.UnitOfMeasureGroupingContents = UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO.UnitOfMeasureGroupingContents?
                .Select(x => new UnitOfMeasureGroupingContent
                {
                    Id = x.Id,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Factor = x.Factor,
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToList();
            UnitOfMeasureGrouping.BaseLanguage = CurrentContext.Language;
            return UnitOfMeasureGrouping;
        }

        private UnitOfMeasureGroupingFilter ConvertFilterDTOToFilterEntity(UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.ALL;
            UnitOfMeasureGroupingFilter.Skip = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Skip;
            UnitOfMeasureGroupingFilter.Take = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Take;
            UnitOfMeasureGroupingFilter.OrderBy = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.OrderBy;
            UnitOfMeasureGroupingFilter.OrderType = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.OrderType;

            UnitOfMeasureGroupingFilter.Id = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Id;
            UnitOfMeasureGroupingFilter.Code = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Code;
            UnitOfMeasureGroupingFilter.Name = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Name;
            UnitOfMeasureGroupingFilter.Description = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.Description;
            UnitOfMeasureGroupingFilter.UnitOfMeasureId = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingFilter.StatusId = UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO.StatusId;
            return UnitOfMeasureGroupingFilter;
        }

        [Route(UnitOfMeasureGroupingRoute.SingleListStatus), HttpPost]
        public async Task<List<UnitOfMeasureGrouping_StatusDTO>> SingleListStatus([FromBody] UnitOfMeasureGrouping_StatusFilterDTO UnitOfMeasureGrouping_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<UnitOfMeasureGrouping_StatusDTO> UnitOfMeasureGrouping_StatusDTOs = Statuses
                .Select(x => new UnitOfMeasureGrouping_StatusDTO(x)).ToList();
            return UnitOfMeasureGrouping_StatusDTOs;
        }
        [Route(UnitOfMeasureGroupingRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<UnitOfMeasureGrouping_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] UnitOfMeasureGrouping_UnitOfMeasureFilterDTO UnitOfMeasureGrouping_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = UnitOfMeasureGrouping_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = UnitOfMeasureGrouping_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = UnitOfMeasureGrouping_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<UnitOfMeasureGrouping_UnitOfMeasureDTO> UnitOfMeasureGrouping_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new UnitOfMeasureGrouping_UnitOfMeasureDTO(x)).ToList();
            return UnitOfMeasureGrouping_UnitOfMeasureDTOs;
        }
    }
}

