using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Annapolis.Web.Client;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Annapolis.Web.Http
{
    public class JsonNetMediaTypeFormatter : JsonMediaTypeFormatter
    {
       
        public JsonNetMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type)
        {
           
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
                var sr = new StreamReader(readStream);
                string json = sr.ReadToEnd();

                object val = ClientModel.FromJson(json, type);
                return val;
            });

            return task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                string json = ClientModel.ToJson(value);
                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();
            });

            return task;
        }

    }
}