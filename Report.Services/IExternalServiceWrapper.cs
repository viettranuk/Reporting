using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Services
{
    public interface IExternalServiceWrapper
    {
        Task<string> GetAsync(string requestUrl);
    }
}
