using Common;
using DMS.Entities;
using DMS.Services.MBrand;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.brand
{
    public class BrandController : RpcController
    {
        private IStatusService StatusService;
        private IBrandService BrandService;
        private ICurrentContext CurrentContext;
        public BrandController(
            IStatusService StatusService,
            IBrandService BrandService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.BrandService = BrandService;
            this.CurrentContext = CurrentContext;
        }

        [Route(BrandRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO);
            int count = await BrandService.Count(BrandFilter);
            return count;
        }

        [Route(BrandRoute.List), HttpPost]
        public async Task<ActionResult<List<Brand_BrandDTO>>> List([FromBody] Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO);
            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Brand_BrandDTO> Brand_BrandDTOs = Brands
                .Select(c => new Brand_BrandDTO(c)).ToList();
            return Brand_BrandDTOs;
        }

        [Route(BrandRoute.Get), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Get([FromBody]Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = await BrandService.Get(Brand_BrandDTO.Id);
            return new Brand_BrandDTO(Brand);
        }

        [Route(BrandRoute.Create), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Create([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Create(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.Update), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Update([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Update(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.Delete), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Delete([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Delete(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.Import), HttpPost]
        public async Task<ActionResult<List<Brand_BrandDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            List<Brand> Brands = new List<Brand>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                int StartColumn = 1;
                int StartRow = 1;

                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString()))
                        break;

                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();

                    Brand Brand = new Brand();

                    Brand.Code = CodeValue;
                    Brand.Name = NameValue;
                    Brand.Description = DescriptionValue;

                    Brands.Add(Brand);
                }
            }
            Brands = await BrandService.Import(Brands);

            List<Brand_BrandDTO> Brand_BrandDTOs = Brands
                .Select(c => new Brand_BrandDTO(c)).ToList();
            return Brand_BrandDTOs;
        }

        [Route(BrandRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO);

            BrandFilter.Skip = 0;
            BrandFilter.Take = int.MaxValue;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var BrandHeaders = new List<string[]>()
                {
                    new string[] {  "Mã nhãn hiệu", "Tên nhãn hiệu", "Mô tả"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Brands.Count; i++)
                {
                    var Brand = Brands[i];
                    data.Add(new Object[]
                    {
                        Brand.Code,
                        Brand.Name,
                        Brand.Description
                    });
                }
                excel.GenerateWorksheet("Brand", BrandHeaders, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Brand.xlsx");
        }

        [Route(BrandRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Brand_Export.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                var nameexcel = "Export nhãn hiệu" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Sonhx5";
                xlPackage.Workbook.Properties.Subject = string.Format("{0}", "RD-DMS");
                xlPackage.Workbook.Properties.Category = "RD-DMS";
                xlPackage.Workbook.Properties.Company = "FPT-FIS-ERP-ESC";
                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Brand" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(BrandRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Id = new IdFilter { In = Ids };
            BrandFilter.Selects = BrandSelect.Id;
            BrandFilter.Skip = 0;
            BrandFilter.Take = int.MaxValue;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            Brands = await BrandService.BulkDelete(Brands);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            BrandFilter BrandFilter = new BrandFilter();
            if (Id == 0)
            {

            }
            else
            {
                BrandFilter.Id = new IdFilter { Equal = Id };
                int count = await BrandService.Count(BrandFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Brand ConvertDTOToEntity(Brand_BrandDTO Brand_BrandDTO)
        {
            Brand Brand = new Brand();
            Brand.Id = Brand_BrandDTO.Id;
            Brand.Code = Brand_BrandDTO.Code;
            Brand.Name = Brand_BrandDTO.Name;
            Brand.Description = Brand_BrandDTO.Description;
            Brand.StatusId = Brand_BrandDTO.StatusId;
            Brand.UpdateTime = Brand_BrandDTO.UpdateTime;
            Brand.Status = Brand_BrandDTO.Status == null ? null : new Status
            {
                Id = Brand_BrandDTO.Status.Id,
                Code = Brand_BrandDTO.Status.Code,
                Name = Brand_BrandDTO.Status.Name,
            };
            Brand.BaseLanguage = CurrentContext.Language;
            return Brand;
        }

        private BrandFilter ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Skip = Brand_BrandFilterDTO.Skip;
            BrandFilter.Take = Brand_BrandFilterDTO.Take;
            BrandFilter.OrderBy = Brand_BrandFilterDTO.OrderBy;
            BrandFilter.OrderType = Brand_BrandFilterDTO.OrderType;

            BrandFilter.Id = Brand_BrandFilterDTO.Id;
            BrandFilter.Code = Brand_BrandFilterDTO.Code;
            BrandFilter.Name = Brand_BrandFilterDTO.Name;
            BrandFilter.Description = Brand_BrandFilterDTO.Description;
            BrandFilter.StatusId = Brand_BrandFilterDTO.StatusId;
            BrandFilter.UpdateTime = Brand_BrandFilterDTO.UpdateTime;
            return BrandFilter;
        }

        [Route(BrandRoute.SingleListStatus), HttpPost]
        public async Task<List<Brand_StatusDTO>> SingleListStatus([FromBody] Brand_StatusFilterDTO Brand_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Brand_StatusFilterDTO.Id;
            StatusFilter.Code = Brand_StatusFilterDTO.Code;
            StatusFilter.Name = Brand_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Brand_StatusDTO> Brand_StatusDTOs = Statuses
                .Select(x => new Brand_StatusDTO(x)).ToList();
            return Brand_StatusDTOs;
        }
        [Route(BrandRoute.FilterListStatus), HttpPost]
        public async Task<List<Brand_StatusDTO>> FilterListStatus([FromBody] Brand_StatusFilterDTO Brand_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Brand_StatusFilterDTO.Id;
            StatusFilter.Code = Brand_StatusFilterDTO.Code;
            StatusFilter.Name = Brand_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Brand_StatusDTO> Brand_StatusDTOs = Statuses
                .Select(x => new Brand_StatusDTO(x)).ToList();
            return Brand_StatusDTOs;
        }

    }
}

