using System;
using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class WorldArea
    {
        public string ID;
        public string Name;
        public List<Location> Locations;
        public ImageContainer ImageContainer;

        public WorldArea()
        {
            ID = Guid.NewGuid().ToString();
            Name = "World";
            Locations = new List<Location>();
            ImageContainer = new ImageContainer();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}