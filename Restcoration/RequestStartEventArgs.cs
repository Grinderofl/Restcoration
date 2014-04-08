using System;
using RestSharp;

namespace Restcoration
{
    public class RequestStartEventArgs : EventArgs
    {
        public IRestRequest Request { get; set; }
    }
}