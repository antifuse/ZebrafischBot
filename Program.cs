namespace ZebrafischBot;
using DSharpPlus;
using System.Linq;

using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    DiscordClient client;
    IConfiguration _config;
    StorageContext storage;

    public Program()
    {

        // load configuration from file and configure client
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json", false);
        _config = configurationBuilder.Build();

        client = new DiscordClient(new DiscordConfiguration()
        {
            Token = _config["token"],
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        });

        // provide database access to all modules
        var services = new ServiceCollection()
            .AddSingleton<Localiser>()
            .AddDbContext<StorageContext>()
            .BuildServiceProvider();

        storage = services.GetService<StorageContext>() ?? new StorageContext();

        // setup commands module
        var commands = client.UseCommandsNext(new CommandsNextConfiguration()
        {
            UseDefaultCommandHandler = false,
            Services = services
        });

        // register command modules
        commands.RegisterCommands<StandardModule>();
        commands.RegisterCommands<RuleApprobation>();
        commands.RegisterCommands<Insulter>();

        client.MessageCreated += CommandHandler;
    }

    private async Task CommandHandler(DiscordClient cl, MessageCreateEventArgs e)
    {
        var msg = e.Message;
        var guild = await storage.GetGuildInfo(e.Guild.Id);
        var prefix = guild.Prefix ?? $"<@{client.CurrentUser.Id}>";

        if (!msg.Content.StartsWith(prefix)) return;
        var callString = msg.Content.Substring(prefix.Length);

        var cnext = client.GetCommandsNext();
        var command = cnext.FindCommand(callString, out var args);
        var ctx = cnext.CreateContext(msg, prefix, command, args);
        _ = Task.Run(async () => await cnext.ExecuteCommandAsync(ctx));
    }

    static async Task Main()
    {
        Program bot = new Program();
        Console.WriteLine("trogi lmao");
        await bot.client.ConnectAsync();
        await Task.Delay(-1);
    }

}


