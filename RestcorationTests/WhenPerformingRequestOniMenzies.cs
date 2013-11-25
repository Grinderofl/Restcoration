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
