using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace DiscordBotEthan.Commands {

    public class Hackban : BaseCommandModule {

        [Command("Hackban"), RequirePermissions(DSharpPlus.Permissions.Administrator), Description("Bans the User without them being needed on the Server")]
        public async Task HackbanCommand(CommandContext ctx, [Description("The User to ban (ID, Mention)")] DiscordUser user, [RemainingText, Description("Reason for the Ban")] string reason) {
            await ctx.Guild.BanMemberAsync(user.Id, 1, reason);

            DiscordEmbedBuilder Hackban = new DiscordEmbedBuilder {
                Title = $"Hackban | {user.Username}",
                Description = $"{user.Mention} **has been banned from the Server without being on it for the following Reason:**\n{reason}",
                Color = Program.EmbedColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Made by JokinAce 😎" },
                Timestamp = DateTimeOffset.Now
            };
            await ctx.RespondAsync(embed: Hackban);
        }
    }
}