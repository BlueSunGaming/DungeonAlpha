using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class Consumable : Item, IStackable, IRequireLevel
    {
        public int CurrentStacks { get; set; }
        public int RequiredLevel { get; set; }

        public Restoration Restoration;
        public bool RestoresVital;


        public float Cooldown;
        public float CurrentCooldown;

        public bool RemovesStatusEffect;
        public string RemoveStatusEffectID ;

        public bool AddsStatusEffect;
        public string AddStatusEffectID;

        public Consumable()
        {
            RestoresVital = true;
            Restoration = new Restoration();
            ItemType = ItemType.Consumable;
            RarityID = "???";
            AddsStatusEffect = false;
            RemovesStatusEffect = false;
            AddStatusEffectID = "";
            RemoveStatusEffectID = "";
            CurrentStacks = 1;

        }

        public override string GetTooltipDescription()
        {
            var tooltip = "";

            if(RestoresVital)
            {
                var vitalName = RPG.Stats.GetVitalName(Restoration.VitalToRestoreID);
                var isFixed = Restoration.FixedRestore;
                if(Restoration.RestorationType == RestorationType.Instant)
                {
                    tooltip += "Restores " + (isFixed ? Restoration.AmountToRestore.ToString() : (Restoration.PercentToRestore * 100).ToString("N2") + "%") + " " + vitalName + "\n";    
                }
                else
                {
                    tooltip += "Restores " + (isFixed ? Restoration.AmountToRestore.ToString() : (Restoration.PercentToRestore * 100).ToString("N2") + "%") + " " + vitalName
                        + " every " + Restoration.SecBetweenRestore + "sec for " + Restoration.Duration + " seconds" + "\n";    
                }
            }
            
            if(AddsStatusEffect)
            {
                var statusEffectName = RPG.Stats.GetStatusEffectName(AddStatusEffectID);
                var statEffect = RPG.Stats.GetStatusEffectById(AddStatusEffectID);
                if(statEffect.Effect.HasDuration)
                {
                    tooltip += "Applies " + statusEffectName + " for " + statEffect.Effect.Duration + " seconds" + "\n";
                }
                else
                {
                    tooltip += "Applies " + statusEffectName + "\n";
                }
            }

            if(RemovesStatusEffect)
            {
                var statusEffectName = RPG.Stats.GetStatusEffectName(RemoveStatusEffectID);
                tooltip += "Removes " + statusEffectName + "\n";
            }

            if(Cooldown > 0)
            {
                tooltip += "Usable every " + Cooldown + " seconds" + "\n";
            }

            tooltip += "\n";

            var baseDescription =  base.GetTooltipDescription();

            return tooltip + baseDescription;
        }
        
    }
}