using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using DMS.Entities;
using DMS.Services.MExportTemplate;
using DMS.Enums;
using DMS.Rpc.export_template.dto;

namespace DMS.Rpc.export_template
{
    public partial class ExportTemplateController : RpcController
    {
        private IExportTemplateService ExportTemplateService;
        private ICurrentContext CurrentContext;
        public ExportTemplateController(
            IExportTemplateService ExportTemplateService,
            ICurrentContext CurrentContext
        )
        {
            this.ExportTemplateService = ExportTemplateService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ExportTemplateRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ExportTemplate_ExportTemplateFilterDTO ExportTemplate_ExportTemplateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExportTemplateFilter ExportTemplateFilter = ConvertFilterDTOToFilterEntity(ExportTemplate_ExportTemplateFilterDTO);
            ExportTemplateFilter = await ExportTemplateService.ToFilter(ExportTemplateFilter);
            int count = await ExportTemplateService.Count(ExportTemplateFilter);
            return count;
        }

        [Route(ExportTemplateRoute.List), HttpPost]
        public async Task<ActionResult<List<ExportTemplate_ExportTemplateDTO>>> List([FromBody] ExportTemplate_ExportTemplateFilterDTO ExportTemplate_ExportTemplateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExportTemplateFilter ExportTemplateFilter = ConvertFilterDTOToFilterEntity(ExportTemplate_ExportTemplateFilterDTO);
            ExportTemplateFilter = await ExportTemplateService.ToFilter(ExportTemplateFilter);
            List<ExportTemplate> ExportTemplates = await ExportTemplateService.List(ExportTemplateFilter);
            List<ExportTemplate_ExportTemplateDTO> ExportTemplate_ExportTemplateDTOs = ExportTemplates
                .Select(c => new ExportTemplate_ExportTemplateDTO(c)).ToList();
            return ExportTemplate_ExportTemplateDTOs;
        }

        [Route(ExportTemplateRoute.Get), HttpPost]
        public async Task<ActionResult<ExportTemplate_ExportTemplateDTO>> Get([FromBody]ExportTemplate_ExportTemplateDTO ExportTemplate_ExportTemplateDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ExportTemplate_ExportTemplateDTO.Id))
                return Forbid();

            ExportTemplate ExportTemplate = await ExportTemplateService.Get(ExportTemplate_ExportTemplateDTO.Id);
            return new ExportTemplate_ExportTemplateDTO(ExportTemplate);
        }

        [Route(ExportTemplateRoute.GetExample), HttpPost]
        public async Task<ActionResult<object>> GetExample([FromBody] ExportTemplate_ExportTemplateDTO ExportTemplate_ExportTemplateDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ExportTemplate_ExportTemplateDTO.Id))
                return Forbid();

            ExportTemplate ExportTemplate = await ExportTemplateService.Get(ExportTemplate_ExportTemplateDTO.Id);
            if(ExportTemplate.Id == ExportTemplateEnum.PRINT_DIRECT.Id)
                return new ExportTemplate_PrintDTO();
            return null;
        }

        [Route(ExportTemplateRoute.Update), HttpPost]
        public async Task<ActionResult<ExportTemplate_ExportTemplateDTO>> Update([FromForm] IFormFile file, long ExportTemplateId)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ExportTemplateId))
                return Forbid();

            ExportTemplate ExportTemplate = await ExportTemplateService.Get(ExportTemplateId);
            if (ExportTemplate == null)
                return BadRequest("Mẫu file không tồn tại");

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            FileInfo FileInfo = new FileInfo(file.FileName);
            ExportTemplate.FileName = file.FileName;
            ExportTemplate.Extension = FileInfo.Extension;
            ExportTemplate.Content = memoryStream.ToArray();

            ExportTemplate = await ExportTemplateService.Update(ExportTemplate);
            ExportTemplate_ExportTemplateDTO ExportTemplate_ExportTemplateDTO = new ExportTemplate_ExportTemplateDTO(ExportTemplate);
            if (ExportTemplate.IsValidated)
                return ExportTemplate_ExportTemplateDTO;
            else
                return BadRequest(ExportTemplate_ExportTemplateDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ExportTemplateFilter ExportTemplateFilter = new ExportTemplateFilter();
            ExportTemplateFilter = await ExportTemplateService.ToFilter(ExportTemplateFilter);
            if (Id == 0)
            {

            }
            else
            {
                ExportTemplateFilter.Id = new IdFilter { Equal = Id };
                int count = await ExportTemplateService.Count(ExportTemplateFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ExportTemplate ConvertDTOToEntity(ExportTemplate_ExportTemplateDTO ExportTemplate_ExportTemplateDTO)
        {
            ExportTemplate ExportTemplate = new ExportTemplate();
            ExportTemplate.Id = ExportTemplate_ExportTemplateDTO.Id;
            ExportTemplate.Code = ExportTemplate_ExportTemplateDTO.Code;
            ExportTemplate.Name = ExportTemplate_ExportTemplateDTO.Name;
            ExportTemplate.Content = ExportTemplate_ExportTemplateDTO.Content;
            ExportTemplate.BaseLanguage = CurrentContext.Language;
            return ExportTemplate;
        }

        private ExportTemplateFilter ConvertFilterDTOToFilterEntity(ExportTemplate_ExportTemplateFilterDTO ExportTemplate_ExportTemplateFilterDTO)
        {
            ExportTemplateFilter ExportTemplateFilter = new ExportTemplateFilter();
            ExportTemplateFilter.Selects = ExportTemplateSelect.ALL;
            ExportTemplateFilter.Skip = ExportTemplate_ExportTemplateFilterDTO.Skip;
            ExportTemplateFilter.Take = ExportTemplate_ExportTemplateFilterDTO.Take;
            ExportTemplateFilter.OrderBy = ExportTemplate_ExportTemplateFilterDTO.OrderBy;
            ExportTemplateFilter.OrderType = ExportTemplate_ExportTemplateFilterDTO.OrderType;

            ExportTemplateFilter.Id = ExportTemplate_ExportTemplateFilterDTO.Id;
            ExportTemplateFilter.Code = ExportTemplate_ExportTemplateFilterDTO.Code;
            ExportTemplateFilter.Name = ExportTemplate_ExportTemplateFilterDTO.Name;
            return ExportTemplateFilter;
        }
    }
}

