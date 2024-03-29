﻿using dotHack_Discord_Game.Models;
using System.Collections.Generic;

namespace dotHack_Discord_Game
{
    #region Key Items
    public class KeyItems
    {
        public static Item Twilight_Bracelet = new Item("Twilight Bracelet", Item.Type.Key);
    }
    #endregion

    public class UsableItems
    {
        public static Item AttackPlus_Book = new Item("Attack+ Book", Item.Type.Usable);
        public static Item CritPlus_Book = new Item("Crit+ Book", Item.Type.Usable);
        public static Item Virus_Core = new Item("Virus Core", Item.Type.Usable);
    }

    #region Longarm Weapons
    public class LongarmWeapons
    {
        //                                              Name                Lvl Atk     Crit     Required Class          Spells
        public static Weapon Bronze_Spear = new Weapon("Bronze Spear",      1,  3,      1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Triple_Doom });
        public static Weapon Amazon_Spear = new Weapon("Amazon Spear",      2,  8,      1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Triple_Doom });
        public static Weapon Gold_Spear = new Weapon("Gold Spear",          6,  13,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Double_Sweep });
        public static Weapon Bloody_Lance = new Weapon("Bloody Lance",      8,  17,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Triple_Doom });
        public static Weapon Berserk_Spear = new Weapon("Berserk Spear",    12, 19,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Vak_Repulse, SkillDictionary.Longarm.Double_Sweep });
        public static Weapon Steel_Spear = new Weapon("Steel Spear",        15, 23,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Repulse_Cage, SkillDictionary.Longarm.Double_Sweep });
        public static Weapon Adamant_Lance = new Weapon("Adamant Lance",    20, 25,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Triple_Doom, SkillDictionary.Longarm.Rai_Wipe });
        public static Weapon Dragnir = new Weapon("Dragnir",                39, 27,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Triple_Doom, SkillDictionary.Longarm.Vak_Repulse, SkillDictionary.Longarm.Rai_Wipe });
        public static Weapon Happiness = new Weapon("Happiness",            50, 28,     1.25,    JobClass.Longarm, new List<Skill> { SkillDictionary.Longarm.Rue_Repulse });
    }
    #endregion

    #region Twinblade Weapons
    public class TwinbladeWeapons
    {
        //                                              Name                 Lvl Atk         Crit  Required Class          Spells
        public static Weapon Amateur_Blades = new Weapon("Amateur Blades",   1,  3,          2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Saber_Dance });
        public static Weapon Steel_Blades = new Weapon("Steel Blades",       2,  4,          2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Tiger_Claws });
        public static Weapon Ronin_Blades = new Weapon("Ronin Blades",       7,  7,          2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Tiger_Claws });
        public static Weapon Masterblades = new Weapon("Masterblades",       13, 11,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Saber_Dance, SkillDictionary.Twinblade.Tiger_Claws, SkillDictionary.Twinblade.Staccatto });
        public static Weapon Shirogane = new Weapon("Shirogane",             15, 13,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Staccatto, SkillDictionary.Twinblade.Thunder_Coil });
        public static Weapon Slayers = new Weapon("Slayers",                 17, 15,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Twin_Darkness, SkillDictionary.Twinblade.Twin_Dragons });
        public static Weapon Kyoura = new Weapon("Kyoura",                   22, 17,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Thunder_Dance, SkillDictionary.Twinblade.Orchid_Dance });
        public static Weapon Enja = new Weapon("Enja",                       35, 19,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Flame_Dance, SkillDictionary.Twinblade.Flame_Vortex });
        public static Weapon Specter_Blades = new Weapon("Specter Blades",   40, 21,         2,    JobClass.Twinblade, new List<Skill> { SkillDictionary.Twinblade.Swirling_Dark, SkillDictionary.Twinblade.Dragon_Rage });

    }
    #endregion

    #region Heavyblade Weapons
    public class HeavybladeWeapons
    {
        //                                            Name                   Lvl Atk     Crit    Required Class          Spells
        public static Weapon Steelblade = new Weapon("Steelblade",           1,  3,      1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Calamity });
        public static Weapon Flamberge = new Weapon("Flamberge",             3,  7,      1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Calamity });
        public static Weapon Nodachi = new Weapon("Nodachi",                 6,  9,      1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Hayabusa });
        public static Weapon Magnifier = new Weapon("Magnifier",             8,  11,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Calamity });
        public static Weapon Sharp_Blade = new Weapon("Sharp Blade",         12, 13,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Gan_Smash, SkillDictionary.Heavyblade.Calamity });
        public static Weapon Claymore = new Weapon("Claymore",               15, 17,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Rai_Smash, SkillDictionary.Heavyblade.Calamity });
        public static Weapon Light_Giver = new Weapon("Light Giver",         20, 19,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Rai_Smash, SkillDictionary.Heavyblade.Rai_Drive });
        public static Weapon Crusher = new Weapon("Crusher",                 24, 22,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Danku, SkillDictionary.Heavyblade.Gohryu });
        public static Weapon High_Forger = new Weapon("High Forger",         30, 25,     1.25,   JobClass.Heavyblade, new List<Skill> { SkillDictionary.Heavyblade.Vak_Smash, SkillDictionary.Heavyblade.Juk_Drive });
    }
    #endregion

    #region Heavyaxe Weapons
    public class HeavyaxeWeapons
    {
        //                                            Name                   Lvl Atk     Crit    Required Class
        public static Weapon Hatchet = new Weapon("Hatchet",                 1,  3,      1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Axel_Pain });
        public static Weapon Water_Axe = new Weapon("Water Axe",             3,  5,      1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Triple_Wield });
        public static Weapon Razor_Axe = new Weapon("Razor Axe",             7,  7,      1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Brandish });
        public static Weapon Earth_Axe = new Weapon("Earth Axe",             9,  10,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Gan_Tornado });
        public static Weapon Masters_Axe = new Weapon("Master's Axe",        14, 12,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Axel_Pain, SkillDictionary.Heavyaxe.Rai_Tornado });
        public static Weapon Full_Swing = new Weapon("Full Swing",           17, 16,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Triple_Wield, SkillDictionary.Heavyaxe.Rai_Basher });
        public static Weapon Sinners_Axe = new Weapon("Sinner's Axe",        21, 19,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Ani_Break, SkillDictionary.Heavyaxe.Ani_Basher });
        public static Weapon Sorcerys_Axe = new Weapon("Sorcery's Axe",      25, 22,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Axel_Pain, SkillDictionary.Heavyaxe.Ani_Basher });
        public static Weapon Alien_Axe = new Weapon("Alien Axe",             37, 26,     1.5,    JobClass.Heavyaxe, new List<Skill> { SkillDictionary.Heavyaxe.Ani_Tornado, SkillDictionary.Heavyaxe.Ani_Basher });
    }
    #endregion

    #region Blademaster Weapons
    public class BlademasterWeapons
    {
        //                                            Name                   Lvl Atk     Crit    Required Class
        public static Weapon Brave_Sword = new Weapon("Brave Sword",         1,   2,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Crack_Beat });
        public static Weapon Strange_Blade = new Weapon("Strange Blade",     4,   5,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Revolver });
        public static Weapon Corpseblade = new Weapon("Corpseblade",         6,   8,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Crack_Beat });
        public static Weapon Souleater = new Weapon("Souleater",             10, 12,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Cross_Slash });
        public static Weapon Glitter = new Weapon("Glitter",                 13, 16,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Crack_Beat, SkillDictionary.Blademaster.Revolver });
        public static Weapon HeavenAndEarth = new Weapon("Heaven & Earth",   16, 20,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Gan_Slash, SkillDictionary.Blademaster.Gan_Revolver });
        public static Weapon Matoi = new Weapon("Matoi",                     23, 23,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Gan_Crack, SkillDictionary.Blademaster.Gan_Revolver });
        public static Weapon Phantom = new Weapon("Phantom",                 40, 25,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Ani_Crack, SkillDictionary.Blademaster.Ani_Revolver });
        public static Weapon Pegasus_Comet = new Weapon("Pegasus Comet",     48, 27,     1.5,    JobClass.Blademaster, new List<Skill> { SkillDictionary.Blademaster.Vak_Revolver, SkillDictionary.Blademaster.Gan_Revolver });
    }
    #endregion

    #region Wavemaster Weapons
    public class WavemasterWeapons
    {
        //                                            Name                   Lvl Atk     Crit    Required Class
        public static Weapon Iron_Rod = new Weapon("Iron Rod",               1,   2,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Gan_Rom });
        public static Weapon Fire_Wand = new Weapon("Fire Wand",             3,   4,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Vak_Don });
        public static Weapon Wand_of_Wisdom = new Weapon("Wand of Wisdom",   6,   5,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Vak_Kruz, SkillDictionary.Wavemaster.Vak_Don });
        public static Weapon Starstorm_Wand = new Weapon("Starstorm Wand",   13,  9,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Yarthkins });
        public static Weapon Bubble_Rod = new Weapon("Bubble Rod",           16, 12,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Rue_Rom, SkillDictionary.Wavemaster.Rue_Zot });
        public static Weapon Nerd_Staff = new Weapon("Nerd Staff",           18, 14,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.Ani_Don, SkillDictionary.Wavemaster.Ani_Zot });
        public static Weapon Gaias_Staff = new Weapon("Gaia's Staff",        20, 16,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.OrGan_Don, SkillDictionary.Wavemaster.GiGan_Zot });
        public static Weapon Tenami = new Weapon("Tenami",                   34, 17,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.OrGan_Don, SkillDictionary.Wavemaster.GiGan_Zot });
        public static Weapon Cygnus_Rod = new Weapon("Cygnus Rod",           45, 20,     2,      JobClass.Wavemaster, new List<Skill> { SkillDictionary.Wavemaster.PhaRue_Rom, SkillDictionary.Wavemaster.MeRue_Zot });

    }
    #endregion

    #region Misc Weapons
    public class KnucklemasterWeapons { }
    #endregion
}
