namespace LogicSpawn.RPGMaker.Core
{
    public class QuestItem : Item, IStackable
    {
        public int CurrentStacks { get; set; }


        public QuestItem()
        {
            ItemType = ItemType.Quest_Item;
            CanBeDropped = false;
            CurrentStacks = 1;
            RarityID = "QuestItem";
        }
    }
}