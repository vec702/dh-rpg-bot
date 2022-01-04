namespace dotHack_Discord_Game.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int Attack { get; set; }
        public JobClass RequiredClass { get; set; }
        public double Crit_Rate { get; set; }

        public Weapon(string _name, int _level, int _attack, double _crit_Rate, JobClass _job)
        {
            Name = _name;
            RequiredLevel = _level;
            Attack = _attack;
            RequiredClass = _job;
            Crit_Rate = _crit_Rate;
        }
    }
}
