using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace WebApplication10.Service
{
    public class RabbitMqPublisher
    {
        public void PublishOrderToQueue(object order)
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
                // 📌 Kuyruğu oluştur (Eğer yoksa)
                channel.QueueDeclare(queue: "orderQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // 📌 Sipariş bilgisini JSON formatına çevir
                string message = JsonSerializer.Serialize(order);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;  // Mesajın kalıcı olmasını sağla

                // 📌 RabbitMQ'ya mesaj gönder
                channel.BasicPublish(exchange: "",
                    routingKey: "orderQueue",
                    basicProperties: properties,
                    body: body);

                Console.WriteLine($"[✔] Sipariş kuyruğa eklendi: {message}");
            }
        }
    }
}