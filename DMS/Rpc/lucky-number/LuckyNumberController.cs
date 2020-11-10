using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MLuckyNumber;
using DMS.Services.MRewardStatus;
using System.Dynamic;
using System.Text;
using DMS.Enums;

namespace DMS.Rpc.lucky_number
{
    public partial class LuckyNumberController : RpcController
    {
        private IRewardStatusService RewardStatusService;
        private ILuckyNumberService LuckyNumberService;
        private ICurrentContext CurrentContext;
        public LuckyNumberController(
            IRewardStatusService RewardStatusService,
            ILuckyNumberService LuckyNumberService,
            ICurrentContext CurrentContext
        )
        {
            this.RewardStatusService = RewardStatusService;
            this.LuckyNumberService = LuckyNumberService;
            this.CurrentContext = CurrentContext;
        }

        [Route(LuckyNumberRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] LuckyNumber_LuckyNumberFilterDTO LuckyNumber_LuckyNumberFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LuckyNumberFilter LuckyNumberFilter = ConvertFilterDTOToFilterEntity(LuckyNumber_LuckyNumberFilterDTO);
            LuckyNumberFilter = await LuckyNumberService.ToFilter(LuckyNumberFilter);
            int count = await LuckyNumberService.Count(LuckyNumberFilter);
            return count;
        }

        [Route(LuckyNumberRoute.List), HttpPost]
        public async Task<ActionResult<List<LuckyNumber_LuckyNumberDTO>>> List([FromBody] LuckyNumber_LuckyNumberFilterDTO LuckyNumber_LuckyNumberFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LuckyNumberFilter LuckyNumberFilter = ConvertFilterDTOToFilterEntity(LuckyNumber_LuckyNumberFilterDTO);
            LuckyNumberFilter = await LuckyNumberService.ToFilter(LuckyNumberFilter);
            List<LuckyNumber> LuckyNumbers = await LuckyNumberService.List(LuckyNumberFilter);
            List<LuckyNumber_LuckyNumberDTO> LuckyNumber_LuckyNumberDTOs = LuckyNumbers
                .Select(c => new LuckyNumber_LuckyNumberDTO(c)).ToList();
            return LuckyNumber_LuckyNumberDTOs;
        }

        [Route(LuckyNumberRoute.Get), HttpPost]
        public async Task<ActionResult<LuckyNumber_LuckyNumberDTO>> Get([FromBody]LuckyNumber_LuckyNumberDTO LuckyNumber_LuckyNumberDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(LuckyNumber_LuckyNumberDTO.Id))
                return Forbid();

            LuckyNumber LuckyNumber = await LuckyNumberService.Get(LuckyNumber_LuckyNumberDTO.Id);
            return new LuckyNumber_LuckyNumberDTO(LuckyNumber);
        }

        [Route(LuckyNumberRoute.Create), HttpPost]
        public async Task<ActionResult<LuckyNumber_LuckyNumberDTO>> Create([FromBody] LuckyNumber_LuckyNumberDTO LuckyNumber_LuckyNumberDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(LuckyNumber_LuckyNumberDTO.Id))
                return Forbid();

