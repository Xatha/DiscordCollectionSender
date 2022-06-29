using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordCollectionSenderBot.FileProcessing;
using DiscordCollectionSenderBot.Logger;
using log4net;
using LoggerLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot.Commands
{
    internal sealed class SlashCommandsGlobals
    {
        private static readonly ILog logger = LogAsync.GetLogger();


        private static readonly DiscordSocketClient client = ClientGlobal.SocketClient;

        private List<SlashCommandBuilder> slashCommandCollection = new List<SlashCommandBuilder>();
        public SlashCommandBuilder PostCollection { get; private set; }
        public SlashCommandBuilder MyPurpose { get; private set; }
        public SlashCommandBuilder Ping { get; private set; }
        public SlashCommandBuilder Stop { get; private set; }
        public SlashCommandBuilder GatewayTest { get; private set; }

        public SlashCommandsGlobals()
        {
            BuildCommands();
            CreateCommandsAsync().Wait();
        }

        private void BuildCommands()
        {
            this.GatewayTest = new SlashCommandBuilder()
                    .WithName("gateway-test").WithDescription("tests the gateway.");
            slashCommandCollection.Add(this.GatewayTest);

            this.PostCollection = new SlashCommandBuilder()
                    .WithName("post-collection")
                    .WithDescription("Post a collection of picture of supply the bot.")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("with")
                        .WithDescription("You can input your collection with a download URL or folder attachment.")
                        .WithDescription("Collection *has* to be a compressed file. DEFAULT: .zip.")
                        .WithType(ApplicationCommandOptionType.SubCommandGroup)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("url")
                            .WithDescription("Gets the collection via the url you supply")
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption(new SlashCommandOptionBuilder().WithName("sorting-type")
                                                                      .WithDescription("Choose in what order the bot should post the collection. DEFAULT: Ascending: By Name")
                                                                      .WithRequired(false)
                                                                      .WithType(ApplicationCommandOptionType.Integer)
                                                                      .AddChoice("Ascending: By Name", (int)FileSortType.ByNameAscending)
                                                                      .AddChoice("Ascending: By Creation Date", (int)FileSortType.ByDateAscending)
                                                                      .AddChoice("Ascending: By File Size", (int)FileSortType.BySizeAscending)
                                                                      .AddChoice("Descending: By Name", (int)FileSortType.ByNameDescending)
                                                                      .AddChoice("Descending: By Creation Date", (int)FileSortType.ByDateDescending)
                                                                      .AddChoice("Descending: By File Size", (int)FileSortType.BySizeDescending))
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption("url", ApplicationCommandOptionType.String, "The url to download.", isRequired: true)
                            .AddOption("file-type", ApplicationCommandOptionType.String, "Extension of the file supplied. Default is .zip.", isRequired: false)
                        ).AddOption(new SlashCommandOptionBuilder()
                            .WithName("attachment")
                            .WithDescription("Gets the collection via the attachment provided.")
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption(new SlashCommandOptionBuilder().WithName("sorting-type")
                                                                      .WithDescription("Choose in what order the bot should post the collection. DEFAULT: Ascending: By Name")
                                                                      .WithRequired(false)
                                                                      .WithType(ApplicationCommandOptionType.Integer)
                                                                      .AddChoice("Ascending: By Name", (int)FileSortType.ByNameAscending)
                                                                      .AddChoice("Ascending: By Creation Date", (int)FileSortType.ByDateAscending)
                                                                      .AddChoice("Ascending: By File Size", (int)FileSortType.BySizeAscending)
                                                                      .AddChoice("Descending: By Name", (int)FileSortType.ByNameDescending)
                                                                      .AddChoice("Descending: By Creation Date", (int)FileSortType.ByDateDescending)
                                                                      .AddChoice("Descending: By File Size", (int)FileSortType.BySizeDescending))
                            .AddOption("attachment", ApplicationCommandOptionType.Attachment, "Compressed file", isRequired: true)
                            .AddOption("file-type", ApplicationCommandOptionType.String, "Extension of the file supplied. DEFAULT: .zip.", isRequired: false)
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

                await logger.InfoAsync($"{command.Name} was successfully created");
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

