using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class AttributeBuff //TODO: attributes, stats, vitals defined by user
    {
        public string AttributeID ;
        public int Amount ;

        public AttributeBuff()
        {
            
        }
        public AttributeBuff(string name, int amount)
        {
            AttributeID = name;
            Amount = amount;
        }
    }
}