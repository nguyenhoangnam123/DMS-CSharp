using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MLuckyNumber
{
    public interface ILuckyNumberService :  IServiceScoped
    {
        Task<int> Count(LuckyNumberFilter LuckyNumberFilter);
        Task<List<LuckyNumber>> List(LuckyNumberFilter LuckyNumberFilter);
        Task<LuckyNumber> LuckyDraw(long RewardHistoryId);
        Task<LuckyNumber> Get(long Id);
        Task<LuckyNumber> Create(LuckyNumber LuckyNumber);
        Task<LuckyNumber> Update(LuckyNumber LuckyNumber);
        Task<LuckyNumber> Delete(LuckyNumber LuckyNumber);
        Task<List<LuckyNumber>> BulkDelete(List<LuckyNumber> LuckyNumbers);
        Task<List<LuckyNumber>> Import(List<LuckyNumber> LuckyNumbers);
        Task<LuckyNumberFilter> ToFilter(LuckyNumberFilter LuckyNumberFilter);
    }

    public class LuckyNumberService : BaseService, ILuckyNumberService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ILuckyNumberValidator LuckyNumberValidator;

        public LuckyNumberService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ILuckyNumberValidator LuckyNumberValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.LuckyNumberValidator = LuckyNumberValidator;
        }
        public async Task<int> Count(LuckyNumberFilter LuckyNumberFilter)
        {
            try
            {
                int result = await UOW.LuckyNumberRepository.Count(LuckyNumberFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<LuckyNumber>> List(LuckyNumberFilter LuckyNumberFilter)
        {
            try
            {
                List<LuckyNumber> LuckyNumbers = await UOW.LuckyNumberRepository.List(LuckyNumberFilter);
                return LuckyNumbers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<LuckyNumber> Get(long Id)
        {
            LuckyNumber LuckyNumber = await UOW.LuckyNumberRepository.Get(Id);
            if (LuckyNumber == null)
                return null;
            return LuckyNumber;
        }
       
        public async Task<LuckyNumber> LuckyDraw(long RewardHistoryId)
        {
            RewardHistory RewardHistory = await UOW.RewardHistoryRepository.Get(RewardHistoryId);
            if(RewardHistory != null && RewardHistory.TurnCounter > 0)
            {
                List<LuckyNumber> LuckyNumbers = await UOW.LuckyNumberRepository.List(new LuckyNumberFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = LuckyNumberSelect.ALL,
                    RewardStatusId = new IdFilter { Equal = RewardStatusEnum.ACTIVE.Id }
                });

                if (LuckyNumbers.Count() >= 1)
                {
                    Random rnd = new Random();
                    var RandomNumber = LuckyNumbers.OrderBy(x => rnd.Next()).FirstOrDefault();

                    RandomNumber.RewardStatusId = RewardStatusEnum.INACTIVE.Id;
                    RandomNumber.Used = true;
                    await UOW.LuckyNumberRepository.Update(RandomNumber);

                    if(RewardHistory.RewardHistoryContents == null)
                    {
                        RewardHistory.RewardHistoryContents = new List<RewardHistoryContent>();
                    }
                    RewardHistory.RewardHistoryContents.Add(new RewardHistoryContent 
                    {
                        LuckyNumberId = RandomNumber.Id,
                        RewardHistoryId = RewardHistory.Id
                    });
                    await UOW.RewardHistoryRepository.Update(RewardHistory);

                    return RandomNumber;
                }
            }
            return null;
        }

        public async Task<LuckyNumber> Create(LuckyNumber LuckyNumber)
        {
            if (!await LuckyNumberValidator.Create(LuckyNumber))
                return LuckyNumber;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberRepository.Create(LuckyNumber);
                await UOW.Commit();
                LuckyNumber = await UOW.LuckyNumberRepository.Get(LuckyNumber.Id);
                await Logging.CreateAuditLog(LuckyNumber, new { }, nameof(LuckyNumberService));
                return LuckyNumber;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<LuckyNumber> Update(LuckyNumber LuckyNumber)
        {
            if (!await LuckyNumberValidator.Update(LuckyNumber))
                return LuckyNumber;
            try
            {
                var oldData = await UOW.LuckyNumberRepository.Get(LuckyNumber.Id);

                await UOW.Begin();
                await UOW.LuckyNumberRepository.Update(LuckyNumber);
                await UOW.Commit();

                LuckyNumber = await UOW.LuckyNumberRepository.Get(LuckyNumber.Id);
                await Logging.CreateAuditLog(LuckyNumber, oldData, nameof(LuckyNumberService));
                return LuckyNumber;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<LuckyNumber> Delete(LuckyNumber LuckyNumber)
        {
            if (!await LuckyNumberValidator.Delete(LuckyNumber))
                return LuckyNumber;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberRepository.Delete(LuckyNumber);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, LuckyNumber, nameof(LuckyNumberService));
                return LuckyNumber;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<LuckyNumber>> BulkDelete(List<LuckyNumber> LuckyNumbers)
        {
            if (!await LuckyNumberValidator.BulkDelete(LuckyNumbers))
                return LuckyNumbers;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberRepository.BulkDelete(LuckyNumbers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, LuckyNumbers, nameof(LuckyNumberService));
                return LuckyNumbers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<LuckyNumber>> Import(List<LuckyNumber> LuckyNumbers)
        {
            if (!await LuckyNumberValidator.Import(LuckyNumbers))
                return LuckyNumbers;
            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberRepository.BulkMerge(LuckyNumbers);
                await UOW.Commit();

                await Logging.CreateAuditLog(LuckyNumbers, new { }, nameof(LuckyNumberService));
                return LuckyNumbers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<LuckyNumberFilter> ToFilter(LuckyNumberFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<LuckyNumberFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                LuckyNumberFilter subFilter = new LuckyNumberFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RewardStatusId))
                        subFilter.RewardStatusId = FilterBuilder.Merge(subFilter.RewardStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
