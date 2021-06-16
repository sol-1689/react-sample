using Domain.DB.Model;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IApplicationRepository : ICommonRepository
    {
        Task<List<M_General>> GetApplicationInfo();
    }
}
