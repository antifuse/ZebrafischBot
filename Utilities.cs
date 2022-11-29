using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class StorageContext : DbContext 
{
    public DbSet<DBGuild> Guilds { get; set; }
    public DbSet<DBUser> Users { get; set; }
    public DbSet<DBMessage> Messages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=storage.db");


    public async Task<DBGuild> GetGuildInfo(ulong id)
    {
        var guild = await FindAsync<DBGuild>(id);

        if (guild == null) {
            guild = new DBGuild() {
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

        if (user == null) {
            user = new DBUser() {
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

        if (message == null) {
            message = new DBMessage() {
                Id = id
            };
            await Messages.AddAsync(message);
            await SaveChangesAsync();
        }
        return message;
    }
}

public class DBGuild 
{
    public ulong Id { get; set; }
    public string? Prefix { get; set; }
    public List<string> ActivatedCogs { get; set; } = new() {"std"};
    public int? MensaId { get; set; } = new();
}

public class DBUser
{
    public ulong Id { get; set; }
}

public class DBMessage
{
    public ulong Id { get; set; }
    public List<ulong> Reactions { get; set; } = new();
    public List<ulong> Roles { get; set; } = new();
}

