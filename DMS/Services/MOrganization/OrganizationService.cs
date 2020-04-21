using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MOrganization
{
    public interface IOrganizationService : IServiceScoped
    {
        Task<int> Count(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(OrganizationFilter OrganizationFilter);
        Task<Organization> Get(long Id);
        Task<DataFile> Export(OrganizationFilter OrganizationFilter);
        OrganizationFilter ToFilter(OrganizationFilter OrganizationFilter);
    }

    public class OrganizationService : BaseService, IOrganizationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IOrganizationValidator OrganizationValidator;

        public OrganizationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IOrganizationValidator OrganizationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.OrganizationValidator = OrganizationValidator;
        }
        public async Task<int> Count(OrganizationFilter OrganizationFilter)
        {
            try
            {
                int result = await UOW.OrganizationRepository.Count(OrganizationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrganizationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Organization>> List(OrganizationFilter OrganizationFilter)
        {
            try
            {
                List<Organization> Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
                return Organizations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrganizationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Organization> Get(long Id)
        {
            Organization Organization = await UOW.OrganizationRepository.Get(Id);
            if (Organization == null)
                return null;
            return Organization;
        }

        public async Task<DataFile> Export(OrganizationFilter OrganizationFilter)
        {
            List<Organization> Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Organization);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int LevelColumn = 5 + StartColumn;
                int StatusIdColumn = 6 + StartColumn;
                int PhoneColumn = 7 + StartColumn;
                int AddressColumn = 8 + StartColumn;
                int LatitudeColumn = 9 + StartColumn;
                int LongitudeColumn = 10 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Organization.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Organization.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Organization.Name);
                worksheet.Cells[1, ParentIdColumn].Value = nameof(Organization.ParentId);
                worksheet.Cells[1, PathColumn].Value = nameof(Organization.Path);
                worksheet.Cells[1, LevelColumn].Value = nameof(Organization.Level);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Organization.StatusId);
                worksheet.Cells[1, PhoneColumn].Value = nameof(Organization.Phone);
                worksheet.Cells[1, AddressColumn].Value = nameof(Organization.Address);
                worksheet.Cells[1, LatitudeColumn].Value = nameof(Organization.Latitude);
                worksheet.Cells[1, LongitudeColumn].Value = nameof(Organization.Longitude);

                for (int i = 0; i < Organizations.Count; i++)
                {
                    Organization Organization = Organizations[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Organization.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Organization.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Organization.Name;
                    worksheet.Cells[i + StartRow, ParentIdColumn].Value = Organization.ParentId;
                    worksheet.Cells[i + StartRow, PathColumn].Value = Organization.Path;
                    worksheet.Cells[i + StartRow, LevelColumn].Value = Organization.Level;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Organization.StatusId;
                    worksheet.Cells[i + StartRow, PhoneColumn].Value = Organization.Phone;
                    worksheet.Cells[i + StartRow, AddressColumn].Value = Organization.Address;
                    worksheet.Cells[i + StartRow, LatitudeColumn].Value = Organization.Latitude;
                    worksheet.Cells[i + StartRow, LongitudeColumn].Value = Organization.Longitude;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Organization),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public OrganizationFilter ToFilter(OrganizationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<OrganizationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                OrganizationFilter subFilter = new OrganizationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = Map(subFilter.ParentId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Path))
                        subFilter.Path = Map(subFilter.Path, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Level))
                        subFilter.Level = Map(subFilter.Level, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = Map(subFilter.Phone, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = Map(subFilter.Address, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Latitude))
                        subFilter.Latitude = Map(subFilter.Latitude, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Longitude))
                        subFilter.Longitude = Map(subFilter.Longitude, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
