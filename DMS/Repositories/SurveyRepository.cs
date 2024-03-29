using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface ISurveyRepository
    {
        Task<int> Count(SurveyFilter SurveyFilter);
        Task<List<Survey>> List(SurveyFilter SurveyFilter);
        Task<Survey> Get(long Id);
        Task<bool> Create(Survey Survey);
        Task<bool> Update(Survey Survey);
        Task<bool> Delete(Survey Survey);
        Task<bool> Used(List<long> Ids);
    }
    public class SurveyRepository : ISurveyRepository
    {
        private DataContext DataContext;
        public SurveyRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SurveyDAO> DynamicFilter(IQueryable<SurveyDAO> query, SurveyFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Title != null)
                query = query.Where(q => q.Title, filter.Title);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.StartAt != null)
                query = query.Where(q => q.StartAt, filter.StartAt);
            if (filter.EndAt != null)
                query = query.Where(q => q.EndAt.HasValue == false).Union(query.Where(q => q.EndAt.HasValue).Where(q => q.EndAt.Value, filter.EndAt));
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<SurveyDAO> OrFilter(IQueryable<SurveyDAO> query, SurveyFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SurveyDAO> initQuery = query.Where(q => false);
            foreach (SurveyFilter SurveyFilter in filter.OrFilter)
            {
                IQueryable<SurveyDAO> queryable = query;
                if (SurveyFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, SurveyFilter.Id);
                if (SurveyFilter.Title != null)
                    queryable = queryable.Where(q => q.Title, SurveyFilter.Title);
                if (SurveyFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, SurveyFilter.Description);
                if (SurveyFilter.StartAt != null)
                    queryable = queryable.Where(q => q.StartAt, SurveyFilter.StartAt);
                if (SurveyFilter.EndAt != null)
                    queryable = queryable.Where(q => q.EndAt, SurveyFilter.EndAt);
                if (SurveyFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, SurveyFilter.StatusId);
                if (SurveyFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, SurveyFilter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<SurveyDAO> DynamicOrder(IQueryable<SurveyDAO> query, SurveyFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SurveyOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SurveyOrder.Title:
                            query = query.OrderBy(q => q.Title);
                            break;
                        case SurveyOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case SurveyOrder.StartAt:
                            query = query.OrderBy(q => q.StartAt);
                            break;
                        case SurveyOrder.EndAt:
                            query = query.OrderBy(q => q.EndAt);
                            break;
                        case SurveyOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case SurveyOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SurveyOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SurveyOrder.Title:
                            query = query.OrderByDescending(q => q.Title);
                            break;
                        case SurveyOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case SurveyOrder.StartAt:
                            query = query.OrderByDescending(q => q.StartAt);
                            break;
                        case SurveyOrder.EndAt:
                            query = query.OrderByDescending(q => q.EndAt);
                            break;
                        case SurveyOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case SurveyOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Survey>> DynamicSelect(IQueryable<SurveyDAO> query, SurveyFilter filter)
        {
            List<Survey> Surveys = await query.Select(q => new Survey()
            {
                Id = filter.Selects.Contains(SurveySelect.Id) ? q.Id : default(long),
                Title = filter.Selects.Contains(SurveySelect.Title) ? q.Title : default(string),
                Description = filter.Selects.Contains(SurveySelect.Description) ? q.Description : default(string),
                StartAt = filter.Selects.Contains(SurveySelect.StartAt) ? q.StartAt : default(DateTime),
                EndAt = filter.Selects.Contains(SurveySelect.EndAt) ? q.EndAt : default(DateTime?),
                StatusId = filter.Selects.Contains(SurveySelect.Status) ? q.StatusId : default(long),
                CreatorId = filter.Selects.Contains(SurveySelect.Creator) ? q.CreatorId : default(long),
                Creator = filter.Selects.Contains(SurveySelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    PositionId = q.Creator.PositionId,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    SexId = q.Creator.SexId,
                    StatusId = q.Creator.StatusId,
                    Avatar = q.Creator.Avatar,
                    Birthday = q.Creator.Birthday,
                    ProvinceId = q.Creator.ProvinceId,
                } : null,
                Status = filter.Selects.Contains(SurveySelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                Used = q.Used,
            }).ToListAsync();
            return Surveys;
        }

        public async Task<int> Count(SurveyFilter filter)
        {
            IQueryable<SurveyDAO> Surveys = DataContext.Survey.AsNoTracking();
            Surveys = DynamicFilter(Surveys, filter);
            return await Surveys.CountAsync();
        }

        public async Task<List<Survey>> List(SurveyFilter filter)
        {
            if (filter == null) return new List<Survey>();
            IQueryable<SurveyDAO> SurveyDAOs = DataContext.Survey.AsNoTracking();
            SurveyDAOs = DynamicFilter(SurveyDAOs, filter);
            SurveyDAOs = DynamicOrder(SurveyDAOs, filter);
            List<Survey> Surveys = await DynamicSelect(SurveyDAOs, filter);
            return Surveys;
        }

        public async Task<Survey> Get(long Id)
        {
            Survey Survey = await DataContext.Survey.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Survey()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                Used = x.Used,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Survey == null)
                return null;
            Survey.SurveyQuestions = await DataContext.SurveyQuestion.AsNoTracking()
                .Where(x => x.SurveyId == Survey.Id)
                .Select(x => new SurveyQuestion
                {
                    Id = x.Id,
                    SurveyId = x.SurveyId,
                    Content = x.Content,
                    SurveyQuestionTypeId = x.SurveyQuestionTypeId,
                    IsMandatory = x.IsMandatory,
                    SurveyQuestionType = new SurveyQuestionType
                    {
                        Id = x.SurveyQuestionType.Id,
                        Code = x.SurveyQuestionType.Code,
                        Name = x.SurveyQuestionType.Name,
                    },
                }).ToListAsync();
            List<long> questionIds = Survey.SurveyQuestions.Select(sq => sq.Id).ToList();
            List<SurveyOption> SurveyOptions = await DataContext.SurveyOption.AsNoTracking()
                .Where(x => questionIds.Contains(x.SurveyQuestionId))
                 .Select(x => new SurveyOption
                 {
                     Id = x.Id,
                     SurveyQuestionId = x.SurveyQuestionId,
                     Content = x.Content,
                     SurveyOptionTypeId = x.SurveyOptionTypeId,
                     SurveyOptionType = new SurveyOptionType
                     {
                         Id = x.SurveyOptionType.Id,
                         Code = x.SurveyOptionType.Code,
                         Name = x.SurveyOptionType.Name,
                     },
                 }).ToListAsync();
            List<SurveyQuestionImageMapping> SurveyQuestionImageMappings = await DataContext.SurveyQuestionImageMapping.AsNoTracking()
                .Where(x => questionIds.Contains(x.SurveyQuestionId))
                .Select(x => new SurveyQuestionImageMapping
                {
                    ImageId = x.ImageId,
                    SurveyQuestionId = x.SurveyQuestionId,
                    Image = new Image { 
                        Id = x.Image.Id,
                        Name =x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    }
                })
                .ToListAsync();
            List<SurveyQuestionFileMapping> SurveyQuestionFileMappings = await DataContext.SurveyQuestionFileMapping.AsNoTracking()
                .Where(x => questionIds.Contains(x.SurveyQuestionId))
                .Select(x => new SurveyQuestionFileMapping
                {
                    FileId = x.FileId,
                    SurveyQuestionId = x.SurveyQuestionId,
                    File = new File
                    {
                        Id = x.File.Id,
                        Name = x.File.Name,
                        Path = x.File.Path
                    }
                })
                .ToListAsync();
            foreach (var SurveyQuestion in Survey.SurveyQuestions)
            {
                SurveyQuestion.SurveyOptions = SurveyOptions.Where(x => x.SurveyQuestionId == SurveyQuestion.Id).ToList();
                SurveyQuestion.SurveyQuestionImageMappings = SurveyQuestionImageMappings.Where(x => x.SurveyQuestionId == SurveyQuestion.Id).ToList();
                SurveyQuestion.SurveyQuestionFileMappings = SurveyQuestionFileMappings.Where(x => x.SurveyQuestionId == SurveyQuestion.Id).ToList();
            }
            return Survey;
        }

        public async Task<bool> Create(Survey Survey)
        {
            SurveyDAO SurveyDAO = new SurveyDAO();
            SurveyDAO.Id = Survey.Id;
            SurveyDAO.Title = Survey.Title;
            SurveyDAO.Description = Survey.Description;
            SurveyDAO.StartAt = Survey.StartAt;
            SurveyDAO.EndAt = Survey.EndAt;
            SurveyDAO.StatusId = Survey.StatusId;
            SurveyDAO.CreatorId = Survey.CreatorId;
            SurveyDAO.CreatedAt = StaticParams.DateTimeNow;
            SurveyDAO.UpdatedAt = StaticParams.DateTimeNow;
            SurveyDAO.RowId = Guid.NewGuid();
            SurveyDAO.Used = false;
            DataContext.Survey.Add(SurveyDAO);
            await DataContext.SaveChangesAsync();
            Survey.Id = SurveyDAO.Id;
            await SaveReference(Survey);
            return true;
        }

        public async Task<bool> Update(Survey Survey)
        {
            SurveyDAO SurveyDAO = DataContext.Survey.Where(x => x.Id == Survey.Id).FirstOrDefault();
            if (SurveyDAO == null)
                return false;
            SurveyDAO.Title = Survey.Title;
            SurveyDAO.Description = Survey.Description;
            SurveyDAO.StartAt = Survey.StartAt;
            SurveyDAO.EndAt = Survey.EndAt;
            SurveyDAO.StatusId = Survey.StatusId;
            SurveyDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            if (SurveyDAO.Used == false)
                await SaveReference(Survey);
            return true;
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Survey.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new SurveyDAO { Used = true });
            return true;
        }
        public async Task<bool> Delete(Survey Survey)
        {
            var SurveyQuestionIds = await DataContext.SurveyQuestion.Where(x => x.SurveyId == Survey.Id).Select(x => x.Id).ToListAsync();
            await DataContext.SurveyOption.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
            await DataContext.SurveyQuestionImageMapping.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
            await DataContext.SurveyQuestionFileMapping.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
            await DataContext.SurveyQuestion.Where(x => SurveyQuestionIds.Contains(x.Id)).DeleteFromQueryAsync();
            await DataContext.Survey.Where(x => x.Id == Survey.Id).UpdateFromQueryAsync(x => new SurveyDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Survey Survey)
        {
            if(Survey.SurveyQuestions != null)
            {
                List<long> SurveyQuestionIds = Survey.SurveyQuestions.Select(x => x.Id).ToList();
                await DataContext.SurveyQuestionFileMapping.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
                await DataContext.SurveyQuestionImageMapping.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
                await DataContext.SurveyOption.Where(x => SurveyQuestionIds.Contains(x.SurveyQuestionId)).DeleteFromQueryAsync();
                await DataContext.SurveyQuestion.Where(x => x.SurveyId == Survey.Id).DeleteFromQueryAsync();
            }

            if (Survey.SurveyQuestions != null)
            {
                List<SurveyQuestionDAO> SurveyQuestionDAOs = new List<SurveyQuestionDAO>();
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    SurveyQuestionDAO SurveyQuestionDAO = new SurveyQuestionDAO
                    {
                        Id = SurveyQuestion.Id,
                        SurveyId = Survey.Id,
                        Content = SurveyQuestion.Content,
                        SurveyQuestionTypeId = SurveyQuestion.SurveyQuestionTypeId,
                        IsMandatory = SurveyQuestion.IsMandatory,
                    };
                    SurveyQuestionDAOs.Add(SurveyQuestionDAO);
                }
                await DataContext.SurveyQuestion.BulkMergeAsync(SurveyQuestionDAOs);
                foreach (SurveyQuestionDAO SurveyQuestionDAO in SurveyQuestionDAOs)
                {
                    SurveyQuestion SurveyQuestion = Survey.SurveyQuestions.Where(sq => sq.Content == SurveyQuestionDAO.Content).FirstOrDefault();
                    SurveyQuestion.Id = SurveyQuestionDAO.Id;
                }

                List<SurveyOptionDAO> SurveyOptionDAOs = new List<SurveyOptionDAO>();
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    foreach (SurveyOption SurveyOption in SurveyQuestion.SurveyOptions)
                    {
                        SurveyOptionDAO SurveyOptionDAO = new SurveyOptionDAO
                        {
                            Content = SurveyOption.Content,
                            SurveyOptionTypeId = SurveyOption.SurveyOptionTypeId,
                            SurveyQuestionId = SurveyQuestion.Id,
                        };
                        SurveyOptionDAOs.Add(SurveyOptionDAO);
                    }
                }

                List<SurveyQuestionImageMappingDAO> SurveyQuestionImageMappingDAOs = new List<SurveyQuestionImageMappingDAO>();
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    if(SurveyQuestion.SurveyQuestionImageMappings != null)
                    {
                        foreach (SurveyQuestionImageMapping SurveyQuestionImageMapping in SurveyQuestion.SurveyQuestionImageMappings)
                        {
                            SurveyQuestionImageMappingDAO SurveyQuestionImageMappingDAO = new SurveyQuestionImageMappingDAO
                            {
                                ImageId = SurveyQuestionImageMapping.ImageId,
                                SurveyQuestionId = SurveyQuestion.Id,
                            };
                            SurveyQuestionImageMappingDAOs.Add(SurveyQuestionImageMappingDAO);
                        }
                    }
                }

                List<SurveyQuestionFileMappingDAO> SurveyQuestionFileMappingDAOs = new List<SurveyQuestionFileMappingDAO>();
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    if(SurveyQuestion.SurveyQuestionFileMappings != null)
                    {
                        foreach (SurveyQuestionFileMapping SurveyQuestionFileMapping in SurveyQuestion.SurveyQuestionFileMappings)
                        {
                            SurveyQuestionFileMappingDAO SurveyQuestionFileMappingDAO = new SurveyQuestionFileMappingDAO
                            {
                                FileId = SurveyQuestionFileMapping.FileId,
                                SurveyQuestionId = SurveyQuestion.Id,
                            };
                            SurveyQuestionFileMappingDAOs.Add(SurveyQuestionFileMappingDAO);
                        }
                    }
                }

                await DataContext.SurveyOption.BulkMergeAsync(SurveyOptionDAOs);
                await DataContext.SurveyQuestionImageMapping.BulkMergeAsync(SurveyQuestionImageMappingDAOs);
                await DataContext.SurveyQuestionFileMapping.BulkMergeAsync(SurveyQuestionFileMappingDAOs);
            }
        }
    }
}
