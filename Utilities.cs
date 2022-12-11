using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class StorageContext : DbContext
{
    public DbSet<DBGuild> Guilds { get; set; }
    public DbSet<DBUser> Users { get; set; }
    public DbSet<DBMessage> Messages { get; set; }
    public DbSet<DBChannel> Channels { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=storage.db");


    public async Task<DBGuild> GetGuildInfo(ulong id)
    {
        var guild = await FindAsync<DBGuild>(id);

        if (guild == null)
        {
            guild = new DBGuild()
            {
                Id = id
            };
            await Guilds.AddAsync(guild);
            await SaveChangesAsync();
        }

        return guild;
    }

    public async Task<DBUser> GetUserInfo(ulong id)
    {
        var user = await FindAsync<DBUser>(id);

        if (user == null)
        {
            user = new DBUser()
            {
                Id = id
            };
            await Users.AddAsync(user);
            await SaveChangesAsync();
        }

        return user;
    }

    public async Task<DBMessage> GetMessageInfo(ulong id)
    {
        var message = await FindAsync<DBMessage>(id);

        if (message == null)
        {
            message = new DBMessage()
            {
                Id = id
            };
            await Messages.AddAsync(message);
            await SaveChangesAsync();
        }
        return message;
    }

    public async Task<DBChannel> GetChannelInfo(ulong id)
    {
        var channel = await FindAsync<DBChannel>(id);
        if (channel == null)
        {
            channel = new DBChannel()
            {
                Id = id
            };
            await Channels.AddAsync(channel);
            await SaveChangesAsync();
        }
        return channel;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DBGuild>()
            .Property(e => e.ActivatedCogs)
            .HasConversion(
                v => String.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }

    
}

public class DBGuild
{
    public ulong Id { get; set; }
    public string? Prefix { get; set; }
    public List<string> ActivatedCogs { get; set; } = new() { "std" };
    public int? MensaId { get; set; } = new();
    public string Locale { get; set; } = "en";
}

public class DBUser
{
    public ulong Id { get; set; }
}

public class DBMessage
{
    public ulong Id { get; set; }
}

public class DBChannel
{
    public ulong Id { get; set; }
    public ulong RouxlsRole { get; set; }
}

