using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;
using Unity;

namespace WebApp.Controllers
{
    [Route("api/kubun")]
    public class KubunApiController: CommonController
    {
        [Dependency]
        public KubunService service { get; set; }

        [Route("list")]
        [HttpPost]
        public async Task<RequestResult> GetList([FromBody] KubunRequestParameter model)
        {
            if(!ModelState.IsValid){
                return RequestResult.CreateErrorResult(ModelState);
            }

            var list = await service.GetKubunList(model.CategoryId);
            return RequestResult.CreateSuccessResult(list);
        }

        [Route("division")]
        [HttpPost]
        public async Task<RequestResult> GetDivisionList()
        {
            if(!ModelState.IsValid){
                return RequestResult.CreateErrorResult(ModelState);
            }

            var list = await service.GetDivisionList();
            return RequestResult.CreateSuccessResult(list);
        }


        [Route("businessCode")]
        [HttpPost]
        public async Task<RequestResult> GetBusinessCodeList()
        {
            if (!ModelState.IsValid)
            {
                return RequestResult.CreateErrorResult(ModelState);
            }

            var list = await service.GetBusinessCodeList();
            return RequestResult.CreateSuccessResult(list);
        }


    }



}