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
using DMS.Enums;

namespace DMS.Services.MColor
{
    public interface IColorService :  IServiceScoped
    {
        Task<int> Count(ColorFilter ColorFilter);
        Task<List<Color>> List(ColorFilter ColorFilter);
    }

    public class ColorService : BaseService, IColorService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IColorValidator ColorValidator;

        public ColorService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IColorValidator ColorValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ColorValidator = ColorValidator;
        }
        public async Task<int> Count(ColorFilter ColorFilter)
        {
            try
            {
                int result = await UOW.ColorRepository.Count(ColorFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ColorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ColorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Color>> List(ColorFilter ColorFilter)
        {
            try
            {
                List<Color> Colors = await UOW.ColorRepository.List(ColorFilter);
                return Colors;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ColorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ColorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
