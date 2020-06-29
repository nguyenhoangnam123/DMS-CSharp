﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreScoutingStatusDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Mobile_StoreScoutingStatusDTO() { }
        public Mobile_StoreScoutingStatusDTO(StoreScoutingStatus StoreScoutingStatus)
        {

            this.Id = StoreScoutingStatus.Id;

            this.Code = StoreScoutingStatus.Code;

            this.Name = StoreScoutingStatus.Name;

            this.Errors = StoreScoutingStatus.Errors;
        }
    }

    public class Mobile_StoreScoutingStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StoreScoutingStatusOrder OrderBy { get; set; }
    }
}