using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using RestSharp;

namespace Restcoration
{
    public class RestClientFactory : IRestClientFactory
    {
        private IRestClient _client;
        public RestClientFactory(string baseUrl)
        {
            _client = new RestClient();
            BaseUrl = baseUrl;
        }

        public IAuthenticator Authenticator
        {
            get { return _client.Authenticator; }
            set { _client.Authenticator = value; }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { return _client.ClientCertificates; }
            set { _client.ClientCertificates = value; }
        }

        public IList<Parameter> DefaultParameters
        {
            get { return _client.DefaultParameters; }
        }

        public string UserAgent
        {
            get { return _client.UserAgent; }
            set { _client.UserAgent = value; }
        }

        public bool UseSynchronizationContext
        {
            get { return _client.UseSynchronizationContext; }
            set { _client.UseSynchronizationContext = value; }
        }

        public string BaseUrl
        {
            get { return _client.BaseUrl; }
            set { _client.BaseUrl = value; }
        }

        public DataFormat RequestFormat { get; set; }
        public string RootElement { get; set; }
        public string DateFormat { get; set; }
        public ICredentials Credentials { get; set; }
        public object UserState { get; set; }
        public int Timeout { get; set; }
        public int Attempts { get; set; }

        /// <summary>
        /// Attempts to request data from resource.
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <typeparam name="T2">Request data type</typeparam>
        /// <param name="requestData">Request data</param>
        /// <returns>Response data, InvalidCastException or ArgumentException</returns>
        public T Get<T, T2>(T2 requestData) where T : new()
        {
            var attribute = GetRestAttribute<T2>();
            if (attribute != null)
            {
                var response = GetResponse(attribute, requestData);
                var value = GetPropertyValue(attribute, response.StatusCode);
                if(value != null)
                    return JsonConvert.DeserializeObject<T>(response.Content);
                if (attribute.ResponseType == typeof(T))
                    return JsonConvert.DeserializeObject<T>(response.Content);
                throw new InvalidCastException("Requested type is not compatible with returning type. Use object Get<T>(T requestData); instead.");
            }
            
            throw new ArgumentException("No attributes on class.");
        }

        /// <summary>
        /// Attempts to request data from resource, boxing it as object.
        /// </summary>
        /// <typeparam name="T">Request data type</typeparam>
        /// <param name="requestData">Request data</param>
        /// <returns>Response data, MissingFieldException or ArgumentException</returns>
        public object Get<T>(T requestData) where T : new()
        {
            var attribute = GetRestAttribute<T>();
            if (attribute != null)
            {
                var response = GetResponse(attribute, requestData);
                var value = GetPropertyValue(attribute, response.StatusCode);
                if (value != null)
                    return JsonConvert.DeserializeObject(response.Content, value);
                if (attribute.ResponseType != null)
                    return JsonConvert.DeserializeObject(response.Content, attribute.ResponseType);
                throw new MissingFieldException(
                    "Missing default response type and no matching set single converters found.");
            }

            throw new ArgumentException("No attributes on class.");
        }

        private IRestResponse GetResponse<T>(RestAttribute attribute, T requestData)
        {
            var request = new RestRequest(attribute.Resource, attribute.Method);
            request.AddBody(requestData);
            var response = _client.Execute(request);
            return response;
        }
        
        private Dictionary<HttpStatusCode, Type> GetMethodAssignments(RestAttribute attribute)
        {
            return Enum.GetValues(typeof (HttpStatusCode)).Cast<object>().ToDictionary(value => (HttpStatusCode) value, value => attribute.GetType().GetProperty(value.ToString()).GetValue(attribute, null).GetType());
        }

        private static RestAttribute GetRestAttribute<T>()
        {
            return typeof (T).GetCustomAttributes(typeof (RestAttribute), true).FirstOrDefault() as RestAttribute;
        }

        private static Type GetPropertyValue(RestAttribute attribute, HttpStatusCode code)
        {
            var prop = attribute.GetType().GetProperty(code.ToString());
            var value = prop == null ? null : prop.GetValue(attribute, null);
            return value == null ? null : value.GetType();
        }
    }
}