using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Annapolis.Web.Client
{
    public class ClientModelMvcModelBinder :  DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var contentType = controllerContext.HttpContext.Request.ContentType;
            if (!contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)) return null;

            string bodyText;

            using (var stream = controllerContext.HttpContext.Request.InputStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    bodyText = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(bodyText)) return (null);

            var obj = JsonConvert.DeserializeObject(bodyText, bindingContext.ModelType);

            return obj;
        }
    }

}
