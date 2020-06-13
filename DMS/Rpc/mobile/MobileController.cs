using Common;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MBanner;
using DMS.Services.MImage;
using DMS.Services.MStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{

    public class MobileController : SimpleController
    {
        private IBannerService BannerService;
        private ICurrentContext CurrentContext;
        public MobileController(
            IBannerService BannerService,
            ICurrentContext CurrentContext
        )
        {
            this.BannerService = BannerService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MobileRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Mobile_BannerFilterDTO Mobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = ConvertFilterDTOToFilterEntity(Mobile_BannerFilterDTO);
            BannerFilter = BannerService.ToFilter(BannerFilter);
            int count = await BannerService.Count(BannerFilter);
            return count;
        }

        [Route(MobileRoute.List), HttpPost]
        public async Task<ActionResult<List<Mobile_BannerDTO>>> List([FromBody] Mobile_BannerFilterDTO Mobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = ConvertFilterDTOToFilterEntity(Mobile_BannerFilterDTO);
            BannerFilter = BannerService.ToFilter(BannerFilter);
            List<Banner> Banners = await BannerService.List(BannerFilter);
            List<Mobile_BannerDTO> Mobile_BannerDTOs = Banners
                .Select(c => new Mobile_BannerDTO(c)).ToList();
            return Mobile_BannerDTOs;
        }

        [Route(MobileRoute.Get), HttpPost]
        public async Task<ActionResult<Mobile_BannerDTO>> Get([FromBody]Mobile_BannerDTO Mobile_BannerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Banner Banner = await BannerService.Get(Mobile_BannerDTO.Id);
            return new Mobile_BannerDTO(Banner);
        }

        private BannerFilter ConvertFilterDTOToFilterEntity(Mobile_BannerFilterDTO Mobile_BannerFilterDTO)
        {
            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = Mobile_BannerFilterDTO.Skip;
            BannerFilter.Take = Mobile_BannerFilterDTO.Take;
            BannerFilter.OrderBy = Mobile_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = Mobile_BannerFilterDTO.OrderType;

            BannerFilter.Id = Mobile_BannerFilterDTO.Id;
            BannerFilter.Code = Mobile_BannerFilterDTO.Code;
            BannerFilter.Title = Mobile_BannerFilterDTO.Title;
            BannerFilter.Priority = Mobile_BannerFilterDTO.Priority;
            BannerFilter.Content = Mobile_BannerFilterDTO.Content;
            BannerFilter.CreatorId = Mobile_BannerFilterDTO.CreatorId;
            BannerFilter.ImageId = Mobile_BannerFilterDTO.ImageId;
            BannerFilter.StatusId = Mobile_BannerFilterDTO.StatusId;
            BannerFilter.CreatedAt = Mobile_BannerFilterDTO.CreatedAt;
            BannerFilter.UpdatedAt = Mobile_BannerFilterDTO.UpdatedAt;
            return BannerFilter;
        }
    }
}

