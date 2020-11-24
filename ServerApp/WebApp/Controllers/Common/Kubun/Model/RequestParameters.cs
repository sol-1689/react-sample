using Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Model
{
    public class KubunRequestParameter
    {
        [Required]
        public string CategoryId { get; set; }

    }

}
