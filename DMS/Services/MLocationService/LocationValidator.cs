using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MProvince
{
    public interface ILocationValidator : IServiceScoped
    {
        Task<bool> ImportAll(List<Province> Provinces);
    }

    public class LocationValidator : ILocationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public LocationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ImportAll(List<Province> Provinces)
        {
            return true;
        }
    }
}
