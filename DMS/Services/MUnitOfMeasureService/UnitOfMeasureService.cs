using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MUnitOfMeasure
{
    public interface IUnitOfMeasureService : IServiceScoped
    {
        Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<UnitOfMeasure> Get(long Id);
        Task<UnitOfMeasure> Create(UnitOfMeasure UnitOfMeasure);
        Task<UnitOfMeasure> Update(UnitOfMeasure UnitOfMeasure);
        Task<UnitOfMeasure> Delete(UnitOfMeasure UnitOfMeasure);
        Task<List<UnitOfMeasure>> BulkMerge(List<UnitOfMeasure> UnitOfMeasures);
        Task<List<UnitOfMeasure>> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
        UnitOfMeasureFilter ToFilter(UnitOfMeasureFilter UnitOfMeasureFilter);
    }

    public class UnitOfMeasureService : BaseService, IUnitOfMeasureService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUnitOfMeasureValidator UnitOfMeasureValidator;

        public UnitOfMeasureService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUnitOfMeasureValidator UnitOfMeasureValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UnitOfMeasureValidator = UnitOfMeasureValidator;
        }
        public async Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter)
        {
            try
            {
                int result = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter)
        {
            try
            {
                List<UnitOfMeasure> UnitOfMeasures = await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter);
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<UnitOfMeasure> Get(long Id)
        {
            UnitOfMeasure UnitOfMeasure = await UOW.UnitOfMeasureRepository.Get(Id);
            if (UnitOfMeasure == null)
                return null;
            return UnitOfMeasure;
        }

        public async Task<UnitOfMeasure> Create(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Create(UnitOfMeasure))
                return UnitOfMeasure;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Create(UnitOfMeasure);
                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasure, new { }, nameof(UnitOfMeasureService));
                return await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasure> Update(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Update(UnitOfMeasure))
                return UnitOfMeasure;
            try
            {
                var oldData = await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);

                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Update(UnitOfMeasure);
                await UOW.Commit();

                var newData = await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(UnitOfMeasureService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasure> Delete(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Delete(UnitOfMeasure))
                return UnitOfMeasure;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Delete(UnitOfMeasure);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasure, nameof(UnitOfMeasureService));
                return UnitOfMeasure;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            if (!await UnitOfMeasureValidator.BulkDelete(UnitOfMeasures))
                return UnitOfMeasures;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.BulkDelete(UnitOfMeasures);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasures, nameof(UnitOfMeasureService));
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> BulkMerge(List<UnitOfMeasure> UnitOfMeasures)
        {
            if (!await UnitOfMeasureValidator.BulkMerge(UnitOfMeasures))
                return UnitOfMeasures;
            try
            {
                await UOW.Begin();
                List<UnitOfMeasure> dbUnitOfMeasures = await UOW.UnitOfMeasureRepository.List(new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = UnitOfMeasureSelect.Id | UnitOfMeasureSelect.Code,
                });
                foreach (UnitOfMeasure UnitOfMeasure in UnitOfMeasures)
                {
                    long UnitOfMeasureId = dbUnitOfMeasures.Where(x => x.Code == UnitOfMeasure.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    UnitOfMeasure.Id = UnitOfMeasureId;
                }
                await UOW.UnitOfMeasureRepository.BulkMerge(UnitOfMeasures);

                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasures, new { }, nameof(UnitOfMeasureService));
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public UnitOfMeasureFilter ToFilter(UnitOfMeasureFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<UnitOfMeasureFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                UnitOfMeasureFilter subFilter = new UnitOfMeasureFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Description))
                    subFilter.Description = Map(subFilter.Description, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
