using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MERoute
{
    public interface IERouteValidator : IServiceScoped
    {
        Task<bool> Create(ERoute ERoute);
        Task<bool> Update(ERoute ERoute);
        Task<bool> Delete(ERoute ERoute);
        Task<bool> BulkDelete(List<ERoute> ERoutes);
        Task<bool> Import(List<ERoute> ERoutes);
    }

    public class ERouteValidator : IERouteValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeOverLength,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            SaleEmployeeNotExisted,
            SaleEmployeeEmpty,
            ERouteTypeNotExisted,
            StatusNotExisted,
            StartDateEmpty,
            EndDateEmpty,
            StoreNotExisted,
            ERouteInUsed,
            ERouteContentsEmpty
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ERouteValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ERoute ERoute)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ERoute.Id },
                Selects = ERouteSelect.Id
            };

            int count = await UOW.ERouteRepository.Count(ERouteFilter);
            if (count == 0)
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(ERoute ERoute)
        {
            if (string.IsNullOrWhiteSpace(ERoute.Code))
            {
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Code), ErrorCode.CodeEmpty);
            }
            else if (ERoute.Code.Length > 255)
            {
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Code), ErrorCode.CodeOverLength);
            }
            else
            {
                var Code = ERoute.Code;
                if (ERoute.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(ERoute.Code))
                {
                    ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                ERouteFilter ERouteFilter = new ERouteFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = ERoute.Id },
                    Code = new StringFilter { Equal = ERoute.Code },
                    Selects = ERouteSelect.Code
                };

                int count = await UOW.ERouteRepository.Count(ERouteFilter);
                if (count != 0)
                    ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Code), ErrorCode.CodeExisted);
            }

            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateName(ERoute ERoute)
        {
            if (string.IsNullOrWhiteSpace(ERoute.Name))
            {
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Name), ErrorCode.NameEmpty);
            }
            else if (ERoute.Name.Length > 255)
            {
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Name), ErrorCode.NameOverLength);
            }
            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateSaleEmployee(ERoute ERoute)
        {
            if (ERoute.SaleEmployeeId == 0)
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.SaleEmployee), ErrorCode.SaleEmployeeEmpty);
            else
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    Selects = AppUserSelect.Id,
                    Id = new IdFilter { Equal = ERoute.SaleEmployeeId },
                    OrganizationId = new IdFilter()
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count == 0)
                    ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.SaleEmployee), ErrorCode.SaleEmployeeNotExisted);
            }

            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateERouteType(ERoute ERoute)
        {
            if (ERoute.ERouteTypeId.HasValue)
            {
                if (ERouteTypeEnum.PERMANENT.Id != ERoute.ERouteTypeId && ERouteTypeEnum.INCURRED.Id != ERoute.ERouteTypeId)
                    ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.ERouteType), ErrorCode.ERouteTypeNotExisted);
            }
            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateStatusId(ERoute ERoute)
        {
            if (StatusEnum.ACTIVE.Id != ERoute.StatusId && StatusEnum.INACTIVE.Id != ERoute.StatusId)
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Status), ErrorCode.StatusNotExisted);
            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateStartDateAndEndDate(ERoute ERoute)
        {
            if (ERoute.ERouteTypeId.HasValue)
            {
                if (ERoute.ERouteTypeId == ERouteTypeEnum.PERMANENT.Id)
                {
                    if (ERoute.StartDate == default(DateTime))
                        ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.StartDate), ErrorCode.StartDateEmpty);
                }

                if (ERoute.ERouteTypeId == ERouteTypeEnum.INCURRED.Id)
                {
                    if (ERoute.StartDate == default(DateTime))
                        ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.StartDate), ErrorCode.StartDateEmpty);
                    if (ERoute.EndDate == null || ERoute.EndDate == default(DateTime))
                        ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.EndDate), ErrorCode.EndDateEmpty);
                }
            }

            return ERoute.IsValidated;
        }

        private async Task<bool> ValidateStore(ERoute ERoute)
        {
            if (ERoute.ERouteContents != null && ERoute.ERouteContents.Any())
            {
                var IdsStore = ERoute.ERouteContents.Select(x => x.StoreId).ToList();

                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = StoreSelect.Id,
                    Id = new IdFilter { In = IdsStore }
                };

                var IdsInDB = (await UOW.StoreRepository.List(StoreFilter)).Select(x => x.Id).ToList();
                var listIdsNotExisted = IdsStore.Except(IdsInDB);
                foreach (var ERouteContent in ERoute.ERouteContents)
                {
                    if (listIdsNotExisted.Contains(ERouteContent.StoreId))
                        ERouteContent.AddError(nameof(ERouteValidator), nameof(ERouteContent.Store), ErrorCode.StoreNotExisted);
                }
            }
            else
            {
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.ERouteContents), ErrorCode.ERouteContentsEmpty);
            }
            return ERoute.IsValidated;
        }

        public async Task<bool> Create(ERoute ERoute)
        {
            await ValidateCode(ERoute);
            await ValidateName(ERoute);
            await ValidateSaleEmployee(ERoute);
            await ValidateERouteType(ERoute);
            await ValidateStatusId(ERoute);
            await ValidateStartDateAndEndDate(ERoute);
            await ValidateStore(ERoute);
            return ERoute.IsValidated;
        }

        public async Task<bool> Update(ERoute ERoute)
        {
            if (await ValidateId(ERoute))
            {
                await ValidateCode(ERoute);
                await ValidateName(ERoute);
                await ValidateSaleEmployee(ERoute);
                await ValidateERouteType(ERoute);
                await ValidateStatusId(ERoute);
                await ValidateStartDateAndEndDate(ERoute);
                await ValidateStore(ERoute);
            }
            return ERoute.IsValidated;
        }

        public async Task<bool> Delete(ERoute ERoute)
        {
            if (await ValidateId(ERoute))
            {
                if (ERoute.RequestStateId != RequestStateEnum.NEW.Id)
                    ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.RequestState), ErrorCode.ERouteInUsed);
            }
            return ERoute.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ERoute> ERoutes)
        {
            foreach (ERoute ERoute in ERoutes)
            {
                await Delete(ERoute);
            }
            return ERoutes.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<ERoute> ERoutes)
        {
            return true;
        }
    }
}
