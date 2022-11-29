using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;

public class ReactionRoles : CommandCog 
{

    public override string CogName => "roles";

    public StorageContext DB { private get; set; }

    [Aliases(new string[] {"addrolemessage"})]
    [Command("addtrigger")]
    public async Task AddRoleMessageCommand(CommandContext ctx, DiscordEmoji emoji, DiscordRole role)
    {
        var message = ctx.Message.ReferencedMessage;
        var messageinfo = await DB.GetMessageInfo(message.Id);
        messageinfo.Reactions.Add(emoji.Id);
        messageinfo.Roles.Add(role.Id);
        await DB.SaveChangesAsync();
        await message.CreateReactionAsync(emoji);
        await ctx.RespondAsync($"Die Reaktion {emoji} verweist nun auf die {role.Name}-Rolle!");
    }

    [Aliases(new string[] {"addrolemessage"})]
    [Command("addtrigger")]
    public async Task AddRoleMessageCommand(CommandContext ctx, DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
    {
        
    }

}