using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public class FilterPermissionDefinition
    {
        public string Name { get; private set; }
        public FieldType Type { get; set; }
        public string Value
        {
            set
            {
                List<string> tmp;
                switch (Type)
                {
                    case FieldType.ID:
                        Ids = value.Split(";").Select(v => long.TryParse(v, out long l) ? l : 0).ToList();
                        break;
                    case FieldType.LONG:
                        tmp = value.Split(";").ToList();
                        if (tmp.Count > 0)
                            StartLong = long.TryParse(tmp[0], out long l0) ? l0 : 0;
                        if (tmp.Count > 1)
                            EndLong = long.TryParse(tmp[1], out long l1) ? l1 : 0;
                        break;
                    case FieldType.DECIMAL:
                        tmp = value.Split(";").ToList();
                        if (tmp.Count > 0)
                            StartDecimal = decimal.TryParse(tmp[0], out decimal db0) ? db0 : 0;
                        if (tmp.Count > 1)
                            EndDecimal = decimal.TryParse(tmp[1], out decimal db1) ? db1 : 0;
                        break;
                    case FieldType.DATE:
                        tmp = value.Split(";").ToList();
                        if (tmp.Count > 0)
                            StartDate = DateTime.TryParse(tmp[0], out DateTime d0) ? d0 : DateTime.MinValue;
                        if (tmp.Count > 1)
                            EndDate = DateTime.TryParse(tmp[1], out DateTime d1) ? d1 : DateTime.MinValue;
                        break;
                    case FieldType.STRING:
                        tmp = value.Split(";").ToList();
                        if (tmp.Count > 0)
                            PrefixString = tmp[0];
                        if (tmp.Count > 1)
                            SuffixString = tmp[0];
                        break;
                }
            }
        }
        public List<long> Ids { get; private set; }
        public string PrefixString { get; private set; }
        public string SuffixString { get; private set; }
        public long? StartLong { get; private set; }
        public long? EndLong { get; private set; }
        public decimal? StartDecimal { get; private set; }
        public decimal? EndDecimal { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public FilterPermissionDefinition(string name, FieldType type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldType
    {
        ID = 1,
        LONG = 2,
        DECIMAL = 3,
        STRING = 4,
        DATE = 5,
    }
}
