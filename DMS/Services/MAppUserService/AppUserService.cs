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
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Helpers;
using Microsoft.AspNetCore.Http;

namespace DMS.Services.MAppUser
{
    public interface IAppUserService : IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(IFormFile DataFile);
        Task<DataFile> Export(AppUserFilter AppUserFilter);
        AppUserFilter ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;

        public AppUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            return AppUser;
        }

        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                await UOW.Begin();
                await UOW.AppUserRepository.Create(AppUser);
                await UOW.Commit();

                await Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return await UOW.AppUserRepository.Get(AppUser.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);

                await UOW.Begin();
                await UOW.AppUserRepository.Update(AppUser);
                await UOW.Commit();

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.Begin();
                await UOW.AppUserRepository.Delete(AppUser);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.Begin();
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, AppUsers, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<AppUser>> Import(IFormFile file)
        {
            var lts = new List<AppUser>();
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                var fileLocation = await Utils.CreateFileImport(file);
                using (ExcelPackage package = new ExcelPackage(fileLocation))
                {
                    //ExcelWorksheet workSheet = package.Workbook.Worksheets["Table1"];
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    int StartColumn = 1;
                    int StartRow = 2;
                    int IdColumn = 0 + StartColumn;
                    int Username = 1 + StartColumn;
                    int Password = 2 + StartColumn;
                    int DisplayName = 3 + StartColumn;
                    int Email = 4 + StartColumn;
                    int Phone = 5 + StartColumn;
                    int UserStatusId = 6 + StartColumn;
                    int SexId = 7 + StartColumn;
                    for (int i = StartRow; i <= workSheet.Dimension.Rows; i++)
                    {
                        AppUser item = new AppUser();
                        item.Username = workSheet.Cells[i, Username].Value.ToString();
                        item.Password = workSheet.Cells[i, Password].Value.ToString();
                        item.DisplayName = workSheet.Cells[i, DisplayName].Value.ToString();
                        item.Email = workSheet.Cells[i, Email].Value.ToString();
                        item.Phone = workSheet.Cells[i, Phone].Value.ToString();
                        item.UserStatusId = int.Parse(workSheet.Cells[i, UserStatusId].Value.ToString() ?? "0"); 

                        await Create(item);
                        lts.Add(item);
                    } 
                }
            } 

            if (!await AppUserValidator.Import(lts))
                return lts;

            try
            {
                await UOW.Begin();
                await UOW.AppUserRepository.BulkMerge(lts);
                await UOW.Commit();

                await Logging.CreateAuditLog(lts, new { }, nameof(AppUserService));
                return lts;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(AppUserFilter AppUserFilter)
        {
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(AppUser);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int DisplayNameColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int PhoneColumn = 5 + StartColumn;
                int UserStatusIdColumn = 6 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(AppUser.Id);
                worksheet.Cells[1, UsernameColumn].Value = nameof(AppUser.Username);
                worksheet.Cells[1, PasswordColumn].Value = nameof(AppUser.Password);
                worksheet.Cells[1, DisplayNameColumn].Value = nameof(AppUser.DisplayName);
                worksheet.Cells[1, EmailColumn].Value = nameof(AppUser.Email);
                worksheet.Cells[1, PhoneColumn].Value = nameof(AppUser.Phone);
                worksheet.Cells[1, UserStatusIdColumn].Value = nameof(AppUser.UserStatusId);

                for (int i = 0; i < AppUsers.Count; i++)
                {
                    AppUser AppUser = AppUsers[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = AppUser.Id;
                    worksheet.Cells[i + StartRow, UsernameColumn].Value = AppUser.Username;
                    worksheet.Cells[i + StartRow, PasswordColumn].Value = AppUser.Password;
                    worksheet.Cells[i + StartRow, DisplayNameColumn].Value = AppUser.DisplayName;
                    worksheet.Cells[i + StartRow, EmailColumn].Value = AppUser.Email;
                    worksheet.Cells[i + StartRow, PhoneColumn].Value = AppUser.Phone;
                    worksheet.Cells[i + StartRow, UserStatusIdColumn].Value = AppUser.UserStatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(AppUser),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public AppUserFilter ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Username))
                    subFilter.Username = Map(subFilter.Username, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Password))
                    subFilter.Password = Map(subFilter.Password, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DisplayName))
                    subFilter.DisplayName = Map(subFilter.DisplayName, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Email))
                    subFilter.Email = Map(subFilter.Email, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Phone))
                    subFilter.Phone = Map(subFilter.Phone, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UserStatusId))
                    subFilter.UserStatusId = Map(subFilter.UserStatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
