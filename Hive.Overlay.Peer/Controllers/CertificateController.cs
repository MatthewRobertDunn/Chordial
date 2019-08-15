using Hive.Cryptography.Certificates;
using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Controllers
{
    [Route("hive/v1/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        public ICertificateStore CertificateStore { get; }

        public CertificateController(ICertificateStore certificateStore)
        {
            CertificateStore = certificateStore;
        }


        /// <summary>
        /// Returns public keys for this node in the pfx format.
        /// Password for the file is 'password'
        /// </summary>
        [HttpGet("pfx/")]
        public FileResult Pfx([FromQuery] string address)
        {
           return File(CertificateStore.ToPublicPfxBytes(), "application/x-pkcs12");
        }
     
    }
}
