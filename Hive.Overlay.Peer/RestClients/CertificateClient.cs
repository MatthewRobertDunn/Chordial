using Hive.Overlay.Api;
using Hive.Overlay.Api.Certificates;
using Hive.Overlay.Peer.Dto;
using Hive.Overlay.Peer.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hive.Overlay.Peer.RestClients
{
    public class CertificateClient : RestClient, ICertificates
    {
        
        public CertificateClient(Uri remoteAddress)
        {
            this.BaseUri = new Uri(remoteAddress, "/hive/v1/certificates/");
        }

        public byte[] GetPkcs12Certificates(byte[] hiveId)
        {
            throw new NotImplementedException();
        }

        public byte[] StorePkcs12Certificates(byte[] hiveId)
        {
            throw new NotImplementedException();
        }
    }
}
