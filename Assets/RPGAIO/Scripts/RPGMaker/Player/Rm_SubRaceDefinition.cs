using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rm_SubRaceDefinition
    {
        public string ID;
        public string Name;
        public string ApplicableRaceID;
        public string Description;
        public ImageContainer Image;

        public Rm_SubRaceDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Sub-Race";
            ApplicableRaceID = "";
            Description = "";
            Image = new ImageContainer();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}