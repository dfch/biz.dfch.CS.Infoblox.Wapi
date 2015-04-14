using System.Collections;
using System;
using System.Net;

namespace biz.dfch.CS.Infoblox.Wapi
{
    class Program
    {
        static void Main(string[] args)
        {
            var Username = "admin";
            var Password = "infoblox";
            var UriServer = "https://192.168.174.199/";

            var MaxResults = "2";
            var ReturnType = "xml-pretty";

            RestHelper rest = new RestHelper();
            NetworkCredential nc = new NetworkCredential(Username, Password);
            rest.Credential = nc;
            rest.UriServer = new System.Uri(UriServer);
            rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;
            
            Hashtable q = new Hashtable();
            q.Add("_max_results", MaxResults);
            q.Add("_return_type", ReturnType);            
            //q.Add("_return_fields%2B", "extattrs,name");
            //q.Add("_return_fields%2B", "extattrs");
            //string s = rest.Invoke("networkview");

            string s = rest.Invoke("networkview", q);
            s = rest.GetNetwork("192.168.1.0", 24);

            System.Console.WriteLine(String.Format("s: '{0}'", s));          
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
