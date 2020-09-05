using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace QRReader.Controllers
{
   
    public class QRController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ReadQRCode()
        {

            HttpResponseMessage response = null;

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var postedFile = httpRequest.Files[0];
                var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                postedFile.SaveAs(filePath);

                using (var client = new HttpClient())
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    HttpContent content = new StringContent("fileToUpload");
                    form.Add(content, "fileToUpload");

                    var stream = new FileStream(filePath, FileMode.Open);
                    content = new StreamContent(stream);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "fileToUpload"
                    };
                    form.Add(content);

                    try
                    {
                        response = (client.PostAsync("http://api.qrserver.com/v1/read-qr-code", form)).Result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                }
            }
            return response;
        }
    }
}
