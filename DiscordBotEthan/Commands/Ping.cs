using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DiscordBotEthan.Commands {

    public class Ping : BaseCommandModule {

        [Command("Ping"), Description("Shows the current latency")]
        public async Task PingCommand(CommandContext ctx) {
            await ctx.RespondAsync($":ping_pong: {ctx.Client.Ping}ms Delay :ping_pong:");
        }
    }
}