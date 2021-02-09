using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DiscordBotEthan.Commands {

    public class Say : BaseCommandModule {

        [Command("Say"), RequirePermissions(DSharpPlus.Permissions.Administrator), Hidden]
        public async Task MirrorCommand(CommandContext ctx, [RemainingText] string message) {
            if (ctx.Member.Id != 447781010315149333) {
                return;
            } else {
                await ctx.Message.DeleteAsync();
                await ctx.RespondAsync(message);
            }
        }
    }
}