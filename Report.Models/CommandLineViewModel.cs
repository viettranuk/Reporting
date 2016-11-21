using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Models
{
    public class CommandLineViewModel
    {
        public string RiverName { get; set; }
        public string Parameter { get; set; }
        public int Limit { get; set; }
        public int Days { get; set; }
    }
}
