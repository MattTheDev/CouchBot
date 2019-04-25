using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTD.CouchBot.Domain.Models.Mixer;
using Newtonsoft.Json;

namespace MTD.CouchBot.Api.Controllers
{
    [Route("api/[controller]")]
    public class MixerController : Controller
    {
        // POST api/values
        [HttpPost]
        public void Post([FromBody]WebhookChannel channel)
        {
            System.IO.File.AppendAllText($"C:\\temp\\output.json", JsonConvert.SerializeObject(channel));
        }
    }
}
