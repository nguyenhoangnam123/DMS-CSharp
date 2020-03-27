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
using DMS.Services.MSexService;

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
        Task<List<AppUser>> Import(DataFile DataFile);
        Task<DataFile> Export(AppUserFilter AppUserFilter);
        AppUserFilter ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        private ISexService SexService;

        public AppUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            ISexService SexService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
            this.SexService = SexService;
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
                AppUser.Id = 0;
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

        public async Task<List<AppUser>> Import(DataFile DataFile)
        {
            List<AppUser> AppUsers = new List<AppUser>(); 
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return AppUsers;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int DisplayNameColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int PhoneColumn = 5 + StartColumn;
                int UserStatusIdColumn = 6 + StartColumn;
                int SexIdColumn = 7 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i + StartRow, UsernameColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i + StartRow, PasswordColumn].Value?.ToString();
                    string DisplayNameValue = worksheet.Cells[i + StartRow, DisplayNameColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string UserStatusIdValue = worksheet.Cells[i + StartRow, UserStatusIdColumn].Value?.ToString();
                    string SexIdValue = worksheet.Cells[i + StartRow, SexIdColumn].Value?.ToString();
                    AppUser AppUser = new AppUser(); 

                    AppUser.Username = UsernameValue;
                    AppUser.Password = PasswordValue;
                    AppUser.DisplayName = DisplayNameValue;
                    AppUser.Email = EmailValue;
                    AppUser.Phone = PhoneValue;
                    AppUser.UserStatusId = int.Parse(UserStatusIdValue);
                    AppUser.SexId = long.Parse(SexIdValue);
                    //AppUser.Sex = await SexService.Get(long.Parse(SexIdValue));

                    await Create(AppUser);
                    AppUsers.Add(AppUser);
                }
            }

            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;

            try
            {
                AppUsers.ForEach(a => a.Id = 0);
                await UOW.Begin();
                await UOW.AppUserRepository.BulkMerge(AppUsers);
                await UOW.Commit();

                await Logging.CreateAuditLog(AppUsers, new { }, nameof(AppUserService));
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
                worksheet.Cells[1, UserStatusIdColumn].Value = nameof(AppUser.StatusId);

                for (int i = 0; i < AppUsers.Count; i++)
                {
                    AppUser AppUser = AppUsers[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = AppUser.Id;
                    worksheet.Cells[i + StartRow, UsernameColumn].Value = AppUser.Username;
                    worksheet.Cells[i + StartRow, PasswordColumn].Value = AppUser.Password;
                    worksheet.Cells[i + StartRow, DisplayNameColumn].Value = AppUser.DisplayName;
                    worksheet.Cells[i + StartRow, EmailColumn].Value = AppUser.Email;
                    worksheet.Cells[i + StartRow, PhoneColumn].Value = AppUser.Phone;
                    worksheet.Cells[i + StartRow, UserStatusIdColumn].Value = AppUser.StatusId;
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
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
