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
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MStatus;

namespace DMS.Rpc.survey
{
    public class SurveyController : RpcController
    {
        private ISurveyQuestionTypeService SurveyQuestionTypeService;
        private ISurveyOptionTypeService SurveyOptionTypeService;
        private ISurveyService SurveyService;
        private IAppUserService AppUserService;
        private IStatusService StatusService;
        private ICurrentContext CurrentContext;
        public SurveyController(
            ISurveyQuestionTypeService SurveyQuestionTypeService,
            ISurveyOptionTypeService SurveyOptionTypeService,
            ISurveyService SurveyService,
            IAppUserService AppUserService,
            IStatusService StatusService,
            ICurrentContext CurrentContext
        )
        {
            this.SurveyOptionTypeService = SurveyOptionTypeService;
            this.SurveyQuestionTypeService = SurveyQuestionTypeService;
            this.SurveyService = SurveyService;
            this.AppUserService = AppUserService;
            this.StatusService = StatusService;
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

        //[Route(SurveyRoute.Export), HttpPost]
        //public async Task<FileResult> Export([FromBody] Survey_SurveyFilterDTO Survey_SurveyFilterDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    MemoryStream memoryStream = new MemoryStream();
        //    using (ExcelPackage excel = new ExcelPackage(memoryStream))
        //    {
        //        #region Survey
        //        Survey Survey = await SurveyService.GetResult(Survey_SurveyFilterDTO.Id.Equal ?? 0);
        //        if (Survey == null)
        //            return null;

        //        List<Survey_SurveyResultSingleDTO> QUESTION_SINGLE_CHOICEs = Survey.SurveyQuestions
        //            .Where(x => x.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
        //            .SelectMany(x => x.SurveyResultSingles)
        //            .Select(x => new Survey_SurveyResultSingleDTO(x)).ToList();

        //        List<Survey_SurveyResultSingleDTO> QUESTION_MULTIPLE_CHOICEs = Survey.SurveyQuestions
        //            .Where(x => x.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id)
        //            .SelectMany(x => x.SurveyResultSingles)
        //            .Select(x => new Survey_SurveyResultSingleDTO(x)).ToList();

        //        List<Survey_SurveyResultCellDTO> TABLE_SINGLE_CHOICEs = Survey.SurveyQuestions
        //            .Where(x => x.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
        //            .SelectMany(x => x.SurveyResultCells)
        //            .Select(x => new Survey_SurveyResultCellDTO(x)).ToList();

        //        List<Survey_SurveyResultCellDTO> TABLE_MULTIPLE_CHOICEs = Survey.SurveyQuestions
        //            .Where(x => x.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id)
        //            .SelectMany(x => x.SurveyResultCells)
        //            .Select(x => new Survey_SurveyResultCellDTO(x)).ToList();
        //        var CellAppUserIds = Survey.SurveyQuestions.SelectMany( sq => sq.SurveyResultCells.Select(x => x.AppUserId)).ToList();
        //        var SingleAppUserIds = Survey.SurveyQuestions.SelectMany( sq => sq.SurveyResultSingles.Select(x => x.AppUserId)).ToList();
        //        Dictionary<AppUser, Dictionary<string, List<string>>> Results = new Dictionary<AppUser, Dictionary<string, List<string>>>();
        //        foreach (var SurveyQuestion in Survey.SurveyQuestions)
        //        {
        //            if(SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
        //            {
        //                var SurveyResultCells = SurveyQuestion.SurveyResultCells.Where(x => x.SurveyQuestionId == SurveyQuestion.Id).ToList();
                        
        //                Dictionary<string, List<string>> questionResult = new Dictionary<string, List<string>>();
        //                Results.Add()
        //                foreach (var AppUserId in AppUserIds)
        //                {
        //                    List<SurveyOption> surveyOptions = SurveyQuestion.SurveyOptions
        //                        .Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();

        //                }
                        
                        
        //                foreach (var SurveyResultCell in SurveyResultCells)
        //                {
        //                    questionResult.Add(SurveyResultCell.RowOption.Content, SurveyResultCell.)
        //                    Results.Add(SurveyResultCell.AppUser, )
        //                }
        //            }
        //        }

        //        var SurveyResultHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Thời gian",
        //                "Cửa hàng",
        //                "Mã cửa hàng",
        //                "Nhân viên",
        //            }
        //        };
        //        List<object[]> SurveyData = new List<object[]>();
        //        for (int i = 0; i < Surveys.Count; i++)
        //        {
        //            var Survey = Surveys[i];
        //            SurveyData.Add(new Object[]
        //            {
        //                Survey.Id,
        //                Survey.Title,
        //                Survey.Description,
        //                Survey.StartAt,
        //                Survey.EndAt,
        //                Survey.StatusId,
        //                Survey.CreatorId,
        //            });
        //        }
        //        excel.GenerateWorksheet("Survey", SurveyHeaders, SurveyData);
        //        #endregion
        //        excel.Save();
        //    }
        //    return File(memoryStream.ToArray(), "application/octet-stream", "Survey.xlsx");
        //}

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
                    SurveyOptions = x.SurveyOptions?.Select(x => new SurveyOption
                    {
                        Id = x.Id,
                        Content = x.Content,
                        SurveyOptionTypeId = x.SurveyOptionTypeId,
                        SurveyQuestionId = x.SurveyOptionTypeId
                    }).ToList(),
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

        [Route(SurveyRoute.FilterListAppUser), HttpPost]
        public async Task<List<Survey_AppUserDTO>> FilterListAppUser([FromBody] Survey_AppUserFilterDTO Survey_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = Survey_AppUserFilterDTO.Id;
            AppUserFilter.Username = Survey_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Survey_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Survey_AppUserDTO> Survey_AppUserDTOs = AppUsers
                .Select(x => new Survey_AppUserDTO(x)).ToList();
            return Survey_AppUserDTOs;
        }
        [Route(SurveyRoute.FilterListStatus), HttpPost]
        public async Task<List<Survey_StatusDTO>> FilterListStatus([FromBody] Survey_StatusFilterDTO Survey_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Survey_StatusDTO> Survey_StatusDTOs = Statuses
                .Select(x => new Survey_StatusDTO(x)).ToList();
            return Survey_StatusDTOs;
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
        [Route(SurveyRoute.SingleListAppUser), HttpPost]
        public async Task<List<Survey_AppUserDTO>> SingleListAppUser([FromBody] Survey_AppUserFilterDTO Survey_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = Survey_AppUserFilterDTO.Id;
            AppUserFilter.Username = Survey_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Survey_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Survey_AppUserDTO> Survey_AppUserDTOs = AppUsers
                .Select(x => new Survey_AppUserDTO(x)).ToList();
            return Survey_AppUserDTOs;
        }
        [Route(SurveyRoute.SingleListStatus), HttpPost]
        public async Task<List<Survey_StatusDTO>> SingleListStatus([FromBody] Survey_StatusFilterDTO Survey_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Survey_StatusDTO> Survey_StatusDTOs = Statuses
                .Select(x => new Survey_StatusDTO(x)).ToList();
            return Survey_StatusDTOs;
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

