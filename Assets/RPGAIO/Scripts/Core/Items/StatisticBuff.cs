using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class StatisticBuff
    {
        public string StatisticID ;
        public float Amount ;

        public StatisticBuff()
        {
            
        }
        public StatisticBuff(string statName, float amount)
        {
            StatisticID = statName;
            Amount = amount;
        }
    }
}