using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DMS.Services.MProblem
{
    public interface IProblemService : IServiceScoped
    {
        Task<int> Count(ProblemFilter ProblemFilter);
        Task<List<Problem>> List(ProblemFilter ProblemFilter);
        Task<Problem> Get(long Id);
        Task<Problem> Create(Problem Problem);
        Task<Problem> Update(Problem Problem);
        Task<Problem> Delete(Problem Problem);
        Task<List<Problem>> BulkDelete(List<Problem> Problems);
        Task<List<Problem>> Import(List<Problem> Problems);
        Task<Image> SaveImage(Image Image);
        ProblemFilter ToFilter(ProblemFilter ProblemFilter);
    }

    public class ProblemService : BaseService, IProblemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private IProblemValidator ProblemValidator;

        public ProblemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IProblemValidator ProblemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.ProblemValidator = ProblemValidator;
        }
        public async Task<int> Count(ProblemFilter ProblemFilter)
        {
            try
            {
                int result = await UOW.ProblemRepository.Count(ProblemFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Problem>> List(ProblemFilter ProblemFilter)
        {
            try
            {
                List<Problem> Problems = await UOW.ProblemRepository.List(ProblemFilter);
                return Problems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Problem> Get(long Id)
        {
            Problem Problem = await UOW.ProblemRepository.Get(Id);
            if (Problem == null)
                return null;
            return Problem;
        }

        public async Task<Problem> Create(Problem Problem)
        {
            if (!await ProblemValidator.Create(Problem))
                return Problem;

            try
            {
                Problem.CreatorId = CurrentContext.UserId;
                Problem.Code = StaticParams.DateTimeNow.ToString();
                await UOW.Begin();
                await UOW.ProblemRepository.Create(Problem);
                Problem = await UOW.ProblemRepository.Get(Problem.Id);
                var Year = StaticParams.DateTimeNow.Year.ToString().Substring(2);
                var Id = (1000000 + Problem.Id).ToString().Substring(1);
                Problem.Code = $"VD{Year}.{Id}";
                Problem.ProblemHistorys = new List<ProblemHistory>
                {
                    new ProblemHistory
                    {
                        ProblemId = Problem.Id,
                        ModifierId = CurrentContext.UserId,
                        ProblemStatusId = Enums.ProblemStatusEnum.WAITING.Id,
                        Time = StaticParams.DateTimeNow
                    }
                };
                await UOW.ProblemRepository.Update(Problem);
                await UOW.Commit();

                await Logging.CreateAuditLog(Problem, new { }, nameof(ProblemService));
                return await UOW.ProblemRepository.Get(Problem.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Problem> Update(Problem Problem)
        {
            if (!await ProblemValidator.Update(Problem))
                return Problem;
            try
            {
                var oldData = await UOW.ProblemRepository.Get(Problem.Id);
                Problem.Code = oldData.Code;
                if(Problem.ProblemStatusId != oldData.ProblemStatusId)
                {
                    Problem.ProblemHistorys = oldData.ProblemHistorys;
                    ProblemHistory ProblemHistory = new ProblemHistory
                    {
                        ProblemId = Problem.Id,
                        ModifierId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                    };
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.WAITING.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.WAITING.Id;
                    }
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.PROCESSING.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.PROCESSING.Id;
                    }
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.DONE.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.DONE.Id;
                    }
                    Problem.ProblemHistorys.Add(ProblemHistory);
                }
                await UOW.Begin();
                await UOW.ProblemRepository.Update(Problem);
                await UOW.Commit();

                var newData = await UOW.ProblemRepository.Get(Problem.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProblemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Problem> Delete(Problem Problem)
        {
            if (!await ProblemValidator.Delete(Problem))
                return Problem;

            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.Delete(Problem);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Problem, nameof(ProblemService));
                return Problem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Problem>> BulkDelete(List<Problem> Problems)
        {
            if (!await ProblemValidator.BulkDelete(Problems))
                return Problems;

            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.BulkDelete(Problems);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Problems, nameof(ProblemService));
                return Problems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Problem>> Import(List<Problem> Problems)
        {
            if (!await ProblemValidator.Import(Problems))
                return Problems;
            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.BulkMerge(Problems);
                await UOW.Commit();

                await Logging.CreateAuditLog(Problems, new { }, nameof(ProblemService));
                return Problems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public ProblemFilter ToFilter(ProblemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProblemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProblemFilter subFilter = new ProblemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {

                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/problem/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }
    }
}
