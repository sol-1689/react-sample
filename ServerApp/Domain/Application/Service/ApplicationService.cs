using Domain.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    public class ApplicationService
    {
         [Dependency]
        public IApplicationRepository repository { get; set; }

        public async Task<ApplicationInfo> GetApplicationInfo()
        {
            var app = new ApplicationInfo();
            app.GeneralList = await repository.GetApplicationInfo();
            return app;
        }
    }

}
