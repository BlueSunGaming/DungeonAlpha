using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class VitalBuff
    {
        public string VitalID ;
        public int Amount ;

        public VitalBuff()
        {
            
        }
        public VitalBuff(string baseVitalType, int amount)
        {
            VitalID = baseVitalType;
            Amount = amount;
        }
    }
}