using Discord;
using Discord.Net;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Security.Claims;
using Discord.Commands;
using UbiServices;

namespace DiscordBot
{
    internal class Program
    {
        public string token = "";
        public ulong guildId = 0;
        public static Task ReadKeyAsync()
        {
            Console.ReadLine();
            return Task.CompletedTask;
        }
#pragma warning disable 4014
        public static void Main() => new Program().AsyncMain();
#pragma warning restore 4014

#pragma warning disable 8618
        private DiscordSocketClient _client;
#pragma warning restore 8618
        public async Task AsyncMain()
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("No token");
                return;
            }
            else
            {
                token = File.ReadAllLines("token.txt")[0];
            
            }

            if (!File.Exists("guild.txt"))
            {
                Console.WriteLine("No guild");
                return;
            }
            else
            {
                guildId = ulong.Parse(File.ReadAllLines("guild.txt")[0]);

            }

            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Ready;
            _client.SlashCommandExecuted += SlashCommandExecuted;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await ReadKeyAsync();
            await _client.StopAsync();
            Console.WriteLine("End");
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        private async Task Ready()
        {
            SocketGuild guild = _client.GetGuild(guildId);
            if (guild == null) { Console.WriteLine("Failed to get guild ID!"); return; }

            List<SlashCommandProperties> vCmd = new();

            // Use this to remove old commands
            //await guild.DeleteApplicationCommandsAsync(new RequestOptions() { AuditLogReason = "Reset" });

            await _client.SetGameAsync("Uplay Failure" , "" , ActivityType.Watching);


            //https://discordnet.dev/guides/int_basics/application-commands/slash-commands/subcommands.html
            vCmd.Add(new SlashCommandBuilder()
            .WithName("getspace")
            .WithDescription("Get space by SpaceId.")
            .AddOption("spaceid", ApplicationCommandOptionType.String, "The SpaceId.", true)
            .Build()
            );


            vCmd.Add(new SlashCommandBuilder()
            .WithName("pingpong")
            .WithDescription("Pong")
            .Build()
            );

            foreach (var cmd in vCmd)
            {
                await guild.CreateApplicationCommandAsync(cmd);
            }
            Console.WriteLine("Slash commands successfully created!");
        }
        private async Task SlashCommandExecuted(SocketSlashCommand cmd)
        {
            var curDB = _client.CurrentUser;
            var embedBuiler = new EmbedBuilder()
            .WithAuthor(curDB.ToString().Split("#")[0], curDB.GetAvatarUrl() ?? curDB.GetDefaultAvatarUrl())
            .WithTitle("")
            .WithDescription("")
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
            switch (cmd.Data.Name)
            {
                case "getspace":
                    embedBuiler = getspace(embedBuiler,cmd);
                    await cmd.RespondAsync(embed: embedBuiler.Build());
                    return;
                case "pingpong":
                    embedBuiler = pong(embedBuiler, cmd);
                    await cmd.RespondAsync(embed: embedBuiler.Build());
                    return;
                default:
                    return;
            }
        }


        private EmbedBuilder getspace(EmbedBuilder embed, SocketSlashCommand cmd)
        {
            var desc = "";
            embed.Title = "Getting Spaces!";
            //Todo check for pattern
            var callback = UbiServices.Public.V1.Spaces.GetSpaces((string)cmd.Data.Options.ToList()[0].Value);
            if (callback == null)
            {
                desc = "Nothing was recieved :(";
            }
            else
            {
                desc = JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
            embed.Description = desc;
            return embed;
        }

        private EmbedBuilder pong(EmbedBuilder embed, SocketSlashCommand cmd)
        {
            embed.Title = "Pong";
            embed.Description = "Pong";
            return embed;
        }
    }
}
