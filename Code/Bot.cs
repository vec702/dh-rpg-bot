#region Usings
using dotHack_Discord_Game.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
#endregion

namespace dotHack_Discord_Game
{
    public class Bot
    {
        #region Declarations
        public static DiscordClient client { get; private set; }
        public CommandsNextExtension commands { get; private set; }
        public EventId BotEventId { get; private set; }

        public static ulong BotChannelID = 927578813972492358;
        public static Dictionary<string, Player> Players = new Dictionary<string, Player>();

        public static double expRate = 3.0;
        public static double dmgRate = 3.0;
        public static double dropRate = 1.0;

        private static bool monsterSpawned = false;
        private static bool portalSpawned = false;
        private static bool despawned = false;
        #endregion

        #region Main Task
        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var file = File.OpenRead("../../config.json"))
            {
                using (var stream = new StreamReader(file, new UTF8Encoding(false)))
                {
                    json = await stream.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);


            var config = new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            client = new DiscordClient(config);
            client.Ready += OnClientReady;
            // client.SocketClosed += SavePlayerData;
            // client.SocketOpened += LoadPlayerData;
            client.MessageCreated += SummonPortalChance;

            client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
            };

            commands = client.UseCommandsNext(commandsConfig);
            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;
            commands.RegisterCommands<Commands>();

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
        #endregion

