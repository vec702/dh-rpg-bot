using dotHack_Discord_Game.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotHack_Discord_Game
{
    public class Commands : BaseCommandModule
    {
        [Command("botchannel"), Description("Set the channel the bot will summon enemies in.")]
        public async Task Botchannel(CommandContext ctx)
        {
            if(ctx.Member.IsOwner)
            {
                var interactivity = ctx.Client.GetInteractivity();
                var message = await ctx.RespondAsync("Enter the ID of a channel the bot will use: ");
                var response = await interactivity.WaitForMessageAsync(xm => true, TimeSpan.FromSeconds(10));
                Bot.BotChannelID = Convert.ToUInt64(response.Result.Content.ToString());
            }
        }

        [Command("exprate"), Description("Set the EXP Rate at which the bot will reward players.")]
        public async Task exprate(CommandContext ctx, params double[] _newRate)
        {
            if(ctx.Member.IsOwner)
            {
                if(_newRate.Count() > 0)
                {
                    Bot.expRate = Convert.ToDouble(_newRate.First());
                    await ctx.RespondAsync("EXP Rate set to: " + Bot.expRate.ToString());
                }
                else
                {
                    await ctx.RespondAsync("Usage: //exprate [number]");
                }
            }
        }

        [Command("droprate"), Description("Set the DROP Rate at which the bot will reward players items.")]
        public async Task droprate(CommandContext ctx, params double[] _newRate)
        {
            if (ctx.Member.IsOwner)
            {
                if (_newRate.Count() > 0)
                {
                    Bot.dropRate = Convert.ToDouble(_newRate.First());
                    await ctx.RespondAsync("DROP Rate set to: " + Bot.dropRate.ToString());
                }
                else
                {
                    await ctx.RespondAsync("Usage: //droprate [number]");
                }
            }
        }

        [Command("dmgrate"), Description("Set the rate at which the bot will calculate damage.")]
        public async Task dmgrate(CommandContext ctx, params double[] _newRate)
        {
            if (ctx.Member.IsOwner)
            {
                if(_newRate.Count() > 0)
                {
                    Bot.dmgRate = Convert.ToDouble(_newRate.First());
                    await ctx.RespondAsync("DMG Rate set to: " + Bot.dmgRate.ToString());
                }
                else
                {
                    await ctx.RespondAsync("Usage: //dmgrate [number]");
                }
            }
        }

        [Command("signup"), Description("Sign-up to fight .hack// enemies.")]
        public async Task Signup(CommandContext ctx, params string[] _class)
        {
            var p = new Player(ctx.User.Id, ctx.User.Username.ToString());
            if (!Bot.Players.ContainsKey(p.Id.ToString()))
            {
                if (_class.Count() > 0)
                {
                    var readClass = _class.First().ToString();

                    var Twinblade = String.Equals(readClass, "Twinblade", StringComparison.OrdinalIgnoreCase);
                    var Blademaster = String.Equals(readClass, "Blademaster", StringComparison.OrdinalIgnoreCase);
                    var Heavyblade = String.Equals(readClass, "Heavyblade", StringComparison.OrdinalIgnoreCase);
                    var Longarm = String.Equals(readClass, "Longarm", StringComparison.OrdinalIgnoreCase);
                    var Heavyaxe = String.Equals(readClass, "Heavyaxe", StringComparison.OrdinalIgnoreCase);
                    var Wavemaster = String.Equals(readClass, "Wavemaster", StringComparison.OrdinalIgnoreCase);

                    if (Twinblade) p.Class = JobClass.Twinblade;
                    else if (Blademaster) p.Class = JobClass.Blademaster;
                    else if (Heavyblade) p.Class = JobClass.Heavyblade;
                    else if (Heavyaxe) p.Class = JobClass.Heavyaxe;
                    else if (Longarm) p.Class = JobClass.Longarm;
                    else if (Wavemaster) p.Class = JobClass.Wavemaster;
                    else await ctx.Client.GetChannelAsync(Bot.BotChannelID).GetAwaiter().GetResult().SendMessageAsync("Class \"" + readClass + "\" not found. Please try to //signup again.");

                    if(Twinblade || Blademaster || Heavyaxe || Heavyblade || Wavemaster || Longarm)
                    {
                        Bot.Players.Add(p.Id.ToString(), p);
                        await Bot.SendMessage("User " + p.Name + " registered as class " + p.Class + ".");
                    }
                }
                else
                {
                    var interactivity = ctx.Client.GetInteractivity();

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Monster Portal // Start your adventure",
                        Description = "What class? Type one of the following:\n - Twinblade\n - Blademaster\n - Heavyblade\n - Longarm\n - Heavyaxe\n - Wavemaster",
                    };

                    await Bot.SendMessage(embed);

                    try
                    {
                        var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Equals("Twinblade", StringComparison.OrdinalIgnoreCase) ||
                        xm.Content.Equals("Blademaster", StringComparison.OrdinalIgnoreCase) ||
                        xm.Content.Equals("Heavyblade", StringComparison.OrdinalIgnoreCase) ||
                        xm.Content.Equals("Longarm", StringComparison.OrdinalIgnoreCase) ||
                        xm.Content.Equals("Heavyaxe", StringComparison.OrdinalIgnoreCase) ||
                        xm.Content.Equals("Wavemaster", StringComparison.OrdinalIgnoreCase),
                        TimeSpan.FromSeconds(60));

                        var Twinblade = String.Equals(msg.Result.Content.ToString(), "Twinblade", StringComparison.OrdinalIgnoreCase);
                        var Blademaster = String.Equals(msg.Result.Content.ToString(), "Blademaster", StringComparison.OrdinalIgnoreCase);
                        var Heavyblade = String.Equals(msg.Result.Content.ToString(), "Heavyblade", StringComparison.OrdinalIgnoreCase);
                        var Longarm = String.Equals(msg.Result.Content.ToString(), "Longarm", StringComparison.OrdinalIgnoreCase);
                        var Heavyaxe = String.Equals(msg.Result.Content.ToString(), "Heavyaxe", StringComparison.OrdinalIgnoreCase);
                        var Wavemaster = String.Equals(msg.Result.Content.ToString(), "Wavemaster", StringComparison.OrdinalIgnoreCase);

                        if (msg.TimedOut || msg.Result.Content.ToString() == null)
                        {
                            await Bot.SendMessage("Timed out. Please try to //signup again.");
                        }
                        else
                        {
                            if (msg.Result.Author.Id == ctx.User.Id)
                            {
                                if (Twinblade) p.Class = JobClass.Twinblade;
                                else if (Blademaster) p.Class = JobClass.Blademaster;
                                else if (Heavyblade) p.Class = JobClass.Heavyblade;
                                else if (Heavyaxe) p.Class = JobClass.Heavyaxe;
                                else if (Longarm) p.Class = JobClass.Longarm;
                                else if (Wavemaster) p.Class = JobClass.Wavemaster;
                                else await Bot.SendMessage("Timed out. Please try to //signup again.");

                                Bot.Players.Add(p.Id.ToString(), p);
                                await Bot.SendMessage("User " + p.Name + " registered as class " + p.Class + ".");
                            }
                            else
                            {
                                await Bot.SendMessage("Another user tried interacting with the bot. Please try to //signup again.");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        await Bot.SendMessage("Timed out. Please try to //signup again.");
                    }
                }
            }
            else
            {
                await Bot.SendMessage("User " + ctx.User.Username.ToString() + " already exists!");
            }
        }

        [Command("equip"), Description("Equip a specific weapon in your inventory.")]
        public async Task Equip(CommandContext ctx, params string[] _item)
        {
            if (Bot.Players.ContainsKey(ctx.User.Id.ToString()))
            {
                if (Bot.Players.TryGetValue(ctx.User.Id.ToString(), out Player p))
                {
                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Monster Portal // " + p.Name,
                        Description = "Inventory: ",
                    };

                    // if item is specified
                    if (_item.Count() > 0)
                    {
                        string readItem = string.Empty;
                        for (int i = 0; i < _item.Count(); i++)
                        {
                            readItem += _item[i] + " ";
                        }

                        readItem = readItem.Substring(0, readItem.Length - 1);
                        string compareItem = readItem.ToUpper();
                        var found = false;


                        // if we can find the item based on the name provided
                        foreach (Weapon item in p.Inventory)
                        {
                            if (compareItem.Contains(item.Name.ToUpper()) || compareItem.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                found = true;
                                // and it matches our class
                                if (p.Class == item.RequiredClass)
                                {
                                    // then equip that item
                                    p.Equip_Weapon(item);
                                    await Bot.SendMessage(p.Name + " equipped " + p.Equip.Name + " (" + p.Attack + " Attack)");
                                }
                                else
                                {
                                    await Bot.SendMessage("You must be a level " + item.RequiredLevel + " " + item.RequiredClass + " to equip this weapon.");
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            await Bot.SendMessage("Could not find any item matching name \"" + readItem + "\"");
                        }
                    }
                    else await Bot.SendMessage("Usage: //equip [item name]");
                    /*
                    else
                    {
                        string inventoryOutput = string.Empty;
                        inventoryOutput += string.Join(", ", p.Inventory.Select(weapon => weapon.Name).ToArray());
                        embed.Description += inventoryOutput;

                        await ctx.Client.GetChannelAsync(Bot.BotChannelID).GetAwaiter().GetResult().SendMessageAsync(embed);

                        // wait for user to respond with input

                        // set equipped weapon to that input
                    }
                    */
                }
            }
            else
            {
                await Bot.SendMessage("User does not exist! Have you tried to \"//signup\"?");
            }
        }

        [Command("me"), Description("See your stats.")]
        public async Task Me(CommandContext ctx)
        {
            if (Bot.Players.ContainsKey(ctx.User.Id.ToString()))
            {
                if(Bot.Players.TryGetValue(ctx.User.Id.ToString(), out Player p))
                {
                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Monster Portal // " + p.Name,
                        Description = "Class: " + p.Class.ToString() + "\nLevel: " + p.Level +
                            "\nExperience: " + p.Experience + " / " + p.Max_Experience +
                            "\nEquipped Weapon: " + p.Equip.Name + " (" + p.Equip.Attack + " Attack)" +
                            "\nEnemies defeated: " + p.Kills.ToString() +
                            "\nInventory: ",
                    };

                    string inventoryOutput = string.Empty;
                    inventoryOutput += inventoryOutput.Count() % 5 == 0 ? string.Join(", ", p.Inventory.Select(weapon => weapon.Name).ToArray()) : "\n";
                    embed.Description += inventoryOutput;

                    await Bot.SendMessage(embed);
                }
            }
            else
            {
                await Bot.SendMessage("User does not exist! Have you tried to \"//signup\"?");
            }
        }
        [Command("top"), Description("See the top players.")]
        public async Task Top(CommandContext ctx)
        {
            string _Description = string.Empty;
#pragma warning disable CS1717 // Assignment made to same variable
            for (int i = 1; i < 10; i = i)
#pragma warning restore CS1717 // Assignment made to same variable
            {
                foreach (KeyValuePair<string, Player> player in Bot.Players.OrderBy(x => x.Value.Level))
                {
                    _Description += ("[" + i + ".] Name: " + player.Value.Name.ToString() + " | Class: " + player.Value.Class.ToString() + " | Level: " + player.Value.Level.ToString() + "\n");
                    i++;
                }
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Monster Portal // Top Players",
                Description = _Description,
            };

            await Bot.SendMessage(embed);
        }
    }
}