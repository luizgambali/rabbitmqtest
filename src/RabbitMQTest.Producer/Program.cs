using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password= "p@ssword123" };

using var connection = factory.CreateConnectionAsync();
using var channel = connection.Result.CreateChannelAsync();

//cria o exchange (canal de comunicação) do tipo direct. Faz o roteamento das mensagens com base na routing key
await channel.Result.ExchangeDeclareAsync(exchange: "test-message", type: ExchangeType.Direct);

//cria a fila e a vincula ao exchange. Essa fila será utilizada pelo consumer para receber as mensagens
//apesar de a fila ser criada aqui, o consumer também pode criá-la, desde que com as mesmas configurações
await channel.Result.QueueDeclareAsync(queue: "test-queue", durable: true, exclusive: false, autoDelete: false);
await channel.Result.QueueBindAsync(queue: "test-queue", exchange: "test-message", routingKey: "test");

string message = "Hello! This is a test message from RabbitMQTest.Producer.";

await Task.Delay(2000); //aguarda o consumer estar em execucao

for (int i = 0; i < 10; i++)
{
    //converte a mensagem para um array de bytes
    var body = System.Text.Encoding.UTF8.GetBytes($"{message} Message number: {i + 1}");

    await channel.Result.BasicPublishAsync(exchange: "test-message",
                                           routingKey: "test",
                                           mandatory: false,                                          
                                           body: body);

    Console.WriteLine($" [x] Sent {message} Message number: {i + 1}");
    await Task.Delay(500);
}