using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotEthan.Commands {

    public class Say : BaseCommandModule {

        [Command("Say"), Cooldown(1, 10, CooldownBucketType.User), Hidden]
        public async Task MirrorCommand(CommandContext ctx, [RemainingText] string message) {
            await ctx.Message.DeleteAsync();
            if (ctx.Message.MessageType == DSharpPlus.MessageType.Reply) {
                _ = await new DiscordMessageBuilder()
                    .WithContent(message)
                    .WithReply(ctx.Message.ReferencedMessage.Id, true)
                    .SendAsync(ctx.Channel);
            } else {
                await ctx.RespondAsync(message);
            }
        }
    }
}