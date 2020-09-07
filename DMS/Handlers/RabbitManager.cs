using Common;
using Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Handlers
{
    public interface IRabbitManager
    {
        IModel Get();
        void PublishList<T>(List<EventMessage<T>> message, GenericEnum routeKey) where T : DataEntity;
        void PublishSingle<T>(EventMessage<T> message, GenericEnum routeKey) where T : DataEntity;
    }
    public class RabbitManager : IRabbitManager
    {
        private readonly IModel _channel;

        private readonly IConnection _connection;
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 16);
        }


        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public IModel Get()
        {
            return _channel;
        }

        public void PublishList<T>(List<EventMessage<T>> message, GenericEnum routeKey) where T : DataEntity
        {
            if (!StaticParams.EnableExternalService)
                return;
            if (message == null)
                return;

            var channel = _channel;

            try
            {
                channel.ExchangeDeclare("exchange", ExchangeType.Topic, true, false, null);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish("exchange", routeKey.Code, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PublishSingle<T>(EventMessage<T> message, GenericEnum routeKey) where T : DataEntity
        {
            if (message == null)
                return;

            List<EventMessage<T>> list = new List<EventMessage<T>> { message };
            PublishList(list, routeKey);
        }
    }
}
