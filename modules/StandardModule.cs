using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;

public class StandardModule : CommandCog 
{

    public override string CogName => "std";

    public StorageContext DB { private get; set; }
    public Localiser Loc { private get; set; }

    [Command("ping")]
    public async Task PingCommand(CommandContext ctx) 
    {
        await ctx.RespondAsync(Loc.GetString((await DB.GetGuildInfo(ctx.Guild.Id)).Locale, "std.pingResp"));
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
        await ctx.RespondAsync(Loc.FormatString((await DB.GetGuildInfo(ctx.Guild.Id)).Locale, "std.prefixSetResp", prefix));
    }

    [Command("togglecog")]
    [RequireUserPermissions(Permissions.ManageMessages)]
    public async Task ToggleCogCommand(CommandContext ctx, [RemainingText] string cog) 
    {        
        var locale = (await DB.GetGuildInfo(ctx.Guild.Id)).Locale;
        if (cog == CogName)
        {
            await ctx.RespondAsync(Loc.GetString(locale, "std.stdCogToggled"));
            return;
        }
        var guildinfo = await DB.GetGuildInfo(ctx.Guild.Id);

        if (guildinfo.ActivatedCogs.Contains(cog)){
            guildinfo.ActivatedCogs.Remove(cog);
            
            await ctx.RespondAsync(Loc.FormatString(locale, "std.cogDeactivatedResp", cog));
        } 
        else {
            guildinfo.ActivatedCogs.Add(cog);
            await ctx.RespondAsync(Loc.FormatString(locale, "std.cogActivatedResp", cog));
        }
        DB.Entry(guildinfo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
    }

    [Command("setlocale")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLocaleCommand(CommandContext ctx, string locale) 
    {
        var guild = await DB.GetGuildInfo(ctx.Guild.Id);
        guild.Locale = locale;
        DB.Entry(guild).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(Loc.GetString(locale, "localeChanged"));
    }
}