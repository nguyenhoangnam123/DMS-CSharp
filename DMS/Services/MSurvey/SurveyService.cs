using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Rpc.survey;
using DMS.Services.MNotification;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Handlers;
using System.IO;
using RestSharp;

namespace DMS.Services.MSurvey
{
    public interface ISurveyService : IServiceScoped
    {
        Task<int> Count(SurveyFilter SurveyFilter);
        Task<List<Survey>> List(SurveyFilter SurveyFilter);
        Task<Survey> Get(long Id);
        Task<Survey> Create(Survey Survey);
        Task<Survey> Update(Survey Survey);
        Task<Survey> Delete(Survey Survey);
        Task<Survey> GetForm(long Id);
        Task<Survey> SaveForm(Survey Survey);
        Task<DMS.Entities.File> SaveFile(DMS.Entities.File File);
        SurveyFilter ToFilter(SurveyFilter SurveyFilter);
    }

    public class SurveyService : BaseService, ISurveyService
    {
        private IUOW UOW;
        private ILogging Logging;
        private INotificationService NotificationService;
        private ICurrentContext CurrentContext;
        private IRabbitManager RabbitManager;
        private ISurveyValidator SurveyValidator;

        public SurveyService(
            IUOW UOW,
            ILogging Logging,
            INotificationService NotificationService,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ISurveyValidator SurveyValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.NotificationService = NotificationService;
            this.CurrentContext = CurrentContext;
            this.RabbitManager = RabbitManager;
            this.SurveyValidator = SurveyValidator;
        }
        public async Task<int> Count(SurveyFilter SurveyFilter)
        {
            try
            {
                int result = await UOW.SurveyRepository.Count(SurveyFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Survey>> List(SurveyFilter SurveyFilter)
        {
            try
            {
                List<Survey> Surveys = await UOW.SurveyRepository.List(SurveyFilter);
                return Surveys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Survey> Get(long Id)
        {
            Survey Survey = await UOW.SurveyRepository.Get(Id);
            if (Survey != null)
            {
                SurveyResultFilter SurveyResultFilter = new SurveyResultFilter
                {
                    SurveyId = new IdFilter { Equal = Survey.Id }
                };

                Survey.ResultCounter = await UOW.SurveyResultRepository.Count(SurveyResultFilter);
            }
            return Survey;
        }

        public async Task<Survey> Create(Survey Survey)
        {
            if (!await SurveyValidator.Create(Survey))
                return Survey;

            try
            {
                Survey.CreatorId = CurrentContext.UserId;
                await UOW.Begin();
                await UOW.SurveyRepository.Create(Survey);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                var RecipientIds = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { }
                })).Select(x => x.Id).ToList();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Khảo sát {Survey.Title} đã được thêm mới lên hệ thống bởi {CurrentUser.DisplayName}, có hiệu lực từ {Survey.StartAt} - {Survey.EndAt}",
                        LinkWebsite = $"{SurveyRoute.Master}#*".Replace("*", Survey.Id.ToString()),
                        LinkMobile = $"{SurveyRoute.Mobile}".Replace("*", Survey.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                List<EventMessage<UserNotification>> EventUserNotifications = UserNotifications.Select(x => new EventMessage<UserNotification>(x, x.RowId)).ToList();
                RabbitManager.PublishList(EventUserNotifications, RoutingKeyEnum.UserNotificationSend);

                await Logging.CreateAuditLog(Survey, new { }, nameof(SurveyService));
                return await UOW.SurveyRepository.Get(Survey.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Survey> Update(Survey Survey)
        {
            if (!await SurveyValidator.Update(Survey))
                return Survey;
            try
            {
                var oldData = await UOW.SurveyRepository.Get(Survey.Id);

                await UOW.Begin();
                await UOW.SurveyRepository.Update(Survey);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                var RecipientIds = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { }
                })).Select(x => x.Id).ToList();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Khảo sát {Survey.Title} đã được cập nhật thông tin bởi {CurrentUser.DisplayName}, có hiệu lực từ {Survey.StartAt} - {Survey.EndAt}",
                        LinkWebsite = $"{SurveyRoute.Master}#*".Replace("*", Survey.Id.ToString()),
                        LinkMobile = $"{SurveyRoute.Mobile}".Replace("*", Survey.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                List<EventMessage<UserNotification>> EventUserNotifications = UserNotifications.Select(x => new EventMessage<UserNotification>(x, x.RowId)).ToList();
                RabbitManager.PublishList(EventUserNotifications, RoutingKeyEnum.UserNotificationSend);

                var newData = await UOW.SurveyRepository.Get(Survey.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(SurveyService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Survey> Delete(Survey Survey)
        {
            if (!await SurveyValidator.Delete(Survey))
                return Survey;

            try
            {
                await UOW.Begin();
                await UOW.SurveyRepository.Delete(Survey);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                var RecipientIds = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { }
                })).Select(x => x.Id).ToList();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Khảo sát {Survey.Title} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName}.",
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                List<EventMessage<UserNotification>> EventUserNotifications = UserNotifications.Select(x => new EventMessage<UserNotification>(x, x.RowId)).ToList();
                RabbitManager.PublishList(EventUserNotifications, RoutingKeyEnum.UserNotificationSend);

                await Logging.CreateAuditLog(new { }, Survey, nameof(SurveyService));
                return Survey;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public SurveyFilter ToFilter(SurveyFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SurveyFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SurveyFilter subFilter = new SurveyFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }

        public async Task<Survey> GetForm(long Id)
        {
            try
            {
                Survey Survey = await UOW.SurveyRepository.Get(Id);
                if (Survey.SurveyQuestions != null)
                {
                    foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                    {
                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                            SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                        {
                            SurveyQuestion.ListResult = new Dictionary<long, bool>();
                            if (SurveyQuestion.SurveyOptions != null)
                            {
                                foreach (SurveyOption SurveyOption in SurveyQuestion.SurveyOptions)
                                {
                                    SurveyQuestion.ListResult.Add(SurveyOption.Id, false);
                                }
                            }
                        }
                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                            SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                        {
                            SurveyQuestion.TableResult = new Dictionary<long, Dictionary<long, bool>>();
                            if (SurveyQuestion.SurveyOptions != null)
                            {
                                List<SurveyOption> Columns = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.COLUMN.Id).ToList();
                                List<SurveyOption> Rows = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                                foreach (SurveyOption Row in Rows)
                                {
                                    Dictionary<long, bool> RowResult = new Dictionary<long, bool>();
                                    SurveyQuestion.TableResult.Add(Row.Id, RowResult);
                                    foreach (SurveyOption Column in Columns)
                                    {
                                        RowResult.Add(Column.Id, false);
                                    }
                                }
                            }
                        }
                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_TEXT.Id)
                        {
                            SurveyQuestion.TextResult = "";
                        }    
                    }
                }
                return Survey;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Survey> SaveForm(Survey Survey)
        {
            if (!await SurveyValidator.SaveForm(Survey))
                return Survey;

            try
            {
                AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                SurveyResult SurveyResult = new SurveyResult();
                SurveyResult.SurveyId = Survey.Id;
                SurveyResult.AppUserId = CurrentContext.UserId;
                SurveyResult.OrganizationId = AppUser.OrganizationId;
                SurveyResult.StoreId = Survey.StoreId;
                SurveyResult.StoreScoutingId = Survey.StoreScoutingId;
                SurveyResult.SurveyRespondentTypeId = Survey.SurveyRespondentTypeId;
                SurveyResult.Time = StaticParams.DateTimeNow;
                SurveyResult.RespondentAddress = Survey.RespondentAddress;
                SurveyResult.RespondentEmail = Survey.RespondentEmail;
                SurveyResult.RespondentName = Survey.RespondentName;
                SurveyResult.RespondentPhone = Survey.RespondentPhone;
               
                SurveyResult.SurveyResultSingles = new List<SurveyResultSingle>();
                SurveyResult.SurveyResultCells = new List<SurveyResultCell>();
                SurveyResult.SurveyResultTexts = new List<SurveyResultText>();
                if (Survey.SurveyQuestions != null)
                {
                    foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                    {
                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                            SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                        {
                            foreach (SurveyOption SurveyOption in SurveyQuestion.SurveyOptions)
                            {
                                if (SurveyQuestion.ListResult.ContainsKey(SurveyOption.Id) && SurveyQuestion.ListResult[SurveyOption.Id])
                                {
                                    SurveyResultSingle SurveyResultSingle = new SurveyResultSingle
                                    {
                                        SurveyOptionId = SurveyOption.Id,
                                        SurveyQuestionId = SurveyQuestion.Id,
                                    };
                                    SurveyResult.SurveyResultSingles.Add(SurveyResultSingle);
                                }
                            }
                        }
                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                            SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                        {
                            if (SurveyQuestion.SurveyOptions != null)
                            {
                                List<SurveyOption> Columns = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.COLUMN.Id).ToList();
                                List<SurveyOption> Rows = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                                foreach (SurveyOption Row in Rows)
                                {
                                    if (SurveyQuestion.TableResult.ContainsKey(Row.Id))
                                    {
                                        Dictionary<long, bool> ColumnResult = SurveyQuestion.TableResult[Row.Id];
                                        foreach (SurveyOption Column in Columns)
                                        {
                                            if (ColumnResult.ContainsKey(Column.Id) && ColumnResult[Column.Id])
                                            {
                                                SurveyResultCell SurveyResultCell = new SurveyResultCell
                                                {
                                                    SurveyQuestionId = SurveyQuestion.Id,
                                                    ColumnOptionId = Column.Id,
                                                    RowOptionId = Row.Id,
                                                };
                                                SurveyResult.SurveyResultCells.Add(SurveyResultCell);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_TEXT.Id)
                        {
                            SurveyResultText SurveyResultText = new SurveyResultText
                            {
                                SurveyQuestionId = SurveyQuestion.Id,
                                Content = SurveyQuestion.TextResult,
                            };
                            SurveyResult.SurveyResultTexts.Add(SurveyResultText);
                        }
                    }
                }
                await UOW.Begin();
                await UOW.SurveyResultRepository.Create(SurveyResult);
                await UOW.Commit();
                return await GetForm(Survey.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DMS.Entities.File> SaveFile(DMS.Entities.File File)
        {
            FileInfo fileInfo = new FileInfo(File.Name);
            string path = $"/category/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";

            RestClient restClient = new RestClient(InternalServices.UTILS);
            RestRequest restRequest = new RestRequest("/rpc/utils/file/upload");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddFile("file", File.Content, File.Name);
            restRequest.AddParameter("path", path);
            try
            {
                var response = restClient.Execute<DMS.Entities.File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    File.Id = response.Data.Id;
                    File.Path = "/rpc/utils/file/download" + response.Data.Path;
                }
                return File;
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}
