using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace Restcoration
{
    public interface IRestClientFactory
    {
        IAuthenticator Authenticator { get; set; }
        X509CertificateCollection ClientCertificates { get; set; }
        IList<Parameter> DefaultParameters { get; }
        string UserAgent { get; set; }
        bool UseSynchronizationContext { get; set; }
        string BaseUrl { get; set; }
        DataFormat RequestFormat { get; set; }
        string RootElement { get; set; }
        string DateFormat { get; set; }
        ICredentials Credentials { get; set; }
        object UserState { get; set; }
        int Timeout { get; set; }
        int Attempts { get; set; }

        /// <summary>
        /// Attempts to request data from resource.
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <typeparam name="T2">Request data type</typeparam>
        /// <param name="requestData">Request data</param>
        /// <param name="cookies">Cookies for request</param>
        /// <param name="parameters">Parameters for request</param>
        /// <param name="headers">Headers for request</param>
        /// /// <param name="urlSegments">URL Segments to replace</param>
        /// <returns>Response data, InvalidCastException or ArgumentException</returns>
        T Get<T>(object requestData, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null, Dictionary<string, string> urlSegments = null) where T : new();

        /// <summary>
        /// Attempts to request data from resource, boxing it as object.
        /// </summary>
        /// <typeparam name="T">Request data type</typeparam>
        /// <param name="requestData">Request data</param>
        /// <param name="cookies">Extra cookies for request</param>
        /// <param name="parameters">Parameters for request</param>
        /// <param name="headers">Headers for request</param>
        /// <param name="urlSegments">URL Segments to replace</param>
        /// <returns>Response data, MissingFieldException or ArgumentException</returns>
        object Get(object requestData, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null, Dictionary<string, string> urlSegments = null);
    }
}