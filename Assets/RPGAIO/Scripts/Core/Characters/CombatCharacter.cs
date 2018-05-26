using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class CombatCharacter : BaseCharacter
    {
        public bool IsAggressive ;
        public bool OverrideAggroRadius ;
        public float OverrideAggroRadiusValue;

        public string MonsterTypeID ;
        public List<LootDefinition> GuaranteedLoot ;
        public int MaxItemsFromLootTable;
        public List<LootOptions> LootTables ;
        public List<Rm_NPCSkill> EnemySkills;
        public string ID;
        public TauntHandler TauntHandler;
        public ProgressionGain ProgressionGain;

        //rename to base damage
        public Damage NpcDamage ;

        public string PrefabReplacementOnDeath;


        public string AutoAttackPrefabPath;
        public string AutoAttackImpactPrefabPath;
        public AudioContainer ProjectileTravelSound;
        public AudioContainer AutoAttackImpactSound;
        public float ProjectileSpeed;

        public bool DropsGold;
        public int MinGoldDrop;
        public int MaxGoldDrop;
        public float GoldDropChance;

        public string ReputationId;

        public int AttackCounter;
        public bool AttackedOnceByPlayer;
        public bool RetreatsWhenLow;
        public string CharPrefabPath;

        public CombatCharacter()
        {
            ID = Guid.NewGuid().ToString();
            Name = "Enemy";
            ProgressionGain = new ProgressionGain();
            MaxItemsFromLootTable = 1;
            ReputationId = "Core_EnemyReputation";
            PrefabReplacementOnDeath = "";
            IsAggressive = false;
            CharacterType = CharacterType.Enemy;
            GuaranteedLoot = new List<LootDefinition>();
            LootTables = new List<LootOptions>();
            EnemySkills = new List<Rm_NPCSkill>();
            NpcDamage = new Damage(){MaxDamage = 1, MinDamage = 1};
            AttackCounter = 1;
            RetreatsWhenLow = false;
            ProjectileTravelSound = new AudioContainer();
            AutoAttackImpactSound = new AudioContainer();
            ProjectileSpeed = 10f;
            TauntHandler = new TauntHandler();
            CharPrefabPath = "";

            OverrideAggroRadius = false;
            OverrideAggroRadiusValue = 25f;
        }

        public void CCInit()
        {
            TauntHandler = new TauntHandler();
            FullUpdateStats();

            foreach (var vital in Vitals)
            {
                vital.CurrentValue = vital.MaxValue;
                if(vital.AlwaysStartsAtZero)
                {
                    vital.CurrentValue = 0;
                }
            }

            foreach (var skill in EnemySkills)
            {
                skill.SkillRef = Rm_RPGHandler.Instance.Repositories.Skills.Get(skill.SkillID);
                skill.SkillRef.CurrentRank = skill.Rank;

                if(skill.CastType == Rm_EnemySkillCastType.EveryNthSeconds)
                {
                    skill.NthSecondsTimer = 0;
                }
            }
        }

        public override string ToString()
        {
            return Name + " [Lv" + Level + "]";
        }
    }

    public class TauntHandler
    {
        [JsonIgnore]
        public Dictionary<BaseCharacterMono, int> Tracker;

        public float TimeDelta;

        [JsonIgnore]
        public BaseCharacterMono Target;

        public TauntHandler()
        {
            Tracker = new Dictionary<BaseCharacterMono, int>();
            TimeDelta = 100;
            Target = null;
        }

        public BaseCharacterMono GetTarget()
        {
            if (TimeDelta < 2.0f)
            {
                TimeDelta += Time.deltaTime;
            }

            if (TimeDelta > 2.0F)
            {
                var targets = Tracker.Where(t => t.Key != null).ToList();
                
                if (targets.Any())
                {
                    targets = targets.Where(p => p.Key.Character.Alive).ToList();
                    if(!targets.Any())
                    {
                        return null;
                    }
                    Target = targets.Aggregate((i1, i2) => i1.Value > i2.Value ? i1 : i2).Key;
                    TimeDelta = 0;
                    return Target;
                }
            }
            else
            {
                return Target;
            }

            return null;
        }
    }
}