using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "biz.dfch.CS.Infoblox.Wapi.Tests.dll.config", Watch = true)]
namespace biz.dfch.CS.Infoblox.Wapi
{
    [TestClass]
    public class WapiTests
    {
        private static TestContext testContext;
        public TestContext TestContext
        {
            get
            {
                return testContext;
            }
            set
            {
                testContext = value;
            }
        }

        [AssemblyInitialize]
        public static void Configure(TestContext value)
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        [ClassInitialize()]
        public static void ClassInitialize(TestContext value)
        {
            testContext = value;
            Trace.WriteLine(String.Format("ClassInitialize: '{0}'", testContext.TestName));
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Trace.WriteLine("ClassCleanup");
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            Trace.WriteLine("TestInitialize");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DoConnectWildcardAddressThrowsAggregateException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "https://0.0.0.0/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(uriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(s));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DoConnectInvalidSchemeThrowsArgumentException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "abcd://0.0.0.0/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(uriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(s));
        }
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void DoConnectInvalidPortThrowsUriFormatException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://127.0.0.1:65536/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(uriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(s));
        }
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DoConnectRefusedPortThrowsAggregateException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://127.0.0.1:65535/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(uriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(s));
        }
        [TestMethod]
        public void DoConnectWithReturnTypeEnumReturnsObject()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var version = "v1.2.1";
            var timeOutSec = 90;

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                RestHelper.ReturnTypes.JsonPretty
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            Assert.AreEqual((new System.Uri(uriServer)).AbsoluteUri, rest.UriServer.AbsoluteUri);
            Assert.AreEqual(version, rest.Version);
            Assert.AreEqual(timeOutSec, rest.TimeoutSec);
            Assert.AreEqual(uriBase, rest.UriBase);
            Assert.AreEqual(RestHelper.ReturnTypes.JsonPretty, rest.ReturnType);
            Assert.AreEqual(RestHelper.ReturnTypes.JsonPretty.ToString(), rest.ReturnType.ToString());
            Assert.AreEqual(contentType, rest.ContentType);
            Assert.AreEqual(username, rest.Credential.UserName);
            Assert.AreEqual(password, rest.Credential.Password);
        }
        [TestMethod]
        public void DoConnectWithReturnTypeStringReturnsObject()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var returnType = "json-pretty";
            var version = "v1.2.1";
            var timeOutSec = 90;

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                returnType
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            Assert.AreEqual((new System.Uri(uriServer)).AbsoluteUri, rest.UriServer.AbsoluteUri);
            Assert.AreEqual(version, rest.Version);
            Assert.AreEqual(timeOutSec, rest.TimeoutSec);
            Assert.AreEqual(uriBase, rest.UriBase);
            Assert.AreEqual(RestHelper.ReturnTypes.JsonPretty, rest.ReturnType);
            Assert.AreEqual(RestHelper.ReturnTypes.JsonPretty.ToString(), rest.ReturnType.ToString());
            Assert.AreEqual(returnType, rest.ReturnType.GetStringValue());
            Assert.AreEqual(returnType, rest.ReturnTypeString);
            Assert.AreEqual(contentType, rest.ContentType);
            Assert.AreEqual(username, rest.Credential.UserName);
            Assert.AreEqual(password, rest.Credential.Password);
        }
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DoConnectRefusedAddressThrowsAggregateException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(uriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(s));
        }
        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void DoParseMissingContentFieldThrowsJsonException()
        {
            var contentError = @"
                {
                    ""Error-field-is-missing"" : ""myError""
                    ,
                    ""code""  : 500
                    ,
                    ""text""  : ""some nifty text""
                }
            ";
            JToken jv = JObject.Parse(contentError);
            var messageError = jv.SelectToken("Error", true).ToString();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(messageError));
        }
        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void DoParseInvalidJsonThrowsJsonReaderException()
        {
            var contentError = @"
                {
                    this is - not - a valid JSON string!!!!
                }
            ";
            JToken jv = JObject.Parse(contentError);
            var messageError = jv.SelectToken("Error", true).ToString();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(messageError));
        }
        [TestMethod]
        public void DoParseContentErrorReturnsTrue()
        {
            var contentError = @"
                {
                    ""Error"" : ""myError""
                    ,
                    ""code""  : 500
                    ,
                    ""text""  : ""some nifty text""
                }
            ";
            JToken jv = JObject.Parse(contentError);
            var messageError = jv.SelectToken("Error", true).ToString();
            var messageCode = jv.SelectToken("code", true).ToString();
            var messageText = jv.SelectToken("text", true).ToString();
            Assert.IsNotNull(messageError);
            Assert.IsNotNull(messageCode);
            Assert.IsNotNull(messageText);
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void InvokeWithUnsupportedMethodThrowsNotImplementedException()
        {
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var returnType = "json-pretty";
            var version = "v1.2.1";
            var timeOutSec = 90;

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                returnType
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            var result = rest.Invoke("PATCH", "any-uri", null, null, null);

            Assert.Fail("ERROR: Reaching this point means a previous expected error has not occurred.");
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void InvokeWithInvalidCredentialsThrowsUnauthorizedAccessException()
        {

            // Arrange
            var username = "any-user";
            var password = "invalid-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var returnType = "json-pretty";
            var version = "v1.2.1";
            var timeOutSec = 90;
            var uri = "networkview";

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                returnType
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            var cl = Mock.Create<HttpClient>();
            var task = Mock.Create<System.Threading.Tasks.Task<HttpResponseMessage>>();

            Mock.Arrange(() => cl.GetAsync(Arg.AnyString))
                .IgnoreInstance()
                .Returns(task);
            Mock.Arrange(() => cl.GetAsync(Arg.AnyString).Result.StatusCode)
                .IgnoreInstance()
                .Returns(HttpStatusCode.Unauthorized);
            
            // Act
            var result = rest.Invoke("GET", uri, null, null, null);

            // Assert
            Assert.Fail("ERROR: Reaching this point means a previous expected error has not occurred.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvokeWithInvalidRequestThrowsArgumentException()
        {
            // Arrange
            var content = @"
                {
                    'Error' : 'someError'
                    , 
                    'text' : 'someText'
                    , 
                    'code' : 42
                }
            ";
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var returnType = "json-pretty";
            var version = "v1.2.1";
            var timeOutSec = 90;
            var uri = "invalid-uri";

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                returnType
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            var cl = Mock.Create<HttpClient>();
            var task = Mock.Create<System.Threading.Tasks.Task<HttpResponseMessage>>();

            Mock.Arrange(() => cl.GetAsync(Arg.AnyString))
                .IgnoreInstance()
                .Returns(task);
            Mock.Arrange(() => cl.GetAsync(Arg.AnyString).Result.StatusCode)
                .IgnoreInstance()
                .Returns(HttpStatusCode.BadRequest);
            Mock.Arrange(() => cl.GetAsync(Arg.AnyString).Result.Content.ReadAsStringAsync().Result)
                .IgnoreInstance()
                .Returns(content);

            // Act
            try
            {
                var result = rest.Invoke("GET", uri, null, null, null);
            }

            // Assert
            catch(ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("someError"));
                Assert.IsTrue(ex.Message.Contains("Code: 42"));
                Assert.IsTrue(ex.Message.Contains("Text: someText"));
                throw;
            }

            Assert.Fail("ERROR: Reaching this point means a previous expected error has not occurred.");
        }

        [TestMethod]
        public void InvokeGetNetworkViewReturnsString()
        {
            // Arrange
            var content = @"
                [{
                    '_ref': 'networkview/AbcdEfghIjlkMnopQrstUvwx:default/true',
                    'is_default': true,
                    'name': 'default'
                },
                {
                    '_ref': 'networkview/AbcdEfghIjlkMnopQr/false',
                    'is_default': false,
                    'name': 'example_net'
                }]
            ";
            var username = "any-user";
            var password = "any-password";
            var uriServer = "http://any-server.example.com";
            var contentType = "application/json";
            var uriBase = "wapi";
            var returnType = "json-pretty";
            var version = "v1.2.1";
            var timeOutSec = 90;
            var uri = "networkview";

            var rest = new RestHelper(
                new System.Uri(uriServer)
                ,
                version
                ,
                timeOutSec
                ,
                uriBase
                ,
                returnType
                ,
                contentType
                );
            var nc = new System.Net.NetworkCredential(username, password);
            rest.Credential = nc;

            var httpClient = Mock.Create<HttpClient>();
            var task = Mock.Create<System.Threading.Tasks.Task<HttpResponseMessage>>();
            var httpResponseMessage = Mock.Create<HttpResponseMessage>();

            Mock.Arrange(() => httpClient.GetAsync(Arg.AnyString))
                .IgnoreInstance()
                .Returns(task);
            Mock.Arrange(() => httpClient.GetAsync(Arg.AnyString).Result.StatusCode)
                .IgnoreInstance()
                .Returns(HttpStatusCode.OK);
            Mock.Arrange(() => httpClient.GetAsync(Arg.AnyString).Result.EnsureSuccessStatusCode())
                .IgnoreInstance()
                .DoNothing();
            Mock.Arrange(() => httpClient.GetAsync(Arg.AnyString).Result.Content.ReadAsStringAsync().Result)
                .IgnoreInstance()
                .Returns(content);

            // Act
            var result = rest.Invoke("GET", uri, null, null, null);

            // Assert
            var jArray = JArray.Parse(content);
            Assert.AreEqual(2, jArray.Count);

            Assert.AreEqual("networkview/AbcdEfghIjlkMnopQrstUvwx:default/true", jArray[0].SelectToken("_ref", true).ToString());
            Assert.AreEqual(true, jArray[0].SelectToken("is_default", true));
            Assert.AreEqual("default", jArray[0].SelectToken("name", true).ToString());

            Assert.AreEqual("networkview/AbcdEfghIjlkMnopQr/false", jArray[1].SelectToken("_ref", true).ToString());
            Assert.AreEqual(false, jArray[1].SelectToken("is_default", true));
            Assert.AreEqual("example_net", jArray[1].SelectToken("name", true).ToString());
        }
    }
}

/**
 *
 *
 * Copyright 2015 Ronald Rink, d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
