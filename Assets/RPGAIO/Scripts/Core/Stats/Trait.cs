using LogicSpawn.RPGMaker.API;

namespace LogicSpawn.RPGMaker.Core
{
    public class Trait
    {
        public string ID;
        public long Exp;
        public string ExpDefinitionID;

        public int Level ;
        public long ExpToLevel
        {
            get { return Rm_RPGHandler.Instance.Experience.ExpToNextPlayerTraitLevel(ExpDefinitionID, Level); }
        }
        public int MaxLevel
        {
            get { return Rm_RPGHandler.Instance.Experience.Get(ExpDefinitionID).MaxLevel; }
        }

        public Trait()
        {
            Exp = 0;
        }

        public bool AddExp(long amount)
        {
            var currentLevel = Level;

            Exp += amount;

            if (Exp >= ExpToLevel && Level < MaxLevel)
            {
                Exp -= ExpToLevel;
                Level++;
            }

            var leveled = currentLevel < Level;

            if(leveled)
            {
                RPG.Popup.ShowInfo("Level " + Level + " " + RPG.Stats.GetTraitName(ID) +  " Achieved!");
            }

            return leveled;
        }

        public string GetName()
        {
            return RPG.Stats.GetTraitName(ID);
        }
    }
}