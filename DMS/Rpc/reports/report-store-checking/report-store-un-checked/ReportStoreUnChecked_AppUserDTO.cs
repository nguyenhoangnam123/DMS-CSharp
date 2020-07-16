﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.reports.report_store_checking.report_store_un_checked
{
    public class ReportStoreUnChecked_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }

        public ReportStoreUnChecked_AppUserDTO() { }
        public ReportStoreUnChecked_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Errors = AppUser.Errors;
        }
    }

    public class ReportStoreUnChecked_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
