using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestReward
    {
        public int Exp ;
        public int Gold ;
        public List<ItemReward> Items ;
        public List<ItemReward> CraftableItems ;
        public List<ItemReward> QuestItems ;

        public bool GivesReputation;
        public Reputation Reputation ;

        public bool UnlocksSkill;
        public string SkillID;

        public bool GivesTraitExp;
        public string TraitID;
        public int TraitExp;

        public bool ApplysStatusEffect;
        public string StatusEffectID;
    
        public QuestReward()
        {
            Exp = 0;
            Gold = 0;
            Items = new List<ItemReward>();
            CraftableItems = new List<ItemReward>();
            QuestItems = new List<ItemReward>();
            Reputation = new Reputation();
        }
    }

    public class ItemReward
    {
        public string ItemID;
        public int Amount;

        public ItemReward()
        {
            ItemID = "";
            Amount = 1;
        }
    }
}