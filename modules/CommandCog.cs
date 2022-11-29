using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;

public abstract class CommandCog : BaseCommandModule 
{

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
}
