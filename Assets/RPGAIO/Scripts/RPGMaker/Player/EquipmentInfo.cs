using System.Collections.Generic;

namespace LogicSpawn.RPGMaker
{
    public class EquipmentInfo
    {
        public List<DynamicEquipmentDefinition> Definitions;

        public EquipmentInfo()
        {
            Definitions = new List<DynamicEquipmentDefinition>();
        }
    }

    public class DynamicEquipmentDefinition
    {
        public string NameOfTransform;
        public string RequiredEquippedItemId;
        public bool SpecificSlot;
        public string SlotId;
        public bool OnlyWeaponSlot;
        public bool OffHandOnly; //todo:remove this


        public DynamicEquipmentDefinition()
        {
            NameOfTransform = "";
            RequiredEquippedItemId = "";
            SlotId = "";
            SpecificSlot = false;
            OnlyWeaponSlot = false;
            OffHandOnly = false;
        }

        public DynamicEquipmentDefinition(string transformName)
        {
            NameOfTransform = transformName;
            RequiredEquippedItemId = "";
            SlotId = "";
            SpecificSlot = false;
            OnlyWeaponSlot = false;
            OffHandOnly = false;
        }
    }
}