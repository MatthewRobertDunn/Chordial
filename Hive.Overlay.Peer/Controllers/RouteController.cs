﻿using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Controllers
{
    [Route("hive/v1/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IKadmeliaServer kadmeliaServer;
        private readonly ILogger<RouteController> log;

        public RouteController(IKadmeliaServer kadmeliaServer, ILogger<RouteController> log)
        {
            this.kadmeliaServer = kadmeliaServer;
            this.log = log;
            log.LogInformation("Route controller starting");
        }

        /// <summary>   
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpPost("closecontacts/")]
        public Contact[] CloseContacts(ClosestNodeSearch request)
        {
            return this.kadmeliaServer.CloseContacts(request.Address, request.RequestedBy);
        }

        /// <summary>
        /// Returns a list of nodes closest to the given network address.
        /// Set 'address' to the hive address you are searching for.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request">Base64 encoded string containing the hive address you are searching for</param>
        /// <returns>A list of nodes closest to the requested address</returns>
        [HttpGet("closecontacts/")]
        public Contact[] CloseContacts([FromQuery] string address)
        {
            return this.kadmeliaServer.CloseContacts(Convert.FromBase64String(address), null);
        }

        /// <summary>
        /// Returns the hiveid of this node
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("address/")]
        public byte[] Address(HiveRequest request)
        {
            return this.kadmeliaServer.Address(request.RequestedBy);
        }

        /// <summary>
        /// Returns the hiveid of this node
        /// </summary>
        /// <returns></returns>
        [HttpGet("address/")]
        public byte[] Address()
        {
            return this.kadmeliaServer.Address();
        }

    }
}
