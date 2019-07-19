using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class HiveController : ControllerBase
    {
        /// <summary>
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpPost("/findnode/")]
        public SearchResult FindNode(ClosestNodeSearch request)
        {
            return new SearchResult();
        }

        /// <summary>
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="request">Base64 encoded string containing the hive address you are searching for</param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpGet("/findnode/{address}")]
        public SearchResult FindNode([FromQuery] string address)
        {
            return new SearchResult();
        }

    }
}
