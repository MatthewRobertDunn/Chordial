using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    public class KadmeliaWcfServerProxy : ClientBase<IKadmeliaServer>, IKadmeliaServer
    {
        private ID remoteId;

        public event EventHandler<RemotePeerRespondedEventArgs> RemotePeerResponded;

        public KadmeliaWcfServerProxy()
        {
        }

        public KadmeliaWcfServerProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public KadmeliaWcfServerProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public KadmeliaWcfServerProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public KadmeliaWcfServerProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        public SearchResult FindNode(Contact senderId, byte[] key)
        {
            return base.Channel.FindNode(senderId, key);
        }

        public SearchResult FindValue(Contact senderId, byte[] key)
        {
            return base.Channel.FindValue(senderId, key);
        }

        public void StoreValue(Contact senderId, byte[] key, string data, DateTime published, DateTime expires)
        {
            base.Channel.StoreValue(senderId, key, data, published, expires);
        }

        public byte[] Ping(Contact senderId)
        {
            return base.Channel.Ping(senderId);
        }

        protected virtual void OnRemotePeerResponded(RemotePeerRespondedEventArgs e)
        {
            if (this.RemotePeerResponded != null)
                this.RemotePeerResponded(this, e);
        }
    }


    public class RemotePeerRespondedEventArgs
    {
        public ID Peer { get; set; }
        public string Url { get; set; }
    }
}
