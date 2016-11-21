using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Presentation
{
    public interface IApplication
    {
        void Run(IEnumerable<string> args);
    }
}
