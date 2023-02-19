using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

public class Music : CommandCog
{
    public override string CogName => "music";
    private bool paused = false;


    [Command("join")]
    public async Task JoinCommand(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.GetIdealNodeConnection();
        if (node == null)
        {
            await ctx.RespondAsync(await TranslateString("music.noNodes", ctx));
            return;
        }
        var channel = ctx.Member?.VoiceState?.Channel;
        if (channel == null)
        {
            await ctx.RespondAsync(await TranslateString("music.notInVoice", ctx));
            return;
        }
        await node.ConnectAsync(channel);
    }

    [Command("leave")]
    public async Task LeaveCommand(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.GetIdealNodeConnection();
        if (node == null)
        {
            await ctx.RespondAsync(await TranslateString("music.noNodes", ctx));
            return;
        }
        var conn = node.GetGuildConnection(ctx.Guild);
        if (!conn.IsConnected)
        {
            await ctx.RespondAsync(await TranslateString("music.alreadyLeft", ctx));
            return;
        }
        await conn.DisconnectAsync();
    }

    [Command("play")]
    public async Task PlayCommand(CommandContext ctx, Uri url)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.GetIdealNodeConnection();
        if (node == null)
        {
            await ctx.RespondAsync(await TranslateString("music.noNodes", ctx));
            return;
        }
        var conn = node.GetGuildConnection(ctx.Guild);
        if (!conn.IsConnected) await JoinCommand(ctx);

        var loadResult = await node.Rest.GetTracksAsync(url);
        if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondAsync(await TranslateString("music.NotFound", ctx));
            return;
        }
        var track = loadResult.Tracks.First();
        await conn.PlayAsync(track);
        await ctx.RespondAsync(await FormatString("music.PlayingTrack", ctx, track.ToString()));
    }

    [Command("pause")]
    public async Task PauseCommand(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.GetIdealNodeConnection();
        if (node == null)
        {
            await ctx.RespondAsync(await TranslateString("music.noNodes", ctx));
            return;
        }
        var conn = node.GetGuildConnection(ctx.Guild);
        if (!conn.IsConnected)
        {
            await ctx.RespondAsync(await TranslateString("music.alreadyLeft", ctx));
            return;
        }

        if (paused)
        {
            await conn.ResumeAsync();
            paused = false;
            await ctx.RespondAsync(await TranslateString("music.Resumed", ctx));
        } 
        else 
        {
            await conn.PauseAsync();
            paused = true;
            await ctx.RespondAsync(await TranslateString("music.Paused", ctx));
        }
    }

    
}