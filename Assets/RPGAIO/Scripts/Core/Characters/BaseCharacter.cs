using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class BaseCharacter
    {
        [JsonIgnore] public const float ImpactPrefabTime = 0.5F; //note: merge with RPGCombat , into Rm_RPGHandler

        public string Name ;
        public int Level ;
        public List<Attribute> Attributes ;
        public List<Vital> Vitals ;
        public List<Statistic> Stats;

        

        public bool Alive;
        public bool Stunned;
        public bool StunFreeze;
        public bool Rooted;
        public bool Retreating;
        public bool Silenced;

        public RPGAnimationType AnimationType;
        public LegacyAnimation LegacyAnimations ;

        [JsonIgnore]
        public VitalHandler VitalHandler ;
        public AttackStyle AttackStyle ;

        [JsonIgnore]
        public float AttackRange
        {
            get { return GetStatByID("Attack Range").TotalValue; }
        }

        [JsonIgnore]
        public float AttackSpeed
        {
            get { return GetStatByID("Attack Speed").TotalValue; }
        }

        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }

        [JsonIgnore]
        public Damage Damage
        {
            get
            {
                if(CharacterType == CharacterType.Player)
                {
                    var player = (PlayerCharacter) this;

                    if(player.Equipment.Unarmed)
                    {
                        var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                        var unarmedDamage = new Damage();
                        unarmedDamage.MinDamage = unarmedDamage.MaxDamage = classDef.UnarmedAttackDamage;
                        return unarmedDamage;
                    }

                    var damage = new Damage();
                    if (player.Equipment.EquippedWeapon != null)
                    {
                        var weapon = (Weapon)player.Equipment.EquippedWeapon;
                        damage.AddDamage(GeneralMethods.CopyObject(weapon.Damage));
                    }

                    if (Rm_RPGHandler.Instance.Items.EnableOffHandSlot && player.Equipment.EquippedOffHand != null)
                    {
                        var offHandWep = player.Equipment.EquippedOffHand as Weapon;
                        if(offHandWep != null)
                        {
                            var offHandDamage = GeneralMethods.CopyObject(offHandWep.Damage);
                            offHandDamage.ApplyMultiplier(Rm_RPGHandler.Instance.Items.DualWieldRatio);
                            damage.AddDamage(offHandDamage);
                        }
                    }

                    return damage;
                }
                else
                {
                    var cc = (CombatCharacter)this;
                    return GeneralMethods.CopyObject(cc.NpcDamage);
                }
            }
        }

        [JsonIgnore] protected Damage _damageDealable;
        [JsonIgnore]
        public Damage DamageDealable
        {
            get { return _damageDealable; }
        }

        public string ImagePath;

        //Duration based effects
        public List<AuraEffect> AuraEffects;
        public List<FriendlyAura> FriendlyAuras;
        public List<TimedPassiveEffect> TimedPassiveEffects;
        public List<StatusEffect> StatusEffects ;
        public List<DamageOverTime> CurrentDoTs ;
        public List<Restoration> Restorations ;
        public List<SkillImmunity> SkillMetaImmunitiesID;
        public List<SkillMetaSusceptibility> SkillMetaSusceptibilities;
        public List<VitalRegenBonus> VitalRegenBonuses;
        public List<ReduceStatusDuration> StatusReductions;
        public List<Rm_ProcEffect> ProcEffects;

        [JsonIgnore]
        public List<SkillMetaSusceptibility>  AllSusceptibilites
        {
            get
            {
                if(CharacterType != CharacterType.Player)
                    return SkillMetaSusceptibilities;

                var playerChar = this as PlayerCharacter;
                var allequips = playerChar.Equipment.AllEquippedItems;
                var sus = new List<SkillMetaSusceptibility>();
                foreach(var item in allequips.Select(i => i as BuffItem))
                {
                    if(item != null && item.SkillMetaSusceptibilities.Any())
                    {
                        sus.AddRange(item.SkillMetaSusceptibilities);
                        if(item.SocketHolder.Sockets.Any())
                        {
                            foreach(var s in item.SocketHolder.Sockets)
                            {
                                if(s.SkillMetaSusceptibilities.Any())
                                {
                                    sus.AddRange(s.SkillMetaSusceptibilities);
                                }
                            }
                        }
                    }
                }

                return SkillMetaSusceptibilities.Concat(sus).ToList();
            }
        }

        [JsonIgnore]
        public List<SkillImmunity> AllImmunities
        {
            get
            {
                if (CharacterType != CharacterType.Player)
                    return SkillMetaImmunitiesID;

                var playerChar = this as PlayerCharacter;
                var allequips = playerChar.Equipment.AllEquippedItems;
                var immun = new List<SkillImmunity>();
                foreach (var item in allequips.Select(i => i as BuffItem))
                {
                    if (item != null && item.SkillMetaImmunitiesID.Any())
                    {
                        immun.AddRange(item.SkillMetaImmunitiesID);
                        if (item.SocketHolder.Sockets.Any())
                        {
                            foreach (var s in item.SocketHolder.Sockets)
                            {
                                if (s.SkillMetaImmunitiesID.Any())
                                {
                                    immun.AddRange(s.SkillMetaImmunitiesID);
                                }
                            }
                        }
                    }
                }

                return SkillMetaImmunitiesID.Concat(immun).ToList();
            }
        }

        public CharacterType CharacterType ;
        
        [JsonIgnore]
        public BaseCharacterMono CharacterMono;

        public BaseCharacter()
        {
            Name = "";
            Level = 1;
            Attributes = new List<Attribute>();
            Vitals = new List<Vital>();
            Stats = new List<Statistic>();
            CharacterType = CharacterType.Enemy;

            AttackStyle = AttackStyle.Melee;
            VitalHandler = new VitalHandler(this);

            AnimationType = RPGAnimationType.Legacy;
            LegacyAnimations = new LegacyAnimation();

            Alive = true;
            Stunned = Silenced = StunFreeze = Retreating = false;
            ImagePath = "";
            AuraEffects = new List<AuraEffect>();
            FriendlyAuras = new List<FriendlyAura>();
            TimedPassiveEffects = new List<TimedPassiveEffect>();
            StatusEffects = new List<StatusEffect>();
            CurrentDoTs = new List<DamageOverTime>();
            SkillMetaImmunitiesID = new List<SkillImmunity>();
            SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
            VitalRegenBonuses = new List<VitalRegenBonus>();
            Restorations = new List<Restoration>();
            StatusReductions = new List<ReduceStatusDuration>();
            ProcEffects = new List<Rm_ProcEffect>();
        }

        public virtual void Init()
        {
            SetupAttributes();
            SetupVitals();
            SetupStats();
        }

        public Attribute GetAttributeByID(string att)
        {
            return Attributes.FirstOrDefault(a => a.ID == att);
        }

        public Attribute GetAttribute(string att)
        {
            var id = RPG.Stats.GetAttributeId(att);
            return id != null ? GetAttributeByID(id) : null;
        }

        public Vital GetVitalByID(string vit)
        {
            return Vitals.FirstOrDefault(v => v.ID == vit);
        }

        public Vital GetHealthVital()
        {
            return Vitals.First(v => v.IsHealth);
        }

        public Vital GetVital(string vit)
        {
            var id = RPG.Stats.GetVitalId(vit);
            return id != null ? GetVitalByID(id) : null;
        }

        public Statistic GetStatByID(string stat)
        {
            return Stats.FirstOrDefault(s => s.ID == stat);
        }

        public Statistic GetStat(string stat)
        {
            var id =  RPG.Stats.GetStatisticId(stat);
            return id != null ? GetStatByID(id) : null;
        }

        private void SetupAttributes(){
            for (var cnt = 0; cnt < Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count; cnt++)
            {
                var attrDef = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions[cnt];

                var attributeToAdd = new Attribute
                                      {
                                          ID = attrDef.ID,
                                          BaseValue = attrDef.DefaultValue,
                                          HasMaxValue = attrDef.HasMaxValue,
                                          MaxValue = attrDef.MaxValue,
                                          Color = attrDef.Color
                                      };

                if (CharacterType == CharacterType.Player)
                {
                    var player = this as PlayerCharacter;
                    var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                    var baseVal = classDef.StartingAttributes.First(s => s.AsvtID == attrDef.ID).Amount;
                    attributeToAdd.BaseValue = baseVal;
                }

                Attributes.Add(attributeToAdd);
            }
        }

        private void SetupVitals()
        {
            for (var cnt = 0; cnt < Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count; cnt++)
            {
                var vitDef = Rm_RPGHandler.Instance.ASVT.VitalDefinitions[cnt];
                var vitalToAdd = new Vital()
                {
                    ID = vitDef.ID,
                    BaseValue = vitDef.DefaultValue,
                    IsHealth = vitDef.IsHealth,
                    HasUpperLimit = vitDef.HasUpperLimit,
                    AlwaysStartsAtZero = vitDef.AlwaysStartsAtZero,
                    RegenWhileInCombat = vitDef.RegenWhileInCombat,
                    UpperLimit = vitDef.UpperLimit,
                    Color = vitDef.Color,
                    RegenPercentValue = vitDef.BaseRegenPercentValue,

                    ReduceHealthIfZero = vitDef.ReduceHealthIfZero,
                    ReductionIntervalSeconds = vitDef.ReductionIntervalSeconds,
                    ReduceByFixedAmount = vitDef.ReduceByFixedAmount,
                    ReductionFixedAmount = vitDef.ReductionFixedAmount,
                    ReductionPercentageAmount = vitDef.ReductionPercentageAmount
                };


                if (CharacterType == CharacterType.Player)
                {
                    var player = this as PlayerCharacter;
                    var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                    var baseVal = classDef.StartingVitals.First(s => s.AsvtID == vitDef.ID).Amount;
                    vitalToAdd.BaseValue = baseVal;
                }

                Vitals.Add(vitalToAdd);
            }
        }

        private void SetupStats()
        {
            for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count; i++)
            {
                var statDef = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions[i];

                var customStat = new Statistic
                                     {
                                         Color = statDef.Color,
                                         MaxValue = statDef.MaxValue,
                                         HasMaxValue = statDef.HasMaxValue,
                                         BaseValue = statDef.DefaultValue,
                                         StatisticType = statDef.StatisticType,
                                         ID = statDef.ID
                                     };

                if (CharacterType == CharacterType.Player)
                {
                    var player = this as PlayerCharacter;
                    var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                    var baseVal = classDef.StartingStats.First(s => s.AsvtID == statDef.ID).Amount;
                    customStat.BaseValue = baseVal;
                }

                Stats.Add(customStat);
            }
        }

        //todo: rethink these, we should add skill or talent, so we can first check if we are immune/susceptible to it
        //in which case we can pre-multiply the damage or just not apply the skill and return "Immune"

        public bool IsFriendly(BaseCharacter target)
        {
            if (target == null) return true;

            //pet can't be attacked if it's idle only
            var targetIsPet = target.CharacterMono.GetComponent<PetMono>();
            if (targetIsPet != null && targetIsPet.PetData.CurrentBehaviour == PetBehaviour.PetOnly) return true;

            //pet can't attack player
            if (CharacterMono == null) return true;

            var iAmPet = CharacterMono.GetComponent<PetMono>();
            if (iAmPet != null && target is PlayerCharacter) return true;

            if(iAmPet)
            {
                return GetObject.PlayerCharacter.IsFriendly(target);
            }

            if(targetIsPet)
            {
                return IsFriendly(GetObject.PlayerCharacter);
            }

            var allReps = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations;
            var players = new PlayerCharacter[2];
            var combatChars = new CombatCharacter[2];

            switch (CharacterType)
            {
                case CharacterType.Player:
                    players[0] = (PlayerCharacter)this;
                    break;
                case CharacterType.NPC:
                case CharacterType.Enemy:
                    combatChars[0] = (CombatCharacter)this;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (target.CharacterType)
            {
                case CharacterType.Player:
                    players[players[0] == null ? 0 : 1] = (PlayerCharacter)target;
                    break;
                case CharacterType.NPC:
                case CharacterType.Enemy:
                    combatChars[combatChars[0] == null ? 0 : 1] = (CombatCharacter)target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (players.Count(p => p != null) == 2)
            {
                return true;
            }
            else if (players.Count(p => p != null) == 1 && combatChars.Count(p => p != null) == 1)
            {
                var targetChar = combatChars[0];
                var rep = allReps.FirstOrDefault(r => r.ID == targetChar.ReputationId);
                var playerRepVal = GetObject.PlayerSave.QuestLog.AllReputations.FirstOrDefault(r => r.ReputationID == targetChar.ReputationId);
                if (rep != null && playerRepVal != null)
                {
                    return !rep.AttackIfBelowReputation || playerRepVal.Value >= rep.BelowReputationValue;
                }
                else
                {
                    Debug.LogError("Could not find reputation for  " + targetChar.Name + "[ RepId: " + targetChar.ReputationId + "]");
                }
            }
            else
            {
                var myrep = allReps.FirstOrDefault(r => r.ID == combatChars[0].ReputationId);
                var enemyrep = allReps.FirstOrDefault(r => r.ID == combatChars[1].ReputationId);


                if (myrep == enemyrep)
                {
                    return true;
                }

                //EE
                if (myrep.EnemyFactions.First(e => e.ID == enemyrep.ID).IsTrue &&
                       enemyrep.EnemyFactions.First(e => e.ID == myrep.ID).IsTrue)
                {
                    return false;
                }
                else if (myrep.EnemyFactions.First(e => e.ID == enemyrep.ID).IsTrue || enemyrep.EnemyFactions.First(e => e.ID == myrep.ID).IsTrue)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public bool ImmuneTo(Skill skill)
        {
            return skill.HasSkillMeta && AllImmunities.FirstOrDefault(s => s.ID == skill.SkillMetaID) != null;
        }
        public bool ImmuneTo(string metaId)
        {
            return AllImmunities.FirstOrDefault(s => s.ID == metaId) != null;
        }

        public bool ToggleAura(AuraSkill auraskill, bool onlyTurnOn = false)
        {
            var aura = GeneralMethods.CopyObject(auraskill.AuraEffect);
            var foundAura = AuraEffects.FirstOrDefault(a => a.SkillId == aura.SkillId);
            var auraIsActive = foundAura != null;
            bool active;
            if (!auraIsActive)
            {
                AuraEffects.Add(aura);
                var foundFriendlyAura = FriendlyAuras.FirstOrDefault(a => a.AuraEffect.SkillId == aura.SkillId);
                if(foundFriendlyAura != null)
                {
                    RemoveFriendlyAura(foundFriendlyAura);
                }

                CustomVariableHandler.HandleCvarSetters(aura.PassiveEffect.CustomVariableSetters);
                active = true;

                DestroyHelper destroyCondition;
                AddEffectPrefabs(aura.PassiveEffect, out destroyCondition);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.AuraEffectNotActive, CharacterMono.Controller, aura.SkillId);
                }
            }
            else
            {
                if(!onlyTurnOn)
                {
                    RemoveAuraEffect(aura);
                    active = false;    
                }
                else
                {
                    active = true;
                }
            }

            FullUpdateStats();

            return active;
        }

        public bool AddFriendlyAura(BaseCharacterMono source, AuraEffect auraEffect)
        {
            var aura = GeneralMethods.CopyObject(auraEffect);
            var foundFriendlyAura = FriendlyAuras.FirstOrDefault(a => a.AuraEffect.SkillId == aura.SkillId);
            var foundSelfAura = AuraEffects.FirstOrDefault(a => a.SkillId == aura.SkillId);
            var canAddAura = foundFriendlyAura == null && foundSelfAura == null;

            bool active;
            if (canAddAura)
            {
                var friendlyAura = new FriendlyAura
                                       {
                                           SourceCharacter = source,
                                           AuraEffect = aura
                                       };
                FriendlyAuras.Add(friendlyAura);
                CustomVariableHandler.HandleCvarSetters(aura.PassiveEffect.CustomVariableSetters);
                active = true;

                DestroyHelper destroyCondition;
                AddEffectPrefabs(aura.PassiveEffect, out destroyCondition, false);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.FriendlyAuraNotAvailable, CharacterMono.Controller, aura.SkillId);
                }
                FullUpdateStats();
            }
            else
            {
                active = true;
            }


            return active;
        }

        public void AddVitalRegenBonus(VitalRegenBonus vitalRegenBonus)
        {
            VitalRegenBonuses.Add(vitalRegenBonus);
            FullUpdateStats();
        }

        public void AddStatusEffect(string statusEffectId)
        {
            var statusEffectToAdd = Rm_RPGHandler.Instance.Repositories.StatusEffects.Get(statusEffectId);
            if(statusEffectToAdd != null)
            {
                AddStatusEffect(statusEffectToAdd);
            }
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            //todo: if in status reductions we have this status, reduce status.duration by amt if status has duration
            StatusEffects.Add(statusEffect);
            CustomVariableHandler.HandleCvarSetters(statusEffect.Effect.CustomVariableSetters);


            DestroyHelper destroyCondition;
            AddEffectPrefabs(statusEffect.Effect, out destroyCondition);
            if (destroyCondition != null)
            {
                destroyCondition.Init(DestroyCondition.StatusEffectNotActive, CharacterMono.Controller, statusEffect.ID);
            }

            if(statusEffect.Effect.RemoveStatusEffect)
            {
                RemoveStatusEffect(statusEffect.Effect.RemoveStatusEffectID);
            }

            for (int index = 0; index < StatusEffects.Count; index++)
            {
                var statusEff = StatusEffects[index];
                if (statusEff.Effect.CanBeCancelled)
                {
                    if (statusEff.Effect.CancellingStatusEffectID == statusEffect.ID)
                    {
                        RemoveStatusEffect(statusEff);
                        index--;
                    }
                }
            }

            for (int index = 0; index < AuraEffects.Count; index++)
            {
                var auraEffect = AuraEffects[index];
                if (auraEffect.PassiveEffect.CanBeCancelled)
                {
                    if (auraEffect.PassiveEffect.CancellingStatusEffectID == statusEffect.ID)
                    {
                        RemoveAuraEffect(auraEffect);
                        index--;
                    }
                }
            }

            for (int index = 0; index < TimedPassiveEffects.Count; index++)
            {
                var timedPassiveEffect = TimedPassiveEffects[index];
                if (timedPassiveEffect.CanBeCancelled)
                {
                    if (timedPassiveEffect.CancellingStatusEffectID == statusEffect.ID)
                    {
                        RemoveTimedPassiveEffect(timedPassiveEffect);
                        index--;
                    }
                }
            }
            
            FullUpdateStats();
        }

        public void AddTimedPassiveEffect(TimedPassiveEffect timedPassiveEffect)
        {
            TimedPassiveEffects.Add(timedPassiveEffect);
            CustomVariableHandler.HandleCvarSetters(timedPassiveEffect.CustomVariableSetters);
            if(timedPassiveEffect.RemoveStatusEffect)
            {
                RemoveStatusEffect(timedPassiveEffect.RemoveStatusEffectID);
            }

            DestroyHelper destroyCondition;
            AddEffectPrefabs(timedPassiveEffect, out destroyCondition);
            if(destroyCondition != null)
            {
                destroyCondition.Init(DestroyCondition.TimedPassiveNotActive, CharacterMono.Controller, timedPassiveEffect.ID);
            }

            FullUpdateStats();
        }

        public void AddEffectPrefabs(TimedPassiveEffect timedPassiveEffect, out DestroyHelper destroyCondition, bool destroyAfterDuration = true)
        {            
            destroyCondition = null;
            if (!string.IsNullOrEmpty(timedPassiveEffect.OnActivatePrefab))
            {
                var o = SpawnPrefab(timedPassiveEffect.OnActivatePrefab, CharacterMono.transform.position, Quaternion.identity, CharacterMono.transform);
                o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, ImpactPrefabTime);
            }

            if (!string.IsNullOrEmpty(timedPassiveEffect.ActivePrefab))
            {
                
                var o = SpawnPrefab(timedPassiveEffect.ActivePrefab, CharacterMono.transform.position, Quaternion.identity, CharacterMono.transform);
                var firstCondition = o.GetComponent<DestroyHelper>();
                if (destroyAfterDuration)
                {
                    firstCondition.Init(DestroyCondition.Time, timedPassiveEffect.Duration);
                    var secondCondition = o.AddComponent<DestroyHelper>();
                    destroyCondition = secondCondition;
                }
                else
                {
                    destroyCondition = firstCondition;
                }
            }
        }

        public void AddRestoration(Restoration restoration)
        {
            if(Rm_RPGHandler.Instance.Combat.MetaAppliesToHealing && !string.IsNullOrEmpty(restoration.SkillMetaId))
            {
                var susceptibility = SkillMetaSusceptibilities.Where(s => s.ID == restoration.SkillMetaId).Sum(s => s.AdditionalDamage);
                if(restoration.FixedRestore)
                {
                    restoration.AmountToRestore = (int)(restoration.AmountToRestore + (restoration.AmountToRestore * -susceptibility));
                }
                else
                {
                    restoration.PercentToRestore = (restoration.PercentToRestore + (restoration.PercentToRestore * -susceptibility));
                }
            }

            if(restoration.RestorationType == RestorationType.Time_Based)
            {
                Restorations.Add(restoration);    
            }
            else
            {
                int amtToRestore;
                var vitalToRestore = GetVitalByID(restoration.VitalToRestoreID);
                if (restoration.FixedRestore)
                {
                    amtToRestore = restoration.AmountToRestore;
                }
                else
                {
                    amtToRestore = (int)(vitalToRestore.MaxValue * restoration.PercentToRestore);
                }
                vitalToRestore.CurrentValue += amtToRestore;
            }
            FullUpdateStats();
        }

        public void AddProcEffect(Rm_ProcEffect procEffect)
        {
            ProcEffects.Add(procEffect);
            //FullUpdateStats();
        }

        public void AddDoT(DamageOverTime damageOverTime)
        {
            damageOverTime.HasDuration = true;
            damageOverTime.InstanceID = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(damageOverTime.OnActivatePrefab))
            {
                var o = SpawnPrefab(damageOverTime.OnActivatePrefab, CharacterMono.transform.position, Quaternion.identity, CharacterMono.transform);
                o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, ImpactPrefabTime);
            }

            if (!string.IsNullOrEmpty(damageOverTime.ActivePrefab))
            {
                var o = SpawnPrefab(damageOverTime.ActivePrefab, CharacterMono.transform.position, Quaternion.identity, CharacterMono.transform);
                o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, damageOverTime.Duration);
                var secondCondition = o.AddComponent<DestroyHelper>();
                secondCondition.Init(DestroyCondition.DoTNotActive, CharacterMono.Controller, damageOverTime.InstanceID);
            }

            CurrentDoTs.Add(damageOverTime);
            FullUpdateStats();
        }

        public GameObject SpawnPrefab(string prefabPath, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go = null;
            var prefab = Resources.Load(prefabPath) as GameObject;
            if(prefab != null)
            {
                go = (GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation);    
                if(parent != null)
                {
                    go.transform.parent = parent;
                }
            }
            return go;
        }

        public void ApplyTimedPassiveEffect(TimedPassiveEffect timedPassiveEffect)
        {
            foreach (var ab in timedPassiveEffect.AttributeBuffs)
            {
                GetAttributeByID(ab.AttributeID).SkillValue += ab.Amount;
            }

            foreach (var stat in timedPassiveEffect.StatisticBuffs)
            {
                GetStatByID(stat.StatisticID).SkillValue += stat.Amount;
            }

            foreach (var vit in timedPassiveEffect.VitalBuffs)
            {
                GetVitalByID(vit.VitalID).SkillValue += vit.Amount;
            }

            //skillimmunity/sus
            if (timedPassiveEffect.AddSkillImmunity)
            {
                if (SkillMetaImmunitiesID.FirstOrDefault(s => s.ID == timedPassiveEffect.SkillImmunityID) == null)
                    SkillMetaImmunitiesID.Add(new SkillImmunity() { ID = timedPassiveEffect.SkillImmunityID });
            }

            if (timedPassiveEffect.AddSkillSusceptibility)
            {
                var existing = SkillMetaSusceptibilities.FirstOrDefault(s => s.ID == timedPassiveEffect.SkillSusceptibilityID);
                if (existing != null)
                {
                    existing.AdditionalDamage += timedPassiveEffect.SkillSusceptibilityAmount;
                }
                else
                {
                    var newSus = new SkillMetaSusceptibility
                    {
                        ID = timedPassiveEffect.SkillSusceptibilityID,
                        AdditionalDamage = timedPassiveEffect.SkillSusceptibilityAmount
                    };
                    SkillMetaSusceptibilities.Add(newSus);
                }
            }

            foreach (var regenBonus in timedPassiveEffect.VitalRegenBonuses)
            {
                var existing = VitalRegenBonuses.FirstOrDefault(s => s.VitalID == regenBonus.VitalID);
                if (existing != null)
                {
                    existing.RegenBonus += regenBonus.RegenBonus;
                }
                else
                {
                    var newSus = new VitalRegenBonus
                    {
                        VitalID = regenBonus.VitalID,
                        RegenBonus = regenBonus.RegenBonus
                    };
                    VitalRegenBonuses.Add(newSus);
                }
            }

            foreach (var statusReduc in timedPassiveEffect.StatusDurationReduction)
            {
                var existing = StatusReductions.FirstOrDefault(s => s.StatusEffectID == statusReduc.StatusEffectID && s.IsPercentageDecrease == statusReduc.IsPercentageDecrease);
                if (existing != null)
                {
                    existing.DecreaseAmount += statusReduc.DecreaseAmount;
                }
                else
                {
                    var newReduc = new ReduceStatusDuration
                    {
                        StatusEffectID = statusReduc.StatusEffectID,
                        IsPercentageDecrease = statusReduc.IsPercentageDecrease,
                        DecreaseAmount = statusReduc.DecreaseAmount
                    };
                    StatusReductions.Add(newReduc);
                }
            }
        }

        public void ApplyPassiveEffect(PassiveEffect talentEffect)
        {
            foreach (var ab in talentEffect.AttributeBuffs)
            {
                GetAttributeByID(ab.AttributeID).SkillValue += ab.Amount;
            }

            foreach (var stat in talentEffect.StatisticBuffs)
            {
                GetStatByID(stat.StatisticID).SkillValue += stat.Amount;
            }

            foreach (var vit in talentEffect.VitalBuffs)
            {
                GetVitalByID(vit.VitalID).SkillValue += vit.Amount;
            }

            //skillimmunity/sus
            if (talentEffect.AddSkillImmunity)
            {
                if (SkillMetaImmunitiesID.FirstOrDefault(s => s.ID == talentEffect.SkillImmunityID) == null)
                    SkillMetaImmunitiesID.Add(new SkillImmunity() { ID = talentEffect.SkillImmunityID });
            }

            if (talentEffect.AddSkillSusceptibility)
            {
                var existing = SkillMetaSusceptibilities.FirstOrDefault(s => s.ID == talentEffect.SkillSusceptibilityID);
                if (existing != null)
                {
                    existing.AdditionalDamage += talentEffect.SkillSusceptibilityAmount;
                }
                else
                {
                    var newSus = new SkillMetaSusceptibility
                    {
                        ID = talentEffect.SkillSusceptibilityID,
                        AdditionalDamage = talentEffect.SkillSusceptibilityAmount
                    };
                    SkillMetaSusceptibilities.Add(newSus);
                }
            }

            foreach (var regenBonus in talentEffect.VitalRegenBonuses)
            {
                var existing = VitalRegenBonuses.FirstOrDefault(s => s.VitalID == regenBonus.VitalID);
                if (existing != null)
                {
                    existing.RegenBonus += regenBonus.RegenBonus;
                }
                else
                {
                    var newSus = new VitalRegenBonus
                    {
                        VitalID = regenBonus.VitalID,
                        RegenBonus = regenBonus.RegenBonus
                    };
                    VitalRegenBonuses.Add(newSus);
                }
            }

            foreach (var statusReduc in talentEffect.StatusDurationReduction)
            {
                var existing = StatusReductions.FirstOrDefault(s => s.StatusEffectID == statusReduc.StatusEffectID && s.IsPercentageDecrease == statusReduc.IsPercentageDecrease);
                if (existing != null)
                {
                    existing.DecreaseAmount += statusReduc.DecreaseAmount;
                }
                else
                {
                    var newReduc = new ReduceStatusDuration
                    {
                        StatusEffectID = statusReduc.StatusEffectID,
                        IsPercentageDecrease = statusReduc.IsPercentageDecrease,
                        DecreaseAmount = statusReduc.DecreaseAmount
                    };
                    StatusReductions.Add(newReduc);
                }
            }
        }

        public void RemoveAuraEffect(AuraEffect auraEffect)
        {
            RemoveAuraEffect(auraEffect.SkillId);
        }

        public void RemoveAuraEffect(string auraId)
        {
            var effect = AuraEffects.FirstOrDefault(e => e.SkillId == auraId);
            if(effect != null)
            {
                AuraEffects.Remove(effect);
                CustomVariableHandler.HandleCvarSetters(effect.PassiveEffect.CustomVariableSettersOnDisable);
                DeactivatePassive(effect.PassiveEffect);
            }
        }

        public void RemoveFriendlyAura(FriendlyAura friendlyAura)
        {
            RemoveFriendlyAura(friendlyAura.AuraEffect.SkillId);
        }

        public void RemoveFriendlyAura(string auraId)
        {
            var effect = FriendlyAuras.FirstOrDefault(e => e.AuraEffect.SkillId == auraId);
            if(effect != null)
            {
                FriendlyAuras.Remove(effect);
                CustomVariableHandler.HandleCvarSetters(effect.AuraEffect.PassiveEffect.CustomVariableSettersOnDisable);
                DeactivatePassive(effect.AuraEffect.PassiveEffect);
            }
        }

        public void RemoveStatusEffect(StatusEffect timedEffect)
        {
            RemoveStatusEffect(timedEffect.ID);
        }

        public void RemoveStatusEffect(string id)
        {
            var timedEffect = StatusEffects.FirstOrDefault(s => s.ID == id);
            if(timedEffect != null)
            {
                StatusEffects.RemoveAll( s => s.ID == timedEffect.ID);
                CustomVariableHandler.HandleCvarSetters(timedEffect.Effect.CustomVariableSettersOnDisable);
                DeactivatePassive(timedEffect.Effect);    
            }
        }

        public void RemoveTimedPassiveEffect(TimedPassiveEffect timedEffect)
        {
            RemoveTimedPassiveEffect(timedEffect.ID);
        }

        public void RemoveTimedPassiveEffect(string id)
        {
            var timedEffect = TimedPassiveEffects.FirstOrDefault(s => s.ID == id);
            if(timedEffect != null)
            {
                TimedPassiveEffects.RemoveAll(s => s.ID == timedEffect.ID);
                CustomVariableHandler.HandleCvarSetters(timedEffect.CustomVariableSettersOnDisable);
                DeactivatePassive(timedEffect);    
            }
        }

        private void DeactivatePassive(TimedPassiveEffect passiveEffect)
        {
            DeactivatePassive(passiveEffect as PassiveEffect);
        }

        protected void DeactivatePassive(PassiveEffect passiveEffect)
        {
            if (passiveEffect.AddSkillImmunity)
            {
                var existing = SkillMetaImmunitiesID.FirstOrDefault(s => s.ID == passiveEffect.SkillImmunityID && !s.HasDuration);
                if (existing != null)
                {
                    SkillMetaImmunitiesID.Remove(existing);
                }
            }

            if (passiveEffect.AddSkillSusceptibility)
            {
                var existing = SkillMetaSusceptibilities.FirstOrDefault(s => s.ID == passiveEffect.SkillSusceptibilityID && !s.HasDuration);
                if (existing != null)
                {
                    SkillMetaSusceptibilities.Remove(existing);
                }
            }

            if (passiveEffect.VitalRegenBonuses.Count > 0)
            {
                foreach (var vitRegen in passiveEffect.VitalRegenBonuses)
                {
                    var existing = VitalRegenBonuses.FirstOrDefault(s => s.VitalID == vitRegen.VitalID && !s.HasDuration);
                    if (existing != null)
                    {
                        VitalRegenBonuses.Remove(existing);
                    }
                }
            }
            if (passiveEffect.StatusDurationReduction.Count > 0)
            {
                foreach (var statusRed in passiveEffect.StatusDurationReduction)
                {
                    var existing = StatusReductions.FirstOrDefault(s => s.StatusEffectID == statusRed.StatusEffectID && s.IsPercentageDecrease == statusRed.IsPercentageDecrease);
                    if (existing != null)
                    {
                        StatusReductions.Remove(existing);
                    }
                }
            }
        }

        protected void ApplyAuras()
        {
            foreach(var aura in AuraEffects)
            {
                ApplyPassiveEffect(aura.PassiveEffect);
            }

            foreach(var aura in FriendlyAuras.Select(f => f.AuraEffect))
            {
                ApplyPassiveEffect(aura.PassiveEffect);
            }
        }

        protected void ApplyBuffDebuffs()
        {
            foreach (var timedPassiveEffect in TimedPassiveEffects)
            {
                ApplyTimedPassiveEffect(timedPassiveEffect);
            }
        }

        public void FullUpdateStats()
        {

            //Add proc effects from Auras, Buffs, Debuffs and Talents
            ResetStats();

            var player = this as PlayerCharacter;
            if(player != null)
            {
                player.ApplyEquipmentStats();
                ScaleStats();
            }
            else
            {
                ScaleStats();
            }


            if (player != null)
            {
                player.ApplyTalents();
            }

            ApplyAuras();
            ApplyBuffDebuffs();
            ApplyStatusEffects();
            DetermineState();

            _damageDealable = CombatCalcEvaluator.EvaluateDamageDealt(this);
            RPG.Events.OnUpdatedPlayerStats(new RPGEvents.UpdatedPlayerStatsArgs());
        }

        protected void ApplyStatusEffects()
        {
            foreach (var statusEffect in StatusEffects)
            {
                ApplyPassiveEffect(statusEffect.Effect);
            }
        }

        protected void DetermineState()
        {
            //if any status, buff, debufff etc says we're stunned then stunned = true
            //etc for silenced, retreating etc

            if(StatusEffects.Any(s => s.CauseStun))
            {
                Stunned = true;
                if (StatusEffects.Any(s => s.CauseAnimationFreeze))
                {
                    StunFreeze = true;
                }
            }
            if(StatusEffects.Any(s => s.CauseSilence))
            {
                Silenced = true;
            }
            if(StatusEffects.Any(s => s.CauseRetreat))
            {
                Retreating = true;
            }
        }

        public void ScaleStats()
        {
            var statScalingTree = Rm_RPGHandler.Instance.Nodes.StatScalingTree;
            for (int i = 0; i < Stats.Count; i++)
            {
                var stat = Stats[i];
                var statBaseValue = stat.BaseValue;
                var nodeChain = new NodeChain(statScalingTree, typeof(CombatStartNode), stat.ID) { Combatant = this, FloatValue = statBaseValue };

                while (!nodeChain.Done)
                {
                    nodeChain.Evaluate();
                }

                stat.ScaledBaseValue = nodeChain.FloatValue;
            }
            var vitalScalingTree = Rm_RPGHandler.Instance.Nodes.VitalScalingTree;
            for (int i = 0; i < Vitals.Count; i++)
            {
                var vital = Vitals[i];
                int vitBaseValue = vital.BaseValue;

                var nodeChain = new NodeChain(vitalScalingTree, typeof(CombatStartNode), vital.ID) { Combatant = this, IntValue = vitBaseValue };

                while (!nodeChain.Done)
                {
                    nodeChain.Evaluate();
                }

                vital.ScaledBaseValue = (int)nodeChain.IntValue;
            }
            
        }
        
        public void ScaleStatsCC()
        {
            for (int i = 0; i < Stats.Count; i++)
            {
                var stat = Stats[i];
                var statBaseValue = stat.BaseValue;
                stat.ScaledBaseValue = statBaseValue;
            }
            for (int i = 0; i < Vitals.Count; i++)
            {
                var vital = Vitals[i];
                int vitBaseValue = vital.BaseValue;
                vital.ScaledBaseValue = vitBaseValue;
            }
            
        }

        public void ResetStats()
        {
            //TODO : Test this reset || Also: this should be called and then scaleStats if NPC/Monster : if player should be RESET->Update->Scale
            Silenced = Stunned = Retreating = StunFreeze =  false;
            foreach(var metaSus in SkillMetaSusceptibilities.Where(s => !s.HasDuration))
            {
                var totalAdditional = 0f;

                if(CharacterType == CharacterType.Player)
                {
                    var player = (PlayerCharacter)this;
                    var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                    var suscepts = classDef.SkillMetaSusceptibilities;
                    totalAdditional = suscepts.Where(s => s.ID == metaSus.ID).Sum(x => x.AdditionalDamage);
                }
                else
                {
                    var CC = (CombatCharacter)this;
                    var suscepts = CC.SkillMetaSusceptibilities;
                    totalAdditional = suscepts.Where(s => s.ID == metaSus.ID).Sum(x => x.AdditionalDamage);
                }

                metaSus.AdditionalDamage = totalAdditional;
            }
            foreach (var vitRegen in VitalRegenBonuses.Where(s => !s.HasDuration))
            {
                vitRegen.RegenBonus = 0;
            }
            foreach (var statusReduc in StatusReductions.Where(s => !s.HasDuration))
            {
                statusReduc.DecreaseAmount = 0;
            }

            foreach(var vit in Vitals)
            {
                var baseVitRegen = VitalRegenBonuses.FirstOrDefault(v => v.VitalID == vit.ID);
                if (baseVitRegen != null)
                {
                    baseVitRegen.RegenBonus = vit.RegenPercentValue;
                }
                else
                {
                    VitalRegenBonuses.Add(new VitalRegenBonus
                    {
                        VitalID = vit.ID,
                        RegenBonus = vit.RegenPercentValue
                    });
                }
            }

            

            //note: IN PROGRESS: we need to keep the above ^ which have a duration (i.e. were from skill or an effect)

            foreach (var vital in Vitals)
            {
                vital.Reset();
            }

            foreach (var att in Attributes)
            {
                att.Reset();
            }

            foreach (var s in Stats)
            {
                s.Reset();
            }

            //End of reset
        }

        public void ClearStats()
        {
            AuraEffects = new List<AuraEffect>();
            StatusEffects  = new List<StatusEffect>();
            CurrentDoTs  = new List<DamageOverTime>();
            Restorations  = new List<Restoration>();
            SkillMetaImmunitiesID = new List<SkillImmunity>();
            SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
            VitalRegenBonuses = new List<VitalRegenBonus>();
            StatusReductions = new List<ReduceStatusDuration>();
            ProcEffects = new List<Rm_ProcEffect>();
        }

    }

    public enum CharacterType
    {
        Player,
        NPC,
        Enemy
    }
}