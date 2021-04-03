using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MProvince
{
    public interface IProvinceService : IServiceScoped
    {
        Task<int> Count(ProvinceFilter ProvinceFilter);
        Task<List<Province>> List(ProvinceFilter ProvinceFilter);
        Task<Province> Get(long Id);
    }

    public class ProvinceService : BaseService, IProvinceService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProvinceValidator ProvinceValidator;

        public ProvinceService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProvinceValidator ProvinceValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProvinceValidator = ProvinceValidator;
        }
        public async Task<int> Count(ProvinceFilter ProvinceFilter)
        {
            try
            {
                int result = await UOW.ProvinceRepository.Count(ProvinceFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProvinceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Province>> List(ProvinceFilter ProvinceFilter)
        {
            try
            {
                List<Province> Provinces = await UOW.ProvinceRepository.List(ProvinceFilter);
                return Provinces;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProvinceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Province> Get(long Id)
        {
            Province Province = await UOW.ProvinceRepository.Get(Id);
            if (Province == null)
                return null;
            return Province;
        }

    }
}
