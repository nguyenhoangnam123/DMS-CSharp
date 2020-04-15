using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private IModel _channel;
        private IConfiguration Configuration;
        private readonly DefaultObjectPool<IModel> _objectPool;
        public ConsumeRabbitMQHostedService(IPooledObjectPolicy<IModel> objectPolicy, IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            string exchangeName = "exchange";
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 1);
            _channel = _objectPool.Get();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, true, false);
            _channel.QueueDeclare(StaticParams.ModuleName, true, false, false, null);
            _channel.QueueBind(StaticParams.ModuleName, exchangeName, $"{nameof(AppUser)}.*", null);
            _channel.QueueBind(StaticParams.ModuleName, exchangeName, $"{nameof(Organization)}.*", null);
            _channel.QueueBind(StaticParams.ModuleName, exchangeName, $"{nameof(Province)}.*", null);
            _channel.QueueBind(StaticParams.ModuleName, exchangeName, $"{nameof(District)}.*", null);
            _channel.QueueBind(StaticParams.ModuleName, exchangeName, $"{nameof(Ward)}.*", null);
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);
                var routingKey = ea.RoutingKey;
                // handle the received message  
                try
                {
                    _= HandleMessage(routingKey, content);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume(StaticParams.ModuleName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(string routingKey, string content)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext context = new DataContext(optionsBuilder.Options);
            IUOW UOW = new UOW(context);
            List<string> path = routingKey.Split(".").ToList();
            if (path.Count < 1)
                throw new Exception();
            switch (path[0])
            {
                case nameof(AppUser):
                    AppUserHandler AppUserHandler = new AppUserHandler(context, UOW);
                    await AppUserHandler.Handle(routingKey, content);
                    break;
                case nameof(Organization):
                    OrganizationHandler OrganizationHandler = new OrganizationHandler(context, UOW);
                    await OrganizationHandler.Handle(routingKey, content);
                    break;
                case nameof(Province):
                    ProvinceHandler ProvinceHandler = new ProvinceHandler(context, UOW);
                    await ProvinceHandler.Handle(routingKey, content);
                    break;
                case nameof(District):
                    DistrictHandler DistrictHandler = new DistrictHandler(context, UOW);
                    await DistrictHandler.Handle(routingKey, content);
                    break;
                case nameof(Ward):
                    WardHandler WardHandler = new WardHandler(context, UOW);
                    await WardHandler.Handle(routingKey, content);
                    break;
            }
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    }
}
