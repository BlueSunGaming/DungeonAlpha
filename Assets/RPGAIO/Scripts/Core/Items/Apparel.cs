using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class Apparel : BuffItem
    {
        public string apparelSlotID ;

        [JsonIgnore]
        public string ApparelSlotName
        {
            get
            {
                return Rm_RPGHandler.Instance.Items.ApparelSlots.First(s => s.ID == apparelSlotID).Name;
            }
        }
        
        public Apparel()
        {
            apparelSlotID = "";
            ItemType = ItemType.Apparel;
        }
    }
}