using System;
using System.Collections.Generic;

namespace dotHack_Discord_Game.Models
{
    public class Player
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Max_Experience { get; set; }
        public JobClass Class { get; set; }
        public ulong Kills { get; set; }
        public List<Weapon> Inventory { get; set; }
        public List<Item> Items { get; set; }
        public Weapon Equip { get; set; }
        public Element Element { get; set; }
        public Skill CastedSpell { get; set; }

        public Player(ulong _Id, string _Name)
        {
            Id = _Id;
            Name = _Name;
            Level = 1;
            Kills = 0;
            Experience = 0;
            Max_Experience = 1000;
            Inventory = new List<Weapon>();
            Items = new List<Item>();
            Equip = new Weapon("Fists", 1, 1, 1.25, Class);
            CastedSpell = null;
            Element = Element.None();
        }

        public void Reset_Casted_Spell(object sender, EventArgs e)
        {
            CastedSpell = null;
        }

        public void Equip_Weapon(Weapon weapon)
        {
            Equip = weapon;
        }

        public void Gain_Experience(int number)
        {
            //await Bot.SendMessage($"{this.Name} gained {number} experience.");
            Experience += number;
            Levelcheck();
        }

        public bool HasTwilightBracelet()
        {
            bool toReturn = false;
            foreach (var k in Items)
            {
                if(k.Name == "Twilight Bracelet") toReturn = true;
            }

            return toReturn;
        }
        public async void Levelcheck()
        {
            while(Experience >= Max_Experience)
            {
                Level++;
                Experience -= Max_Experience;
                await Bot.SendMessage($"**LVL UP!** {this.Name} has achieved level {this.Level}.");
            }
        }
    }
}
