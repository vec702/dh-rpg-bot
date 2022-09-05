using dotHack_Discord_Game.Models;

namespace dotHack_Discord_Game
{
    #region Elements
    
    #endregion

    #region Skill Dictionary
    public class SkillDictionary
    {
        #region Twinblade Spells
        public class Twinblade
        {
            public static Skill Saber_Dance = new Skill("Saber Dance", Element.None());
            public static Skill Tiger_Claws = new Skill("Tiger Claws", Element.None());
            public static Skill Staccatto = new Skill("Staccatto", Element.None());
            public static Skill Thunder_Coil = new Skill("Thunder Coil", Element.Thunder());
            public static Skill Twin_Dragons = new Skill("Twin Dragons", Element.None());
            public static Skill Twin_Darkness = new Skill("Twin Darkness", Element.Darkness());
            public static Skill Thunder_Dance = new Skill("Thunder Dance", Element.Thunder());
            public static Skill Orchid_Dance = new Skill("Orchid Dance", Element.None());
            public static Skill Flame_Dance = new Skill("Flame Dance", Element.Fire());
            public static Skill Flame_Vortex = new Skill("Flame Vortex", Element.Fire());
            public static Skill Swirling_Dark = new Skill("Swirling Dark", Element.Darkness());
            public static Skill Dragon_Rage = new Skill("Dragon Rage", Element.None());
        }
        #endregion

        #region Blademaster Spells
        public class Blademaster
        {
            public static Skill Crack_Beat = new Skill("Crack Beat", Element.None());
            public static Skill Cross_Slash = new Skill("Cross Slash", Element.None());
            public static Skill Revolver = new Skill("Revolver", Element.None());
            public static Skill Gan_Slash = new Skill("Gan Slash", Element.Earth());
            public static Skill Gan_Revolver = new Skill("Gan Revolver", Element.Earth());
            public static Skill Gan_Crack = new Skill("Gan Crack", Element.Earth());
            public static Skill Ani_Crack = new Skill("Ani Crack", Element.Darkness());
            public static Skill Ani_Revolver = new Skill("Ani Revolver", Element.Darkness());
            public static Skill Vak_Revolver = new Skill("Vak Revolver", Element.Fire());
        }
        #endregion

        #region Heavyblade Spells
        public class Heavyblade
        {
            public static Skill Death_Bringer = new Skill("Death Bringer", Element.None());
            public static Skill Hayabusa = new Skill("Hayabusa", Element.None());
            public static Skill Calamity = new Skill("Calamity", Element.None());
            public static Skill Gan_Smash = new Skill("Gan Smash", Element.Earth());
            public static Skill Rai_Smash = new Skill("Rai Smash", Element.Thunder());
            public static Skill Rai_Drive = new Skill("Rai Drive", Element.Thunder());
            public static Skill Danku= new Skill("Danku", Element.None());
            public static Skill Gohryu = new Skill("Gohryu", Element.None());
            public static Skill Vak_Smash = new Skill("Vak Smash", Element.Fire());
            public static Skill Juk_Drive = new Skill("Juk Drive", Element.Wood());

        }
        #endregion

        #region Heavyaxe Spells
        public class Heavyaxe
        {
            public static Skill Axel_Pain = new Skill("Axel Pain", Element.None());
            public static Skill Triple_Wield = new Skill("Triple Wield", Element.None());
            public static Skill Brandish = new Skill("Brandish", Element.None());
            public static Skill Gan_Tornado = new Skill("Gan Tornado", Element.Earth());
            public static Skill Rai_Tornado = new Skill("Rai Tornado", Element.Thunder());
            public static Skill Rai_Basher = new Skill("Rai Basher", Element.Thunder());
            public static Skill Ani_Break = new Skill("Ani Break", Element.Darkness());
            public static Skill Ani_Basher = new Skill("Ani Basher", Element.Darkness());
            public static Skill Ani_Tornado = new Skill("Ani Tornado", Element.Darkness());
        }
        #endregion
        #region Longarm Spells
        public class Longarm
        {
            public static Skill Triple_Doom = new Skill("Triple Doom", Element.None());
            public static Skill Double_Sweep = new Skill("Double Sweep", Element.None());
            public static Skill Vak_Repulse = new Skill("Vak Repulse", Element.Fire());
            public static Skill Repulse_Cage = new Skill("Repulse Cage", Element.None());
            public static Skill Rai_Wipe = new Skill("Rai Wipe", Element.Thunder());
            public static Skill Rue_Repulse = new Skill("Rue Repulse", Element.Water());
        }
        #endregion
        #region Wavemaster Spells
        public class Wavemaster
        {
            public static Skill Gan_Rom = new Skill("Gan Rom", Element.Earth());
            public static Skill Vak_Don = new Skill("Vak Don", Element.Fire());
            public static Skill Vak_Kruz = new Skill("Vak Kruz", Element.Fire());
            public static Skill Yarthkins = new Skill("Yarthkins", Element.Earth());
            public static Skill Rue_Rom = new Skill("Rue Rom", Element.Water());
            public static Skill Rue_Zot = new Skill("Rue Zot", Element.Water());
            public static Skill Ani_Don = new Skill("Ani Don", Element.Darkness());
            public static Skill Ani_Zot = new Skill("Ani Zot", Element.Darkness());
            public static Skill OrGan_Don = new Skill("OrGan Don", Element.Earth());
            public static Skill GiGan_Zot = new Skill("GiGan Zot", Element.Earth());
            public static Skill PhaRue_Rom = new Skill("PhaRue Rom", Element.Water());
            public static Skill MeRue_Zot = new Skill("MeRue Zot", Element.Water());
        }
        #endregion
    }
    #endregion
}