using System;

namespace LogicSpawn.RPGMaker
{
    public class RarityDefinition
    {

        public string ID;
        public string Name;
        public Rm_UnityColors Color;

        public RarityDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Rarity";
            Color = Rm_UnityColors.None;
        }
    }
}