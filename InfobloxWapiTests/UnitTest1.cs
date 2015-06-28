using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "InfobloxWapiTests.dll.config", Watch = true)]
namespace biz.dfch.CS.Infoblox.Wapi
{
    [TestClass]
    public class UnitTest1
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
            var username = "admin";
            var password = "infoblox";
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
            var username = "admin";
            var password = "infoblox";
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
            var username = "admin";
            var password = "infoblox";
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
            var username = "admin";
            var password = "infoblox";
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
            var username = "admin";
            var password = "infoblox";
            var uriServer = "http://infoblox";
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
            var username = "admin";
            var password = "infoblox";
            var uriServer = "http://infoblox";
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
            var username = "admin";
            var password = "infoblox";
            var uriServer = "http://1.1.1.1/";

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
