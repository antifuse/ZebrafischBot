using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;

public class StandardModule : CommandCog 
{

    public override string CogName => "std";

    [Command("ping")]
    public async Task PingCommand(CommandContext ctx) 
    {
        await ctx.RespondAsync(await TranslateString("std.pingResp", ctx));
    }


    [Aliases(new string[] {"sendmessage"})]
    [Command("sendmsg")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task SendMessageCommand(CommandContext ctx, DiscordChannel channel, [RemainingText] string message) 
    {
       await channel.SendMessageAsync(message);
    }

    [Command("setprefix")]
    [RequireUserPermissions(Permissions.ManageMessages)]
    public async Task SetPrefixCommand(CommandContext ctx, [RemainingText] string prefix) 
    {
        var guildinfo = await DB.GetGuildInfo(ctx.Guild.Id);
        guildinfo.Prefix = prefix;
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(await FormatString("std.prefixSetResp", ctx, prefix));
    }

    [Command("togglecog")]
    [RequireUserPermissions(Permissions.ManageMessages)]
    public async Task ToggleCogCommand(CommandContext ctx, [RemainingText] string cog) 
    {        
        
        if (cog == CogName)
        {
            await ctx.RespondAsync(await TranslateString("std.stdCogToggled", ctx));
            return;
        }
        var guildinfo = await DB.GetGuildInfo(ctx.Guild.Id);

        if (guildinfo.ActivatedCogs.Contains(cog)){
            guildinfo.ActivatedCogs.Remove(cog);
            
            await ctx.RespondAsync(await FormatString("std.cogDeactivatedResp", ctx, cog));
        } 
        else {
            guildinfo.ActivatedCogs.Add(cog);
            await ctx.RespondAsync(await FormatString("std.cogActivatedResp", ctx, cog));
        }
        DB.Entry(guildinfo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
    }

    [Command("setguildlocale")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetGuildLocaleCommand(CommandContext ctx, string locale) 
    {
        var guild = await DB.GetGuildInfo(ctx.Guild.Id);
        guild.Locale = locale;
        DB.Entry(guild).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(Loc.GetString(locale, "localeChanged"));
    }

    [Command("setlocale")]
    [Aliases(new string[] {"setmylocale", "setlanguage"})]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLocaleCommand(CommandContext ctx, string locale) 
    {
        var user = await DB.GetUserInfo(ctx.User.Id);
        user.Locale = locale;
        DB.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(Loc.GetString(locale, "localeChanged"));
    }
}