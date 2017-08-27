using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public byte[] Upload { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
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
        public IEnumerable<RadioBox> RadioBoxList { get; set; }
        public ICollection<RadioBox> RadioBoxList2 { get; set; }

    }
}
