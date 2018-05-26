using System;

namespace LogicSpawn.RPGMaker
{
    public class SlotDefinition
    {

        public string ID;
        public string Name;
        public string PrefabPath;

        public SlotDefinition()
        {
            ID = Guid.NewGuid().ToString();
        }
    }
}