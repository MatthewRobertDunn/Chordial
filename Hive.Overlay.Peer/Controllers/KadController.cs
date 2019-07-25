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
        private readonly IKadmeliaServer kadmeliaServer;

        public HiveController(IKadmeliaServer kadmeliaServer)
        {
            this.kadmeliaServer = kadmeliaServer;
        }
        /// <summary>
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpPost("/closecontacts/")]
        public SearchResult CloseContacts(ClosestNodeSearch request)
        {
            return this.kadmeliaServer.CloseContacts(request.Address, request.RequestedBy);
        }

        /// <summary>
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="request">Base64 encoded string containing the hive address you are searching for</param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpGet("/closecontacts/")]
        public SearchResult CloseContacts([FromQuery] string address)
        {
            return this.kadmeliaServer.CloseContacts(Convert.FromBase64String(address), null);
        }

    }
}
