/*using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication10.Service
{
    public class RabbitMqConsumer
    {
        public async Task ConsumeOrdersAsync()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[✓] Yeni Sipariş Alındı: {message}");

                    // 📌 Burada siparişi işleyebilirsin (veritabanına kaydetme vb.)
                    await Task.CompletedTask;
                };

                channel.BasicConsume(queue: "orderQueue",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("📢 Siparişleri dinliyorum...");
                await Task.Delay(Timeout.Infinite); // Sonsuza kadar dinlemeye devam et
            }
        }
    }
}*/