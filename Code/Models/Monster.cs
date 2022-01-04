namespace dotHack_Discord_Game.Models
{
    public class Monster
    {
        public string Name {  get; set; }
        public string ImageUrl { get; set; }
        public int Health { get; set; }
        public Weapon[] Drops { get; set; }
        public Item[] ItemDrops { get; set; }

        public Monster(string _name, string _imageUrl, int _health, Weapon[] _drops)
        {
            Name = _name;
            ImageUrl = _imageUrl;
            Health = _health;
            Drops = _drops;
        }

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
