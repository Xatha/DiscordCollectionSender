using BenchmarkDotNet.Attributes;
using CompressionLibrary;
using Discord;
using Discord.WebSocket;
using DiscordCollectionSenderBot.MessageService;
using DiscordCollectionSenderBot.MessageService.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestingConsoleApp
{
    [MemoryDiagnoser]
    public class Benchy
    {
        private readonly static DiscordSocketClient socketClient = new DiscordSocketClient();
        static SocketTextChannel? _textChannel;
        static MessageQueueHandler mqhInst = (MessageQueueHandler)MessageQueueHandler.Instance;
        static FactoryContainer factoryContainer = new FactoryContainer();

        public static async Task InitLogin()
        {
            var token = File.ReadAllText(@"C:\Users\Luca\Desktop\Programming\DiscordBots\token.txt");
            await socketClient.LoginAsync(TokenType.Bot, token);
            await socketClient.StartAsync();
            await Task.Delay(2000);

            _textChannel = socketClient.GetGuild(867504370525274153).GetTextChannel(986687943668285470);

            //Console.WriteLine(socketClient.ToString());

            //Console.WriteLine(_textChannel.ToString());

        }

        [GlobalSetup]
        public static void InitContainer()
        {
            factoryContainer.Register<TestClass, ITestClass>();
        }

        //[Benchmark]
        public void GetImplementationNormally()
        {
            var myClass = new TestClass();
            myClass.HelloWorld();
        }


        //[Benchmark]
        public async Task BuildMessageAndQueue()
        {
            var msg = await new MessageBuilder(_textChannel).SetMessageType(DiscordCollectionSenderBot.MessageService.MessageType.SendFile)
                                                           .SetMessageContent(null, "Dummytext")
                                                           .BuildAsync();
            await mqhInst.AddMessage(msg);
        }

        //[Benchmark]
        public int CheapOp()
        {
            return 6 + 10;
        }

        //[Benchmark]
        public int ExpensiveOp()
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                result = i ^ i;
            }
            return result;
        }
        //[Benchmark]
        public async ValueTask<double> CTRClassAsyncAwait()
        {
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            var ctr = new CompressionLibrary.CompressionRatioGenerator();
            return await ctr.GenerateCompressionRatioAsync(fileSizeInBytes, targetSize);
        }
        //[Benchmark]
        public ValueTask<double> CTRClass()
        {
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            var ctr = new CompressionLibrary.CompressionRatioGenerator();
            return ctr.GenerateCompressionRatioAsync(fileSizeInBytes, targetSize);
        }


        //[Benchmark]
        public async ValueTask<double> FuncUsingGCRValueTaskAsync()
        {
            var x = await GCRValueTaskAsync();
            return x + 1.5;
        }

        //[Benchmark]
        public async Task<double> FuncUsingGCRTaskc()
        {
            var x = await GCRTask();
            return x + 1.5;
        }

        //[Benchmark]
        public async ValueTask<double> GCRValueTaskWrappedAsync()
        {
            return await ValueTask.FromResult(await Task.Run(() => GCR()));
        }

        //[Benchmark]
        public async ValueTask<double> GCRValueTaskAsync()
        {
            long tolerance = 524288;
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            long adjustedTargetSize = targetSize - tolerance;
            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return await ValueTask.FromResult(compressionRatio);
        }

        //[Benchmark]
        public async Task<double> GCRTaskAsync()
        {
            long tolerance = 524288;
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            long adjustedTargetSize = targetSize - tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return await Task.FromResult(compressionRatio);
        }


        //[Benchmark]
        public ValueTask<double> GCRValueTask()
        {
            long tolerance = 524288;
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            long adjustedTargetSize = targetSize - tolerance;
            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return ValueTask.FromResult(compressionRatio);
        }

        // [Benchmark]
        public Task<double> GCRTask()
        {
            long tolerance = 524288;
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            long adjustedTargetSize = targetSize - tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return Task.FromResult(compressionRatio);
        }


        //[Benchmark]
        public double GCR()
        {
            long tolerance = 524288;
            long fileSizeInBytes = (long)16.389e+6;
            long targetSize = (long)8.389e+6;
            long adjustedTargetSize = targetSize - tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return compressionRatio;
        }
    }
}
