﻿using DMS.Common;
using DMS.Entities;
using DMS.Helpers;
using DMS.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MFile
{
    public interface IFileService : IServiceScoped
    {
        Task<List<DMS.Entities.File>> List(DMS.Entities.FileFilter FileFilter);
        Task<DMS.Entities.File> Get(long Id);
        Task<DMS.Entities.File> Create(DMS.Entities.File File, string path);
        Task<DMS.Entities.File> Delete(DMS.Entities.File File);
    }
    public class FileService : BaseService, IFileService
    {
        private IUOW UOW;
        private ILogging Logging;
        private IFileValidator FileValidator;
        private ICurrentContext CurrentContext;

        public FileService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFileValidator FileValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FileValidator = FileValidator;
        }
        public async Task<File> Create(File File, string path)
        {
            // save original File
            RestClient restClient = new RestClient(InternalServices.UTILS);
            RestRequest restRequest = new RestRequest("/rpc/utils/file/upload");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddFile("file", File.Content, File.Name);
            restRequest.AddParameter("path", path);
            try
            {
                var response = restClient.Execute<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    File.Id = response.Data.Id;
                    File.Path = "/rpc/utils/file/download" + response.Data.Path;
                    File.RowId = response.Data.RowId;
                }
                await UOW.Begin();
                await UOW.FileRepository.Create(File);
                await UOW.Commit();
                return File;
            }
            catch
            {
                return null;
            }
        }

        public async Task<File> Delete(File File)
        {
            if (!await FileValidator.Delete(File))
                return File;

            try
            {
                await UOW.Begin();
                await UOW.FileRepository.Delete(File);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, File, nameof(FileService));
                return File;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(FileService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(FileService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<File> Get(long Id)
        {
            DMS.Entities.File File = await UOW.FileRepository.Get(Id);
            if (File == null)
                return null;
            return File;
        }

        public async Task<List<File>> List(FileFilter FileFilter)
        {
            try
            {
                List<DMS.Entities.File> Files = await UOW.FileRepository.List(FileFilter);
                return Files;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(FileService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(FileService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
