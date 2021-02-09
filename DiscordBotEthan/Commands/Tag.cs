using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace DiscordBotEthan.Commands {

    public class Tag : BaseCommandModule {

        [Command("Tag"), Description("Requests a Tag")]
        public async Task TagCommand(CommandContext ctx, [Description("`highlighting`,`slashcommands`")] string tagtoshow) {
            switch (tagtoshow.ToLower()) {
                case "highlighting": {
                        DiscordEmbedBuilder Tags = new DiscordEmbedBuilder {
                            Title = "Tag | Highlighting",
                            Description = @"You can denote a specific language for syntax highlighting, by typing the name of the language you want the code block to expect right after the first three backticks (\`\`\`) beginning your code block. An example...

\`\`\`python
def CallFunction():
`    `print('allah')
CallFunction()
\`\`\`
would be
```python
def CallFunction():
    print('allah')
CallFunction()
```",
                            Color = Program.EmbedColor,
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Made by JokinAce 😎" },
                            Timestamp = DateTimeOffset.Now
                        };
                        await ctx.RespondAsync(embed: Tags);
                        break;
                    }

                case "slashcommands": {
                        DiscordEmbedBuilder Tags = new DiscordEmbedBuilder {
                            Title = "Tag | SlashCommands",
                            Description = @"You can do Slash Commands that do ASCII-Art Emojis or quick Shortcuts for something. An example...

/shrug

would be

¯\_(ツ)_/¯

**List of default Slash Commands:**
/shrug
/tenor
/giphy
/tableflip
/unflip
/tts
/me
/spoiler
/nick",

                            Color = Program.EmbedColor,
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Made by JokinAce 😎" },
                            Timestamp = DateTimeOffset.Now
                        };
                        await ctx.RespondAsync(embed: Tags);
                        break;
                    }

                default:
                    throw new ArgumentNullException();
            }
        }
    }
}