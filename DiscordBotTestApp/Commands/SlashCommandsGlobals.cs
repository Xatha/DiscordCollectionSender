using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp.Commands
{
    class SlashCommandsGlobals
    {
        private List<SlashCommandBuilder> slashCommandCollection = new List<SlashCommandBuilder>();
        private static LoggerLibrary.Logger logger = new LoggerLibrary.Logger();

        private DiscordSocketClient client;

        public SlashCommandBuilder PostCollection { get; private set; }
        public SlashCommandBuilder MyPurpose { get; private set; }
        
        public SlashCommandBuilder Ping { get; private set; }

        public SlashCommandBuilder Stop { get; private set; }


        public SlashCommandsGlobals(DiscordSocketClient client)
        {
            this.client = client;

            BuildCommands();
            CreateCommandsAsync().Wait();
        }

        private void BuildCommands()
        {
            this.PostCollection = new SlashCommandBuilder()
                    .WithName("post-collection")
                    .WithDescription("Post a collection of picture of supply the bot.")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("with")
                        .WithDescription("You can input your collection with a download URL or folder attachment.")
                        .WithDescription("Collection *has* to be a compressed file. Default filetype is .zip.")
                        .WithType(ApplicationCommandOptionType.SubCommandGroup)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("url")
                            .WithDescription("Gets the collection via the url you supply")
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption("url", ApplicationCommandOptionType.String, "The url to download.", isRequired: true)
                            .AddOption("file-type", ApplicationCommandOptionType.String, "Extension of the file supplied. Default is .zip.", isRequired: false)
                        ).AddOption(new SlashCommandOptionBuilder()
                            .WithName("attachment")
                            .WithDescription("Gets the collection via the attachment provided.")
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption("attachment", ApplicationCommandOptionType.Attachment, "Compressed file", isRequired: true)
                            .AddOption("file-type", ApplicationCommandOptionType.String, "Extension of the file supplied. Default is .zip.", isRequired: false)
                        ));
            slashCommandCollection.Add(this.PostCollection);

            this.MyPurpose = new SlashCommandBuilder()
                          .WithName("my-purpose")
                          .WithDescription("Beeb Boop, I am.. and my purpose is...");
            slashCommandCollection.Add(this.MyPurpose);

            this.Ping = new SlashCommandBuilder().WithName("ping").WithDescription("ping!");
            slashCommandCollection.Add(this.Ping);

            this.Stop = new SlashCommandBuilder().WithName("stop").WithDescription("terminate the bot");
            slashCommandCollection.Add(this.Stop);
        }

        internal async Task CreateCommandsAsync()
        {
            foreach (var command in slashCommandCollection)
            {
                await client.Rest.CreateGuildCommand(command.Build(), 867504370525274153);
                await client.Rest.CreateGuildCommand(command.Build(), 438045930323968002);
                await logger.Log($"{command.Name} was successfully created", nameof(SlashCommandsGlobals));
            }



            foreach (var command in slashCommandCollection)
            {
                //await client.CreateGlobalApplicationCommandAsync(command.Build());

            }
        }

        internal async Task DeleteAllGlobalSlashCommands()
        {
            await client.Rest.DeleteAllGlobalCommandsAsync();
        }

    }
}

