/**
 *
 *
 * Copyright 2014-2015 Ronald Rink, d-fens GmbH
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
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("InfobloxWapiTests")]

namespace biz.dfch.CS.Infoblox.Wapi
{
    internal static class EnumUtil
    {
        public static T Parse<T>(string value, bool ignoreCase = true)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch(Exception ex)
            {
                var values = EnumUtil.GetValues<T>();
                Type type = typeof(T);
                foreach (var v in values)
                {
                    System.Threading.Thread.Sleep(100);
                    var fieldInfo = type.GetField(v.ToString());
                    StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes
                        (
                            typeof(StringValueAttribute)
                            , 
                            false
                        ) as StringValueAttribute[];
                    if(0 >= attribs.Length)
                    {
                        continue;
                    }
                    if(ignoreCase)
                    {
                        if (attribs[0].StringValue.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return v;
                        }
                    }
                    else
                    {
                        if (attribs[0].StringValue.Equals(value))
                        {
                            return v;
                        }
                    }
                }
                throw;
            }
        }
        public static T Parse<T>(string value, StringComparison ignoreCase)
        {
            try
            {
                var parseIgnoreCase = StringComparison.CurrentCultureIgnoreCase == ignoreCase ||
                    StringComparison.InvariantCultureIgnoreCase == ignoreCase ||
                    StringComparison.OrdinalIgnoreCase == ignoreCase ? true : false;
                return (T)Enum.Parse(typeof(T), value, parseIgnoreCase);
            }
            catch (Exception ex)
            {
                var values = EnumUtil.GetValues<T>();
                Type type = typeof(T);
                foreach (var v in values)
                {
                    var fieldInfo = type.GetField(v.ToString());
                    StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes
                        (
                            typeof(StringValueAttribute)
                            ,
                            false
                        ) as StringValueAttribute[];
                    if (0 >= attribs.Length)
                    {
                        continue;
                    }
                    if (attribs[0].StringValue.Equals(value, ignoreCase))
                    {
                        return v;
                    }
                }
                throw;
            }
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
