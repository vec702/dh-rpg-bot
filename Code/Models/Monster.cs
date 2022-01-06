namespace dotHack_Discord_Game.Models
{
    public class CorruptedMonster
    {
        public string Name {  get; set; }
        public string ImageUrl { get; set; }
        public Monster DrainedMonster { get; set; }
        public int Health { get; set; }
        public Weapon[] Drops { get; set; }
        public Item[] ItemDrops { get; set; }

        public CorruptedMonster(Monster _drainedMonster, string _name, string _imageUrl, int _health, Weapon[] _drops, Item[] _itemDrops)
        {
            DrainedMonster = _drainedMonster;
            Name = _name;
            ImageUrl = _imageUrl;
            Health = _health;
            Drops = _drops;
            ItemDrops = _itemDrops;
        }
    }
    public class Monster
    {
        public string Name {  get; set; }
        public string ImageUrl { get; set; }
        public int Health { get; set; }
        public Weapon[] Drops { get; set; }
        public Item[] ItemDrops { get; set; }

        public Monster(string _name, string _imageUrl, int _health, Weapon[] _drops, Item[] _itemDrops)
        {
            Name = _name;
            ImageUrl = _imageUrl;
            Health = _health;
            Drops = _drops;
            ItemDrops = _itemDrops;
        }
    }
}
