using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.field
{
    public class Field_FieldDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }
        public Field_MenuDTO Menu { get; set; }
        public Field_FieldDTO() {}
        public Field_FieldDTO(Field Field)
        {
            this.Id = Field.Id;
            this.Name = Field.Name;
            this.Type = Field.Type;
            this.MenuId = Field.MenuId;
            this.IsDeleted = Field.IsDeleted;
            this.Menu = Field.Menu == null ? null : new Field_MenuDTO(Field.Menu);
        }
    }

    public class Field_FieldFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Type { get; set; }
        public IdFilter MenuId { get; set; }
        public FieldOrder OrderBy { get; set; }
    }
}
