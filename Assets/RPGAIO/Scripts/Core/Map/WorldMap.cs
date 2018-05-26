using System.Collections.Generic;
using System.Linq;

namespace LogicSpawn.RPGMaker.Core
{
    public class WorldMap
    {
        public List<string> VisitedLocations;
        public string CurrentWorldAreaID;
        public string CurrentLocationID;

        public WorldMap()
        {
            VisitedLocations = new List<string>();
            CurrentWorldAreaID = null;
            CurrentLocationID = null;
        }

        public bool VisitedLocation(string worldAreaId, string locationId)
        {
            var location = Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(w =>w.ID == worldAreaId).Locations.First(l => l.ID == locationId);
            return VisitedLocation(location);
        }

        public bool VisitedLocation(Location location)
        {
            return VisitedLocations.FirstOrDefault(s => s == location.ID) != null;
        }

        public void AddVisitedLocation(string id)
        {
            if(VisitedLocations.FirstOrDefault(s => s == id) == null)
            {
                VisitedLocations.Add(id);
            }
        }
    }
}