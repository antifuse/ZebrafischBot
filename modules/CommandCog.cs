using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;

public abstract class CommandCog : BaseCommandModule 
{

    public StorageContext DB { get; set; }
    public Localiser Loc { get; set;}

    public virtual string CogName
    {
        get {
            return "std";
        }
    }

    public override async Task BeforeExecutionAsync(CommandContext ctx)
    {
        // nun
        StorageContext storage = (StorageContext) (ctx.Services.GetService(typeof(StorageContext)) ?? new StorageContext());
        var guildinfo = await storage.GetGuildInfo(ctx.Guild.Id);
        if (!guildinfo.ActivatedCogs.Contains(CogName)) throw new ChecksFailedException(ctx.Command, ctx, new CheckBaseAttribute[] {});
        await base.BeforeExecutionAsync(ctx);
    }

    public async Task<string> TranslateString(string token, CommandContext ctx) 
    {
        var userLocale = (await DB.GetUserInfo(ctx.User.Id)).Locale;
        if (userLocale == null || userLocale.Equals("")) userLocale = (await DB.GetGuildInfo(ctx.Guild.Id)).Locale;
        return Loc.GetString(userLocale, token);
    }

    public async Task<string> FormatString(string token, CommandContext ctx, params string[] inserts)
    {
        var str = await TranslateString(token, ctx);
        return String.Format(str, inserts);
    }
}
