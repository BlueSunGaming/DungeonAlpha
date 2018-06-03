using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    //TODO:
    public class VitalHandler
    {
        [JsonIgnore]
        public BaseCharacter Character ;
        [JsonIgnore]
        public Vital Health
        {
            get { return Character.Vitals.First(v => v.IsHealth); }
        }

        private GameObject _floatingCombatText = Resources.Load("RPGMakerAssets/PrefabGen/FloatingCombatText") as GameObject;

        public VitalHandler(BaseCharacter character)
        {
            Character = character;
        }

        public DamageOutcome TakeDamage(BaseCharacter attacker, Damage damage, bool evaluateDamageToDeal = true)
        {
            if (attacker != null && attacker.IsFriendly(Character))
            {
                return new DamageOutcome(new Damage() { MinDamage = 0, MaxDamage = 0, ElementalDamages = new List<ElementalDamage>() }, new DamageDealt(0, new Dictionary<string, int>()), AttackOutcome.FriendlyFire, false);    
            }

            if(Character.ImmuneTo(damage.SkillMetaID))
            {
                return new DamageOutcome(new Damage() { MinDamage = 0, MaxDamage = 0, ElementalDamages = new List<ElementalDamage>() }, new DamageDealt(0, new Dictionary<string, int>()), AttackOutcome.Immune, false);    
            }

            Damage damageObj = null;
            DamageDealt damageToDeal;
            DamageOutcome result = null;
            DamageDealt damageDealt = null;
            result = CombatCalcEvaluator.Instance.Evaluate(attacker, Character, damage);
            var killedTarget = false;
            if(result.AttackOutcome == AttackOutcome.Success || result.AttackOutcome == AttackOutcome.Critical)
            {
                damageObj = result.DamageToDeal;
                damageToDeal = result.DamageDealt;

                if (!string.IsNullOrEmpty(damageObj.SkillMetaID))
                {
                    var susceptibility = Character.AllSusceptibilites.Where(s => s.ID == damageObj.SkillMetaID).Sum(s => s.AdditionalDamage);
                    var multiplier = Mathf.Max(0, 1 + susceptibility);
                    damageToDeal.ApplyMultiplier(multiplier);
                }

                if (Character is PlayerCharacter)
                {
                    var difficulty = GetObject.PlayerSave.Difficulty;
                    var diffMultiplier = Rm_RPGHandler.Instance.Player.Difficulties.FirstOrDefault(d => d.ID == difficulty);
                    if(diffMultiplier != null)
                    {
                        damageToDeal.ApplyMultiplier(diffMultiplier.DamageMultiplier);
                    }
                }

                damageDealt = damageToDeal;
                
                Health.CurrentValue -= damageDealt.Total;
                killedTarget = false;

                if (Health.CurrentValue <= 0)
                {
                    Health.CurrentValue = 0;
                    Character.Alive = false;
                    killedTarget = true;
                }
            }
            

            //todo:enemies to player only
            if(Rm_RPGHandler.Instance.Combat.EnableFloatingCombatText && !(Character.CharacterMono is PlayerMono))
            {
                var pos = Character.CharacterMono.transform;

                //Default damage
                if (result.AttackOutcome == AttackOutcome.Success || result.AttackOutcome == AttackOutcome.Critical)
                {
                    if(damageDealt.Physical > 0)
                    {
                        var go = Object.Instantiate(_floatingCombatText, pos.position + pos.up, Quaternion.identity) as GameObject;
                        go.GetComponent<FloatingCombatText>().SetUp(damageDealt.Physical.ToString());
                    }
                    

                    var distanceBetweenValuesconst = Rm_RPGHandler.Instance.Combat.FloatDistBetweenVals;
                    var distanceBetweenValues = distanceBetweenValuesconst;
                    foreach (var dmg in damageDealt.Elementals)
                    {
                        var eleDef = GetElement(dmg.Key);
                        if (eleDef != null && dmg.Value > 0)
                        {
                            var eleGo = Object.Instantiate(_floatingCombatText, pos.position + pos.up + new Vector3(0, distanceBetweenValues, 0), Quaternion.identity) as GameObject;
                            eleGo.GetComponent<FloatingCombatText>().SetUp(string.Format("<color={0}>{1}</color>", eleDef.Color.ToString(), dmg.Value));
                            distanceBetweenValues += distanceBetweenValuesconst;
                        }
                    }
                }
                else
                {
                    if(result.AttackOutcome != AttackOutcome.FriendlyFire && result.AttackOutcome != AttackOutcome.Critical &&
                        result.AttackOutcome != AttackOutcome.Success)
                    {
                        var go = Object.Instantiate(_floatingCombatText, pos.position + pos.up, Quaternion.identity) as GameObject;
                        go.GetComponent<FloatingCombatText>().SetUp(result.AttackOutcome.ToString());
                    }
                    
                }

                //add delay and iterate through damages to deal > 0 that are elemental, adding more delay for each one displayed
                //add random X/Y pos on the floating text

            }
            

            return new DamageOutcome(damageObj, damageDealt, AttackOutcome.Success, killedTarget);    
        }

        public void IncreaseHealth(int value)
        {
            Health.CurrentValue += value;
        }

        public void ReduceHealth(int value)
        {
            Health.CurrentValue -= value;
        }

        //todo:
        public DamageOutcome TakeDoTDamage(BaseCharacter attacker, DamageOverTime dot)
        {
            var dotDamage = GeneralMethods.CopyObject(dot);
            dotDamage.DamagePerTick.SkillMetaID = dotDamage.SkillMetaID;

            if(attacker != null)
            {
                var outcome =  TakeDamage(attacker, dotDamage.DamagePerTick);
                return outcome;
            }

            //todo: test a dot with no attacker
            var outcomex =  TakeDamage(null, dotDamage.DamagePerTick, false);
            Debug.Log(Character.Name + " took " + outcomex.DamageDealt + " from " + dot.DoTName);
            return outcomex;
        }

        public DamageOutcome TakeFallDamage(float timeInAir)
        {
            var damageToTake = Health.MaxValue * 0.1f * timeInAir;  //todo: Rm_RPGHandler.Instance.Combat.FallDamagePerSec;
            var damage = new Damage();
            damage.MinDamage = damage.MaxDamage = (int)damageToTake;
            var outcome = TakeDamage(null, damage, false);
            Debug.Log(Character.Name + " lost " + outcome.DamageDealt.Total + "HP from fall damage");
            return outcome;
        }

        private ElementalDamageDefinition GetElement(string key)
        {
            var eleDef = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == key);
            return eleDef;
        }
    }
}