using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Annapolis.Web.Client;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Annapolis.WebSite.Application
{
    public class JsonNetFormatter : JsonMediaTypeFormatter
    {
        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type)
        {
            // don't serialize JsonValue structure use default for that
            //if (type == typeof(JsonValue) || type == typeof(JsonObject) || type == typeof(JsonArray))
            //    return false;
            
            return true;
        }

        public override bool CanReadType(Type type)
        {
           return true;
        }

    

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                //var settings = new JsonSerializerSettings()
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //};

                var sr = new StreamReader(readStream);
                string json = sr.ReadToEnd();

                object val = JsonConvert.DeserializeObject(json, type);
                return val;
            });

            return task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                //var settings = new JsonSerializerSettings()
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //};

                //string json = JsonConvert.SerializeObject(value, Formatting.Indented,
                //                                          new JsonConverter[1] { new IsoDateTimeConverter() });

                string json = ClientModel.ToJson(value);

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();
            });

            return task;
        }

    }
}