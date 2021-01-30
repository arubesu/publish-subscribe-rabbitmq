using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
  class Consumer
  {
    static void Main(string[] args)
    {
      var factory = new ConnectionFactory { HostName = "localhost" };

      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;

        channel.QueueBind(queue: queueName,
                          exchange: "logs",
                          routingKey: "");

        Console.WriteLine(" [*] Waiting for logs.");

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, eventArgs) =>
        {
          var body = eventArgs.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);

          System.Console.WriteLine($" [x] {message}");
        };

        channel.BasicConsume(queue: queueName,
                              autoAck: true,
                              consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
      }
    }
  }
}
