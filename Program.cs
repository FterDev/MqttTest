using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MqttTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883)
                .Build();

            var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
            
            await mqttServer.StartAsync();
            Console.WriteLine("MQTT Broker started.");

            var cancellationToken = new CancellationTokenSource();

            Task.Run(() => PublishRandomNumbersPeriodically(mqttServer, cancellationToken.Token));

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

            await cancellationToken.CancelAsync();
            await mqttServer.StopAsync();


        }


        static async Task PublishRandomNumbersPeriodically(MqttServer mqttServer, CancellationToken cancellationToken)
        {
            var random = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                int randomNumber = random.Next(10);
                string payload = randomNumber.ToString();
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("span/number")
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await mqttServer.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(message)
                );

                Console.WriteLine($"Published random number: {payload}");

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); 
            }
        }
    }
}
