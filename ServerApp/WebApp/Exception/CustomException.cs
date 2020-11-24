using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    public class CustomException : Exception
    {
        public CustomException()
            :base()
        {

        }

        public CustomException(string message)
            : base(message)
        {

        }

        public CustomException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
