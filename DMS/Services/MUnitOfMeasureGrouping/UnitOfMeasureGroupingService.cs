using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MUnitOfMeasureGrouping
{
    public interface IUnitOfMeasureGroupingService : IServiceScoped
    {
        Task<int> Count(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<UnitOfMeasureGrouping> Get(long Id);
    }

    public class UnitOfMeasureGroupingService : BaseService, IUnitOfMeasureGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUnitOfMeasureGroupingValidator UnitOfMeasureGroupingValidator;

        public UnitOfMeasureGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUnitOfMeasureGroupingValidator UnitOfMeasureGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UnitOfMeasureGroupingValidator = UnitOfMeasureGroupingValidator;
        }
        public async Task<int> Count(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter)
        {
            try
            {
                int result = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter)
        {
            try
            {
                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(UnitOfMeasureGroupingFilter);
                return UnitOfMeasureGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<UnitOfMeasureGrouping> Get(long Id)
        {
            UnitOfMeasureGrouping UnitOfMeasureGrouping = await UOW.UnitOfMeasureGroupingRepository.Get(Id);
            if (UnitOfMeasureGrouping == null)
                return null;
            return UnitOfMeasureGrouping;
        }
    }
}
