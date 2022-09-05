using System.Timers;
using System;
using Timer = System.Timers.Timer;

namespace dotHack_Discord_Game.Models
{
    public class Skill
    {
        public string Name { get; set; }
        public Element Element { get; set; }

        public Skill(string _name, Element _element)
        {
            Name = _name;
            Element = _element;
        }

        public async void Use(Player p)
        {
            Timer timer = new Timer();

            p.Element = Element;
            p.CastedSpell = this;

            await Bot.SendMessage($"{p.Name} prepares to cast {p.CastedSpell.Name}.");

            if (timer.Enabled) timer.Stop();
            timer.Start();

            timer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
            timer.Elapsed += p.Reset_Casted_Spell;
        }
    }
}