using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LogicSpawn.RPGMaker.Core
{
    public class DamageOutcome
    {
        public AttackOutcome AttackOutcome ;
        public Damage DamageToDeal;
        public DamageDealt DamageDealt;
        public bool KilledTarget;

        public DamageOutcome (Damage damageToDeal, AttackOutcome outcome)
        {
            DamageToDeal = damageToDeal;
            AttackOutcome = outcome;
        }

        public DamageOutcome (Damage damageToDeal, DamageDealt damageDealt, AttackOutcome outcome)
        {
            DamageToDeal = damageToDeal;
            DamageDealt = damageDealt;
            AttackOutcome = outcome;
        }

        public DamageOutcome(Damage damageToDeal, DamageDealt damageDealt, AttackOutcome outcome, bool killedTarget)
        {
            DamageToDeal = damageToDeal;
            DamageDealt = damageDealt;
            AttackOutcome = outcome;
            KilledTarget = killedTarget;
        }
    }

    public class DamageDealt
    {
        public int Physical;
        public Dictionary<string, int> Elementals;
        
        public int Total
        {
            get
            {
                if(Physical < 0)
                {
                    Physical = 0;
                }

                foreach (var element in Elementals)
                {
                    if(element.Value < 0)
                    {
                        Elementals[element.Key] = 0;
                    }
                }

                return Physical + Elementals.Values.Sum();
            }
        }

        public DamageDealt(int physical, Dictionary<string, int> elementals)
        {
            Physical = physical;
            Elementals = elementals;
        }

        public override string ToString()
        {
            var damageVals = string.Join(",",
                                               Enumerable.Concat(new[] {Physical.ToString(CultureInfo.InvariantCulture)},
                                                                 Elementals.Select(
                                                                     e => GetElementName(e.Key) + ": " + e.Value).
                                                                     ToArray()).ToArray());

            return string.Format("Physical: {0}", damageVals);
        }

        public void ApplyMultiplier(float multiplier)
        {
            Physical = (int)(Physical * multiplier);


            for (int index = 0; index < Elementals.Count; index++)
            {
                var item = Elementals.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;
                Elementals[itemKey] = (int)(itemValue * multiplier);
            }
        }

        private string GetElementName(string key)
        {
            var eleDef = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == key);
            return eleDef == null ? "" : eleDef.Name;
        }
    }
}