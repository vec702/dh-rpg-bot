using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace dotHack_Discord_Game.Models
{
    public class Weapon
    {
        #region Weapon Declarations
        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int Attack { get; set; }
        public JobClass RequiredClass { get; set; }
        public double Crit_Rate { get; set; }
        public List<Skill> Spells { get; set; }
        #endregion

        #region Weapon Functions
        public Weapon(string _name, int _level, int _attack, double _crit_Rate, JobClass _job)
        {
            Name = _name;
            RequiredLevel = _level;
            Attack = _attack;
            RequiredClass = _job;
            Crit_Rate = _crit_Rate;
            Spells = null;
        }

        public Weapon(string _name, int _level, int _attack, double _crit_Rate, JobClass _job, List<Skill> _spells)
        {
            Name = _name;
            RequiredLevel = _level;
            Attack = _attack;
            RequiredClass = _job;
            Crit_Rate = _crit_Rate;
            Spells = _spells;
        }

        public string calculateEquipStats(Player p)
        {
            string result = string.Empty;
            var equation = (this.Attack - p.Equip.Attack);

            if (equation >= 0)
            {
                result = "+" + equation.ToString();
            }
            else result = equation.ToString();

            return result;
        }

        public string ListSpells()
        {
            string result = string.Empty;

            if(Spells != null)
            {
                foreach (var s in Spells)
                {
                    if (s == Spells[0]) result += s.Name;
                    else result += $", {s.Name}";
                }
            }

            return result;
        }
        #endregion
    }
}
