using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class ColorEnum
    {
        public static GenericEnum WHITE = new GenericEnum { Id = 1, Code = "#FFFFFF", Name = "WHITE" };
        public static GenericEnum SILVER = new GenericEnum { Id = 2, Code = "#C0C0C0", Name = "SILVER" };
        public static GenericEnum GRAY = new GenericEnum { Id = 3, Code = "#808080", Name = "GRAY" };
        public static GenericEnum BLACK = new GenericEnum { Id = 4, Code = "#000000", Name = "BLACK" };
        public static GenericEnum RED = new GenericEnum { Id = 5, Code = "#FF0000", Name = "RED" };
        public static GenericEnum MAROON = new GenericEnum { Id = 6, Code = "#800000", Name = "MAROON" };
        public static GenericEnum YELLOW = new GenericEnum { Id = 7, Code = "#FFFF00", Name = "YELLOW" };
        public static GenericEnum OLIVE = new GenericEnum { Id = 8, Code = "#808000", Name = "OLIVE" };
        public static GenericEnum LIME = new GenericEnum { Id = 9, Code = "#00FF00", Name = "LIME" };
        public static GenericEnum GREEN = new GenericEnum { Id = 10, Code = "#008000", Name = "GREEN" };
        public static GenericEnum AQUA = new GenericEnum { Id = 11, Code = "#00FFFF", Name = "AQUA" };
        public static GenericEnum TEAL = new GenericEnum { Id = 12, Code = "#008080", Name = "TEAL" };
        public static GenericEnum BLUE = new GenericEnum { Id = 13, Code = "#0000FF", Name = "BLUE" };
        public static GenericEnum NAVY = new GenericEnum { Id = 14, Code = "#000080", Name = "NAVY" };
        public static GenericEnum FUCHSIA = new GenericEnum { Id = 15, Code = "#FF00FF", Name = "FUCHSIA" };
        public static GenericEnum PURPLE = new GenericEnum { Id = 16, Code = "#800080", Name = "PURPLE" };
        public static List<GenericEnum> ColorEnumList = new List<GenericEnum>
        {
            WHITE, SILVER, GRAY, BLACK, 
            RED,MAROON, YELLOW, OLIVE, 
            LIME,GREEN, AQUA, TEAL, 
            BLUE, NAVY, FUCHSIA, PURPLE,
        };
    }
}
