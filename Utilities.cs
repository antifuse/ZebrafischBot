using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class StorageContext : DbContext
{
    public DbSet<DBGuild> Guilds { get; set; }
    public DbSet<DBUser> Users { get; set; }
    public DbSet<DBMessage> Messages { get; set; }
    public DbSet<DBChannel> Channels { get; set; }
    public DbSet<Insult> Insults { get; set; }

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
        modelBuilder.Entity<Insult>().HasKey(e => new {e.UserId, e.Insulted});
        modelBuilder.Entity<DBUser>().HasMany<Insult>(u => u.Insults).WithOne(i => i.Sender).HasForeignKey(i => i.UserId);
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

    public string? Locale { get; set; }

    public List<Insult> Insults { get; set; } = new List<Insult>();
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

public class Insult 
{
    public ulong UserId { get; set; }

    public DBUser Sender { get; set; }
    public ulong Insulted { get; set; }
    public string Message { get; set; }
}

