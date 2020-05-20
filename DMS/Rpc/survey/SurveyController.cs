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
using DMS.Services.MSurvey;

namespace DMS.Rpc.survey
{
    public class SurveyRoute : Root
    {
        public const string Master = Module + "/survey/survey-master";
        public const string Detail = Module + "/survey/survey-detail";
        private const string Default = Rpc + Module + "/survey";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        
        public const string FilterListSurveyOptionType = Default + "/filter-list-survey-option-type";
        public const string FilterListSurveyQuestionType = Default + "/filter-list-survey-question-type";
        public const string SingleListSurveyOptionType = Default + "/single-list-survey-option-type";
        public const string SingleListSurveyQuestionType = Default + "/single-list-survey-question-type";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(SurveyFilter.Title), FieldType.STRING },
            { nameof(SurveyFilter.Description), FieldType.STRING },
            { nameof(SurveyFilter.StartAt), FieldType.DATE },
            { nameof(SurveyFilter.EndAt), FieldType.DATE },
        };
    }

    public class SurveyController : RpcController
    {
        private ISurveyQuestionTypeService SurveyQuestionTypeService;
        private ISurveyOptionTypeService SurveyOptionTypeService;
        private ISurveyService SurveyService;
        private ICurrentContext CurrentContext;
        public SurveyController(
            ISurveyQuestionTypeService SurveyQuestionTypeService,
            ISurveyOptionTypeService SurveyOptionTypeService,
            ISurveyService SurveyService,
            ICurrentContext CurrentContext
        )
        {
            this.SurveyOptionTypeService = SurveyOptionTypeService;
            this.SurveyQuestionTypeService = SurveyQuestionTypeService;
            this.SurveyService = SurveyService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SurveyRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Survey_SurveyFilterDTO Survey_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyFilter SurveyFilter = ConvertFilterDTOToFilterEntity(Survey_SurveyFilterDTO);
            SurveyFilter = SurveyService.ToFilter(SurveyFilter);
            int count = await SurveyService.Count(SurveyFilter);
            return count;
        }

        [Route(SurveyRoute.List), HttpPost]
        public async Task<ActionResult<List<Survey_SurveyDTO>>> List([FromBody] Survey_SurveyFilterDTO Survey_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyFilter SurveyFilter = ConvertFilterDTOToFilterEntity(Survey_SurveyFilterDTO);
            SurveyFilter = SurveyService.ToFilter(SurveyFilter);
            List<Survey> Surveys = await SurveyService.List(SurveyFilter);
            List<Survey_SurveyDTO> Survey_SurveyDTOs = Surveys
                .Select(c => new Survey_SurveyDTO(c)).ToList();
            return Survey_SurveyDTOs;
        }

        [Route(SurveyRoute.Get), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> Get([FromBody]Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Survey_SurveyDTO.Id))
                return Forbid();

            Survey Survey = await SurveyService.Get(Survey_SurveyDTO.Id);
            return new Survey_SurveyDTO(Survey);
        }

        [Route(SurveyRoute.Create), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> Create([FromBody] Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Survey_SurveyDTO.Id))
                return Forbid();

            Survey Survey = ConvertDTOToEntity(Survey_SurveyDTO);
            Survey = await SurveyService.Create(Survey);
            Survey_SurveyDTO = new Survey_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Survey_SurveyDTO;
            else
                return BadRequest(Survey_SurveyDTO);
        }

        [Route(SurveyRoute.Update), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> Update([FromBody] Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Survey_SurveyDTO.Id))
                return Forbid();

            Survey Survey = ConvertDTOToEntity(Survey_SurveyDTO);
            Survey = await SurveyService.Update(Survey);
            Survey_SurveyDTO = new Survey_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Survey_SurveyDTO;
            else
                return BadRequest(Survey_SurveyDTO);
        }

        [Route(SurveyRoute.Delete), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> Delete([FromBody] Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Survey_SurveyDTO.Id))
                return Forbid();

            Survey Survey = ConvertDTOToEntity(Survey_SurveyDTO);
            Survey = await SurveyService.Delete(Survey);
            Survey_SurveyDTO = new Survey_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Survey_SurveyDTO;
            else
                return BadRequest(Survey_SurveyDTO);
        }
        

        private async Task<bool> HasPermission(long Id)
        {
            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter = SurveyService.ToFilter(SurveyFilter);
            if (Id == 0)
            {

            }
            else
            {
                SurveyFilter.Id = new IdFilter { Equal = Id };
                int count = await SurveyService.Count(SurveyFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Survey ConvertDTOToEntity(Survey_SurveyDTO Survey_SurveyDTO)
        {
            Survey Survey = new Survey();
            Survey.Id = Survey_SurveyDTO.Id;
            Survey.Title = Survey_SurveyDTO.Title;
            Survey.Description = Survey_SurveyDTO.Description;
            Survey.StartAt = Survey_SurveyDTO.StartAt;
            Survey.EndAt = Survey_SurveyDTO.EndAt;
            Survey.StatusId = Survey_SurveyDTO.StatusId;
            Survey.SurveyQuestions = Survey_SurveyDTO.SurveyQuestions?
                .Select(x => new SurveyQuestion
                {
                    Id = x.Id,
                    Content = x.Content,
                    SurveyQuestionTypeId = x.SurveyQuestionTypeId,
                    IsMandatory = x.IsMandatory,
                    SurveyQuestionType = x.SurveyQuestionType == null ? null : new SurveyQuestionType
                    {
                        Id = x.SurveyQuestionType.Id,
                        Code = x.SurveyQuestionType.Code,
                        Name = x.SurveyQuestionType.Name,
                    },
                }).ToList();
            Survey.BaseLanguage = CurrentContext.Language;
            return Survey;
        }

        private SurveyFilter ConvertFilterDTOToFilterEntity(Survey_SurveyFilterDTO Survey_SurveyFilterDTO)
        {
            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = Survey_SurveyFilterDTO.Skip;
            SurveyFilter.Take = Survey_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = Survey_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = Survey_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = Survey_SurveyFilterDTO.Id;
            SurveyFilter.Title = Survey_SurveyFilterDTO.Title;
            SurveyFilter.Description = Survey_SurveyFilterDTO.Description;
            SurveyFilter.StartAt = Survey_SurveyFilterDTO.StartAt;
            SurveyFilter.EndAt = Survey_SurveyFilterDTO.EndAt;
            SurveyFilter.StatusId = Survey_SurveyFilterDTO.StatusId;
            SurveyFilter.CreatedAt = Survey_SurveyFilterDTO.CreatedAt;
            SurveyFilter.UpdatedAt = Survey_SurveyFilterDTO.UpdatedAt;
            return SurveyFilter;
        }

       
        [Route(SurveyRoute.FilterListSurveyQuestionType), HttpPost]
        public async Task<List<Survey_SurveyQuestionTypeDTO>> FilterListSurveyQuestionType([FromBody] Survey_SurveyQuestionTypeFilterDTO Survey_SurveyQuestionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyQuestionTypeFilter SurveyQuestionTypeFilter = new SurveyQuestionTypeFilter();
            SurveyQuestionTypeFilter.Skip = 0;
            SurveyQuestionTypeFilter.Take = int.MaxValue;
            SurveyQuestionTypeFilter.Take = 20;
            SurveyQuestionTypeFilter.OrderBy = SurveyQuestionTypeOrder.Id;
            SurveyQuestionTypeFilter.OrderType = OrderType.ASC;
            SurveyQuestionTypeFilter.Selects = SurveyQuestionTypeSelect.ALL;

            List<SurveyQuestionType> SurveyQuestionTypes = await SurveyQuestionTypeService.List(SurveyQuestionTypeFilter);
            List<Survey_SurveyQuestionTypeDTO> Survey_SurveyQuestionTypeDTOs = SurveyQuestionTypes
                .Select(x => new Survey_SurveyQuestionTypeDTO(x)).ToList();
            return Survey_SurveyQuestionTypeDTOs;
        }

        [Route(SurveyRoute.SingleListSurveyQuestionType), HttpPost]
        public async Task<List<Survey_SurveyQuestionTypeDTO>> SingleListSurveyQuestionType([FromBody] Survey_SurveyQuestionTypeFilterDTO Survey_SurveyQuestionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyQuestionTypeFilter SurveyQuestionTypeFilter = new SurveyQuestionTypeFilter();
            SurveyQuestionTypeFilter.Skip = 0;
            SurveyQuestionTypeFilter.Take = int.MaxValue;
            SurveyQuestionTypeFilter.Take = 20;
            SurveyQuestionTypeFilter.OrderBy = SurveyQuestionTypeOrder.Id;
            SurveyQuestionTypeFilter.OrderType = OrderType.ASC;
            SurveyQuestionTypeFilter.Selects = SurveyQuestionTypeSelect.ALL;

            List<SurveyQuestionType> SurveyQuestionTypes = await SurveyQuestionTypeService.List(SurveyQuestionTypeFilter);
            List<Survey_SurveyQuestionTypeDTO> Survey_SurveyQuestionTypeDTOs = SurveyQuestionTypes
                .Select(x => new Survey_SurveyQuestionTypeDTO(x)).ToList();
            return Survey_SurveyQuestionTypeDTOs;
        }


        [Route(SurveyRoute.FilterListSurveyOptionType), HttpPost]
        public async Task<List<Survey_SurveyOptionTypeDTO>> FilterListSurveyOptionType([FromBody] Survey_SurveyOptionTypeFilterDTO Survey_SurveyOptionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyOptionTypeFilter SurveyOptionTypeFilter = new SurveyOptionTypeFilter();
            SurveyOptionTypeFilter.Skip = 0;
            SurveyOptionTypeFilter.Take = int.MaxValue;
            SurveyOptionTypeFilter.Take = 20;
            SurveyOptionTypeFilter.OrderBy = SurveyOptionTypeOrder.Id;
            SurveyOptionTypeFilter.OrderType = OrderType.ASC;
            SurveyOptionTypeFilter.Selects = SurveyOptionTypeSelect.ALL;

            List<SurveyOptionType> SurveyOptionTypes = await SurveyOptionTypeService.List(SurveyOptionTypeFilter);
            List<Survey_SurveyOptionTypeDTO> Survey_SurveyOptionTypeDTOs = SurveyOptionTypes
                .Select(x => new Survey_SurveyOptionTypeDTO(x)).ToList();
            return Survey_SurveyOptionTypeDTOs;
        }

        [Route(SurveyRoute.SingleListSurveyOptionType), HttpPost]
        public async Task<List<Survey_SurveyOptionTypeDTO>> SingleListSurveyOptionType([FromBody] Survey_SurveyOptionTypeFilterDTO Survey_SurveyOptionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SurveyOptionTypeFilter SurveyOptionTypeFilter = new SurveyOptionTypeFilter();
            SurveyOptionTypeFilter.Skip = 0;
            SurveyOptionTypeFilter.Take = int.MaxValue;
            SurveyOptionTypeFilter.Take = 20;
            SurveyOptionTypeFilter.OrderBy = SurveyOptionTypeOrder.Id;
            SurveyOptionTypeFilter.OrderType = OrderType.ASC;
            SurveyOptionTypeFilter.Selects = SurveyOptionTypeSelect.ALL;

            List<SurveyOptionType> SurveyOptionTypes = await SurveyOptionTypeService.List(SurveyOptionTypeFilter);
            List<Survey_SurveyOptionTypeDTO> Survey_SurveyOptionTypeDTOs = SurveyOptionTypes
                .Select(x => new Survey_SurveyOptionTypeDTO(x)).ToList();
            return Survey_SurveyOptionTypeDTOs;
        }

    }
}

