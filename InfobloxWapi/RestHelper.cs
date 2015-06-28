using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace biz.dfch.CS.Infoblox.Wapi
{
    public class RestHelper
    {
        #region Constants and Properties
        private const String URIBASE = "wapi";
        private const int TIMEOUTSEC = 90;
        private const String VERSION = "v1.2.1";
        private const String CONTENTTYPE = "application/json";
        private const ReturnTypes RETURNTYPE = ReturnTypes.Default;
        private const String RETURNTYPESTRING = "default";
        private const String AUTHORIZATIONHEADERFORMAT = "Basic {0}";
        private const String USERAGENT = "d-fens biz.dfch.CS.Infoblox.Wapi.RestHelper";

        public enum ReturnTypes
        {
            [StringValue("default")]
            Default = 0
            ,
            [StringValue("json")]
            Json
            ,
            [StringValue("json-pretty")]
            JsonPretty
            ,
            [StringValue("xml")]
            Xml
            ,
            [StringValue("xml-pretty")]
            XmlPretty
        }

        private ReturnTypes _ReturnType;
        public ReturnTypes ReturnType
        {
            get { return _ReturnType;  }
            set { _ReturnType = value; }
        }
        public String ReturnTypeString
        {
            get { return _ReturnType.GetStringValue(); }
            set { _ReturnType = EnumUtil.Parse<ReturnTypes>(value); }
        }

        private String _ContentType;
        public String ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        private Uri _UriServer;
        public Uri UriServer
        {
            get { return _UriServer; }
            set { _UriServer = value; }
        }

        private String _UriBase;
        public String UriBase
        {
            get { return _UriBase; }
            set { _UriBase = value; }
        }
        private String _Version;
        public String Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private NetworkCredential _Credential;
        private String _CredentialBase64;
        public NetworkCredential Credential
        {
            get 
            {
                return _Credential; 
            }
            set 
            { 
                _Credential = value ?? (new NetworkCredential(String.Empty, String.Empty)); 
                var abCredential = System.Text.UTF8Encoding.UTF8.GetBytes(String.Format("{0}:{1}", _Credential.UserName, _Credential.Password));
                _CredentialBase64 = Convert.ToBase64String(abCredential);
            }
        }
        public void SetCredential(String username, String password) 
        {
            this.Credential = new NetworkCredential(username, password);
        }

        private int _TimeoutSec;
        public int TimeoutSec
        {
            get 
            {
                return _TimeoutSec;
            }
            set
            {
                _TimeoutSec = value;
            }
        }

        #endregion

        #region Invoke
        public String Invoke(
            String method
            ,
            String uri
            ,
            Hashtable queryParameters
            ,
            Hashtable headers
            ,
            String body
            )
        {
            // Parameter validation
            if (String.IsNullOrWhiteSpace(method)) throw new ArgumentException(String.Format("Method: Parameter validation FAILED. Parameter cannot be null or empty."), "Method");
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentException(String.Format("Uri: Parameter validation FAILED. Parameter cannot be null or empty."), "Uri");

            headers = headers ?? (new Hashtable());
            queryParameters = queryParameters ?? (new Hashtable());

            Debug.WriteLine(String.Format("Invoke: UriServer '{0}'. TimeoutSec '{1}'. Method '{2}'. Uri '{3}'. ReturnType '{4}'.", _UriServer.AbsoluteUri, _TimeoutSec, method, uri, _ReturnType.GetStringValue()));
            if(null == Credential) 
            {
                Debug.WriteLine(String.Format("No Credential specified."));
            }
            else
            {
                if ( String.IsNullOrWhiteSpace(Credential.Password) )
                {
                    Debug.WriteLine(String.Format("Username '{0}', Password '{1}'.", Credential.UserName, "*"));
                }
                else
                {
                    Debug.WriteLine(String.Format("Username '{0}', Password '{1}'.", Credential.UserName, "********"));
                }
            }

            using (var cl = new HttpClient())
            {
                char[] achTrim = { '/' };
                var s = String.Format("{0}/{1}/{2}/", _UriServer.AbsoluteUri.TrimEnd(achTrim), _UriBase.Trim(achTrim), _Version.Trim(achTrim));
                cl.BaseAddress = new Uri(s);
                cl.Timeout = new TimeSpan(0, 0, _TimeoutSec);
                cl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));
                cl.DefaultRequestHeaders.Add("Authorization", String.Format(RestHelper.AUTHORIZATIONHEADERFORMAT, _CredentialBase64));
                cl.DefaultRequestHeaders.Add("User-Agent", RestHelper.USERAGENT);

                var QueryParametersString = "?";
                foreach (DictionaryEntry item in queryParameters)
                {
                    QueryParametersString += String.Format("{0}={1}&", item.Key, item.Value);
                }
                char[] achTrimAmp = { '&' };
                QueryParametersString = QueryParametersString.TrimEnd(achTrimAmp);

                // Invoke request
                HttpResponseMessage response;
                var _method = new HttpMethod(method);
                uri += QueryParametersString;
                char[] achTrimQuestion = { '?' };
                uri = uri.TrimEnd(achTrimQuestion);
                switch (_method.ToString().ToUpper())
                {
                    case "GET":
                    case "HEAD":
                        response = cl.GetAsync(uri).Result;
                        break;
                    case "POST":
                        {
                            var _body = new StringContent(body);
                            _body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PostAsync(uri, _body).Result;
                        }
                        break;
                    case "PUT":
                        {
                            var _body = new StringContent(body);
                            _body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PutAsync(uri, _body).Result;
                        }
                        break;
                    case "DELETE":
                        response = cl.DeleteAsync(uri).Result;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    throw new UnauthorizedAccessException(String.Format("Invoking '{0}' with username '{1}' FAILED.", uri, _Credential.UserName));
                }
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    var message = String.Empty;
                    var contentError = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        JToken jv = JObject.Parse(contentError);
                        var messageError = jv.SelectToken("Error", true).ToString();
                        var messageCode = jv.SelectToken("code", true).ToString();
                        var messageText = jv.SelectToken("text", true).ToString(); 
                        message = String.Format("{0}\r\nCode: {1}\r\nText: {2}", messageError, messageCode, messageText);
                    }
                    catch
                    {
                        message = contentError;
                    }
                    throw new ArgumentException(message);
                }
                response.EnsureSuccessStatusCode();

                Debug.WriteLine(String.Format("response '{0}'", response.ToString()));
                var content = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(String.Format("content '{0}'", content.ToString()));

                return content;
            }
        }

        public String Invoke(
            String uri
            ,
            Hashtable queryParameters
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), uri, queryParameters, null, null);
        }

        public String Invoke(
            String uri
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), uri, null, null, null);
        }
        #endregion

        public String GetNetwork(String network)
        {
            var result = String.Empty;
            var q = new Hashtable();
            q.Add("_return_fields%2B", "extattrs");
            if (network.StartsWith("network"))
            {
                result = this.Invoke(network, q);
            }
            else
            {
                q.Add("network", network);
                result = this.Invoke("network", q);
            }
            return result;
        }
        public String GetNetwork(String network, int subnetMask)
        {
            return this.GetNetwork(String.Format("{0}/{1}", network, subnetMask));
        }

        public String GetNetwork(String network, String subnetMask)
        {
            var ip = IPAddress.Parse(subnetMask);
            UInt32 j = (UInt32) ip.Address;
            var maskLength = 0;
            while (j != 0)
            {
                j = j >> 1;
                maskLength++;
            }
            return this.GetNetwork(network, maskLength);
        }
        #region Constructor And Initialisation
        public RestHelper()
        {
            this.Initialise(null, RestHelper.VERSION, RestHelper.TIMEOUTSEC, RestHelper.URIBASE, RestHelper.RETURNTYPE, RestHelper.CONTENTTYPE);
        }
        public RestHelper(
            Uri uriServer
            ,
            String version = RestHelper.VERSION
            ,
            int timeoutSec = RestHelper.TIMEOUTSEC
            ,
            String uriBase = RestHelper.URIBASE
            ,
            String returnTypeString = RestHelper.RETURNTYPESTRING
            ,
            String contentType = RestHelper.CONTENTTYPE
            )
        {
            this.Initialise(uriServer, version, timeoutSec, uriBase, EnumUtil.Parse<ReturnTypes>(returnTypeString), contentType);
        }
        public RestHelper(
            Uri uriServer
            ,
            String version = RestHelper.VERSION
            ,
            int timeoutSec = RestHelper.TIMEOUTSEC
            ,
            String uriBase = RestHelper.URIBASE
            ,
            ReturnTypes returnType = RestHelper.RETURNTYPE
            ,
            String contentType = RestHelper.CONTENTTYPE
            )
        {
            this.Initialise(uriServer, version, timeoutSec, uriBase, returnType, contentType);
        }
        private void Initialise(
            Uri uriServer
            ,
            String version
            ,
            int timeoutSec
            ,
            String uriBase
            ,
            ReturnTypes returnType
            ,
            String contentType
            )
        {
            this.UriServer = uriServer;
            this.Version = version;
            this.TimeoutSec = timeoutSec;
            this.UriBase = uriBase;
            this.ReturnType = returnType;
            this.ContentType = contentType;
            this.Credential = null;
        }
        #endregion
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
