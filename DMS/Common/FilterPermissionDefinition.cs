using DMS.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class FilterPermissionDefinition
    {
        public string Name { get; private set; }
        public long FieldTypeId { get; private set; }
        public IdFilter IdFilter { get; private set; }
        public DecimalFilter DecimalFilter { get; private set; }
        public LongFilter LongFilter { get; private set; }
        public DateFilter DateFilter { get; private set; }
        public StringFilter StringFilter { get; private set; }

        public FilterPermissionDefinition(string name, long FieldTypeId, long PermissionOperatorId, string value)
        {
            this.Name = name;
            this.FieldTypeId = FieldTypeId;
            if (FieldTypeId == FieldTypeEnum.ID.Id)
            {
                if (IdFilter == null) IdFilter = new IdFilter();
                if (long.TryParse(value, out long result))
                {
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_EQ.Id)
                        IdFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_NE.Id)
                        IdFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_IN.Id)
                    {
                        if (IdFilter.In == null) IdFilter.In = new List<long>();
                        IdFilter.In.Add(result);
                    }
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_NI.Id)
                    {
                        if (IdFilter.NotIn == null) IdFilter.NotIn = new List<long>();
                        IdFilter.NotIn.Add(result);
                    }
                }
            }

            if (FieldTypeId == FieldTypeEnum.STRING.Id)
            {
                if (StringFilter == null) StringFilter = new StringFilter();
                if (string.IsNullOrWhiteSpace(value))
                {
                    string result = value;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_EQ.Id)
                        StringFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_NE.Id)
                        StringFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_SW.Id)
                        StringFilter.StartWith = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_NSW.Id)
                        StringFilter.NotStartWith = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_EW.Id)
                        StringFilter.EndWith = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_NEW.Id)
                        StringFilter.NotEndWith = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_CT.Id)
                        StringFilter.Contain = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.STRING_NC.Id)
                        StringFilter.NotContain = result;
                }
            }

            if (FieldTypeId == FieldTypeEnum.LONG.Id)
            {
                if (LongFilter == null) LongFilter = new LongFilter();
                if (long.TryParse(value, out long result))
                {
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_EQ.Id)
                        LongFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_NE.Id)
                        LongFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_GT.Id)
                        if (LongFilter.Greater == null || LongFilter.Greater > result) LongFilter.Greater = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_GE.Id)
                        if (LongFilter.GreaterEqual == null || LongFilter.GreaterEqual >= result) LongFilter.GreaterEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_LT.Id)
                        if (LongFilter.Less == null || LongFilter.Less < result) LongFilter.Less = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.LONG_LE.Id)
                        if (LongFilter.LessEqual == null || LongFilter.LessEqual <= result) LongFilter.LessEqual = result;
                }
            }

            if (FieldTypeId == FieldTypeEnum.DECIMAL.Id)
            {
                if (DecimalFilter == null) DecimalFilter = new DecimalFilter();
                if (decimal.TryParse(value, out decimal result))
                {
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_EQ.Id)
                        DecimalFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_NE.Id)
                        DecimalFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_GT.Id)
                        if (DecimalFilter.Greater == null || DecimalFilter.Greater > result) DecimalFilter.Greater = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_GE.Id)
                        if (DecimalFilter.GreaterEqual == null || DecimalFilter.GreaterEqual >= result) DecimalFilter.GreaterEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_LT.Id)
                        if (DecimalFilter.Less == null || DecimalFilter.Less < result) DecimalFilter.Less = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_LE.Id)
                        if (DecimalFilter.LessEqual == null || DecimalFilter.LessEqual <= result) DecimalFilter.LessEqual = result;
                }
            }

            if (FieldTypeId == FieldTypeEnum.DATE.Id)
            {
                if (DateFilter == null) DateFilter = new DateFilter();
                if (DateTime.TryParse(value, out DateTime result))
                {
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_EQ.Id)
                        DateFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_NE.Id)
                        DateFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_GT.Id)
                        if (DateFilter.Greater == null || DateFilter.Greater > result) DateFilter.Greater = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_GE.Id)
                        if (DateFilter.GreaterEqual == null || DateFilter.GreaterEqual >= result) DateFilter.GreaterEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_LT.Id)
                        if (DateFilter.Less == null || DateFilter.Less < result) DateFilter.Less = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.DATE_LE.Id)
                        if (DateFilter.LessEqual == null || DateFilter.LessEqual <= result) DateFilter.LessEqual = result;
                }
            }
        }
    }
}
