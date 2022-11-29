using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;

public class StandardModule : CommandCog 
{

    public override string CogName => "std";

    public StorageContext DB { private get; set; }

    [Command("ping")]
    public async Task PingCommand(CommandContext ctx) 
    {
        await ctx.RespondAsync("ja mei");
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
        await ctx.RespondAsync($"Das Präfix wurde zu \"{prefix}\" geändert.");
    }

    [Command("togglecog")]
    [RequireUserPermissions(Permissions.ManageMessages)]
    public async Task ToggleCogCommand(CommandContext ctx, [RemainingText] string cog) 
    {
        var guildinfo = await DB.GetGuildInfo(ctx.Guild.Id);
        if (guildinfo.ActivatedCogs.Contains(cog)){
            guildinfo.ActivatedCogs.Remove(cog);
            ctx.RespondAsync($"Modul {cog} wurde deaktiviert.");
        } 
        else {
            guildinfo.ActivatedCogs.Add(cog);
            ctx.RespondAsync($"Modul {cog} wurde aktiviert.");
        }
        await DB.SaveChangesAsync();
    }
}