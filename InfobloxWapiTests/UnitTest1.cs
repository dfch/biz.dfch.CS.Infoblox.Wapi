using System;
using System.Net.Sockets;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Infoblox.Wapi;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "InfobloxWapiTests.dll.config", Watch = true)]
namespace biz.dfch.CS.Infoblox.Wapi
{
    [TestClass]
    public class UnitTest1
    {
        private static TestContext _testContext;
        public TestContext testContext
        {
            get
            {
                return _testContext;
            }
            set
            {
                _testContext = value;
            }
        }

        [AssemblyInitialize]
        public static void Configure(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        [ClassInitialize()]
        public static void classInitialize(TestContext testContext)
        {
            _testContext = testContext;
            Trace.WriteLine(String.Format("classInitialize: '{0}'", testContext.TestName));
        }

        [ClassCleanup()]
        public static void classCleanup()
        {
            Trace.WriteLine("classCleanup");
        }

        [TestInitialize()]
        public void testInitialize()
        {
            Trace.WriteLine("testInitialize");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void doConnectWildcardAddressThrowsAggregateException()
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "https://0.0.0.0/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void doConnectInvalidSchemeThrowsArgumentException()
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "abcd://0.0.0.0/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
        }
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void doConnectInvalidPortThrowsUriFormatException()
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "http://127.0.0.1:65536/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
        }
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void doConnectRefusedPortThrowsAggregateException()
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "http://127.0.0.1:65535/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
        }
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void doConnectRefusedAddressThrowsAggregateException()
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "http://1.1.1.1/";

            var rest = new RestHelper();
            var nc = new System.Net.NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

            var q = new Hashtable();
            var s = rest.Invoke("networkview", q);
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
