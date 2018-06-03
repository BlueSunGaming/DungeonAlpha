namespace LogicSpawn.RPGMaker.Core
{
    public class Material : Item, IStackable
    {
        public int CurrentStacks { get; set; }


        public Material()
        {
            CurrentStacks = 1;
            ItemType = ItemType.Material;
        }
    }
}