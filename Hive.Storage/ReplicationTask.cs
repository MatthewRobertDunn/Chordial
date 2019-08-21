using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Storage
{
    /// <summary>
    /// Base class all replication tasks implement
    /// </summary>
    public abstract class ReplicationTask : BackgroundService
    {
        public TimeSpan ReplicationInterval { get; private set; }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => OnStartup(stoppingToken));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(ReplicationInterval, stoppingToken);
                await Task.Run(() => Replicate(stoppingToken), stoppingToken);
            }
        }

        protected virtual void OnStartup(CancellationToken stoppingToken) { }

        protected virtual void Replicate(CancellationToken stoppingToken) { }
    }
}
