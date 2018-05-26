namespace LogicSpawn.RPGMaker
{
    public class Rm_ExpPoints
    {
        public int Level;
        public int ExpAtLevel;

        public Rm_ExpPoints(int levelToAdd, int expAtLevel)
        {
            Level = levelToAdd;
            ExpAtLevel = expAtLevel;
        }
    }
}