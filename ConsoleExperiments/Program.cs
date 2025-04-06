using System.ComponentModel;
using Confluent.Kafka;

namespace KafkaExample
{
    class Program
    {
        // Kafka broker address (typically would be your Kafka cluster address)
        private const string BootstrapServers = "localhost:9092";

        // Topic name our producer will send messages to
        private const string Topic = "example-topic";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Kafka Producer...");
            Console.WriteLine("Press 'P' to produce a message, 'C' to consume messages, or 'Q' to quit.");

            while (true)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        await ProduceMessages();
                        break;

                    case ConsoleKey.C:
                        await ConsumeMessages();
                        break;

                    case ConsoleKey.Q:
                        return;
                }

            }
        }

        static async Task ProduceMessages()
        {
            Console.WriteLine("Producer starting...");

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
            };

            // create the producer
            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                try
                {
                    Console.WriteLine("Enter message to send (or 'exit' to stop producing):");

                    while (true)
                    {
                        Console.Write("> ");
                        var message = Console.ReadLine();

                        if (message.ToLower() == "exit")
                            break;

                        string key = DateTime.Now.Ticks.ToString();

                        var result = await producer.ProduceAsync(Topic, new Message<string, string>
                        {
                            Key = key,
                            Value = message
                        });

                        Console.WriteLine($"Delivered '{message}' to {result.TopicPartitionOffset}");
                    }
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"Failed to deliver message: {e.Error.Reason}");
                }
            }
        }

        static async Task ConsumeMessages()
        {
            Console.WriteLine("Consumer starting ...");

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = "example-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.WriteLine("Press 'X' to stop consuming");

            var consumerTask = Task.Run(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
                {
                    try
                    {
                        consumer.Subscribe(Topic);
                        Console.WriteLine($"Subscribed to: {Topic}");
                        Console.WriteLine("Waiting for messages...");

                        while (!cts.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));

                                if (consumeResult != null)
                                {
                                    var message = consumeResult.Message;
                                    Console.WriteLine($"Received: Key={message.Key}, Value={message.Value} from partition {consumeResult.Partition.Value} at offset {consumeResult.Offset.Value}");
                                }
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Consume error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Normal shutdown
                    }
                    finally
                    {
                        consumer.Close();
                    }
                }
            }, cts.Token);

            while (!consumerTask.IsCompleted)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.X)
                    {
                        cts.Cancel();
                        Console.WriteLine("Stopping consumer...");
                        break;
                    }
                }
                await Task.Delay(100);
            }

            await consumerTask;
            Console.WriteLine("Consumer stopped");
        }
    }
}