        #region Bot-specific Tasks
        private Task SummonPortalChance(DiscordClient _client, MessageCreateEventArgs e)
        {
            var cnext = _client.GetCommandsNext();
            var command = cnext.FindCommand("monster", out var args);

            Random random = new Random();
            var chance = random.Next(1, 100);
            if(chance > 90)
            {
                var ctx = cnext.CreateContext(e.Message, "//", command, args);
                Task.Run(async () => await FightMonsters(ctx)); return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public async Task FightMonsters(CommandContext ctx)
        {
            // declarations for outside loop
            Timer timer = new Timer();
            Monster randomMonster = RandomMonster();
            DiscordMessage monsterMessage = null;

            despawned = false;

            timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            timer.Elapsed += despawnMonster;

            // check that a spawn isn't already active
            if (!monsterSpawned && !portalSpawned && !ctx.Message.Author.IsBot)
            {
                // declarations for inner loop
                bool portalOpened = false;
                var _ImageUrl = randomMonster.ImageUrl;
                var _Title = randomMonster.Name;
                var health_left = randomMonster.Health;
                var _Description = "Health: " + health_left + "/" + randomMonster.Health;
                List<DiscordUser> attackers = new List<DiscordUser>();

                portalSpawned = true;

                // create the portal message
                var portalMessage = await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync("A monster portal has appeared.");

                var interactivity = client.GetInteractivity();

                // and the "fight" reaction
                var emoji = DiscordEmoji.FromName(client, ":crossed_swords:");
                await portalMessage.CreateReactionAsync(emoji);

                var portalResult = await interactivity.CollectReactionsAsync(portalMessage, TimeSpan.FromSeconds(1));

                while (portalSpawned)
                {
                    // read the users who reacted
                    foreach (var user in await portalMessage.GetReactionsAsync(emoji))
                    {
                        // and exclude bots
                        if (!user.IsBot)
                        {
                            // and they're registered for the game
                            if (Players.TryGetValue(user.Id.ToString(), out Player p))
                            {
                                portalOpened = true;
                                portalSpawned = false;
                            }
                            else
                            {
                                await SendMessage("You need to \"//signup\" before you can start fighting!");
                            }
                        }
                    }
                }

                // check if the portal has been opened
                while (portalOpened)
                {
                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = _Title,
                        ImageUrl = _ImageUrl,
                        Description = _Description,
                    };

                    // delete the portal message
                    await portalMessage.DeleteAsync();
                    portalSpawned = false;

                    // spawn the randomly generated monster
                    monsterSpawned = true;

                    // start the despawn timer when the monster appears
                    timer.Start();

                    monsterMessage = await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(embed);
                    await monsterMessage.CreateReactionAsync(emoji);

                    var monsterResult = await interactivity.CollectReactionsAsync(monsterMessage, TimeSpan.FromSeconds(1));

                    while (health_left > 0)
                    {
                        foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                        {
                            if(!Players.TryGetValue(user.Id.ToString(), out Player p3) && !user.IsBot)
                            {
                                await SendMessage("You need to \"//signup\" before you can start fighting!");
                            }
                            else
                            {
                                if (!attackers.Contains(user) && !user.IsBot)
                                {
                                    attackers.Add(user);
                                    var damage = 0;
                                    Random random = new Random();

                                    if (Players.TryGetValue(user.Id.ToString(), out Player p))
                                    {
                                        damage = Convert.ToInt32(random.Next(p.Attack, p.Attack * Convert.ToInt32(p.Equip.Crit_Rate)) * dmgRate);
                                        damage = (int)Math.Round(Convert.ToDouble(damage), 0);
                                    }

                                    health_left = health_left - damage;
                                    if (health_left < 0) health_left = 0;
                                    _Description = "Health: " + health_left + "/" + randomMonster.Health;

                                    if (damage > 1) await SendMessage(user.Username.ToString() + " attacks " + _Title + " for " + damage + " points of damage!");
                                    else await SendMessage(user.Username.ToString() + " attacks " + _Title + " for " + damage + " point of damage!");

                                    await monsterMessage.ModifyAsync(x =>
                                    {
                                        x.Content = "";
                                        x.Embed = new DiscordEmbedBuilder()
                                            .WithTitle(_Title)
                                            .WithDescription(_Description)
                                            .WithImageUrl(_ImageUrl)
                                            .Build();
                                    });
                                }
                            }
                        }

                        if (despawned)
                        {
                            await SendMessage("The " + randomMonster.Name + " has got away.");
                            if(monsterMessage != null) await monsterMessage.DeleteAsync();
                            if(portalMessage != null) await portalMessage.DeleteAsync();
                        }
                    }

                    if (health_left <= 0)
                    {
                        foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                        {
                            if (Players.TryGetValue(user.Id.ToString(), out Player p))
                            {
                                _Description = "Befallen by " + attackers.First().Username.ToString();
                                foreach (var attacker in attackers)
                                {
                                    if (attacker.Id == p.Id)
                                    {
                                        Random random = new Random();

                                        var exp = Convert.ToInt32(random.Next(150, 175) * expRate);
                                        var dropChance = Convert.ToInt32(random.Next(1,100) * dropRate);
                                        Weapon drop = null;

                                        if(dropChance > 80)
                                        {
                                            var randomDrop = random.Next(1, randomMonster.Drops.Count());
                                            drop = randomMonster.Drops[randomDrop];
                                        }

                                        if(drop != null)
                                        {
                                            if (Players.TryGetValue(attacker.Id.ToString(), out Player p2))
                                            {
                                                await SendMessage(p2.Name + " received " + drop.Name + " from " + randomMonster.Name);
                                                p2.Inventory.Add(drop);
                                            }
                                        }

                                        p.Kills++;
                                        p.Experience += Convert.ToInt32(Math.Round(Convert.ToDouble(exp)));
                                        p.Levelcheck();
                                    }

                                    if (attacker != attackers[0])
                                    {
                                        _Description += ", " + attacker.Username.ToString();
                                    }
                                }
                                await monsterMessage.ModifyAsync(x =>
                                {
                                    x.Content = "";
                                    x.Embed = new DiscordEmbedBuilder()
                                        .WithTitle(_Title)
                                        .WithDescription(_Description)
                                        .WithImageUrl(_ImageUrl)
                                        .Build();
                                });
                            }
                        }

                        monsterSpawned = false;
                    }

                }
                await portalMessage.DeleteAsync();
            }
        }

        private void despawnMonster(object sender, ElapsedEventArgs e)
        {
            despawned = true;
        }

        public static async Task SendMessage(string Message)
        {
            await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(Message);
        }

        public static async Task SendMessage(DiscordEmbed Message)
        {
            await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(Message);
        }
        #endregion

        #region Random Monster Generator
        private Monster RandomMonster()
        {
            Random random = new Random();
            var response = random.Next(1, 9);
            switch (response)
            {
                case 0:
                    return new Monster("Aura", "https://static.wikia.nocookie.net/dothack/images/f/fc/Auraface.jpg", 1, new Weapon[] { }, new Item[] { KeyItems.Twilight_Bracelet });
                case 1:
                    return new Monster("Razine", "https://static.wikia.nocookie.net/dothack/images/5/53/%28001%29_Razine.jpg", 5, new Weapon[] { LongarmWeapons.Bronze_Spear, BlademasterWeapons.Brave_Sword, TwinbladeWeapons.Amateur_Blades, HeavyaxeWeapons.Hatchet, WavemasterWeapons.Iron_Rod, HeavybladeWeapons.Steelblade });
                case 2:
                    return new Monster("Tetra Armor", "https://static.wikia.nocookie.net/dothack/images/2/22/(011)_Tetra_Armor.jpg", 10, new Weapon[] { LongarmWeapons.Amazon_Spear, TwinbladeWeapons.Steel_Blades, HeavybladeWeapons.Flamberge, HeavyaxeWeapons.Water_Axe, BlademasterWeapons.Strange_Blade, WavemasterWeapons.Fire_Wand });
                case 3:
                    return new Monster("Nobunaga's Soul", "https://static.wikia.nocookie.net/dothack/images/7/7a/(005)_Nobunaga_Soul.jpg", 5, new Weapon[] { LongarmWeapons.Bronze_Spear, BlademasterWeapons.Brave_Sword, TwinbladeWeapons.Amateur_Blades, HeavyaxeWeapons.Hatchet, WavemasterWeapons.Iron_Rod, HeavybladeWeapons.Steelblade });
                case 4:
                    return new Monster("Menhir", "https://static.wikia.nocookie.net/dothack/images/e/e7/(180)_Menhir.jpg", 10, new Weapon[] { LongarmWeapons.Gold_Spear, TwinbladeWeapons.Ronin_Blades, HeavybladeWeapons.Nodachi, HeavyaxeWeapons.Razor_Axe, BlademasterWeapons.Corpseblade, WavemasterWeapons.Wand_of_Wisdom });
                case 5:
                    return new Monster("Drygon", "https://static.wikia.nocookie.net/dothack/images/2/2d/(212)_Drygon.jpg", 20, new Weapon[] { LongarmWeapons.Bloody_Lance, TwinbladeWeapons.Master_Blades, HeavybladeWeapons.Magnifier, HeavyaxeWeapons.Earth_Axe, BlademasterWeapons.Souleater, WavemasterWeapons.Starstorm_Wand });
                case 6:
                    return new Monster("Ectoplasm", "https://static.wikia.nocookie.net/dothack/images/a/a1/(281)_Ectoplasm.jpg", 5, new Weapon[] { LongarmWeapons.Bronze_Spear, BlademasterWeapons.Brave_Sword, TwinbladeWeapons.Amateur_Blades, HeavyaxeWeapons.Hatchet, WavemasterWeapons.Iron_Rod, HeavybladeWeapons.Steelblade });
                case 7:
                    return new Monster("Noisy Wisp", "https://static.wikia.nocookie.net/dothack/images/f/f2/(282)_Noisy_Wisp.jpg", 5, new Weapon[] { LongarmWeapons.Bronze_Spear, BlademasterWeapons.Brave_Sword, TwinbladeWeapons.Amateur_Blades, HeavyaxeWeapons.Hatchet, WavemasterWeapons.Iron_Rod, HeavybladeWeapons.Steelblade });
                case 8:
                    return new Monster("Mimic", "https://static.wikia.nocookie.net/dothack/images/8/8b/(241)_Mimic.jpg", 5, new Weapon[] { LongarmWeapons.Amazon_Spear, TwinbladeWeapons.Steel_Blades, HeavybladeWeapons.Flamberge, HeavyaxeWeapons.Water_Axe, BlademasterWeapons.Strange_Blade, WavemasterWeapons.Fire_Wand });
                case 9:
                    return new Monster("Squidbod", "https://static.wikia.nocookie.net/dothack/images/1/19/(012)_Squidbod.jpg", 20, new Weapon[] { LongarmWeapons.Bloody_Lance, TwinbladeWeapons.Master_Blades, HeavybladeWeapons.Magnifier, HeavyaxeWeapons.Earth_Axe, BlademasterWeapons.Souleater, WavemasterWeapons.Starstorm_Wand });
                default:
                    return new Monster("", "", 0, null);
                
                /* 

                    old format, still need to merge these...
                  
                    return new string[] { "General Armor", "https://static.wikia.nocookie.net/dothack/images/0/0f/(013)_General_Armor.jpg", "25" };
                case 11:
                    return new string[] { "Bone Army", "https://static.wikia.nocookie.net/dothack/images/2/23/(248)_Bone_Army.jpg", "15" };
                case 12:
                    return new string[] { "Comad Goo", "https://static.wikia.nocookie.net/dothack/images/8/8a/(062)_Comad_Goo.jpg", "30" };
                case 13:
                    return new string[] { "Dalaigon Anecdote", "https://static.wikia.nocookie.net/dothack/images/b/b0/(213)_Dalaigon_Anecdote.jpg", "30" };
                case 14:
                    return new string[] { "Dark Guru", "https://static.wikia.nocookie.net/dothack/images/f/fd/(028)_Dark_Guru.jpg", "15" };
                case 15:
                    return new string[] { "Dark Horse", "https://static.wikia.nocookie.net/dothack/images/f/f9/(009)_Dark_Horse.jpg", "20" };
                case 16:
                    return new string[] { "Dark Lord", "https://static.wikia.nocookie.net/dothack/images/8/8d/(035)_Dark_Lord.jpg", "20" };
                case 17:
                    return new string[] { "Deadly Present", "https://static.wikia.nocookie.net/dothack/images/b/b5/(244)_Deadly_Present.jpg", "15" };
                case 18:
                    return new string[] { "Fake Money", "https://static.wikia.nocookie.net/dothack/images/b/be/(240)_Fake_Money.jpg", "20" };
                case 19:
                    return new string[] { "Death Head", "https://static.wikia.nocookie.net/dothack/images/a/ac/(246)_Death_Head.jpg", "20" };
                case 20:
                    return new string[] { "Gladiator", "https://static.wikia.nocookie.net/dothack/images/6/6f/(003)_Gladiator.jpg", "25" };
                case 21:
                    return new string[] { "Grand Electric", "https://static.wikia.nocookie.net/dothack/images/b/bc/(026)_Grand_Electric.jpg", "15" };
                case 22:
                    return new string[] { "Headhunter", "https://static.wikia.nocookie.net/dothack/images/2/2e/(253)_Headhunter.jpg", "25" };
                case 23:
                    return new string[] { "Heavy Metal", "https://static.wikia.nocookie.net/dothack/images/b/b0/(006)_Heavy_Metal.jpg", "20" };
                case 24:
                    return new string[] { "Hell Box", "https://static.wikia.nocookie.net/dothack/images/6/69/(242)_Hell_Box.jpg", "15" };
                case 25:
                    return new string[] { "Killer Box", "https://static.wikia.nocookie.net/dothack/images/0/0f/(243)_Killer_Box.jpg", "15" };
                case 26:
                    return new string[] { "Lich Lord", "https://static.wikia.nocookie.net/dothack/images/b/b8/(034)_Lich_Lord.jpg", "25" };
                case 27:
                    return new string[] { "Nightmare", "https://static.wikia.nocookie.net/dothack/images/1/1f/(010)_Nightmare.jpg", "20" };
                case 28:
                    return new string[] { "Nomadic Bones", "https://static.wikia.nocookie.net/dothack/images/0/0e/(247)_Nomadic_Bones.jpg", "15" };
                case 29:
                    return new string[] { "Ochimusha", "https://static.wikia.nocookie.net/dothack/images/f/f1/(004)_Ochimusha.jpg", "20" };
                case 30:
                    return new string[] { "Pandora's Box", "https://static.wikia.nocookie.net/dothack/images/7/71/'s_Box.jpg", "15" };
                case 31:
                    return new string[] { "Phalanx", "https://static.wikia.nocookie.net/dothack/images/0/0f/(007)_Phalanx.jpg", "20" };
                case 32:
                    return new string[] { "Skull Hero", "https://static.wikia.nocookie.net/dothack/images/0/0b/(249)_Skull_Hero.jpg", "15" };
                case 33:
                    return new string[] { "Spin Figure", "https://static.wikia.nocookie.net/dothack/images/a/a7/(063)_Spin_Figure.jpg", "30" };
                case 34:
                    return new string[] { "Swordmanoid", "https://static.wikia.nocookie.net/dothack/images/5/54/(002)_Swordmanoid.jpg", "15" };
                case 35:
                    return new string[] { "Undead Voodoo", "https://static.wikia.nocookie.net/dothack/images/8/88/(252)_Undead_Voodoo.jpg", "20" };
                case 36:
                    return new string[] { "Twilight Guardian", "https://static.wikia.nocookie.net/dothack/images/f/fa/Guardian.jpg", "300" };
                case 37:
                    return new string[] { "Skeith", "https://static.wikia.nocookie.net/dothack/images/d/d6/Skeith.png", "150" };
                case 38:
                    return new string[] { "Innis", "https://static.wikia.nocookie.net/dothack/images/e/ef/Innis.JPG", "150" };
                case 39:
                    return new string[] { "Magus", "https://static.wikia.nocookie.net/dothack/images/4/4f/Magus.JPG", "150" };
                case 40:
                    return new string[] { "Fidchell", "https://static.wikia.nocookie.net/dothack/images/b/bf/Fidchell.JPG", "150" };
                case 41:
                    return new string[] { "Gorre", "https://static.wikia.nocookie.net/dothack/images/3/31/Gorre.png", "150" };
                case 42:
                    return new string[] { "Macha", "https://static.wikia.nocookie.net/dothack/images/b/b2/Macha.JPG", "150" };
                case 43:
                    return new string[] { "Tarvos", "https://static.wikia.nocookie.net/dothack/images/f/f9/Tarvos.JPG", "150" };
                case 44:
                    return new string[] { "Corbenik", "https://static.wikia.nocookie.net/dothack/images/9/99/Corbenik.JPG", "150" };
                default:
                    return new string[] { "", "", "" };
                */
            }
        }
        #endregion

        #region Other Tasks
        private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync(embed);
            }
        }

        private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            return Task.CompletedTask;
        }

        private Task OnClientReady(DiscordClient c, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}