            LuckyNumber LuckyNumber = ConvertDTOToEntity(LuckyNumber_LuckyNumberDTO);
            LuckyNumber = await LuckyNumberService.Create(LuckyNumber);
            LuckyNumber_LuckyNumberDTO = new LuckyNumber_LuckyNumberDTO(LuckyNumber);
            if (LuckyNumber.IsValidated)
                return LuckyNumber_LuckyNumberDTO;
            else
                return BadRequest(LuckyNumber_LuckyNumberDTO);
        }

        [Route(LuckyNumberRoute.Update), HttpPost]
        public async Task<ActionResult<LuckyNumber_LuckyNumberDTO>> Update([FromBody] LuckyNumber_LuckyNumberDTO LuckyNumber_LuckyNumberDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(LuckyNumber_LuckyNumberDTO.Id))
                return Forbid();

            LuckyNumber LuckyNumber = ConvertDTOToEntity(LuckyNumber_LuckyNumberDTO);
            LuckyNumber = await LuckyNumberService.Update(LuckyNumber);
            LuckyNumber_LuckyNumberDTO = new LuckyNumber_LuckyNumberDTO(LuckyNumber);
            if (LuckyNumber.IsValidated)
                return LuckyNumber_LuckyNumberDTO;
            else
                return BadRequest(LuckyNumber_LuckyNumberDTO);
        }

        [Route(LuckyNumberRoute.Delete), HttpPost]
        public async Task<ActionResult<LuckyNumber_LuckyNumberDTO>> Delete([FromBody] LuckyNumber_LuckyNumberDTO LuckyNumber_LuckyNumberDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(LuckyNumber_LuckyNumberDTO.Id))
                return Forbid();

            LuckyNumber LuckyNumber = ConvertDTOToEntity(LuckyNumber_LuckyNumberDTO);
            LuckyNumber = await LuckyNumberService.Delete(LuckyNumber);
            LuckyNumber_LuckyNumberDTO = new LuckyNumber_LuckyNumberDTO(LuckyNumber);
            if (LuckyNumber.IsValidated)
                return LuckyNumber_LuckyNumberDTO;
            else
                return BadRequest(LuckyNumber_LuckyNumberDTO);
        }
        
        [Route(LuckyNumberRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LuckyNumberFilter LuckyNumberFilter = new LuckyNumberFilter();
            LuckyNumberFilter = await LuckyNumberService.ToFilter(LuckyNumberFilter);
            LuckyNumberFilter.Id = new IdFilter { In = Ids };
            LuckyNumberFilter.Selects = LuckyNumberSelect.Id;
            LuckyNumberFilter.Skip = 0;
            LuckyNumberFilter.Take = int.MaxValue;

            List<LuckyNumber> LuckyNumbers = await LuckyNumberService.List(LuckyNumberFilter);
            LuckyNumbers = await LuckyNumberService.BulkDelete(LuckyNumbers);
            if (LuckyNumbers.Any(x => !x.IsValidated))
                return BadRequest(LuckyNumbers.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(LuckyNumberRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FileInfo FileInfo = new FileInfo(file.FileName);
            StringBuilder errorContent = new StringBuilder();
            if (!FileInfo.Extension.Equals(".xlsx"))
            {
                errorContent.AppendLine("Định dạng file không hợp lệ");
                return BadRequest(errorContent.ToString());
            }

            List<LuckyNumber> OldDatas = await LuckyNumberService.List(new LuckyNumberFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = LuckyNumberSelect.ALL,
            });
            HashSet<string> Codes = OldDatas.Select(x => x.Code).ToHashSet();

            List<LuckyNumber> LuckyNumbers = new List<LuckyNumber>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["LuckyNumber"];

                if (worksheet == null)
                {
                    errorContent.AppendLine("File không đúng biểu mẫu import");
                    return BadRequest(errorContent.ToString());
                }

                int StartColumn = 1;
                int StartRow = 2;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ValueColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string CodeValue = worksheet.Cells[i, CodeColumn].Value?.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(CodeValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập mã quay thưởng");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(CodeValue) && i == worksheet.Dimension.End.Row)
                        break;
                    else if (Codes.Contains(CodeValue))
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Mã quay thưởng đã tồn tại");
                        continue;
                    }

                    Codes.Add(CodeValue);
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string ValueValue = worksheet.Cells[i, ValueColumn].Value?.ToString();
                    
                    LuckyNumber LuckyNumber = new LuckyNumber();
                    LuckyNumber.Code = CodeValue;
                    LuckyNumber.Name = NameValue;
                    LuckyNumber.RewardStatusId = RewardStatusEnum.ACTIVE.Id;
                    LuckyNumber.RowId = Guid.NewGuid();
                    if (decimal.TryParse(ValueValue, out decimal Value))
                    {
                        LuckyNumber.Value = Value;
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Giá trị nhập không hợp lệ");
                        continue;
                    }

                    LuckyNumbers.Add(LuckyNumber);
                }
            }

            if (errorContent.Length > 0)
                return BadRequest(errorContent.ToString());
            
            LuckyNumbers = await LuckyNumberService.Import(LuckyNumbers);
            List<LuckyNumber_LuckyNumberDTO> LuckyNumber_LuckyNumberDTOs = LuckyNumbers
                .Select(c => new LuckyNumber_LuckyNumberDTO(c)).ToList();
            return Ok(LuckyNumber_LuckyNumberDTOs);
        }
        
        [Route(LuckyNumberRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] LuckyNumber_LuckyNumberFilterDTO LuckyNumber_LuckyNumberFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region LuckyNumber
                var LuckyNumberFilter = ConvertFilterDTOToFilterEntity(LuckyNumber_LuckyNumberFilterDTO);
                LuckyNumberFilter.Skip = 0;
                LuckyNumberFilter.Take = int.MaxValue;
                LuckyNumberFilter = await LuckyNumberService.ToFilter(LuckyNumberFilter);
                List<LuckyNumber> LuckyNumbers = await LuckyNumberService.List(LuckyNumberFilter);

                var LuckyNumberHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "RewardStatusId",
                        "RowId",
                    }
                };
                List<object[]> LuckyNumberData = new List<object[]>();
                for (int i = 0; i < LuckyNumbers.Count; i++)
                {
                    var LuckyNumber = LuckyNumbers[i];
                    LuckyNumberData.Add(new Object[]
                    {
                        LuckyNumber.Id,
                        LuckyNumber.Code,
                        LuckyNumber.Name,
                        LuckyNumber.RewardStatusId,
                        LuckyNumber.RowId,
                    });
                }
                excel.GenerateWorksheet("LuckyNumber", LuckyNumberHeaders, LuckyNumberData);
                #endregion
                
                #region RewardStatus
                var RewardStatusFilter = new RewardStatusFilter();
                RewardStatusFilter.Selects = RewardStatusSelect.ALL;
                RewardStatusFilter.OrderBy = RewardStatusOrder.Id;
                RewardStatusFilter.OrderType = OrderType.ASC;
                RewardStatusFilter.Skip = 0;
                RewardStatusFilter.Take = int.MaxValue;
                List<RewardStatus> RewardStatuses = await RewardStatusService.List(RewardStatusFilter);

                var RewardStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> RewardStatusData = new List<object[]>();
                for (int i = 0; i < RewardStatuses.Count; i++)
                {
                    var RewardStatus = RewardStatuses[i];
                    RewardStatusData.Add(new Object[]
                    {
                        RewardStatus.Id,
                        RewardStatus.Code,
                        RewardStatus.Name,
                    });
                }
                excel.GenerateWorksheet("RewardStatus", RewardStatusHeaders, RewardStatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "LuckyNumber.xlsx");
        }

        [Route(LuckyNumberRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] LuckyNumber_LuckyNumberFilterDTO LuckyNumber_LuckyNumberFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            string path = "Templates/Lucky_Number.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "LuckyNumberTemplate.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            LuckyNumberFilter LuckyNumberFilter = new LuckyNumberFilter();
            LuckyNumberFilter = await LuckyNumberService.ToFilter(LuckyNumberFilter);
            if (Id == 0)
            {

            }
            else
            {
                LuckyNumberFilter.Id = new IdFilter { Equal = Id };
                int count = await LuckyNumberService.Count(LuckyNumberFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private LuckyNumber ConvertDTOToEntity(LuckyNumber_LuckyNumberDTO LuckyNumber_LuckyNumberDTO)
        {
            LuckyNumber LuckyNumber = new LuckyNumber();
            LuckyNumber.Id = LuckyNumber_LuckyNumberDTO.Id;
            LuckyNumber.Code = LuckyNumber_LuckyNumberDTO.Code;
            LuckyNumber.Name = LuckyNumber_LuckyNumberDTO.Name;
            LuckyNumber.RewardStatusId = LuckyNumber_LuckyNumberDTO.RewardStatusId;
            LuckyNumber.RowId = LuckyNumber_LuckyNumberDTO.RowId;
            LuckyNumber.RewardStatus = LuckyNumber_LuckyNumberDTO.RewardStatus == null ? null : new RewardStatus
            {
                Id = LuckyNumber_LuckyNumberDTO.RewardStatus.Id,
                Code = LuckyNumber_LuckyNumberDTO.RewardStatus.Code,
                Name = LuckyNumber_LuckyNumberDTO.RewardStatus.Name,
            };
            LuckyNumber.BaseLanguage = CurrentContext.Language;
            return LuckyNumber;
        }

        private LuckyNumberFilter ConvertFilterDTOToFilterEntity(LuckyNumber_LuckyNumberFilterDTO LuckyNumber_LuckyNumberFilterDTO)
        {
            LuckyNumberFilter LuckyNumberFilter = new LuckyNumberFilter();
            LuckyNumberFilter.Selects = LuckyNumberSelect.ALL;
            LuckyNumberFilter.Skip = LuckyNumber_LuckyNumberFilterDTO.Skip;
            LuckyNumberFilter.Take = LuckyNumber_LuckyNumberFilterDTO.Take;
            LuckyNumberFilter.OrderBy = LuckyNumber_LuckyNumberFilterDTO.OrderBy;
            LuckyNumberFilter.OrderType = LuckyNumber_LuckyNumberFilterDTO.OrderType;

            LuckyNumberFilter.Id = LuckyNumber_LuckyNumberFilterDTO.Id;
            LuckyNumberFilter.Code = LuckyNumber_LuckyNumberFilterDTO.Code;
            LuckyNumberFilter.Name = LuckyNumber_LuckyNumberFilterDTO.Name;
            LuckyNumberFilter.RewardStatusId = LuckyNumber_LuckyNumberFilterDTO.RewardStatusId;
            LuckyNumberFilter.RowId = LuckyNumber_LuckyNumberFilterDTO.RowId;
            LuckyNumberFilter.CreatedAt = LuckyNumber_LuckyNumberFilterDTO.CreatedAt;
            LuckyNumberFilter.UpdatedAt = LuckyNumber_LuckyNumberFilterDTO.UpdatedAt;
            return LuckyNumberFilter;
        }
    }
}

