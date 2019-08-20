using Hive.Overlay.Kademlia;
using Hive.Overlay.Peer.Settings;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Overlay.Peer.Tasks
{
    public class BootsrapPeer : BackgroundService
    {
        private readonly IKademilaClient client;
        private readonly PeerSettings settings;

        public BootsrapPeer(IKademilaClient client, PeerSettings settings)
        {
            this.client = client;
            this.settings = settings;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => client.Booststrap(settings.BootstrapPeers));
        }
    }
}
