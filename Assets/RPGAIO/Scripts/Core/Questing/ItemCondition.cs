namespace LogicSpawn.RPGMaker.Core
{
    public class ItemCondition : QuestCondition
    {
        public ItemConditionType ItemType;
        public string ItemToCollectID ;

        public bool NPCDropsItem; 
        public string CombatantIDThatDropsItem;
        public int NumberToObtain ;
        public int NumberObtained ;
        

        public ItemCondition()
        {
            ItemType = ItemConditionType.QuestItem;
            ItemToCollectID = "";
            CombatantIDThatDropsItem = "";
            NumberToObtain = 1;

            ConditionType = ConditionType.Item;
        }
    }

    public enum ItemConditionType
    {
        QuestItem,
        Item,
        CraftItem
    }
}