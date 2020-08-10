using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MProblemType;

namespace DMS.Rpc.problem_type
{
    public class ProblemTypeRoute : Root
    {
        public const string Master = Module + "/problem-type/problem-type-master";
        public const string Detail = Module + "/problem-type/problem-type-detail";
        private const string Default = Rpc + Module + "/problem-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        
        

        
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProblemTypeFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ProblemTypeFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ProblemTypeFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ProblemTypeFilter.StatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                
                
                 } },
        };
    }
}