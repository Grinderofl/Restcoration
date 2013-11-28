using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Restcoration;
using RestSharp;

namespace RestcorationTests
{
    [TestFixture]
    public class WhenPerformingRequestOniMenzies
    {
        [Test]
        public void UsersLoginShouldSucceed()
        {
            var client = new RestClientFactory("http://imenzies.apiary.io/");
            Assert.DoesNotThrow(
                () =>
                    client.Get(new UsersLoginRequest()
                    {
                        Username = "username",
                        Password = "password"
                    }));
        }

        [Test]
        public void GettingValuesShouldSucceed()
        {
            var client = new RestClientFactory("http://imenzies.apiary.io/");
            var response = client.Get<CustomerCustomeridRange200>(new CustomerCustomeridRangeRequest(), parameters: new Dictionary<string, object>() { { "customerid", 0 } });
            Assert.That(response.Range.TitleCount, Is.GreaterThan(0));
        }
    }

    [Rest(Resource = "/customer/{customerid}/range/", Method = Method.GET, OK = typeof(CustomerCustomeridRange200))]
    public class CustomerCustomeridRangeRequest { }
    public class CustomerCustomeridRange200
    {
        public class Issue
        {

            [JsonProperty("IssueNo")]
            public string IssueNo { get; set; }

            [JsonProperty("CopyNo")]
            public string CopyNo { get; set; }

            [JsonProperty("PublicationDate")]
            public DateTime PublicationDate { get; set; }

            [JsonProperty("PackStatus")]
            public string PackStatus { get; set; }

            [JsonProperty("DemandQty")]
            public int DemandQty { get; set; }

            [JsonProperty("ExtrasOrderable")]
            public bool ExtrasOrderable { get; set; }
        }

        public class Variant
        {

            [JsonProperty("VariantType")]
            public string VariantType { get; set; }

            [JsonProperty("StdOrdQty")]
            public int StdOrdQty { get; set; }

            [JsonProperty("issues")]
            public IList<Issue> Issues { get; set; }
        }

        public class Title
        {

            [JsonProperty("TitleNo")]
            public string TitleNo { get; set; }

            [JsonProperty("Mandatory")]
            public bool Mandatory { get; set; }

            [JsonProperty("variants")]
            public IList<Variant> Variants { get; set; }
        }

        public class Range2
        {

            [JsonProperty("titles")]
            public IList<Title> Titles { get; set; }

            [JsonProperty("titleCount")]
            public int TitleCount { get; set; }
        }


        [JsonProperty("range")]
        public Range2 Range { get; set; }

        [JsonProperty("messages")]
        public IList<object> Messages { get; set; }

    }

    [Rest(Resource = "/users/login", Method = Method.POST, OK = typeof(UsersLogin200), NotFound = typeof(UsersLogin404), Conflict = typeof(UsersLogin409))]
    public class UsersLoginRequest
    {

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

    }

    public class UsersLogin200
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("lastlogin")]
        public string Lastlogin { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }


    public class UsersLogin404
    {

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }


    public class UsersLogin409
    {

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }
}
