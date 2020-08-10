using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MProblemType;

namespace DMS.Rpc.problem_type
{
    public partial class ProblemTypeController : RpcController
    {
        private IProblemTypeService ProblemTypeService;
        private ICurrentContext CurrentContext;
        public ProblemTypeController(
            IProblemTypeService ProblemTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.ProblemTypeService = ProblemTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProblemTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ProblemType_ProblemTypeFilterDTO ProblemType_ProblemTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = ConvertFilterDTOToFilterEntity(ProblemType_ProblemTypeFilterDTO);
            int count = await ProblemTypeService.Count(ProblemTypeFilter);
            return count;
        }

        [Route(ProblemTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<ProblemType_ProblemTypeDTO>>> List([FromBody] ProblemType_ProblemTypeFilterDTO ProblemType_ProblemTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = ConvertFilterDTOToFilterEntity(ProblemType_ProblemTypeFilterDTO);
            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<ProblemType_ProblemTypeDTO> ProblemType_ProblemTypeDTOs = ProblemTypes
                .Select(c => new ProblemType_ProblemTypeDTO(c)).ToList();
            return ProblemType_ProblemTypeDTOs;
        }


        private ProblemTypeFilter ConvertFilterDTOToFilterEntity(ProblemType_ProblemTypeFilterDTO ProblemType_ProblemTypeFilterDTO)
        {
            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;
            ProblemTypeFilter.Skip = ProblemType_ProblemTypeFilterDTO.Skip;
            ProblemTypeFilter.Take = ProblemType_ProblemTypeFilterDTO.Take;
            ProblemTypeFilter.OrderBy = ProblemType_ProblemTypeFilterDTO.OrderBy;
            ProblemTypeFilter.OrderType = ProblemType_ProblemTypeFilterDTO.OrderType;

            ProblemTypeFilter.Id = ProblemType_ProblemTypeFilterDTO.Id;
            ProblemTypeFilter.Code = ProblemType_ProblemTypeFilterDTO.Code;
            ProblemTypeFilter.Name = ProblemType_ProblemTypeFilterDTO.Name;
            ProblemTypeFilter.StatusId = ProblemType_ProblemTypeFilterDTO.StatusId;
            ProblemTypeFilter.CreatedAt = ProblemType_ProblemTypeFilterDTO.CreatedAt;
            ProblemTypeFilter.UpdatedAt = ProblemType_ProblemTypeFilterDTO.UpdatedAt;
            return ProblemTypeFilter;
        }
    }
}

