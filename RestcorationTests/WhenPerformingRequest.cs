using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Restcoration;
using RestSharp;

namespace RestcorationTests
{
    [TestFixture]
    public class WhenPerformingRequest
    {
        [Rest(Method = Method.GET, ResponseType = typeof(TestSuccessResponse), Resource = "/ip")]
        public class TestRequestWithSpecifiedDefaultType
        {}

        [Rest(Method = Method.GET, OK = typeof(TestSuccessResponse), Resource = "/ip")]
        public class TestRequestWithSpecifiedOKType
        {}

        [Test]
        public void AttributeWithSpecifiedDefaultTypeShouldReturnNonEmptyValue()
        {
            var factory = new RestClientFactory("http://httpbin.org");
            var c = factory.Get<TestSuccessResponse>(new TestRequestWithSpecifiedDefaultType());
            Assert.That(c.Origin, Is.Not.Empty);
        }

        [Test]
        public void AttributeWithSpecifiedOkTypeShouldReturnNonEmptyValue()
        {
            var factory = new RestClientFactory("http://httpbin.org");
            var c = factory.Get<TestSuccessResponse>(new TestRequestWithSpecifiedOKType());
            Assert.That(c.Origin, Is.Not.Empty);
        }

        [Test]
        public void AttributeWithSpecifiedDefaultTypeShouldRecognizeAnonymousFunctionObject()
        {
            var factory = new RestClientFactory("http://httpbin.org");
            var c = factory.Get(new TestRequestWithSpecifiedDefaultType());
            Assert.That(c, Is.TypeOf<TestSuccessResponse>());
        }

        public class TestSuccessResponse
        {
            public string Origin { get; set; }
        }
    }
}