using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    //TODO: Take into account damageVariance etc
    public class Damage
    {
        public int MinDamage ;
        public int MaxDamage ;
        public float CriticalChance;

        public List<ElementalDamage> ElementalDamages ;
        public string SkillMetaID;
        public bool IsCritical;

        public Damage()
        {
            SkillMetaID = null;
            MinDamage = 0;
            MaxDamage = 0;
            CriticalChance = 0.0f;
            ElementalDamages = new List<ElementalDamage>();
            var asvt = Rm_RPGHandler.Instance.ASVT;
            for (int i = 0; i < asvt.ElementalDamageDefinitions.Count; i++)
            {
                var elementalDamageToAdd = new ElementalDamage()
                                               {
                                                   MinDamage = 0,
                                                   MaxDamage = 0,
                                                   ElementID = asvt.ElementalDamageDefinitions[i].ID
                                               };
                ElementalDamages.Add(elementalDamageToAdd);
            }
        }

        public Damage(Damage damage)
        {
            SkillMetaID = damage.SkillMetaID;
            MinDamage = damage.MinDamage;
            MaxDamage = damage.MaxDamage;
            CriticalChance = damage.CriticalChance;
            ElementalDamages = GeneralMethods.CopyObject(damage.ElementalDamages);
        }

        public void ApplyMultiplier(float multiplier)
        {
            MinDamage = (int)(MinDamage * multiplier);
            MaxDamage = (int)(MaxDamage * multiplier);

            foreach(var eleDmg in ElementalDamages)
            {
                eleDmg.MinDamage = (int)(eleDmg.MinDamage * multiplier);
                eleDmg.MaxDamage = (int)(eleDmg.MaxDamage * multiplier);
            }
        }

        public void AddDamage(Damage damage)
        {
            MinDamage += damage.MinDamage;
            MaxDamage += damage.MaxDamage;

            for (int index = 0; index < ElementalDamages.Count; index++)
            {
                var eleDmg = ElementalDamages[index];
                eleDmg.MinDamage += damage.ElementalDamages[index].MinDamage;
                eleDmg.MaxDamage += damage.ElementalDamages[index].MaxDamage;
            }
        }

        [JsonIgnore]
        public int SumOfElementalDamage
        {
            get
            {
                var val = ElementalDamages.Sum(e => e.MaxDamage);
                return val > 0 ? val : 0;
            }
        }

        [JsonIgnore]
        public int SumOfElementalMinDamage
        {
            get
            {
                var val = ElementalDamages.Sum(e => e.MinDamage);
                return val > 0 ? val : 0;
            }
        }

        [JsonIgnore]
        public int MinTotal
        {
            get { return MinDamage + SumOfElementalMinDamage > 0 ? MinDamage + SumOfElementalMinDamage : 0; }
        }
        [JsonIgnore]
        public int MaxTotal
        {
            get { return MaxDamage + SumOfElementalDamage > 0 ? MaxDamage + SumOfElementalDamage : 0; }
        }

        [JsonIgnore]
        public DamageDealt DamageDealt
        {
            get
            {
                var physical = Random.Range(MinDamage, MaxDamage + 1);
                var elementals = new Dictionary<string, int>();
                foreach(var element in ElementalDamages)
                {
                    var elementDamage = Random.Range(element.MinDamage, element.MaxDamage + 1);
                    elementals.Add(element.ElementID, elementDamage);
                }

                if(physical < 0)
                {
                    physical = 0;
                }

                foreach(var e in elementals.Where(e => e.Value < 0))
                {
                    elementals[e.Key] = 0;
                }

                return new DamageDealt(physical, elementals);
            }
        }


        public override string ToString()
        {
            return string.Format("Damage: Physical : {0} | Elemental: {1} | Total: {3} | CritChance: {2}",
                          MinDamage + "-" + MaxDamage, SumOfElementalMinDamage + "-" + SumOfElementalDamage, CriticalChance.ToString("N2"), MinTotal + "-" + MaxTotal );
        }
    }

    public class ElementalDamage
    {
        public string ElementID;
        public int MinDamage;
        public int MaxDamage;
    }
}