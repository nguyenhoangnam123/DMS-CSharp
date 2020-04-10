﻿using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProvince
{
    public interface IProvinceService : IServiceScoped
    {
        Task<int> Count(ProvinceFilter ProvinceFilter);
        Task<List<Province>> List(ProvinceFilter ProvinceFilter);
        Task<Province> Get(long Id);
        Task<Province> Create(Province Province);
        Task<Province> Update(Province Province);
        Task<Province> Delete(Province Province);
        Task<List<Province>> BulkDelete(List<Province> Provinces);
        Task<List<Province>> BulkMerge(List<Province> Provinces);

        Task<DataFile> Export(ProvinceFilter ProvinceFilter);
        ProvinceFilter ToFilter(ProvinceFilter ProvinceFilter);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Province> Get(long Id)
        {
            Province Province = await UOW.ProvinceRepository.Get(Id);
            if (Province == null)
                return null;
            return Province;
        }

        public async Task<Province> Create(Province Province)
        {
            if (!await ProvinceValidator.Create(Province))
                return Province;

            try
            {
                await UOW.Begin();
                await UOW.ProvinceRepository.Create(Province);
                await UOW.Commit();

                await Logging.CreateAuditLog(Province, new { }, nameof(ProvinceService));
                return await UOW.ProvinceRepository.Get(Province.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Province> Update(Province Province)
        {
            if (!await ProvinceValidator.Update(Province))
                return Province;
            try
            {
                var oldData = await UOW.ProvinceRepository.Get(Province.Id);

                await UOW.Begin();
                await UOW.ProvinceRepository.Update(Province);
                await UOW.Commit();

                var newData = await UOW.ProvinceRepository.Get(Province.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProvinceService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Province> Delete(Province Province)
        {
            if (!await ProvinceValidator.Delete(Province))
                return Province;

            try
            {
                await UOW.Begin();
                await UOW.ProvinceRepository.Delete(Province);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Province, nameof(ProvinceService));
                return Province;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Province>> BulkDelete(List<Province> Provinces)
        {
            if (!await ProvinceValidator.BulkDelete(Provinces))
                return Provinces;

            try
            {
                await UOW.Begin();
                await UOW.ProvinceRepository.BulkDelete(Provinces);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Provinces, nameof(ProvinceService));
                return Provinces;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Province>> BulkMerge(List<Province> Provinces)
        {
            try
            {
                await UOW.Begin();
                #region merge province
                List<Province> dbProvinces = await UOW.ProvinceRepository.List(new ProvinceFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ProvinceSelect.Id | ProvinceSelect.Code,
                });
                foreach (Province province in Provinces)
                {
                    long provinceId = dbProvinces.Where(x => x.Code == province.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    province.Id = provinceId;
                }
                await UOW.ProvinceRepository.BulkMerge(Provinces);
                dbProvinces = await UOW.ProvinceRepository.List(new ProvinceFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ProvinceSelect.ALL,
                });
                foreach (Province province in Provinces)
                {
                    long provinceId = dbProvinces.Where(x => x.Code == province.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    province.Id = provinceId;
                }
                #endregion

                #region merge District
                List<District> dbDistricts = await UOW.DistrictRepository.List(new DistrictFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = DistrictSelect.ALL,
                });
                foreach (Province province in Provinces)
                {
                    foreach (District district in province.Districts)
                    {
                        district.ProvinceId = province.Id;
                        long districtId = dbDistricts
                            .Where(x => x.Code == district.Code && x.ProvinceId == province.Id)
                            .Select(x => x.Id)
                            .FirstOrDefault();
                        district.Id = districtId;
                    }
                }
                List<District> Districts = Provinces.SelectMany(x => x.Districts).ToList();
                await UOW.DistrictRepository.BulkMerge(Districts);
                dbDistricts = await UOW.DistrictRepository.List(new DistrictFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = DistrictSelect.ALL,
                });
                foreach (District district in Districts)
                {
                    long districtId = dbDistricts
                        .Where(x => x.Code == district.Code && x.ProvinceId == district.ProvinceId)
                        .Select(x => x.Id).FirstOrDefault();
                    district.Id = districtId;
                }
                #endregion

                #region merge Ward
                List<Ward> dbWards = await UOW.WardRepository.List(new WardFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = WardSelect.ALL,
                });
                foreach (District district in Districts)
                {
                    foreach (Ward ward in district.Wards)
                    {
                        ward.DistrictId = district.Id;
                        long wardId = dbWards
                            .Where(x => x.Code == ward.Code && x.DistrictId == district.Id)
                            .Select(x => x.Id)
                            .FirstOrDefault();
                        ward.Id = wardId;
                    }
                }
                List<Ward> Wards = Districts.SelectMany(x => x.Wards).ToList();
                await UOW.WardRepository.BulkMerge(Wards);
                dbWards = await UOW.WardRepository.List(new WardFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = WardSelect.ALL,
                });
                foreach (Ward ward in Wards)
                {
                    long wardId = dbDistricts
                        .Where(x => x.Code == ward.Code && x.ProvinceId == ward.DistrictId)
                        .Select(x => x.Id).FirstOrDefault();
                    ward.Id = wardId;
                }
                #endregion

                await UOW.Commit();

                await Logging.CreateAuditLog(Provinces, new { }, nameof(ProvinceService));
                return Provinces;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProvinceService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }



        public async Task<DataFile> Export(ProvinceFilter ProvinceFilter)
        {
            List<Province> Provinces = await UOW.ProvinceRepository.List(ProvinceFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Province);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Province.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Province.Name);
                worksheet.Cells[1, PriorityColumn].Value = nameof(Province.Priority);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Province.StatusId);

                for (int i = 0; i < Provinces.Count; i++)
                {
                    Province Province = Provinces[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Province.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Province.Name;
                    worksheet.Cells[i + StartRow, PriorityColumn].Value = Province.Priority;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Province.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Province),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public ProvinceFilter ToFilter(ProvinceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProvinceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProvinceFilter subFilter = new ProvinceFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Priority))
                    subFilter.Priority = Map(subFilter.Priority, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
