using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace WTS.Caching
{
    public class WTSCache
    {
        private static WTSCache _instance;
        private static object _lockObj = new object();

        private Dictionary<string, object> cache;
        private List<string> masterDataTypes;
        private List<string> rqmtDataTypes;

        public static WTSCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new WTSCache();
                        }
                    }
                }

                return _instance;
            }
        }
        
        public WTSCache()
        {
            cache = new Dictionary<string, object>();

            // the items in the following list are loaded automatically by the system when needed (if loadIfEmtpy is TRUE)
            masterDataTypes = new List<string>()
            {
                WTSCacheType.SYSTEM_SUITE,
                WTSCacheType.WTS_SYSTEM,
                WTSCacheType.WORK_AREA
            };

            rqmtDataTypes = new List<string>()
            {
                WTSCacheType.RQMT_TYPE,
                WTSCacheType.RQMT_COMPLEXITY
            };
        }

        public void AddItemToCache(string cacheType, string propertyType, int key, object obj)
        {
            AddItemToCache(cacheType, propertyType, key.ToString(), obj);
        }

        public void AddItemToCache(string cacheType, string propertyType, string key, object obj)
        {
            if (string.IsNullOrWhiteSpace(cacheType))
            {
                throw new ArgumentException("CacheType cannot be NULL.");
            }

            if (string.IsNullOrWhiteSpace(propertyType))
            {
                throw new ArgumentException("PropertyType cannot be NULL.");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be NULL.");
            }

            string cacheKey = cacheType + "." + propertyType + "." + key;

            cache[cacheKey] = obj;
        }

        public DataRow GetDataRowFromCache(string cacheType, string propertyType, int key, bool loadIfEmpty = true)
        {
            return GetDataRowFromCache(cacheType, propertyType, key.ToString(), loadIfEmpty);
        }

        public DataRow GetDataRowFromCache(string cacheType, string propertyType, string key, bool loadIfEmpty = true)
        {
            return (DataRow)GetObjectFromCache(cacheType, propertyType, key, loadIfEmpty);
        }

        public int GetIntFromCache(string cacheType, string propertyType, int key, bool loadIfEmpty = true)
        {
            return GetIntFromCache(cacheType, propertyType, key.ToString(), loadIfEmpty);
        }

        public int GetIntFromCache(string cacheType, string propertyType, string key, bool loadIfEmpty = true)
        {
            int i = 0;

            string value = GetStringFromCache(cacheType, propertyType, key, loadIfEmpty);

            if (value != null)
            {
                Int32.TryParse(value, out i);
            }

            return i;
        }

        public string GetStringFromCache(string cacheType, string propertyType, int key, bool loadIfEmpty = true)
        {
            return GetStringFromCache(cacheType, propertyType, key.ToString(), loadIfEmpty);
        }

        public string GetStringFromCache(string cacheType, string propertyType, string key, bool loadIfEmpty = true)
        {
            return (string)GetObjectFromCache(cacheType, propertyType, key, loadIfEmpty);
        }

        public object GetObjectFromCache(string cacheType, string propertyType, string key, bool loadIfEmpty = true)
        {
            object obj = null;

            if (string.IsNullOrWhiteSpace(cacheType))
            {
                throw new ArgumentException("CacheType cannot be NULL.");
            }

            if (string.IsNullOrWhiteSpace(propertyType))
            {
                throw new ArgumentException("PropertyType cannot be NULL.");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be NULL.");
            }

            string cacheKey = cacheType + "." + propertyType + "." + key;

            if (cache.ContainsKey(cacheKey))
            {
                obj = cache[cacheKey].ToString();
            }
            else if (loadIfEmpty)
            {
                if (masterDataTypes.Contains(cacheType))
                {
                    obj = MasterData.GetMasterDataProperty(cacheType, propertyType, key);
                }
                else if (rqmtDataTypes.Contains(cacheType))
                {
                    obj = RQMT.GetRQMTDataProperty(cacheType, propertyType, key);
                }

                if (obj != null) // ? allow nulls in the cache? if we keep reloading the same missing item over and over that might be bad
                {
                    cache[cacheKey] = obj;
                }
            }

            return obj;
        }

        public void ClearCache()
        {
            cache = new Dictionary<string, object>();
        }

        public void ClearCache(string cacheType = null, string propertyType = null, string key = null)
        {
            if (cacheType == null && propertyType == null && key != null)
            {
                throw new ArgumentException("Must specify a cache type if a key is supplied.");
            }

            if (cacheType == null)
            {
                cache = new Dictionary<string, object>();
            }
            else
            {
                string cacheKeyTarget = (cacheType + "." + (propertyType != null ? (propertyType + ".") : "") + (key != null ? key : "")).ToLower();

                foreach (string cacheKey in cache.Keys)
                {
                    if (cacheKey.ToLower().StartsWith(cacheKeyTarget))
                    {
                        cache[cacheKey] = "[[[DELETED]]]";
                    }
                }

                foreach (var item in cache.Where(kvp => kvp.Value.Equals("[[[DELETED]]]")).ToList())
                {
                    cache.Remove(item.Key);
                }
            }
        }
    }
}