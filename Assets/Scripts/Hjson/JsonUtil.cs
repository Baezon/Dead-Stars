﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Hjson
{
    /// <summary>Provides Json extension methods.</summary>
    public static class JsonUtil
    {
        static Exception failQ(JsonValue forObject, string op)
        {
            string type = forObject != null ? forObject.JsonType.ToString().ToLower() : "null";
            return new Exception("JsonUtil." + op + " not supported for type " + type + "!");
        }

        static Exception failM(Exception e, string key)
        {
            string msg = e.Message;
            if (msg.EndsWith("!")) msg = msg.Substring(0, msg.Length - 1);
            return new Exception(msg + " [key:" + key + "]!");
        }

        /// <summary>Gets the bool from a JsonValue.</summary>
        public static bool Qb(this JsonValue json)
        {
            if (json != null && json.JsonType == JsonType.Boolean) return (bool)json.ToValue();
            else throw failQ(json, "Qb");
        }

        /// <summary>Gets the bool value of a key in a JsonObject.</summary>
        public static bool Qb(this JsonObject json, string key, bool defaultValue = false)
        {
            try
            {
                if (json.ContainsKey(key)) return json[key].Qb();
                else return defaultValue;
            }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the int from a JsonValue.</summary>
        public static int Qi(this JsonValue json)
        {
            if (json != null && json.JsonType == JsonType.Number) return Convert.ToInt32(json.ToValue());
            else throw failQ(json, "Qi");
        }

        /// <summary>Gets the int value of a key in a JsonObject.</summary>
        public static int Qi(this JsonObject json, string key, int defaultValue = 0)
        {
            try
            {
                if (json.ContainsKey(key)) return json[key].Qi();
                else return defaultValue;
            }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the long from a JsonValue.</summary>
        public static long Ql(this JsonValue json)
        {
            if (json != null && json.JsonType == JsonType.Number) return Convert.ToInt64(json.ToValue());
            else throw failQ(json, "Ql");
        }

        /// <summary>Gets the long value of a key in a JsonObject.</summary>
        public static long Ql(this JsonObject json, string key, long defaultValue = 0)
        {
            try
            {
                if (json.ContainsKey(key)) return json[key].Ql();
                else return defaultValue;
            }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the double from a JsonValue.</summary>
        public static double Qd(this JsonValue json)
        {
            if (json != null && json.JsonType == JsonType.Number) return Convert.ToDouble(json.ToValue());
            else throw failQ(json, "Qd");
        }

        /// <summary>Gets the double value of a key in a JsonObject.</summary>
        public static double Qd(this JsonObject json, string key, double defaultValue = 0)
        {
            try
            {
                if (json.ContainsKey(key)) return json[key].Qd();
                else return defaultValue;
            }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the string from a JsonValue.</summary>
        public static string Qs(this JsonValue json)
        {
            if (json == null) return null;
            else if (json.JsonType == JsonType.String) return (string)json;
            else throw failQ(json, "Qs");
        }

        /// <summary>Gets the string value of a key in a JsonObject.</summary>
        public static string Qs(this JsonObject json, string key, string defaultValue = "")
        {
            try
            {
                if (json.ContainsKey(key)) return json[key].Qs();
                else return defaultValue;
            }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the JsonValue of a key in a JsonObject.</summary>
        public static JsonValue Qv(this JsonObject json, string key)
        {
            if (json.ContainsKey(key)) return json[key];
            else return null;
        }

        /// <summary>Gets a JsonObject from a JsonObject.</summary>
        public static JsonObject Qo(this JsonObject json, string key)
        {
            try { return (JsonObject)json.Qv(key); }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the JsonObject from a JsonValue.</summary>
        public static JsonObject Qo(this JsonValue json)
        {
            try { return (JsonObject)json; }
            catch { throw failQ(json, "Qo"); }
        }

        /// <summary>Gets a JsonArray from a JsonObject.</summary>
        public static JsonArray Qa(this JsonObject json, string key)
        {
            try { return (JsonArray)json.Qv(key); }
            catch (Exception e) { throw failM(e, key); }
        }

        /// <summary>Gets the JsonArray from a JsonValue.</summary>
        public static JsonArray Qa(this JsonValue json)
        {
            try { return (JsonArray)json; }
            catch { throw failQ(json, "Qo"); }
        }

        /// <summary>Enumerates JsonObjects from a JsonObject.</summary>
        public static IEnumerable<KeyValuePair<string, JsonObject>> Qqo(this JsonObject json)
        {
            return json.Select(x => new KeyValuePair<string, JsonObject>(x.Key, x.Value.Qo()));
        }

        /// <summary>Enumerates JsonObjects from a JsonObject.</summary>
        public static UnityEngine.Color Qcolor(this JsonValue json)
        {
            return new UnityEngine.Color(json.Qa()[0].Qi() / 255f, json.Qa()[1].Qi() / 255f, json.Qa()[2].Qi() / 255f);
        }

        static readonly DateTime UnixEpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>Convert the date to json (unix epoch date offset).</summary>
        public static long ToJsonDate(this DateTime dt)
        {
            if (dt == DateTime.MinValue) return 0;
            else return (long)(dt.ToUniversalTime() - UnixEpochUtc).TotalMilliseconds;
        }

        /// <summary>Convert the json date (unix epoch date offset) to a DateTime.</summary>
        public static DateTime ToDateTime(long unixEpochDateOffset)
        {
            if (unixEpochDateOffset > 0) return UnixEpochUtc.AddMilliseconds(unixEpochDateOffset);
            else return DateTime.MinValue;
        }

        /// <summary>Convert the date to JSON/ISO 8601, compatible with ES5 Date.toJSON().</summary>
        /// <remarks>Use DateTime.Parse() to convert back (will be of local kind).</remarks>
        public static string ToJson(this DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            else if (dt.Kind == DateTimeKind.Unspecified) return dt.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            else return dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        /// <summary>Convert the timespan to JSON/ISO 8601.</summary>
        public static string ToJson(this TimeSpan ts)
        {
            StringBuilder rc = new StringBuilder(), rct = new StringBuilder();
            if (ts < TimeSpan.Zero) { rc.Append('-'); ts = ts.Negate(); }
            rc.Append('P');
            if (ts.Days > 0) rc.Append(ts.Days.ToString(CultureInfo.InvariantCulture) + 'D');
            if (ts.Hours > 0) rct.Append(ts.Hours.ToString(CultureInfo.InvariantCulture) + 'H');
            if (ts.Minutes > 0) rct.Append(ts.Minutes.ToString(CultureInfo.InvariantCulture) + 'M');
            if (ts.Seconds > 0 || ts.Milliseconds > 0)
            {
                rct.Append(ts.Seconds.ToString(CultureInfo.InvariantCulture));
                if (ts.Milliseconds > 0) rct.Append("." + ts.Milliseconds.ToString(CultureInfo.InvariantCulture));
                rct.Append('S');
            }
            if (rct.Length > 0) { rc.Append('T'); rc.Append(rct.ToString()); }
            return rc.ToString();
        }

        /// <summary>Convert the timespan to JSON/ISO 8601.</summary>
        public static JsonArray ToJson(this UnityEngine.Color color)
        {
            return new JsonArray((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }
    }
}
