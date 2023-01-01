#nullable enable
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SrsManageCommon
{

    /// <summary>
    /// json tool class
    /// </summary>
    public static class JsonHelper
    {
        private static JsonSerializerSettings _jsonSettings;

        static JsonHelper()
        {
            IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
            datetimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.MissingMemberHandling = MissingMemberHandling.Error;
            _jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            _jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            _jsonSettings.Converters.Add(datetimeConverter);
        }


        //format json string
        public static string ConvertJsonString(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object? obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


        /// <summary>
        /// Serialize the specified object into JSON data
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, MissingMemberHandling p = MissingMemberHandling.Error)
        {
            _jsonSettings.MissingMemberHandling = p;
            try
            {
                if (null == obj)
                    return null!;

                return JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
            }
            catch
            {
                return null!;
            }
        }

        /// <summary>
        /// Deserialize the specified JSON data into the specified object
        /// </summary>
        /// <typeparam name="T">object type.</typeparam>
        /// <param name="json">JSON data.</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json, MissingMemberHandling p = MissingMemberHandling.Error)
        {
            _jsonSettings.MissingMemberHandling = p;
            try
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings)!;
            }
            catch
            {
                return default(T)!;
            }
        }
    }
}