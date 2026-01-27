using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeTools.ExcelGenerator.Core.Models
{
    internal class GenerationState
    {
        public int CurrentNumber { get; set; }
        public int Ip1 { get; set; }
        public int Ip2 { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
