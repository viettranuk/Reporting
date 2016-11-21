using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Report.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = ApplicationContainer.Configure();

            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IApplication>();

                app.Run(args);
            }
        }
    }
}
