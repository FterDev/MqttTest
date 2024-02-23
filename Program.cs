using MQTTnet;

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


        }
    }
}
