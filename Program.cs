using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;


namespace MqttTest
{
    internal class Program
    {
        private static ILogger logger;

        static async Task Main(string[] args)
        {

            await Publish_Message_From_Broker();
           
        }

        public static async Task Publish_Message_From_Broker()
        {
            var mqttFactory = new MqttFactory(new ConsoleLogger());
               
               var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();
               
               
               
               
               using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
               {
                   
                   await mqttServer.StartAsync();

                   await PublishRandomNumbersPeriodically(mqttServer);

                   
                Console.WriteLine("Press Enter to exit.");
                   Console.ReadLine();
               
                   
                  
                

                // Stop and dispose the MQTT server if it is no longer needed!
                await mqttServer.StopAsync();
               }


            
        }


         static async Task PublishRandomNumbersPeriodically(MqttServer mqttServer)
        {
            var random = new Random();

            while (true)
            {
                int randomNumber = random.Next(10);
                string payload = randomNumber.ToString();
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("span/number")
                    .WithPayload(payload)
                    .Build();

                await mqttServer.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(message));
               Console.WriteLine($"Published random number: {payload}");

                await Task.Delay(TimeSpan.FromSeconds(5)); // Adjust the interval as needed.
            }
        }



        class ConsoleLogger : IMqttNetLogger
        {
            readonly object _consoleSyncRoot = new();

            public bool IsEnabled => true;

            public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
            {
                var foregroundColor = ConsoleColor.White;
                switch (logLevel)
                {
                    case MqttNetLogLevel.Verbose:
                        foregroundColor = ConsoleColor.White;
                        break;

                    case MqttNetLogLevel.Info:
                        foregroundColor = ConsoleColor.Green;
                        break;

                    case MqttNetLogLevel.Warning:
                        foregroundColor = ConsoleColor.DarkYellow;
                        break;

                    case MqttNetLogLevel.Error:
                        foregroundColor = ConsoleColor.Red;
                        break;
                }

                if (parameters?.Length > 0)
                {
                    message = string.Format(message, parameters);
                }

                lock (_consoleSyncRoot)
                {
                    Console.ForegroundColor = foregroundColor;
                    Console.WriteLine(message);

                    if (exception != null)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }
        }
    }
}
