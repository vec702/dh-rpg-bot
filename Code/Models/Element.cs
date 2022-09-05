using System;

namespace dotHack_Discord_Game.Models
{
    public class Element
    {
        public string Name { get; set; }
        public string Weakness { get; set; }

        public Element(string _name, string _weakness)
        {
            Name = _name;
            Weakness = _weakness;
        }

        public static Element Fire() { return new Element("Fire", "Water"); }
        public static Element Water() { return new Element("Water", "Fire"); }
        public static Element Thunder() { return new Element("Thunder", "Darkness"); }
        public static Element Darkness() { return new Element("Darkness", "Thunder"); }
        public static Element Earth() { return new Element("Earth", "Wood"); }
        public static Element Wood() { return new Element("Wood", "Earth"); }
        public static Element None() { return new Element("None", ""); }

        public static int CalculateElementalDamage(Element User, Element Target, int number)
        {
            if(Target.Weakness == User.Name)
            {
                number *= 2;
            }

            return number;
        }
    }
}