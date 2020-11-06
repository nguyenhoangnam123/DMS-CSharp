using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MLuckyNumber;
using DMS.Services.MRewardStatus;

namespace DMS.Rpc.lucky_number
{
    public partial class LuckyNumberController : RpcController
    {
        [Route(LuckyNumberRoute.FilterListRewardStatus), HttpPost]
        public async Task<List<LuckyNumber_RewardStatusDTO>> FilterListRewardStatus([FromBody] LuckyNumber_RewardStatusFilterDTO LuckyNumber_RewardStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RewardStatusFilter RewardStatusFilter = new RewardStatusFilter();
            RewardStatusFilter.Skip = 0;
            RewardStatusFilter.Take = int.MaxValue;
            RewardStatusFilter.Take = 20;
            RewardStatusFilter.OrderBy = RewardStatusOrder.Id;
            RewardStatusFilter.OrderType = OrderType.ASC;
            RewardStatusFilter.Selects = RewardStatusSelect.ALL;

            List<RewardStatus> RewardStatuses = await RewardStatusService.List(RewardStatusFilter);
            List<LuckyNumber_RewardStatusDTO> LuckyNumber_RewardStatusDTOs = RewardStatuses
                .Select(x => new LuckyNumber_RewardStatusDTO(x)).ToList();
            return LuckyNumber_RewardStatusDTOs;
        }

        [Route(LuckyNumberRoute.SingleListRewardStatus), HttpPost]
        public async Task<List<LuckyNumber_RewardStatusDTO>> SingleListRewardStatus([FromBody] LuckyNumber_RewardStatusFilterDTO LuckyNumber_RewardStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RewardStatusFilter RewardStatusFilter = new RewardStatusFilter();
            RewardStatusFilter.Skip = 0;
            RewardStatusFilter.Take = int.MaxValue;
            RewardStatusFilter.Take = 20;
            RewardStatusFilter.OrderBy = RewardStatusOrder.Id;
            RewardStatusFilter.OrderType = OrderType.ASC;
            RewardStatusFilter.Selects = RewardStatusSelect.ALL;

            List<RewardStatus> RewardStatuses = await RewardStatusService.List(RewardStatusFilter);
            List<LuckyNumber_RewardStatusDTO> LuckyNumber_RewardStatusDTOs = RewardStatuses
                .Select(x => new LuckyNumber_RewardStatusDTO(x)).ToList();
            return LuckyNumber_RewardStatusDTOs;
        }

    }
}

