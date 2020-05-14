using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MAlbum
{
    public interface IAlbumService :  IServiceScoped
    {
        Task<int> Count(AlbumFilter AlbumFilter);
        Task<List<Album>> List(AlbumFilter AlbumFilter);
        Task<Album> Get(long Id);
        Task<Album> Create(Album Album);
        Task<Album> Update(Album Album);
        Task<Album> Delete(Album Album);
        Task<List<Album>> BulkDelete(List<Album> Albums);
        Task<List<Album>> Import(List<Album> Albums);
        AlbumFilter ToFilter(AlbumFilter AlbumFilter);
    }

    public class AlbumService : BaseService, IAlbumService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAlbumValidator AlbumValidator;

        public AlbumService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAlbumValidator AlbumValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AlbumValidator = AlbumValidator;
        }
        public async Task<int> Count(AlbumFilter AlbumFilter)
        {
            try
            {
                int result = await UOW.AlbumRepository.Count(AlbumFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Album>> List(AlbumFilter AlbumFilter)
        {
            try
            {
                List<Album> Albums = await UOW.AlbumRepository.List(AlbumFilter);
                return Albums;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Album> Get(long Id)
        {
            Album Album = await UOW.AlbumRepository.Get(Id);
            if (Album == null)
                return null;
            return Album;
        }
       
        public async Task<Album> Create(Album Album)
        {
            if (!await AlbumValidator.Create(Album))
                return Album;

            try
            {
                await UOW.Begin();
                await UOW.AlbumRepository.Create(Album);
                await UOW.Commit();

                await Logging.CreateAuditLog(Album, new { }, nameof(AlbumService));
                return await UOW.AlbumRepository.Get(Album.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Album> Update(Album Album)
        {
            if (!await AlbumValidator.Update(Album))
                return Album;
            try
            {
                var oldData = await UOW.AlbumRepository.Get(Album.Id);

                await UOW.Begin();
                await UOW.AlbumRepository.Update(Album);
                await UOW.Commit();

                var newData = await UOW.AlbumRepository.Get(Album.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(AlbumService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Album> Delete(Album Album)
        {
            if (!await AlbumValidator.Delete(Album))
                return Album;

            try
            {
                await UOW.Begin();
                await UOW.AlbumRepository.Delete(Album);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Album, nameof(AlbumService));
                return Album;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Album>> BulkDelete(List<Album> Albums)
        {
            if (!await AlbumValidator.BulkDelete(Albums))
                return Albums;

            try
            {
                await UOW.Begin();
                await UOW.AlbumRepository.BulkDelete(Albums);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Albums, nameof(AlbumService));
                return Albums;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Album>> Import(List<Album> Albums)
        {
            if (!await AlbumValidator.Import(Albums))
                return Albums;
            try
            {
                await UOW.Begin();
                await UOW.AlbumRepository.BulkMerge(Albums);
                await UOW.Commit();

                await Logging.CreateAuditLog(Albums, new { }, nameof(AlbumService));
                return Albums;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(AlbumService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public AlbumFilter ToFilter(AlbumFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AlbumFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AlbumFilter subFilter = new AlbumFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
