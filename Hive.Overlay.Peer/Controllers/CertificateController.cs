using Hive.Cryptography.Certificates;
using Hive.Cryptography.Primitives;
using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Hive.Storage.Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Controllers
{
    /// <summary>
    /// Controller responsible for storing and returning certificates
    /// </summary>
    [Route("hive/v1/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateRepository certificateRepository;
        private readonly ILogger<CertificateController> log;

        /// <summary>
        /// Construct a new Certificate controller
        /// </summary>
        /// <param name="certificateRepository"></param>
        /// <param name="log">logger</param>
        public CertificateController(ICertificateRepository certificateRepository, ILogger<CertificateController> log)
        {
            this.certificateRepository = certificateRepository;
            this.log = log;
            log.LogInformation("Certificate controller starting");
        }


        /// <summary>
        /// Returns public keys for a given node in pfx format.
        /// Password for the file is '' (empty string)
        /// </summary>
        [HttpGet("pfx/")]
        public FileResult Pfx(string hiveId)
        {
            var hiveBytes = hiveId.FromBase64();
           return File(certificateRepository.GetCertificate(hiveBytes), "application/x-pkcs12");
        }

        /// <summary>
        /// Stores a pkcs12 formatted certificate chain in this node.
        /// </summary>
        /// <param name="request"></param>
        [HttpPost("pfx/")]
        public void Store (StorePfxCertificate request)
        {
            certificateRepository.AddCertificate(request.HiveId, request.Certificate);
        }
    }
}
