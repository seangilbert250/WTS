﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WTS.Util
{
    public class StringUtil
    {
        public StringUtil()
        {
            
        }

        public static string StripHTML(string str)
        {
            if (str != null)
            {
                return Regex.Replace(str, "<.*?>", String.Empty);
            }
            else
            {
                return null;
            }
        }

        public static string StrongEscape(string str)
        {
            if (str != null)
            {
                return str.Replace("[", "|LB|")
                    .Replace("]", "|RB|")

                    .Replace("\\", "[BS]")
                    .Replace("/", "[FS]")
                    .Replace("\"", "[QUOTE]")
                    .Replace("'", "[APOS]")
                    .Replace("<", "[LT]")
                    .Replace(">", "[GT]")
                    .Replace("+", "[PL]")
                    .Replace("&", "[AMP]")
                    .Replace("\t", "[TAB]")
                    .Replace("\r\n", "[NEWLINERN]")
                    .Replace("\n", "[NEWLINE]");
            }
            else
            {
                return null;
            }
        }

        public static string UndoStrongEscape(string str)
        {
            if (str != null)
            {
                return str.Replace("[BS]", "\\")
                    .Replace("[FS]", "/")
                    .Replace("[QUOTE]", "\"")
                    .Replace("[APOS]", "'")
                    .Replace("[LT]", "<")
                    .Replace("[GT]", ">")
                    .Replace("[PL]", "+")
                    .Replace("[AMP]", "&")
                    .Replace("[TAB]", "\t")
                    .Replace("[NEWLINERN]", "\r\n")
                    .Replace("[NEWLINE]", "\n")

                    .Replace("|LB|", "[")
                    .Replace("|RB|", "]");
            }
            else
            {
                return null;
            }
        }

        public static List<object> DeserializeJsonToList(string json)
        {
            return (List<object>)DeserializeJsonToDictionaryOrList(json);
        }

        public static Dictionary<string, object> DeserializeJsonToDictionary(string json)
        {
            return (Dictionary<string, object>)DeserializeJsonToDictionaryOrList(json);
        }

        public static object DeserializeJsonToDictionaryOrList(string json, bool isArray = false)
        {
            if (!isArray)
            {
                isArray = json.Substring(0, 1) == "[";
            }

            if (!isArray)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var values2 = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> d in values)
                {
                    if (d.Value is JObject)
                    {
                        values2.Add(d.Key, DeserializeJsonToDictionaryOrList(d.Value.ToString()));
                    }
                    else if (d.Value is JArray)
                    {
                        values2.Add(d.Key, DeserializeJsonToDictionaryOrList(d.Value.ToString(), true));
                    }
                    else
                    {
                        values2.Add(d.Key, d.Value);
                    }
                }
                return values2;
            }
            else
            {
                var values = JsonConvert.DeserializeObject<List<object>>(json);
                var values2 = new List<object>();
                foreach (var d in values)
                {
                    if (d is JObject)
                    {
                        values2.Add(DeserializeJsonToDictionaryOrList(d.ToString()));
                    }
                    else if (d is JArray)
                    {
                        values2.Add(DeserializeJsonToDictionaryOrList(d.ToString(), true));
                    }
                    else
                    {
                        values2.Add(d);
                    }
                }

                return values2;
            }
        }
    }
}