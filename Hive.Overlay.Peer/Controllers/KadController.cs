using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Controllers
{
    [Route("1.0/[controller]")]
    [ApiController]
    public class KadController : ControllerBase
    {
        /// <summary>
        /// Hey there cute stuff
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/findnode/")]
        public SearchResult FindNode(FindRequest request)
        {
            return new SearchResult();
        }

        
    }
}
