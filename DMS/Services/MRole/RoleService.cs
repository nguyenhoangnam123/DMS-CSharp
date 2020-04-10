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

namespace DMS.Services.MRole
{
    public interface IRoleService : IServiceScoped
    {
        Task<int> Count(RoleFilter RoleFilter);
        Task<List<Role>> List(RoleFilter RoleFilter);
        Task<Role> Get(long Id);
        Task<Role> Create(Role Role);
        Task<Role> Update(Role Role);
        Task<Role> Delete(Role Role);
        Task<List<Role>> BulkDelete(List<Role> Roles);
        Task<List<Role>> Import(DataFile DataFile);
        Task<DataFile> Export(RoleFilter RoleFilter);
        RoleFilter ToFilter(RoleFilter RoleFilter);
    }

    public class RoleService : BaseService, IRoleService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IRoleValidator RoleValidator;

        public RoleService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRoleValidator RoleValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RoleValidator = RoleValidator;
        }
        public async Task<int> Count(RoleFilter RoleFilter)
        {
            try
            {
                int result = await UOW.RoleRepository.Count(RoleFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Role>> List(RoleFilter RoleFilter)
        {
            try
            {
                List<Role> Roles = await UOW.RoleRepository.List(RoleFilter);
                return Roles;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Role> Get(long Id)
        {
            Role Role = await UOW.RoleRepository.Get(Id);
            if (Role == null)
                return null;
            return Role;
        }

        public async Task<Role> Create(Role Role)
        {
            if (!await RoleValidator.Create(Role))
                return Role;

            try
            {
                Role.Id = 0;
                await UOW.Begin();
                await UOW.RoleRepository.Create(Role);
                await UOW.Commit();

                await Logging.CreateAuditLog(Role, new { }, nameof(RoleService));
                return await UOW.RoleRepository.Get(Role.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Role> Update(Role Role)
        {
            if (!await RoleValidator.Update(Role))
                return Role;
            try
            {
                var oldData = await UOW.RoleRepository.Get(Role.Id);

                await UOW.Begin();
                await UOW.RoleRepository.Update(Role);
                await UOW.Commit();

                var newData = await UOW.RoleRepository.Get(Role.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(RoleService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Role> Delete(Role Role)
        {
            if (!await RoleValidator.Delete(Role))
                return Role;

            try
            {
                await UOW.Begin();
                await UOW.RoleRepository.Delete(Role);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Role, nameof(RoleService));
                return Role;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Role>> BulkDelete(List<Role> Roles)
        {
            if (!await RoleValidator.BulkDelete(Roles))
                return Roles;

            try
            {
                await UOW.Begin();
                await UOW.RoleRepository.BulkDelete(Roles);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Roles, nameof(RoleService));
                return Roles;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Role>> Import(DataFile DataFile)
        {
            List<Role> Roles = new List<Role>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Roles;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    Role Role = new Role();
                    Role.Code = CodeValue;
                    Role.Name = NameValue;
                    Roles.Add(Role);
                }
            }

            if (!await RoleValidator.Import(Roles))
                return Roles;

            try
            {
                Roles.ForEach(r => r.Id = 0);
                await UOW.Begin();
                await UOW.RoleRepository.BulkMerge(Roles);
                await UOW.Commit();

                await Logging.CreateAuditLog(Roles, new { }, nameof(RoleService));
                return Roles;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(RoleFilter RoleFilter)
        {
            List<Role> Roles = await UOW.RoleRepository.List(RoleFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Role);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Role.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Role.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Role.Name);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Role.StatusId);

                for (int i = 0; i < Roles.Count; i++)
                {
                    Role Role = Roles[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Role.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Role.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Role.Name;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Role.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Role),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public RoleFilter ToFilter(RoleFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<RoleFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                RoleFilter subFilter = new RoleFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
