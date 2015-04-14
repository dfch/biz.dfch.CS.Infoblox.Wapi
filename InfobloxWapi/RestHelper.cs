﻿using Newtonsoft.Json.Linq;
using StringValueAttributeExtension;
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
            get {
                return _Credential; 
            }
            set { 
                _Credential = value ?? (new NetworkCredential(String.Empty, String.Empty)); 
                var abCredential = System.Text.UTF8Encoding.UTF8.GetBytes(String.Format("{0}:{1}", _Credential.UserName, _Credential.Password));
                _CredentialBase64 = Convert.ToBase64String(abCredential);
            }
        }
        public void SetCredential(String Username, String Password) 
        {
            this.Credential = new NetworkCredential(Username, Password);
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
            String Method
            ,
            String Uri
            ,
            Hashtable QueryParameters
            ,
            Hashtable Headers
            ,
            String Body
            )
        {
            // Parameter validation
            if (String.IsNullOrWhiteSpace(Method)) throw new ArgumentException(String.Format("Method: Parameter validation FAILED. Parameter cannot be null or empty."), "Method");
            if (String.IsNullOrWhiteSpace(Uri)) throw new ArgumentException(String.Format("Uri: Parameter validation FAILED. Parameter cannot be null or empty."), "Uri");

            Headers = Headers ?? (new Hashtable());
            QueryParameters = QueryParameters ?? (new Hashtable());

            Debug.WriteLine(String.Format("Invoke: UriServer '{0}'. TimeoutSec '{1}'. Method '{2}'. Uri '{3}'. ReturnType '{4}'.", _UriServer.AbsoluteUri, _TimeoutSec, Method, Uri, _ReturnType.GetStringValue()));
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
                foreach (DictionaryEntry item in QueryParameters)
                {
                    QueryParametersString += String.Format("{0}={1}&", item.Key, item.Value);
                }
                char[] achTrimAmp = { '&' };
                QueryParametersString = QueryParametersString.TrimEnd(achTrimAmp);

                // Invoke request
                HttpResponseMessage response;
                var _Method = new HttpMethod(Method);
                Uri += QueryParametersString;
                char[] achTrimQuestion = { '?' };
                Uri = Uri.TrimEnd(achTrimQuestion);
                switch (_Method.ToString().ToUpper())
                {
                    case "GET":
                    case "HEAD":
                        response = cl.GetAsync(Uri).Result;
                        break;
                    case "POST":
                        {
                            var _Body = new StringContent(Body);
                            _Body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PostAsync(Uri, _Body).Result;
                        }
                        break;
                    case "PUT":
                        {
                            var payload = new StringContent(Body);
                            payload.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PutAsync(Uri, payload).Result;
                        }
                        break;
                    case "DELETE":
                        response = cl.DeleteAsync(Uri).Result;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    throw new UnauthorizedAccessException(String.Format("Invoking '{0}' with username '{1}' FAILED.", Uri, _Credential.UserName));
                }
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    var Message = String.Empty;
                    var contentError = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        JToken jv = JObject.Parse(contentError);
                        var MessageError = jv.SelectToken("Error", true).ToString();
                        var MessageCode = jv.SelectToken("code", true).ToString();
                        var MessageText = jv.SelectToken("text", true).ToString(); 
                        Message = String.Format("{0}\r\nCode: {1}\r\nText: {2}", MessageError, MessageCode, MessageText);
                    }
                    catch
                    {
                        Message = contentError;
                    }
                    throw new ArgumentException(Message);
                }
                response.EnsureSuccessStatusCode();

                Debug.WriteLine(String.Format("response '{0}'", response.ToString()));
                var content = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(String.Format("content '{0}'", content.ToString()));

                return content;
            }
        }

        public String Invoke(
            String Uri
            ,
            Hashtable QueryParameters
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), Uri, QueryParameters, null, null);
        }

        public String Invoke(
            String Uri
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), Uri, null, null, null);
        }
        #endregion

        public String GetNetwork(String Network)
        {
            var result = String.Empty;
            var q = new Hashtable();
            q.Add("_return_fields%2B", "extattrs");
            if (Network.StartsWith("network"))
            {
                result = this.Invoke(Network, q);
            }
            else
            {
                q.Add("network", Network);
                result = this.Invoke("network", q);
            }
            return result;
        }
        public String GetNetwork(String Network, int SubnetMask)
        {
            return this.GetNetwork(String.Format("{0}/{1}", Network, SubnetMask));
        }

        public String GetNetwork(String Network, String SubnetMask)
        {
            var ip = IPAddress.Parse(SubnetMask);
            UInt32 j = (UInt32) ip.Address;
            var MaskLength = 0;
            while (j != 0)
            {
                j = j >> 1;
                MaskLength++;
            }
            return this.GetNetwork(Network, MaskLength);
        }
        #region Constructor And Initialisation
        public RestHelper()
        {
            this.Initialise(null, RestHelper.VERSION, RestHelper.TIMEOUTSEC, RestHelper.URIBASE, RestHelper.RETURNTYPE, RestHelper.CONTENTTYPE);
        }
        public RestHelper(
            Uri UriServer
            ,
            String Version = RestHelper.VERSION
            ,
            int TimeoutSec = RestHelper.TIMEOUTSEC
            ,
            String UriBase = RestHelper.URIBASE
            ,
            ReturnTypes ReturnType = RestHelper.RETURNTYPE
            ,
            String ContentType = RestHelper.CONTENTTYPE
            )
        {
            this.Initialise(UriServer, Version, TimeoutSec, UriBase, ReturnType, ContentType);
        }
        private void Initialise(
            Uri UriServer
            ,
            String Version
            ,
            int TimeoutSec
            ,
            String UriBase
            ,
            ReturnTypes ReturnType
            ,
            String ContentType
            )
        {
            this.UriServer = UriServer;
            this.Version = Version;
            this.TimeoutSec = TimeoutSec;
            this.UriBase = UriBase;
            this.ReturnType = ReturnType;
            this.ContentType = ContentType;
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