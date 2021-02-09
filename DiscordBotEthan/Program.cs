using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotEthan {

    internal class Program {
        public static DiscordClient discord;
        public static DiscordColor EmbedColor = new DiscordColor("#3299E0");
        public static readonly ulong MutedRole = 765286908133638204;
        public static readonly Random gen = new Random();

        private static readonly string[] Statuses = new[] { "Allah is watchin", "Despacito", "Fuck", "Janitor cleanup", "Cheaing in CSGO", "EAC Bypass" };
        private static readonly ulong LearnerRole = 734242782092329101;
        //private static string SpamFilterMSG;

        private static void Main() {
            Console.WriteLine("Started");
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task Warn(DiscordChannel channel, DiscordUser member, string reason) {
            var WarnS = await PlayerSystem.GetPlayer(member.Id);
            WarnS.Warns.Add(reason);

            bool IsMuted = false;

            if (WarnS.Warns.Count >= 3) {
                _ = Task.Run(async () => {
                    try {
                        DiscordRole muterole = channel.Guild.GetRole(MutedRole);
                        var CMember = await channel.Guild.GetMemberAsync(member.Id);
                        await CMember.GrantRoleAsync(muterole);
                        await CMember.SendMessageAsync("You got muted for 24 Hours because you have equal or more then 3 Warns.");
                        await Task.Delay(86400000);
                        WarnS.Muted = false;
                        WarnS.Save(member.Id);
                        await CMember.RevokeRoleAsync(muterole);
                    } catch (Exception) {
                        discord.Logger.LogInformation($"Failed the Warn Tempmute process for {member.Mention}");
                    }
                });
                IsMuted = true;
                WarnS.Muted = true;
            }
            WarnS.Save(member.Id);

            DiscordEmbedBuilder Warns = new DiscordEmbedBuilder {
                Title = $"Warns | {member.Username}",
                Description = $"**{member.Mention} has been warned for the following Reason:**\n{reason}\n**Muted: {(IsMuted ? $"True\nUnmuted on {DateTime.Now.AddMilliseconds(86400000):dd.MM.yyyy HH:mm}" : "False")}**",
                Color = EmbedColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Made by JokinAce 😎" },
                Timestamp = DateTimeOffset.Now
            };
            await channel.SendMessageAsync(embed: Warns);
        }

        private static async Task MainAsync() {
            discord = new DiscordClient(new DiscordConfiguration {
                Token = "",
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Information,
                Intents = DiscordIntents.GuildMembers | DiscordIntents.AllUnprivileged
            });

            discord.Ready += (dc, args) => {
                _ = Task.Run(() => ThreadsShit.StatusChanger(dc));
                return Task.CompletedTask;
            };

            discord.GuildMemberAdded += async (dc, args) => {
                await args.Member.GrantRoleAsync(args.Guild.GetRole(LearnerRole));

                var PS = await PlayerSystem.GetPlayer(args.Member.Id);
                if (PS.Muted) {
                    _ = Task.Run(async () => {
                        try {
                            DiscordRole MutedRole = args.Guild.GetRole(Program.MutedRole);
                            await args.Member.GrantRoleAsync(MutedRole);
                            await Task.Delay(86400000);
                            PS.Muted = false;
                            PS.Save(args.Member.Id);
                            await args.Member.RevokeRoleAsync(MutedRole);
                        } catch (Exception) {
                            dc.Logger.LogInformation($"Failed the Mute Bypass detection process for {args.Member.Mention}");
                        }
                    });
                }
            };

            discord.MessageCreated += (dc, args) => {
                _ = Task.Run(async () => {
                    if (args.Author.IsBot)
                        return;

                    var PS = await PlayerSystem.GetPlayer(args.Author.Id);
                    if (PS.LastMessages.Count > 1 && PS.LastMessages.Contains(args.Message.Content)) {
                        await Warn(args.Channel, args.Author, "Spamming");
                        PS.LastMessages.Clear();
                    } else if (PS.LastMessages.Contains(args.Message.Content)) {
                        PS.LastMessages.Add(args.Message.Content);
                    } else {
                        PS.LastMessages.Clear();
                        PS.LastMessages.Add(args.Message.Content);
                    }
                    PS.Save(args.Author.Id);

                    if (gen.Next(500) == 1) {
                        using WebClient client = new WebClient();
                        await args.Channel.SendMessageAsync(client.DownloadString("https://insult.mattbas.org/api/insult"));
                    }

                    if (args.Message.Attachments.Count > 0) {
                        foreach (var attach in args.Message.Attachments) {
                            if (attach.FileName.Contains("exe")) {
                                await args.Message.DeleteAsync("EXE File");
                                await Warn(args.Channel, args.Author, "Uploading a EXE File");
                            }
                        }
                    } else if (args.Message.Content.Replace(" ", "").Replace(".", "").ToLower().Contains("discordgg")) {
                        await args.Message.DeleteAsync();
                        await Warn(args.Channel, args.Author, "Invite Link");
                    } else if (args.Message.Content.ToLower().Contains("nigger") || args.Message.Content.ToLower().Contains("nigga")) {
                        await args.Channel.SendMessageAsync("Keep up the racism and you will get banned\nUse nig, nibba instead atleast");
                    }
                });
                return Task.CompletedTask;
            };

            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
                StringPrefixes = new[] { "." },
                EnableMentionPrefix = false
            });

            commands.CommandErrored += async (dc, args) => {
                switch (args.Exception) {
                    case ArgumentException _:
                        await args.Context.RespondAsync("Idk what the fuck you want to do with that Command (Arguments are faulty)");
                        break;

                    case DSharpPlus.CommandsNext.Exceptions.ChecksFailedException _:
                        await args.Context.RespondAsync("The FBI has been contacted (You don't have **the** (Thx Sven for correction) rights for that **c**ommand (Another correction))");
                        break;
                } 
            };

            commands.SetHelpFormatter<CustomHelpFormatter>();
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            discord.UseInteractivity(new InteractivityConfiguration() {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(180)
            });

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        public class CustomHelpFormatter : DefaultHelpFormatter {

            public CustomHelpFormatter(CommandContext ctx) : base(ctx) {
            }

            public override CommandHelpMessage Build() {
                EmbedBuilder.Color = EmbedColor;
                return base.Build();
            }
        }

        public struct PlayerSystem {

            [JsonProperty("LastMessages")]
            public List<string> LastMessages { get; set; }

            [JsonProperty("Warns")]
            public List<string> Warns { get; set; }

            [JsonProperty("Muted")]
            public bool Muted { get; set; }

            public static async Task<PlayerSystem> GetPlayer(ulong id) {
                if (!File.Exists($"./Players/{id}.json"))
                    File.WriteAllText($"./Players/{id}.json", File.ReadAllText($"./Players/playertemplate.json"));

                string json;
                using (StreamReader sr = new StreamReader(File.OpenRead($"./Players/{id}.json"), new UTF8Encoding(false))) {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
                return JsonConvert.DeserializeObject<PlayerSystem>(json);
            }

            public void Save(ulong id) {
                File.WriteAllText($"./Players/{id}.json", JsonConvert.SerializeObject(this));
            }
        }

        public static class ThreadsShit {

            public static async void StatusChanger(DiscordClient dc) {
                while (true) {
                    foreach (var Status in Statuses) {
                        DiscordActivity activity = new DiscordActivity {
                            ActivityType = ActivityType.Playing,
                            Name = Status
                        };
                        await dc.UpdateStatusAsync(activity, UserStatus.DoNotDisturb);
                        dc.Logger.LogInformation("Status Update");
                        await Task.Delay(120000);
                    }
                }
            }
        }
    }
}
