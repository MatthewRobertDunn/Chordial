using Hive.Cryptography.Certificates;
using Hive.Overlay.Api;
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
    public class CertificateController : ControllerBase
    {
        private readonly ILogger<CertificateController> log;

        public ICertificateStore CertificateStore { get; }

        public CertificateController(ICertificateStore certificateStore, ILogger<CertificateController> log)
        {
            CertificateStore = certificateStore;
            this.log = log;
            log.LogInformation("Certificate controller starting");
        }


        /// <summary>
        /// Returns public keys for this node in the pfx format.
        /// Password for the file is 'password'
        /// </summary>
        [HttpGet("pfx/")]
        public FileResult Pfx()
        {
           return File(CertificateStore.ToPublicPfxBytes(), "application/x-pkcs12");
        }
     
    }
}
