using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Restcoration.Exceptions
{
    public class UnexpectedResponseTypeException : Exception
    {
        public UnexpectedResponseTypeException(string message, object requestData, IRestResponse responseData) : base(message)
        {
            RequestData = requestData;
            ResponseData = responseData;
        }

        public object RequestData { get; set; }
        public IRestResponse ResponseData { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
