using System;

namespace LogicSpawn.RPGMaker
{
    public class ElementalDamageDefinition
    {
        public string ID;
        public string Name;
        public Rm_UnityColors Color;

        public ElementalDamageDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Damage Type";
            Color = Rm_UnityColors.White;
        }
    }
}