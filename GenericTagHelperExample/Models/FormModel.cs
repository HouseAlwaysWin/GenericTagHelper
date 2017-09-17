using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        [Required]
        [DataType("HiddenInput")]
        public int Id { get; set; }

        [Required]
        public string TextBox { get; set; }

        [DataType("TextArea")]
        [Required]
        public string TextArea { get; set; }

        public byte[] Upload { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailTextBox { get; set; }

        public DateTime DateTimeTextBox { get; set; }

        [Required]
        public Level LevelSelectList { get; set; }

        [Required]
        [DataType("Select")]
        public int SelectList { get; set; }

        [Required]
        [DataType("Select")]
        public int SelectList2 { get; set; }

        [Required]
        public bool CheckBox { get; set; }
        [DataType(DataType.Password)]
        public string PasswordTextBox { get; set; }

        [Required]
        [DataType("Radio")]
        public int SelectRadio { get; set; }

        // Doesn't appeart in the form
        public IEnumerable<RadioBox> RadioBoxList { get; set; }
        public ICollection<RadioBox> RadioBoxList2 { get; set; }

    }
}
