using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexmo.Api.Voice.Nccos;
using Nexmo.Api.Voice.Nccos.Endpoints;

namespace TranscriptionBlogPostDemo.Controllers
{
    public class VoiceController : Controller
    {
        const string BASE_URL = "BASE_URL";
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public HttpStatusCode Events()
        {
            return HttpStatusCode.OK;
        }

        [HttpGet]
        public string Answer()
        {
            var webSocketAction = new ConnectAction()
            {
                Endpoint = new[]
                {
                    new WebsocketEndpoint()
                    {
                        Uri = $"wss://{BASE_URL}/ws",
                        ContentType="audio/l16;rate=8000",                        

                    }
                }
            };

            var ncco = new Ncco(webSocketAction);
            return ncco.ToString();
        }
    }
}