using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public class FormViewModel
    {
        public FormModel FormModel { get; set; }
        public ComplexType ComplexType { get; set; }
        public ComplexType1 ComplexType1 { get; set; }
        public IEnumerable<RadioBox> RadioBoxList { get; set; }
        public ICollection<RadioBox> RadioBoxList2 { get; set; }
    }
}
