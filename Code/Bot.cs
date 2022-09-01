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

        private static bool monsterSpawned = false;
        private static bool portalSpawned = false;
        private static bool despawned = false;

        #region Rates
        public static double expRate = 3.0;
        public static double dmgRate = 3.0;
        public static double dropRate = 3.0;
        #endregion

        #region Drops
        private readonly Weapon[] t1Drops = new Weapon[] { LongarmWeapons.Bronze_Spear, BlademasterWeapons.Brave_Sword, TwinbladeWeapons.Amateur_Blades, HeavyaxeWeapons.Hatchet, WavemasterWeapons.Iron_Rod, HeavybladeWeapons.Steelblade };
        private readonly Weapon[] t2Drops = new Weapon[] { LongarmWeapons.Amazon_Spear, TwinbladeWeapons.Steel_Blades, HeavybladeWeapons.Flamberge, HeavyaxeWeapons.Water_Axe, BlademasterWeapons.Strange_Blade, WavemasterWeapons.Fire_Wand };
        private readonly Weapon[] t3Drops = new Weapon[] { LongarmWeapons.Gold_Spear, TwinbladeWeapons.Ronin_Blades, HeavybladeWeapons.Nodachi, HeavyaxeWeapons.Razor_Axe, BlademasterWeapons.Corpseblade, WavemasterWeapons.Wand_of_Wisdom };
        private readonly Weapon[] t4Drops = new Weapon[] { LongarmWeapons.Bloody_Lance, TwinbladeWeapons.Masterblades, HeavybladeWeapons.Magnifier, HeavyaxeWeapons.Earth_Axe, BlademasterWeapons.Souleater, WavemasterWeapons.Starstorm_Wand };
        private readonly Weapon[] t5Drops = new Weapon[] { LongarmWeapons.Berserk_Spear, TwinbladeWeapons.Shirogane, HeavybladeWeapons.Sharp_Blade, HeavyaxeWeapons.Masters_Axe, BlademasterWeapons.Glitter, WavemasterWeapons.Bubble_Rod };
        private readonly Weapon[] t6Drops = new Weapon[] { LongarmWeapons.Steel_Spear, TwinbladeWeapons.Slayers, HeavybladeWeapons.Claymore, HeavyaxeWeapons.Full_Swing, BlademasterWeapons.HeavenAndEarth, WavemasterWeapons.Nerd_Staff };
        private readonly Weapon[] t7Drops = new Weapon[] { LongarmWeapons.Adamant_Lance, TwinbladeWeapons.Kyoura, HeavybladeWeapons.Light_Giver, HeavyaxeWeapons.Sinners_Axe, BlademasterWeapons.Matoi, WavemasterWeapons.Gaias_Staff };

        private readonly Item[] t1iDrops = new Item[] { UsableItems.AttackPlus_Book, UsableItems.CritPlus_Book };
        private readonly Item[] t2iDrops = new Item[] { UsableItems.Virus_Core };
        #endregion

        #endregion

        #region Main Task
        public async Task RunAsync()
        {
            #region Configuration
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
            #endregion

            #region Event Triggers
            //client.Ready += OnClientReady;
            //client.SocketClosed += SavePlayerData;
            //client.SocketOpened += LoadPlayerData;
            client.MessageCreated += MonsterPortalEventChance;
            #endregion

            #region Commands
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
            #endregion Commands

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
        #endregion

        #region Bot-specific Tasks

        #region Event Spawner
        private Task MonsterPortalEventChance(DiscordClient _client, MessageCreateEventArgs e)
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
            else if (chance == 1)
            {
                var ctx = cnext.CreateContext(e.Message, "//", command, args);
                Task.Run(async () => await GiftTwilightBracelet(ctx)); return Task.CompletedTask;
            }
            else if(chance == 2)
            {
                var ctx = cnext.CreateContext(e.Message, "//", command, args);
                Task.Run(async () => await FightCorruptedMonsters(ctx)); return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }
        #endregion

        #region Monster Fight
        public async Task FightMonsters(CommandContext ctx)
        {
            // declarations for outside loop
            Timer timer = new Timer();
            Monster randomMonster = RandomMonster();
            DiscordMessage monsterMessage = null;

            var interactivity = client.GetInteractivity();

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

                // and the "fight" reaction
                var emoji = DiscordEmoji.FromName(client, ":crossed_swords:");
                await portalMessage.CreateReactionAsync(emoji);

                var portalResult = await interactivity.CollectReactionsAsync(portalMessage, TimeSpan.FromSeconds(0.5));

                while (portalSpawned)
                {
                    // read the users who reacted
                    foreach (var user in await portalMessage.GetReactionsAsync(emoji))
                    {
                        // and exclude bots
                        if (!user.IsBot)
                        {
                            // and they're registered for the game
                            if (IsRegistered(user))
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

                    monsterMessage = await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(embed);
                    await monsterMessage.CreateReactionAsync(emoji);

                    var monsterResult = await interactivity.CollectReactionsAsync(monsterMessage, TimeSpan.FromSeconds(0.5));

                    // start the despawn timer when the monster appears
                    if (timer.Enabled) timer.Stop();
                    timer.Start();

                    while (health_left > 0)
                    {
                        foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                        {
                            if (!IsRegistered(user) && !user.IsBot)
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

                                    var p = GetPlayer(user);
                                    damage = Convert.ToInt32(random.Next(p.Equip.Attack, p.Equip.Attack * Convert.ToInt32(p.Equip.Crit_Rate)) * dmgRate);
                                    damage = (int)Math.Round(Convert.ToDouble(damage), 0);

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
                            timer.Stop();
                            await SendMessage("The " + randomMonster.Name + " has got away.");
                            if (monsterMessage != null) await monsterMessage.DeleteAsync();
                            if (portalMessage != null) await portalMessage.DeleteAsync();
                            monsterSpawned = false;
                        }
                    }

                    if (health_left <= 0)
                    {
                        _Description = "Befallen by " + attackers.First().Username.ToString();

                        foreach (var attacker in attackers)
                        {
                            Random random = new Random();

                            var p = GetPlayer(attacker);
                            var exp = Convert.ToInt32(random.Next(150, 175) * expRate);

                            p.Kills++;
                            p.Gain_Experience(Convert.ToInt32(Math.Round(Convert.ToDouble(exp))));

                            if (attacker != attackers[0])
                            {
                                _Description += ", " + attacker.Username.ToString();
                            }

                            HandleDrops(attacker, randomMonster);

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
                        monsterSpawned = false;
                    }
                }
                await portalMessage.DeleteAsync();
            }
        }
        #endregion

        #region Random Monster Generator
        private Monster RandomMonster()
        {
            Random random = new Random();
            var response = random.Next(1, 15);
            switch (response)
            {
                case 0:
                    return new Monster("Aura", "https://static.wikia.nocookie.net/dothack/images/f/fc/Auraface.jpg", 1, null, new Item[] { KeyItems.Twilight_Bracelet });
                case 1:
                    return new Monster("Razine", "https://static.wikia.nocookie.net/dothack/images/5/53/%28001%29_Razine.jpg", 5, t1Drops, t1iDrops);
                case 2:
                    return new Monster("Tetra Armor", "https://static.wikia.nocookie.net/dothack/images/2/22/(011)_Tetra_Armor.jpg", 10, t2Drops, t1iDrops);
                case 3:
                    return new Monster("Nobunaga's Soul", "https://static.wikia.nocookie.net/dothack/images/7/7a/(005)_Nobunaga_Soul.jpg", 5, t1Drops, t1iDrops);
                case 4:
                    return new Monster("Menhir", "https://static.wikia.nocookie.net/dothack/images/e/e7/(180)_Menhir.jpg", 10, t2Drops, t1iDrops);
                case 5:
                    return new Monster("Drygon", "https://static.wikia.nocookie.net/dothack/images/2/2d/(212)_Drygon.jpg", 20, t4Drops, t1iDrops);
                case 6:
                    return new Monster("Ectoplasm", "https://static.wikia.nocookie.net/dothack/images/a/a1/(281)_Ectoplasm.jpg", 5, t1Drops, t1iDrops);
                case 7:
                    return new Monster("Noisy Wisp", "https://static.wikia.nocookie.net/dothack/images/f/f2/(282)_Noisy_Wisp.jpg", 5, t1Drops, t1iDrops);
                case 8:
                    return new Monster("Mimic", "https://static.wikia.nocookie.net/dothack/images/8/8b/(241)_Mimic.jpg", 5, t1Drops, t1iDrops);
                case 9:
                    return new Monster("Squidbod", "https://static.wikia.nocookie.net/dothack/images/1/19/(012)_Squidbod.jpg", 15, t3Drops, null);
                case 10:
                    return new Monster("General Armor", "https://static.wikia.nocookie.net/dothack/images/0/0f/(013)_General_Armor.jpg", 20, t4Drops, t1iDrops);
                case 11:
                    return new Monster("Bone Army", "https://static.wikia.nocookie.net/dothack/images/2/23/(248)_Bone_Army.jpg", 5, t1Drops, t1iDrops);
                case 12:
                    return new Monster("Comad Goo", "https://static.wikia.nocookie.net/dothack/images/8/8a/(062)_Comad_Goo.jpg", 30, t6Drops, null);
                case 13:
                    return new Monster("Dalaigon Anecdote", "https://static.wikia.nocookie.net/dothack/images/b/b0/(213)_Dalaigon_Anecdote.jpg", 35, t7Drops, null);
                case 14:
                    return new Monster("Dark Guru", "https://static.wikia.nocookie.net/dothack/images/f/fd/(028)_Dark_Guru.jpg", 25, t5Drops, t1iDrops);
                case 15:
                    return new Monster("Dark Horse", "https://static.wikia.nocookie.net/dothack/images/f/f9/(009)_Dark_Horse.jpg", 20, t4Drops, t1iDrops);
                default:
                    return new Monster("", "", 0, null, null);

                    /* 

                        old format, still need to merge these...

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

        #region Corrupted Monster Functions

        #region Corrupted Monster Fight
        public async Task FightCorruptedMonsters(CommandContext ctx)
        {
            // declarations for outside loop
            Timer timer = new Timer();
            CorruptedMonster randomMonster = RandomCorruptedMonster();
            DiscordMessage monsterMessage = null, protectBreakMessage = null;

            var interactivity = client.GetInteractivity();

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

                // and the "fight" reaction
                var emoji = DiscordEmoji.FromName(client, ":crossed_swords:");
                await portalMessage.CreateReactionAsync(emoji);

                var portalResult = await interactivity.CollectReactionsAsync(portalMessage, TimeSpan.FromSeconds(0.5));

                while (portalSpawned)
                {
                    // read the users who reacted
                    foreach (var user in await portalMessage.GetReactionsAsync(emoji))
                    {
                        // and exclude bots
                        if (!user.IsBot)
                        {
                            // and they're registered for the game
                            if (IsRegistered(user))
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

                    monsterMessage = await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(embed);
                    await monsterMessage.CreateReactionAsync(emoji);

                    var monsterResult = await interactivity.CollectReactionsAsync(monsterMessage, TimeSpan.FromSeconds(0.5));

                    // start the despawn timer when the monster appears
                    if (timer.Enabled) timer.Stop();
                    timer.Start();

                    while (health_left > 0)
                    {
                        foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                        {
                            if (!IsRegistered(user) && !user.IsBot)
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

                                    var p = GetPlayer(user);
                                    damage = Convert.ToInt32(random.Next(p.Equip.Attack, p.Equip.Attack * Convert.ToInt32(p.Equip.Crit_Rate)) * dmgRate);
                                    damage = (int)Math.Round(Convert.ToDouble(damage), 0);

                                    health_left -= damage;
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
                            timer.Stop();
                            if (randomMonster.Drained) await SendMessage("The " + randomMonster.DrainedMonster.Name + " has got away.");
                            else await SendMessage("The " + randomMonster.Name + " has got away.");
                            if (monsterMessage != null) await monsterMessage.DeleteAsync();
                            if (portalMessage != null) await portalMessage.DeleteAsync();
                            if (protectBreakMessage != null) await protectBreakMessage.DeleteAsync();
                            portalOpened = false;
                            monsterSpawned = false;
                        }
                    }

                    if (health_left <= 0)
                    {
                        foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                        {
                            // TODO HandleCorruptedDrops
                        }

                        var pbEmbed = new DiscordEmbedBuilder()
                        {
                            Title = "PROTECT BREAK!",
                            Description = randomMonster.Name + " is out of control, use data drain now!",
                            ImageUrl = "https://static.wikia.nocookie.net/dothack/images/8/81/Datadrainkite.jpg",
                        };

                        protectBreakMessage = await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(pbEmbed);
                        await protectBreakMessage.CreateReactionAsync(emoji);

                        while (!randomMonster.Drained)
                        {
                            foreach (var user in await protectBreakMessage.GetReactionsAsync(emoji))
                            {
                                if (!user.IsBot)
                                {
                                    var player = GetPlayer(user);
                                    if (player.HasTwilightBracelet())
                                    {
                                        await SendMessage("The monster's data has been drained by " + player.Name.ToString() + ", it's changing form!");
                                        randomMonster.Drained = true;
                                    }
                                    else await SendMessage("The data isn't responding to you...");
                                }
                            }
                        }

                        if (randomMonster.Drained)
                        {
                            await protectBreakMessage.DeleteAsync();

                            health_left = randomMonster.DrainedMonster.Health;

                            _Title = randomMonster.DrainedMonster.Name;
                            _ImageUrl = randomMonster.DrainedMonster.ImageUrl;
                            _Description = "Health: " + health_left + "/" + randomMonster.DrainedMonster.Health;
                            await monsterMessage.ModifyAsync(x =>
                            {
                                x.Content = "";
                                x.Embed = new DiscordEmbedBuilder()
                                    .WithTitle(_Title)
                                    .WithDescription(_Description)
                                    .WithImageUrl(_ImageUrl)
                                    .Build();
                            });

                            if (timer.Enabled) timer.Stop();
                            timer.Start();

                            while (health_left > 0)
                            {
                                foreach (var user in await monsterMessage.GetReactionsAsync(emoji))
                                {
                                    if (!IsRegistered(user) && !user.IsBot)
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

                                            var p = GetPlayer(user);
                                            damage = Convert.ToInt32(random.Next(p.Equip.Attack, p.Equip.Attack * Convert.ToInt32(p.Equip.Crit_Rate)) * dmgRate);
                                            damage = (int)Math.Round(Convert.ToDouble(damage), 0);

                                            health_left -= damage;
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
                                    timer.Stop();
                                    if (randomMonster.Drained) await SendMessage("The " + randomMonster.DrainedMonster.Name + " has got away.");
                                    else await SendMessage("The " + randomMonster.Name + " has got away.");
                                    if (monsterMessage != null) await monsterMessage.DeleteAsync();
                                    if (portalMessage != null) await portalMessage.DeleteAsync();
                                    if (protectBreakMessage != null) await protectBreakMessage.DeleteAsync();
                                    portalOpened = false;
                                    monsterSpawned = false;
                                }
                            }

                            if (health_left <= 0)
                            {
                                _Description = "Befallen by " + attackers.First().Username.ToString();

                                foreach (var attacker in attackers)
                                {
                                    Random random = new Random();

                                    var p = GetPlayer(attacker);
                                    var exp = Convert.ToInt32(random.Next(150, 175) * expRate);

                                    HandleDrops(attacker, randomMonster.DrainedMonster);

                                    p.Kills++;
                                    p.Gain_Experience(Convert.ToInt32(Math.Round(Convert.ToDouble(exp))));

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
                                portalOpened = false;
                                monsterSpawned = false;
                            }
                        }
                    }
                }
                await portalMessage.DeleteAsync();
            }
        }
        #endregion

        #region Random Corrupted Monster Generator
        private CorruptedMonster RandomCorruptedMonster()
        {
            Random random = new Random();
            var response = random.Next(1, 3);
            Monster randomMonster = RandomMonster();

            switch (response)
            {
                case 1:
                    return new CorruptedMonster(randomMonster, "Twilight Guardian", "https://static.wikia.nocookie.net/dothack/images/f/fa/Guardian.jpg", 200, null, t2iDrops);
                case 2:
                    return new CorruptedMonster(randomMonster, "Skeith", "https://static.wikia.nocookie.net/dothack/images/d/d6/Skeith.png", 150, null, t2iDrops);
                default:
                    return new CorruptedMonster(new Monster("", "", 0, null, null), "", "", 0, null, null);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Bracelet Gifter Function
        public async Task GiftTwilightBracelet(CommandContext ctx)
        {
            if (!ctx.User.IsBot && IsRegistered(ctx.User))
            {
                var player = GetPlayer(ctx.User);

                if (!player.HasTwilightBracelet())
                {
                    string memeImageUrl = "https://i.imgur.com/3QBLIF7.png";

                    Random random = new Random();
                    var chance = random.Next(1, 100);
                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "A#$a",
                        Description = "A girl dressed in white hands you a book... you feel compelled to open it.",
                        ImageUrl = "https://static.wikia.nocookie.net/dothack/images/f/fc/Auraface.jpg",
                    };

                    if (chance > 99)
                    {
                        embed.ImageUrl = memeImageUrl;
                    }

                    await SendMessage(embed);
                    player.Items.Add(KeyItems.Twilight_Bracelet);
                    await SendMessage(player.Name + " has received a Twilight Bracelet.");
                }
            }
        }
        #endregion

        #region Other Tasks

        #region Send Message
        public static async Task SendMessage(string Message)
        {
            await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(Message);
        }

        public static async Task SendMessage(DiscordEmbed Message)
        {
            await client.GetChannelAsync(BotChannelID).GetAwaiter().GetResult().SendMessageAsync(Message);
        }
        #endregion

        #region Despawn Monster
        private void despawnMonster(object sender, ElapsedEventArgs e)
        {
            despawned = true;
        }
        #endregion

        #region Player Functions
        public Player GetPlayer(DiscordUser user)
        {
            if (Players.TryGetValue(user.Id.ToString(), out Player p)) return p;
            return new Player(0, "null");
        }

        public bool IsRegistered(DiscordUser user)
        {
            if (Players.TryGetValue(user.Id.ToString(), out Player p)) return true;
            return false;
        }
        #endregion

        #region Drop Functions
        public async void AwardDrops(DiscordUser user, Monster Dropper, Weapon WeaponDrop, Item ItemDrop)
        {
            var p = GetPlayer(user);

            if (ItemDrop != null)
            {
                await SendMessage(p.Name + " received " + ItemDrop.Name + " from " + Dropper.Name + ".");
                p.Items.Add(ItemDrop);
            }

            if (WeaponDrop != null)
            {
                await SendMessage(p.Name + " received " + WeaponDrop.Name + " from " + Dropper.Name + ".");
                p.Inventory.Add(WeaponDrop);
            }
        }

        public void HandleDrops(DiscordUser user, Monster randomMonster)
        {
            Random random = new Random();
            Weapon drop = null;
            Item idrop = null;

            var dropChance = Convert.ToInt32(random.Next(1, 100) * dropRate);

            if (dropChance > 80)
            {
                // if they roll a 99+, award them two drops
                if (dropChance > 98)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        var itemOrWeapon = random.Next(1, 3);

                        switch (itemOrWeapon)
                        {
                            case 1:
                                var weaponDropIndex = random.Next(1, randomMonster.Drops.Count());
                                drop = randomMonster.Drops[weaponDropIndex];
                                AwardDrops(user, randomMonster, drop, idrop);
                                break;
                            case 2:
                                var itemDropIndex = random.Next(1, randomMonster.ItemDrops.Count());
                                idrop = randomMonster.ItemDrops[itemDropIndex];
                                AwardDrops(user, randomMonster, drop, idrop);
                                break;
                        }
                    }
                }
                else
                {
                    var itemOrWeapon = random.Next(1, 3);

                    switch (itemOrWeapon)
                    {
                        case 1:
                            var weaponDropIndex = random.Next(1, randomMonster.Drops.Count());
                            drop = randomMonster.Drops[weaponDropIndex];
                            AwardDrops(user, randomMonster, drop, idrop);
                            break;
                        case 2:
                            var itemDropIndex = random.Next(1, randomMonster.ItemDrops.Count());
                            idrop = randomMonster.ItemDrops[itemDropIndex];
                            AwardDrops(user, randomMonster, drop, idrop);
                            break;
                    }
                }
            }
        }
        #endregion

        #region Logging Functions
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
        #endregion

        #endregion
    }
}
