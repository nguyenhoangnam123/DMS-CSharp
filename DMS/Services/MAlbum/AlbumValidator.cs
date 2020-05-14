using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MAlbum
{
    public interface IAlbumValidator : IServiceScoped
    {
        Task<bool> Create(Album Album);
        Task<bool> Update(Album Album);
        Task<bool> Delete(Album Album);
        Task<bool> BulkDelete(List<Album> Albums);
        Task<bool> Import(List<Album> Albums);
    }

    public class AlbumValidator : IAlbumValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AlbumValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Album Album)
        {
            AlbumFilter AlbumFilter = new AlbumFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Album.Id },
                Selects = AlbumSelect.Id
            };

            int count = await UOW.AlbumRepository.Count(AlbumFilter);
            if (count == 0)
                Album.AddError(nameof(AlbumValidator), nameof(Album.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Album Album)
        {
            return Album.IsValidated;
        }

        public async Task<bool> Update(Album Album)
        {
            if (await ValidateId(Album))
            {
            }
            return Album.IsValidated;
        }

        public async Task<bool> Delete(Album Album)
        {
            if (await ValidateId(Album))
            {
            }
            return Album.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Album> Albums)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Album> Albums)
        {
            return true;
        }
    }
}
