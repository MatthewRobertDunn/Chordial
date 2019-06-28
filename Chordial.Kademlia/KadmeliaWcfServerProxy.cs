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
            return ExceptionWrap(() => base.Channel.FindNode(senderId, key));
        }

        public SearchResult FindValue(Contact senderId, byte[] key)
        {
            return ExceptionWrap(() => base.Channel.FindValue(senderId, key));
        }

        public bool StoreValue(Contact senderId, byte[] key, string data, DateTime published, DateTime expires)
        {
            return ExceptionWrap(() => (base.Channel.StoreValue(senderId, key, data, published, expires)));
        }

        public byte[] Ping(Contact senderId)
        {
            return ExceptionWrap(() => (base.Channel.Ping(senderId)));
        }

        private T ExceptionWrap<T>(Func<T> wrap)
        {
            try
            {
                this.Open();
                return wrap();
            }
            catch (Exception ex)
            {
                this.Abort();
                return default(T);
            }
            finally
            {
                if (this.State == CommunicationState.Opened)
                    this.Close();
            }
        }
    }
}
