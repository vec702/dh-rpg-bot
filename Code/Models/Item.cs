using System;

namespace dotHack_Discord_Game.Models
{
    public class Item
    {
        public enum Type
        {
            Key,
            Usable
        }

        public string Name;
        public Type ItemType;

        public Item(string _name, Type _itemType)
        {
            this.Name = _name;
            this.ItemType = _itemType;
        }

        public async void Use(Player p)
        {
            p.Items.Remove(this);

            switch(this.Name)
            {
                case "Attack+ Book":
                    p.Equip.Attack = p.Equip.Attack + 1;
                    await Bot.SendMessage("The equipped " + p.Equip.Name + "'s attack stat has increased by +1.");
                    break;

                case "Crit+ Book":
                    p.Equip.Crit_Rate = p.Equip.Crit_Rate + 0.02;
                    await Bot.SendMessage("The equipped " + p.Equip.Name + "'s crit rate stat has increased by .02%.");
                    break;

                case "Virus Core":
                    Random random = new Random();
                    if (random.Next(1, 3) == 1)
                    {
                        p.Equip.Attack += 2;
                        await Bot.SendMessage("The equipped " + p.Equip.Name + "'s attack stat has increased by +2.");
                    }
                    else
                    {
                        p.Equip.Crit_Rate = p.Equip.Crit_Rate + 0.04;
                        await Bot.SendMessage("The equipped " + p.Equip.Name + "'s crit rate stat has increased by .04%.");
                    }
                    break;

                default: break;
            }
        }
    }
}
