using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class ItemGroundPrefab
    {
        public GroundPrefabType PrefabType;
        public ItemType ItemType;
        public string SlotVariable;
        public string PrefabPath;

        public ItemGroundPrefab()
        {
            PrefabType = GroundPrefabType.ItemType;
            ItemType = ItemType.Miscellaneous;
            SlotVariable = "";
            PrefabPath = "";
        }
    }
}