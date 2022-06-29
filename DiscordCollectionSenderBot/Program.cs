[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DiscordCollectionSenderBot
{
    public class Program
    {
        private Client? _instance;

        public static Task Main() => new Program().MainAsync();
        public async Task MainAsync()
        {
            _instance = await Client.CreateAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}