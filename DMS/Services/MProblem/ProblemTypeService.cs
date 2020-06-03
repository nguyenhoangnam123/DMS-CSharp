using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MProblemType
{
    public interface IProblemTypeService :  IServiceScoped
    {
        Task<int> Count(ProblemTypeFilter ProblemTypeFilter);
        Task<List<ProblemType>> List(ProblemTypeFilter ProblemTypeFilter);
    }

    public class ProblemTypeService : BaseService, IProblemTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ProblemTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ProblemTypeFilter ProblemTypeFilter)
        {
            try
            {
                int result = await UOW.ProblemTypeRepository.Count(ProblemTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                string ClassName = nameof(ProblemTypeService);
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, ClassName);
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, ClassName);
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProblemType>> List(ProblemTypeFilter ProblemTypeFilter)
        {
            try
            {
                List<ProblemType> ProblemTypes = await UOW.ProblemTypeRepository.List(ProblemTypeFilter);
                return ProblemTypes;
            }
            catch (Exception ex)
            {
                string ClassName = nameof(ProblemTypeService);
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, ClassName);
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, ClassName);
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}