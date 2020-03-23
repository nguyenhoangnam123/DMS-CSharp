using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MSupplier;
using DMS.Services.MStatus;

namespace DMS.Rpc.supplier
{
    public class SupplierRoute : Root
    {
        public const string Master = Module + "/supplier/supplier-master";
        public const string Detail = Module + "/supplier/supplier-detail";
        private const string Default = Rpc + Module + "/supplier";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(SupplierFilter.Id), FieldType.ID },
            { nameof(SupplierFilter.Code), FieldType.STRING },
            { nameof(SupplierFilter.Name), FieldType.STRING },
            { nameof(SupplierFilter.TaxCode), FieldType.STRING },
            { nameof(SupplierFilter.StatusId), FieldType.ID },
        };
    }

    public class SupplierController : RpcController
    {
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private ICurrentContext CurrentContext;
        public SupplierController(
            IStatusService StatusService,
            ISupplierService SupplierService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SupplierRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            int count = await SupplierService.Count(SupplierFilter);
            return count;
        }

        [Route(SupplierRoute.List), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> List([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Get), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Get([FromBody]Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = await SupplierService.Get(Supplier_SupplierDTO.Id);
            return new Supplier_SupplierDTO(Supplier);
        }

        [Route(SupplierRoute.Create), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Create([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Create(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Update), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Update([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Update(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Delete), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Delete([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Delete(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Import), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Supplier> Suppliers = await SupplierService.Import(DataFile);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            DataFile DataFile = await SupplierService.Export(SupplierFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(SupplierRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Id.In = Ids;
            SupplierFilter.Selects = SupplierSelect.Id;
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = int.MaxValue;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            Suppliers = await SupplierService.BulkDelete(Suppliers);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            if (Id > 0)
                SupplierFilter.Id = new IdFilter { Equal = Id };
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            int count = await SupplierService.Count(SupplierFilter);
            if (count == 0)
                return false;
            return true;
        }

        private Supplier ConvertDTOToEntity(Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            Supplier Supplier = new Supplier();
            Supplier.Id = Supplier_SupplierDTO.Id;
            Supplier.Code = Supplier_SupplierDTO.Code;
            Supplier.Name = Supplier_SupplierDTO.Name;
            Supplier.TaxCode = Supplier_SupplierDTO.TaxCode;
            Supplier.StatusId = Supplier_SupplierDTO.StatusId;
            Supplier.Status = Supplier_SupplierDTO.Status == null ? null : new Status
            {
                Id = Supplier_SupplierDTO.Status.Id,
                Code = Supplier_SupplierDTO.Status.Code,
                Name = Supplier_SupplierDTO.Status.Name,
            };
            Supplier.BaseLanguage = CurrentContext.Language;
            return Supplier;
        }

        private SupplierFilter ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Skip = Supplier_SupplierFilterDTO.Skip;
            SupplierFilter.Take = Supplier_SupplierFilterDTO.Take;
            SupplierFilter.OrderBy = Supplier_SupplierFilterDTO.OrderBy;
            SupplierFilter.OrderType = Supplier_SupplierFilterDTO.OrderType;

            SupplierFilter.Id = Supplier_SupplierFilterDTO.Id;
            SupplierFilter.Code = Supplier_SupplierFilterDTO.Code;
            SupplierFilter.Name = Supplier_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Supplier_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = Supplier_SupplierFilterDTO.StatusId;
            return SupplierFilter;
        }

        [Route(SupplierRoute.SingleListStatus), HttpPost]
        public async Task<List<Supplier_StatusDTO>> SingleListStatus([FromBody] Supplier_StatusFilterDTO Supplier_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Supplier_StatusFilterDTO.Id;
            StatusFilter.Code = Supplier_StatusFilterDTO.Code;
            StatusFilter.Name = Supplier_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Supplier_StatusDTO> Supplier_StatusDTOs = Statuses
                .Select(x => new Supplier_StatusDTO(x)).ToList();
            return Supplier_StatusDTOs;
        }

    }
}

