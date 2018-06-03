using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class MetaDataValue
    {
        public string ID;
        public string Name;
        public string Description;
        public ImageContainer Image;

        public MetaDataValue()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Metadata Value";
            Description = "";
            Image = new ImageContainer();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}