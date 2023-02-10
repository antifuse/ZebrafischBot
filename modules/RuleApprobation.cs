using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using DSharpPlus.Entities;

public class RuleApprobation : CommandCog
{
    public override string CogName => "rouxls";

    [Command("setrole")]
    [RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
    public async Task SetRoleCommand(CommandContext ctx, DiscordRole role) 
    {
        var channel = await DB.GetChannelInfo(ctx.Channel.Id);
        channel.RouxlsRole = role.Id;
        DB.Entry(channel).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(await FormatString("rouxls.setRoleResp", ctx, ctx.Guild.GetRole(channel.RouxlsRole).Name));
    }

    [Command("accept")]
    public async Task AcceptRulesCommand(CommandContext ctx) 
    {
        var channel = await DB.GetChannelInfo(ctx.Channel.Id);
        var role = ctx.Guild.GetRole(channel.RouxlsRole);
        if (role != null && ctx.Member != null) 
        {
            await ctx.Member.GrantRoleAsync(role, "accepted rules");
        }
    }
}


