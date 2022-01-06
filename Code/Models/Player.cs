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
        }

        public void Equip_Weapon(Weapon weapon)
        {
            Equip = weapon;
        }

        public void Gain_Experience(int number)
        {
            Experience += number;
            Levelcheck();
        }

        public void Levelcheck()
        {
            while(Experience >= Max_Experience)
            {
                Level++;
                Experience -= Max_Experience;
            }
        }
    }
}
