using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class StatusEffectRepository
    {
        public List<StatusEffect> AllStatusEffects;

        public StatusEffectRepository()
        {
            AllStatusEffects = new List<StatusEffect>();
        }

        public StatusEffect Get(string statusID)
        {
            return GeneralMethods.CopyObject(AllStatusEffects.First(i => i.ID == statusID));
        }
    }
}