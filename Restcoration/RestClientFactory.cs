using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Restcoration.Exceptions;
using RestSharp;
using RestSharp.Extensions;

namespace Restcoration
{
    public class RestClientFactory : IRestClientFactory
    {
        public virtual IRestClient Client { get; set; }
        private readonly IList<Task> _tasks;

        public RestClientFactory(IRestClient client, DataFormat requestFormat = DataFormat.Json)
        {
            Client = client;
            RequestFormat = requestFormat;
            _tasks = new List<Task>();
        }

        public RestClientFactory(string baseUrl, DataFormat requestFormat = DataFormat.Json) : this (new RestClient(baseUrl), requestFormat)
        {
        }
        
        public RestClientFactory() : this(new RestClient())
        {
        }

        /// <summary>
        /// Request format
        /// </summary>
        public virtual DataFormat RequestFormat { get; set; }

        /// <summary>
        /// Default root element
        /// </summary>
        public virtual string RootElement { get; set; }
        public virtual string DateFormat { get; set; }
        public virtual ICredentials Credentials { get; set; }
        public virtual object UserState { get; set; }
                
        /// <summary>
        /// Occurs when request is fired towards the server
        /// </summary>
        public event RequestStart OnRequestStart;

        /// <summary>
        /// Occurs when response comes back from the server
        /// </summary>
        public event RequestEnd OnRequestEnd;

        /// <summary>
        /// Attempts to request data from resource.
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="requestData">Request data</param>
        /// <param name="cookies">Cookies for request</param>
        /// <param name="parameters">Parameters for request</param>
        /// <param name="headers">Headers for request</param>
        /// <param name="urlSegments">URL Segments to replace</param>
        /// <returns>Response data, InvalidCastException or ArgumentException</returns>
        public T Get<T>(object requestData, Dictionary<string, string> cookies = null,
            Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> urlSegments = null) where T : new()
        {
            var attribute = GetRestAttribute(requestData);
            if (attribute != null)
            {
                var response = GetResponse(attribute, requestData, cookies, parameters, headers, urlSegments);
                var value = GetPropertyValue(attribute, response.StatusCode);
                if (value != null)
                    return JsonConvert.DeserializeObject<T>(response.Content);
                if (attribute.ResponseType == typeof (T))
                    return JsonConvert.DeserializeObject<T>(response.Content);
                throw new UnexpectedResponseTypeException(
                    "Unable to find compatible returning type. See ResponseData property in the exception or consider using non-generic Get() instead.", requestData,
                    response);
            }

            throw new ArgumentNullException("requestData","No attributes on requestData class.");
        }

        /// <summary>
        /// Attempts to request data from resource, boxing it as object.
        /// </summary>
        /// <param name="requestData">Request data</param>
        /// <param name="cookies">Extra cookies for request</param>
        /// <param name="parameters">Parameters for request</param>
        /// <param name="headers">Headers for request</param>
        /// <param name="urlSegments">URL Segments to replace</param>
        /// <returns>Response data, MissingFieldException or ArgumentException</returns>
        public object Get(object requestData, Dictionary<string, string> cookies = null,
            Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> urlSegments = null)
        {
            var attribute = GetRestAttribute(requestData);
            if (attribute != null)
            {
                var response = GetResponse(attribute, requestData, cookies, parameters, headers, urlSegments);
                var value = GetPropertyValue(attribute, response.StatusCode);
                if (value != null)
                    return JsonConvert.DeserializeObject(response.Content, value);
                if (attribute.ResponseType != null)
                    return JsonConvert.DeserializeObject(response.Content, attribute.ResponseType);
                throw new UnexpectedResponseTypeException(
                    "Unable to find compatible returning type. See ResponseData property in the exception.", requestData,
                    response);
            }

            throw new ArgumentException("No attributes on class.");
        }

        public void GetAsyncWithAction<T>(object requestData, Action<T> action, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> urlSegments = null) where T : new()
        {
            _tasks.Add(Task.Factory.StartNew(() =>
            {
                var data = Get<T>(requestData, cookies, parameters, headers, urlSegments);
                action(data);
            }));
        }

        public void GetAsyncWithAction(object requestData, Action<object> action, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> urlSegments = null)
        {
            _tasks.Add(Task.Factory.StartNew(() =>
            {
                var data = Get(requestData, cookies, parameters, headers, urlSegments);
                action(data);
            }));
        }

