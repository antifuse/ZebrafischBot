using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

public class Insulter : CommandCog
{
    public override string CogName => "insult";

    [Command("setInsult")]
    public async Task SetInsultCommand(CommandContext ctx, DiscordUser opfer, [RemainingText] string insultText)
    {
        var user = await DB.GetUserInfo(ctx.User.Id);
        var insult = user.Insults.Find(i => i.Insulted == opfer.Id);
        if (insult == null)
        {
            insult = new Insult()
            {
                Insulted = opfer.Id,
                Message = insultText
            };
            
            DB.Find<DBUser>(user.Id)?.Insults.Add(insult);
        } 
        else 
        {
            insult.Message = insultText;
        }
        await DB.SaveChangesAsync();
        await ctx.RespondAsync(await FormatString("insult.InsultAdded", ctx, opfer.Username, insultText));
    }

    [Command("insult")]
    public async Task InsultCommand(CommandContext ctx, DiscordUser opfer)
    {
        var insult = await DB.Insults.FindAsync(ctx.User.Id, opfer.Id);
        if (insult == null) {
            await ctx.RespondAsync(await TranslateString("insult.UserNotFound", ctx));
            return;
        }
        await ctx.Channel.SendMessageAsync(opfer.Mention + " " + insult.Message);
    }
}