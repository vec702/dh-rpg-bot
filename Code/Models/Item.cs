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
    }
}
