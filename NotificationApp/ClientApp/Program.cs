using Microsoft.Azure.ServiceBus;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static Random random = new Random();
        static void Main(string[] args)
        {
            var numberOfMessages = 100;
            Console.WriteLine("Sending Messages...");
            MainAsync(numberOfMessages).GetAwaiter().GetResult();
        }

        static async Task MainAsync(int numberOfMessages)
        {
            string ServiceBusConnectionString = "Endpoint=sb://jomitnotificaitons.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=BvC1DEn3cO0+knb9p6a4mGjssxLwmcs4cNvEJtUJvMg=";
            string QueueName = "devicemessages";

            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            await SendMessagesAsync(queueClient, numberOfMessages);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            await queueClient.CloseAsync();
        }

        static async Task SendMessagesAsync(IQueueClient queueClient, int numberOfMessages)
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                for (var i = 0; i < numberOfMessages; i++)
                {
                    var tenantId = random.Next(1, 10).ToString();
                    string messageBody = $"Test Message {i} for TenantId {tenantId}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    message.Label = tenantId.ToString();

                    Console.WriteLine($"Sending message: {messageBody}");
                    await queueClient.SendAsync(message);
                }
                timer.Stop();
                Console.WriteLine($"Total {timer.Elapsed.Seconds} seconds, {numberOfMessages / timer.Elapsed.Seconds} msg/sec");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
