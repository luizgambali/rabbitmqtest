using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "p@ssword123" };

using var connection = factory.CreateConnectionAsync();
using var channel = connection.Result.CreateChannelAsync();


//cria o exchange (canal de comunicação) do tipo direct. Faz o roteamento das mensagens com base na routing key
await channel.Result.ExchangeDeclareAsync(exchange: "test-message", type: ExchangeType.Direct);

//cria a fila e a vincula ao exchange. Essa fila será utilizada pelo consumer para receber as mensagens
//garante que a fila exista, caso contrário o consumer não conseguirá receber as mensagens
await channel.Result.QueueDeclareAsync(queue: "test-queue", durable: true, exclusive: false, autoDelete: false);
await channel.Result.QueueBindAsync(queue: "test-queue",exchange: "test-message",routingKey: "test");

var consumer = new RabbitMQ.Client.Events.AsyncEventingBasicConsumer(channel.Result);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    await Task.Yield();
};

await channel.Result.BasicConsumeAsync(queue: "test-queue",
                             autoAck: true,
                             consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();