using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MStatus;
using DMS.Services.MSurvey;
using DMS.Services.MSurveyResult;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Rpc.survey
{
    public class SurveyController : RpcController
    {
        private ISurveyQuestionTypeService SurveyQuestionTypeService;
        private ISurveyOptionTypeService SurveyOptionTypeService;
        private ISurveyService SurveyService;
        private ISurveyResultService SurveyResultService;
        private IAppUserService AppUserService;
        private IStatusService StatusService;
        private ICurrentContext CurrentContext;
        public SurveyController(
            ISurveyQuestionTypeService SurveyQuestionTypeService,
            ISurveyOptionTypeService SurveyOptionTypeService,
            ISurveyService SurveyService,
            ISurveyResultService SurveyResultService,
            IAppUserService AppUserService,
            IStatusService StatusService,
            ICurrentContext CurrentContext
        )
        {
            this.SurveyOptionTypeService = SurveyOptionTypeService;
            this.SurveyQuestionTypeService = SurveyQuestionTypeService;
            this.SurveyService = SurveyService;
            this.SurveyResultService = SurveyResultService;
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

        [Route(SurveyRoute.GetSurveyForm), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> GetSurveyForm([FromBody] Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (!await HasPermission(Survey_SurveyDTO.Id))
            //    return Forbid();

            Survey Survey = await SurveyService.GetForm(Survey_SurveyDTO.Id);
            Survey_SurveyDTO = new Survey_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Survey_SurveyDTO;
            else
                return BadRequest(Survey_SurveyDTO);
        }

        [Route(SurveyRoute.SaveSurveyForm), HttpPost]
        public async Task<ActionResult<Survey_SurveyDTO>> SaveSurveyForm([FromBody] Survey_SurveyDTO Survey_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Survey_SurveyDTO.Id))
                return Forbid();

            Survey Survey = ConvertDTOToEntity(Survey_SurveyDTO);
            Survey = await SurveyService.SaveForm(Survey);
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

        [Route(SurveyRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Survey_SurveyFilterDTO Survey_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SurveyId = Survey_SurveyFilterDTO.Id?.Equal ?? 0;
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Survey
                Survey Survey = await SurveyService.Get(SurveyId);
                if (Survey == null)
                    return null;
                List<SurveyResult> SurveyResults = await SurveyResultService.List(new SurveyResultFilter
                {
                    SurveyId = new IdFilter { Equal = SurveyId },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = SurveyResultSelect.ALL,
                });

                List<string> header = new List<string>
                {
                    "Thời gian",
                    "Đại lý",
                    "Mã đại lý",
                    "Nhân viên",
                };

                if (Survey.SurveyQuestions != null)
                    foreach (SurveyQuestion surveyQuestion in Survey.SurveyQuestions)
                    {
                        if (surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                            surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                        {
                            header.Add($"{surveyQuestion.Content}");
                        }
                        if (surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                            surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                        {
                            List<SurveyOption> SurveyOptions = surveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                            foreach (SurveyOption SurveyOption in SurveyOptions)
                            {
                                header.Add($"{surveyQuestion.Content} [{SurveyOption.Content}]");
                            }
                        }
                    }

                List<long> AppUserIds = SurveyResults.Select(sr => sr.AppUserId).ToList();
                List<AppUser> appUsers = await AppUserService.List(new AppUserFilter
                {
                    Id = new IdFilter { In = AppUserIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id | AppUserSelect.DisplayName,
                });
                List<List<string>> result = new List<List<string>>();
                foreach (long AppUserId in AppUserIds)
                {
                    List<string> optionResults = new List<string>();
                    result.Add(optionResults);
                    SurveyResult SurveyResult = SurveyResults.Where(sr => sr.AppUserId == AppUserId && sr.SurveyId == SurveyId).FirstOrDefault();
                    if (SurveyResult == null)
                        continue;

                    string AppUserName = appUsers.Where(a => a.Id == AppUserId).Select(a => a.DisplayName).FirstOrDefault();
                    string Time = SurveyResult.Time.ToString("dd/MM/yyyy");
                    string StoreCode = SurveyResult.Store.Code;
                    string StoreName = SurveyResult.Store.Name;
                    optionResults.Add(Time);
                    optionResults.Add(StoreName);
                    optionResults.Add(StoreCode);
                    optionResults.Add(AppUserName);
                    if (Survey.SurveyQuestions != null)
                        foreach (SurveyQuestion surveyQuestion in Survey.SurveyQuestions)
                        {

                            if (surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                            surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                            {
                                List<SurveyResultSingle> SurveyResultSingles = SurveyResult.SurveyResultSingles.Where(sr => sr.SurveyQuestionId == surveyQuestion.Id).ToList();
                                foreach (SurveyResultSingle SurveyResultSingle in SurveyResultSingles)
                                {
                                    SurveyOption SurveyOption = surveyQuestion.SurveyOptions.Where(so => so.Id == SurveyResultSingle.SurveyOptionId).FirstOrDefault();
                                    if (SurveyOption != null)
                                        optionResults.Add(SurveyOption.Content);
                                }

                            }
                            if (surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                                surveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                            {
                                List<SurveyOption> RowOptions = surveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                                foreach (SurveyOption RowOption in RowOptions)
                                {
                                    List<SurveyResultCell> SurveyResultCells = SurveyResult.SurveyResultCells.Where(sr => sr.SurveyQuestionId == surveyQuestion.Id && sr.RowOptionId == RowOption.Id).ToList();
                                    foreach (SurveyResultCell SurveyResultCell in SurveyResultCells)
                                    {
                                        SurveyOption ColumnOption = surveyQuestion.SurveyOptions.Where(so => so.Id == SurveyResultCell.ColumnOptionId).FirstOrDefault();
                                        if (ColumnOption != null)
                                            optionResults.Add(ColumnOption.Content);
                                    }
                                }
                            }
                        }
                }

                var SurveyResultHeaders = new List<string[]>()
                {
                    header.ToArray(),
                };
                List<Object[]> SurveyData = new List<object[]>();
                foreach (var list in result)
                {
                    Object[] obj = list.ToArray();
                    SurveyData.Add(obj);
                }
                excel.GenerateWorksheet("Survey", SurveyResultHeaders, SurveyData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Survey.xlsx");
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
            Survey.StoreId = Survey_SurveyDTO.StoreId;
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
                    TableResult = x.TableResult,
                    ListResult = x.ListResult,
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
            SurveyFilter.CreatorId = Survey_SurveyFilterDTO.CreatorId;
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

