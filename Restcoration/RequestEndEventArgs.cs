using System;
using RestSharp;

namespace Restcoration
{
    public class RequestEndEventArgs : EventArgs
    {
        public IRestResponse Response { get; set; }
    }
}