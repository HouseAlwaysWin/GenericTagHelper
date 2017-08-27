using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public enum Level
    {
        Bronze,
        Sliver,
        Gold,
        Patinum
    }
    public class FormModel
    {
        public int Id { get; set; }

        public string TextBox { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailTextBox { get; set; }

        public DateTime DateTimeTextBox { get; set; }

        public Level LevelSelectList { get; set; }

        public bool CheckBox { get; set; }

        [DataType(DataType.Password)]
        public string PasswordTextBox { get; set; }

        [Required]
        [DataType("Radio")]
        public int SelectRadio { get; set; }
        //public Dictionary<string,string> RadioBoxes { get; set; }

        public IEnumerable<RadioBox> RadioBoxList { get; set; }


        public ComplexType ComplexType { get; set; }
        public ComplexType1 ComplexType1 { get; set; }
        public ComplexType2 ComplexType2 { get; set; }
        public ComplexType3 ComplexType3 { get; set; }
        public ComplexType4 ComplexType4 { get; set; }
        public ComplexType5 ComplexType5 { get; set; }
        public ComplexType6 ComplexType6 { get; set; }
        public ComplexType7 ComplexType7 { get; set; }
        public ComplexType8 ComplexType8 { get; set; }
        public ComplexType9 ComplexType9 { get; set; }
    }
}
