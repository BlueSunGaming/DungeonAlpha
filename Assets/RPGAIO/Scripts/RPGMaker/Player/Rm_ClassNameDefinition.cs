using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rm_ClassNameDefinition
    {
        public string ID;
        public string Name;
        public string Description;
        public ImageContainer Image;

        public Rm_ClassNameDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Class Name";
            Description = "";
            Image = new ImageContainer();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}