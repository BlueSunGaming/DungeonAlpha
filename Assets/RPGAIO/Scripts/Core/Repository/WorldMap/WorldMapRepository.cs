using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class WorldMapRepository
    {
        public List<Location> Locations ;
        public List<AreaTypeDefinition> AreaTypes ;

        public WorldMapRepository()
        {
            Locations = new List<Location>();
            AreaTypes = new List<AreaTypeDefinition>();
        }

        [JsonIgnore]
        public int TotalAreas
        {
            get { return Locations.Count; }
        }

        public List<Location> ExploredLocations(List<string> exploredAreasIDs )
        {
            var list = new List<Location>();
            for (int i = 0; i <Locations.Count; i++)
            {
                for (int j = 0; j < exploredAreasIDs.Count; j++)
                {
                    if (Locations[i].ID == exploredAreasIDs[j])
                    {
                        list.Add(Locations[i]);
                    }
                }
            }
            return list;
        }

        public Location GetLocation(string locationId)
        {
            return Locations.First(l => l.ID == locationId);
        }
    }

    public class AreaTypeDefinition
    {
        public string ID;
        public string Name;

        public AreaTypeDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "";
        }
    }
}