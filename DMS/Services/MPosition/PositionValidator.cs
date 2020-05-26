using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MPosition
{
    public interface IPositionValidator : IServiceScoped
    {
        Task<bool> Create(Position Position);
        Task<bool> Update(Position Position);
        Task<bool> Delete(Position Position);
        Task<bool> BulkDelete(List<Position> Positions);
        Task<bool> Import(List<Position> Positions);
    }

    public class PositionValidator : IPositionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PositionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Position Position)
        {
            PositionFilter PositionFilter = new PositionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Position.Id },
                Selects = PositionSelect.Id
            };

            int count = await UOW.PositionRepository.Count(PositionFilter);
            if (count == 0)
                Position.AddError(nameof(PositionValidator), nameof(Position.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Position Position)
        {
            return Position.IsValidated;
        }

        public async Task<bool> Update(Position Position)
        {
            if (await ValidateId(Position))
            {
            }
            return Position.IsValidated;
        }

        public async Task<bool> Delete(Position Position)
        {
            if (await ValidateId(Position))
            {
            }
            return Position.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Position> Positions)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Position> Positions)
        {
            return true;
        }
    }
}
