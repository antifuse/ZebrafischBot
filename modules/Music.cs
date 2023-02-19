using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

public class Music : CommandCog
{
    public override string CogName => "music";
    private bool paused = false;

    private Dictionary<ulong, MusicQueue> queues = new Dictionary<ulong, MusicQueue>();
    private MusicQueue GetQueue(ulong id)
    {
        if (!queues.ContainsKey(id)) queues.Add(id, new MusicQueue());
        return queues[id];
    }

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
        var conn = await node.ConnectAsync(channel);
        conn.PlaybackFinished += async (c, e) =>
        {
            var nextTrack = GetQueue(c.Guild.Id).NextTrack();
            if (nextTrack == null)
            {
                await LeaveCommand(ctx);
                return;
            }
            await c.PlayAsync(nextTrack);
            return;
        };
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
        var queue = GetQueue(ctx.Guild.Id);
        var toPlay = queue.AddTrack(track);


        await ctx.RespondAsync(await FormatString("music.addedTrack", ctx, track.Author + " – " + track.Title));
        if (toPlay != null)
        {
            await conn.PlayAsync(track);
            await ctx.RespondAsync(await FormatString("music.playingTrack", ctx, track.Author + " – " + track.Title));
        }
    }

    [Command("pause")]
    public async Task PauseCommand(CommandContext ctx)
    {
        var conn = await AssertGuildConnection(ctx);
        if (conn == null) return;

        if (paused)
        {
            await conn.ResumeAsync();
            paused = false;
            await ctx.RespondAsync(await TranslateString("music.resumed", ctx));
        }
        else
        {
            await conn.PauseAsync();
            paused = true;
            await ctx.RespondAsync(await TranslateString("music.paused", ctx));
        }
    }



    public async Task<LavalinkGuildConnection?> AssertGuildConnection(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.GetIdealNodeConnection();
        if (node == null)
        {
            await ctx.RespondAsync(await TranslateString("music.noNodes", ctx));
            return null;
        }
        var conn = node.GetGuildConnection(ctx.Guild);
        if (!conn.IsConnected)
        {
            await ctx.RespondAsync(await TranslateString("music.alreadyLeft", ctx));
            return null;
        }
        return conn;
    }


}

public class MusicQueue
{
    List<LavalinkTrack> list = new List<LavalinkTrack>();
    int pos = 0;

    public void Clear()
    {
        list = new List<LavalinkTrack>();
        pos = 0;
    }

    public LavalinkTrack? AddTrack(LavalinkTrack t)
    {
        list.Add(t);
        if (list.Count == 1) return t;
        return null;
    }

    public LavalinkTrack? Goto(int nPos)
    {
        if (nPos >= list.Count)
        {
            return null;
        }
        pos = nPos;
        return list[pos];
    }

    public LavalinkTrack? NextTrack()
    {
        if (list.Count == 0) return null;
        pos = pos + 1 == list.Count ? 0 : pos + 1;
        return list[pos];
    }

    public LavalinkTrack? RemoveTrack(int rPos)
    {
        if (rPos == pos)
        {
            var track = NextTrack();
            pos--;
            list.RemoveAt(pos);
            return track;
        }
        list.RemoveAt(rPos);
        if (rPos < pos) pos--;
        return null;
    }

    public override string ToString()
    {
        if (list.Count == 0) return "--";
        var s = "```\n";
        for (int i = 0; i < list.Count; i++)
        {
            s += $"[{(i == pos ? "\\*" : "")}]\t{list[i].Author} \t – \t {list[i].Title}\n";
        }
        return s + "```";
    }
}