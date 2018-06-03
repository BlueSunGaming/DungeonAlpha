using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class Location
    {
        public string ID;
        public string Name ;
        public string Description ;
        public string SceneName ;
        public ImageContainer ImageContainer;
        public string WorldAreaID;

        public bool UseCustomLocation;
        public RPGVector3 CustomSpawnLocation;

        public Location()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Location";
            Description = "";
            SceneName = "";
            ImageContainer = new ImageContainer();
            CustomSpawnLocation = new RPGVector3(0,0,0);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}