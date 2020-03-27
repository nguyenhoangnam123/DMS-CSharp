using Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Services.MProvince;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MLocationService
{
    public interface ILocationService : IServiceScoped
    {
        Task<List<LocationImport>> ImportAll(DataFile DataFile);
    }

    public class LocationService : BaseService, ILocationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ILocationValidator LocationValidator;
        //private ILocationRepository LocationRepository;

        public LocationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ILocationValidator LocationValidator
        //ILocationRepository LocationRepository
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.LocationValidator = LocationValidator;
            //this.LocationRepository = LocationRepository;
        }

        public async Task<List<LocationImport>> ImportAll(DataFile DataFile)
        {
            List<LocationImport> ltsLocation = new List<LocationImport>();
            List<Province> ltsProvince = new List<Province>();
            List<District> ltsDistrict = new List<District>();
            List<Ward> ltsWard = new List<Ward>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return null;
                int StartColumn = 1;
                int StartRow = 2;

                int ProvinceName = 0 + StartColumn;
                int ProvinceNameCode = 1 + StartColumn;

                int DistrictName = 2 + StartColumn;
                int DistrictCode = 3 + StartColumn;

                int WardName = 4 + StartColumn;
                int WardCode = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    // Lấy thông tin từng dòng
                    string ProvinceNameValue = worksheet.Cells[i + StartRow, ProvinceName].Value?.ToString();
                    string ProvinceCodeValue = worksheet.Cells[i + StartRow, ProvinceNameCode].Value?.ToString();
                    string DistrictNameValue = worksheet.Cells[i + StartRow, DistrictName].Value?.ToString();
                    string DistrictCodeValue = worksheet.Cells[i + StartRow, DistrictCode].Value?.ToString();
                    string WardNameValue = worksheet.Cells[i + StartRow, WardName].Value?.ToString();
                    string WardCodeValue = worksheet.Cells[i + StartRow, WardCode].Value?.ToString();

                    if (!string.IsNullOrEmpty(ProvinceNameValue) || !string.IsNullOrEmpty(ProvinceCodeValue))
                    {
                        Province p = new Province();
                        p.Code = ProvinceCodeValue;
                        p.Name = ProvinceNameValue;
                        ltsProvince.Add(p);
                    }
                    if (!string.IsNullOrEmpty(DistrictNameValue) || !string.IsNullOrEmpty(DistrictCodeValue))
                    {
                        District p = new District();
                        p.Code = DistrictCodeValue;
                        p.Name = DistrictNameValue;
                        p.ProvinceCode = ProvinceCodeValue;
                        ltsDistrict.Add(p);
                    }
                    if (!string.IsNullOrEmpty(WardNameValue) || !string.IsNullOrEmpty(WardCodeValue))
                    {
                        Ward p = new Ward();
                        p.Code = WardCodeValue;
                        p.Name = WardNameValue;
                        p.DistrictCode = DistrictCodeValue;
                        ltsWard.Add(p);
                    }

                }
            }
            try
            {
                // danh sách province
                ltsProvince = ltsProvince.GroupBy(p => p.Code).Select(group => group.First()).ToList();
                ltsDistrict = ltsDistrict.GroupBy(p => p.Code).Select(group => group.First()).ToList();
                ltsWard = ltsWard.GroupBy(p => p.Code).Select(group => group.First()).ToList();


                await UOW.Begin();
                // Thêm mới 1 đống 
                await UOW.ProvinceRepository.BulkMergeImport(ltsProvince);
                await UOW.DistrictRepository.BulkMergeImport(ltsDistrict);
                await UOW.WardRepository.BulkMergeImport(ltsWard);
                await UOW.Commit();

                await Logging.CreateAuditLog(ltsLocation, new { }, nameof(LocationService));
                return ltsLocation;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(LocationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }

        }

    }
}