        public async Task<T> GetAsync<T>(object requestData, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> urlSegments = null) where T : new()
        {
            return await Task<T>.Factory.StartNew(() => Get<T>(requestData, cookies, parameters, headers, urlSegments));
        }

        public async Task<object> GetAsync(object requestData, Dictionary<string, string> cookies = null, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> urlSegments = null)
        {
            return
                await Task<object>.Factory.StartNew(() => Get(requestData, cookies, parameters, headers, urlSegments));
        }
        
        public void WaitForAsync()
        {
            Task.WaitAll(_tasks.ToArray());
        }

        private IRestResponse GetResponse(RestAttribute attribute, object requestData, Dictionary<string, string> cookies,
            Dictionary<string, object> parameters, Dictionary<string, string> headers, Dictionary<string, string> urlSegments)
        {
            var request = new RestRequest(attribute.Resource, attribute.Method)
            {
                JsonSerializer = new JsonSerializer(),
                RequestFormat = RequestFormat,
                RootElement = RootElement,
                Timeout = Client.Timeout
            };

            if (attribute.Timeout != null)
                request.Timeout = attribute.Timeout.Value;

            if (Credentials != null)
                request.Credentials = Credentials;

            if (UserState != null)
                request.UserState = UserState;
            
            if (DateFormat != null)
                request.DateFormat = DateFormat;

            request.AddBody(requestData);
            var tempBaseUrl = Client.BaseUrl;
            if (!string.IsNullOrWhiteSpace(attribute.BaseUrl))
                Client.BaseUrl = attribute.BaseUrl;

            var values = GetJsonPropertyValues(requestData);
            foreach (var value in values)
                request.AddUrlSegment(value.Key, value.Value);

            if (cookies != null)
                foreach (var cookie in cookies)
                    request.AddCookie(cookie.Key, cookie.Value);
            if (parameters != null)
                foreach (var param in parameters)
                    request.AddParameter(param.Key, param.Value);
            if (headers != null)
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            if(urlSegments != null)
                foreach (var segment in urlSegments)
                    request.AddUrlSegment(segment.Key, segment.Value);

            if (OnRequestStart != null)
                OnRequestStart(this, new RequestStartEventArgs() {Request = request});

            var response = Client.Execute(request);

            if (OnRequestEnd != null)
                OnRequestEnd(this, new RequestEndEventArgs() {Response = response});
            Client.BaseUrl = tempBaseUrl;
            return response;
        }

        private Dictionary<HttpStatusCode, Type> GetMethodAssignments(RestAttribute attribute)
        {
            return Enum.GetValues(typeof (HttpStatusCode)).Cast<object>().ToDictionary(value => (HttpStatusCode) value, value => attribute.GetType().GetProperty(value.ToString()).GetValue(attribute, null).GetType());
        }

        private static RestAttribute GetRestAttribute(object obj)
        {
            return obj.GetType().GetCustomAttributes(typeof (RestAttribute), true).FirstOrDefault() as RestAttribute;
        }

        private static T GetAttribute<T>(object obj) where T : class
        {
            return obj.GetType().GetCustomAttributes(typeof (T), true).FirstOrDefault() as T;
        }

        private static Dictionary<string, string> GetJsonPropertyValues(object requestData)
        {
            var result = new Dictionary<string, string>();
            var properties =
                requestData.GetType()
                    .GetProperties()
                    .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(UrlSegmentAttribute)));
            foreach (var prop in properties)
            {
                var attribute = prop.GetCustomAttributes(typeof (UrlSegmentAttribute), true).First();

                var key = attribute.GetType().GetProperty("Segment").GetValue(attribute, null) as string;
                var value = requestData.GetType().GetProperty(prop.Name).GetValue(requestData, null) as string;
                if(key == null) throw new NullReferenceException("Unable to find attribute name. This should never happen - report it to Microsoft.");
                result.Add(key, value);
            }

            return result;
        }

        private static RestAttribute GetRestAttribute<T>()
        {
            return typeof (T).GetCustomAttributes(typeof (RestAttribute), true).FirstOrDefault() as RestAttribute;
        }

        private static Type GetPropertyValue(RestAttribute attribute, HttpStatusCode code)
        {
            var prop = attribute.GetType().GetProperty(code.ToString());
            return prop.GetValue(attribute, null) as Type;
        }
    }
}