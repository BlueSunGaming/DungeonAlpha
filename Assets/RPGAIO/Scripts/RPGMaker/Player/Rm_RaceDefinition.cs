using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rm_RaceDefinition
    {
        public string ID;
        public string Name;
        public string Description;
        public ImageContainer Image;

        public Rm_RaceDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Race";
            Description = "";
            Image = new ImageContainer();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}