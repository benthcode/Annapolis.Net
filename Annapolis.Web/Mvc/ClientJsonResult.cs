using Annapolis.Web.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Annapolis.Web.Mvc
{
    public class ClientJsonResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public ClientModel Model { get; set; }

        public ClientJsonResult() { }

        public ClientJsonResult(ClientModel data)
        {
            Model = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null) { response.ContentEncoding = ContentEncoding; }

            if (Model == null) return;
            var writer = new JsonTextWriter(response.Output);
            writer.WriteValue(Model.ToJson());
            writer.Flush();
        }

    }
}
