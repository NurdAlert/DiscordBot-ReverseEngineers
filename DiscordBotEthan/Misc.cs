﻿using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DiscordBotEthan.Program;

namespace DiscordBotEthan {

    public class Misc {

        public static double TimeConverter(string timestring) { // Takes 7d -> 7 DaysConverter() -> 604,800,000 Milliseconds

            if (timestring.Remove(timestring.Length - 1).Any(x => char.IsLetter(x)))
                throw new ArgumentException();
            else {
                return (timestring[^1..].ToLower()) switch {
                    "d" => ConvertDaysToMilliseconds(timestring.Remove(timestring.Length - 1)),
                    "h" => ConvertHoursToMilliseconds(timestring.Remove(timestring.Length - 1)),
                    "m" => ConvertMinutesToMilliseconds(timestring.Remove(timestring.Length - 1)),
                    "s" => ConvertSecondsToMilliseconds(timestring.Remove(timestring.Length - 1)),
                    _ => throw new ArgumentException(),
                };
            }

            static double ConvertSecondsToMilliseconds(string seconds) { // NUMBERS ONLY
                return TimeSpan.FromSeconds(Convert.ToDouble(seconds)).TotalMilliseconds;
            }

            static double ConvertMinutesToMilliseconds(string minutes) {
                return TimeSpan.FromMinutes(Convert.ToDouble(minutes)).TotalMilliseconds;
            }

            static double ConvertHoursToMilliseconds(string hours) {
                return TimeSpan.FromHours(Convert.ToDouble(hours)).TotalMilliseconds;
            }

            static double ConvertDaysToMilliseconds(string days) {
                return TimeSpan.FromDays(Convert.ToDouble(days)).TotalMilliseconds;
            }
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
                        var WarnS = await PlayerSystem.GetPlayer(member.Id);
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
    }
}