using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LogicSpawn.RPGMaker.Beta
{
    public class BaseCharacterMono : MonoBehaviour
    {
        public BaseCharacter Character;
        private IRPGController _controller;
        public IRPGController Controller
        {
            get { return _controller ?? (_controller = GetComponent<RPGController>()); }
            set { _controller = value; }
        }
        public string ID;

        private const float DoTTickPrefabTime = 0.3f;
        private const float DoTExpiredPrefabTime = 0.3f;
        public bool Initialised;

        void Awake()
        {
            Initialised = false;
            Controller = GetComponent<RPGController>();
            ID = Guid.NewGuid().ToString();
        }

        void Update()
        {
            HandleHealthReduction();
            HandleCooldowns();
            HandleTimers();
            HandleRestorations();
            HandleDoTs();
            HandleStatusEffects();
            HandleAuraEffects();
            HandleFriendlyAuras();
            HandleTalentEffects();
            HandleTimedPassives();
            HandleTalentEvents();
            DoUpdate();
        }

        private void HandleHealthReduction()
        {
            for (int index = 0; index < Character.Vitals.Count; index++)
            {
                var vital = Character.Vitals[index];
                if (vital.IsHealth) continue;

                if (vital.ReduceHealthIfZero && vital.CurrentValue <= 0)
                {
                    vital.ReductionIntervalTimer += Time.deltaTime;
                    if (vital.ReductionIntervalTimer >= vital.ReductionIntervalSeconds)
                    {
                        var health = Character.GetHealthVital();
                        if (vital.ReduceByFixedAmount)
                        {
                            health.CurrentValue -= vital.ReductionFixedAmount;
                        }
                        else
                        {
                            var reduction = (int)(Math.Ceiling(health.MaxValue * vital.ReductionPercentageAmount));
                            health.CurrentValue -= reduction;
                        }
                        vital.ReductionIntervalTimer = 0;

                        if(Character is PlayerCharacter)
                        {
                            RPG.Events.OnUpdatedPlayerStats(new RPGEvents.UpdatedPlayerStatsArgs());
                        }
                    }
                }
            }
        }

        private void HandleCooldowns()
        {
            if(Character is PlayerCharacter)
            {
                var player = Character as PlayerCharacter;
                for (int index = 0; index < player.SkillHandler.AvailableSkills.Count; index++)
                {
                    var skill = player.SkillHandler.AvailableSkills[index];
                    if(skill.CurrentCoolDownTime > 0)
                        skill.CurrentCoolDownTime -= Time.deltaTime;
                }

                foreach(var item in player.Inventory.AllItems.Select(i => i as Consumable).Where(i => i != null))
                {
                    if (item.CurrentCooldown > 0)
                        item.CurrentCooldown -= Time.deltaTime;
                }
            }
            else
            {
                var cc = Character as CombatCharacter;
                var skills = cc.EnemySkills;
                for (int index = 0; index < skills.Count; index++)
                {
                    var skill = skills[index].SkillRef;
                    if (skill.CurrentCoolDownTime > 0)
                        skill.CurrentCoolDownTime -= Time.deltaTime;
                }
            }
        }

        private void HandleTimedPassives()
        {
            foreach (var timedPassive in Character.TimedPassiveEffects)
            {
                if (timedPassive.RemoveStatusEffect)
                {
                    var effectToRemove = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == timedPassive.RemoveStatusEffectID);
                    Character.RemoveStatusEffect(effectToRemove);
                }

                if (timedPassive.RunsEvent)
                {
                    HandleEvent(timedPassive.RunEventID);
                }
            }
        }

        private void HandleTalentEffects()
        {
            var player = Character as PlayerCharacter;
            if (player == null) return;
            foreach (var talent in player.TalentHandler.Talents)
            {
                if(talent.TalentEffect.Effect.RemoveStatusEffect)
                {
                    var effectToRemove = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == talent.TalentEffect.Effect.RemoveStatusEffectID);
                    player.RemoveStatusEffect(effectToRemove);
                }
            }
        }

        private void HandleTalentEvents()
        {
            var player = Character as PlayerCharacter;
            if(player == null) return;
            foreach (var talent in player.TalentHandler.Talents.Where(talent => talent.TalentEffect.Effect.RunsEvent))
            {
                HandleEvent(talent.TalentEffect.Effect.RunEventID);
            }
        }

        private void HandleAuraEffects()
        {
            for (int index = 0; index < Character.AuraEffects.Count; index++)
            {
                var removeAura = false;
                var auraEffect = Character.AuraEffects[index];
                if (auraEffect.TakeResourceAmountPerSec)
                {
                    auraEffect.ResourcePerSecTimer += Time.deltaTime;
                    if (auraEffect.ResourcePerSecTimer >= 1.0f)
                    {
                        var resourceVit = Character.GetVitalByID(auraEffect.AuraEffectStats.ResourceRequiredId);
                        if (resourceVit.CurrentValue - auraEffect.AuraEffectStats.ResourceRequirement >= 0)
                        {
                            resourceVit.CurrentValue -= auraEffect.AuraEffectStats.ResourceRequirement;
                        }
                        else
                        {
                            removeAura = true;
                        }
                        auraEffect.ResourcePerSecTimer = 0;
                    }
                }

                if(auraEffect.ApplyToAllies)
                {
                    var nearbyAllies = _controller.GetNearbyAllies(auraEffect.Radius);
                    foreach(var ally in nearbyAllies)
                    {
                        ally.Character.AddFriendlyAura(this, auraEffect);
                    }
                }

                if (!removeAura)
                {
                    if (auraEffect.PassiveEffect.RunsEvent)
                    {
                        HandleEvent(auraEffect.PassiveEffect.RunEventID);
                    }

                    if (auraEffect.PassiveEffect.RemoveStatusEffect)
                    {
                        var effectToRemove = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == auraEffect.PassiveEffect.RemoveStatusEffectID);
                        Character.RemoveStatusEffect(effectToRemove);
                    }
                }
                else
                {
                    Character.RemoveAuraEffect(auraEffect);
                    SpawnExpiredPrefab(auraEffect.PassiveEffect.OnExpiredPrefab);
                    Character.FullUpdateStats();
                    index--;
                }
            }
        }

        private void HandleFriendlyAuras()
        {
            for (int index = 0; index < Character.FriendlyAuras.Count; index++)
            {
                var removeAura = false;
                var friendlyAura = Character.FriendlyAuras[index];
                if (friendlyAura == null)
                {
                    Character.RemoveFriendlyAura(friendlyAura);
                    Character.FullUpdateStats();
                    index--;
                    continue;
                }
                    var source = friendlyAura.SourceCharacter;
                    var effect = friendlyAura.AuraEffect;

                    if (source == null)
                    {
                        removeAura = true;
                    }
                    else if (!source.Character.Alive)
                    {
                        removeAura = true;
                    }
                    else if (Vector3.Distance(source.transform.position, transform.position) > effect.Radius)
                    {
                        //Debug.Log(Vector3.Distance(source.transform.position, transform.position));
                        removeAura = true;
                    }
                    else
                    {
                        var auraEffect = friendlyAura.SourceCharacter.Character.AuraEffects.FirstOrDefault(a => a.SkillId == friendlyAura.AuraEffect.SkillId);
                        if(auraEffect == null)
                        {
                            removeAura = true;
                        }
                    }

                    if (!removeAura)
                    {
                        var eff = friendlyAura.AuraEffect.PassiveEffect;
                        if (eff.RunsEvent)
                        {
                            HandleEvent(eff.RunEventID);
                        }

                        if (eff.RemoveStatusEffect)
                        {
                            var effectToRemove = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == eff.RemoveStatusEffectID);
                            Character.RemoveStatusEffect(effectToRemove);
                        }
                    }
                    else
                    {
                        Character.RemoveFriendlyAura(friendlyAura);
                        SpawnExpiredPrefab(friendlyAura.AuraEffect.PassiveEffect.OnExpiredPrefab);
                        Character.FullUpdateStats();
                        index--;
                    }
            }
        }

        private void HandleStatusEffects()
        {
            //TODO: applie to allies similar to auras
            for (int index = 0; index < Character.StatusEffects.Count; index++)
            {
                var statusEffect = Character.StatusEffects[index];
                if (statusEffect.Effect.RunsEvent)
                {
                    HandleEvent(statusEffect.Effect.RunEventID);
                }

                if(statusEffect.CausesDOT)
                {
                    var dot = statusEffect.DamageOverTime;
                    dot.IntervalTimer += Time.deltaTime;
                    if (!(dot.IntervalTimer >= dot.TimeBetweenTick)) continue;
                    SpawnDoTDamageTick(dot);
                    Character.VitalHandler.TakeDoTDamage(dot.Attacker, dot);
                    dot.IntervalTimer = 0;
                }

                if (statusEffect.Effect.RemoveStatusEffect)
                {
                    var effectToRemove = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == statusEffect.Effect.RemoveStatusEffectID);
                    Character.RemoveStatusEffect(effectToRemove);
                }
            }
        }
        
        private void HandleDoTs()
        {
            foreach (var dot in Character.CurrentDoTs)
            {
                dot.IntervalTimer += Time.deltaTime;
                if (!(dot.IntervalTimer >= dot.TimeBetweenTick)) continue;
                Character.VitalHandler.TakeDoTDamage(dot.Attacker, dot);
                SpawnDoTDamageTick(dot);
                dot.IntervalTimer = 0;
            }
        }

        private void HandleRestorations()
        {
            foreach (var restoration in Character.Restorations)
            {
                restoration.IntervalTimer += Time.deltaTime;
                if(restoration.IntervalTimer >= restoration.SecBetweenRestore)
                {
                    int amtToRestore;
                    var vitalToRestore = Character.GetVitalByID(restoration.VitalToRestoreID);
                    if(restoration.FixedRestore)
                    {
                        amtToRestore = restoration.AmountToRestore;
                    }
                    else
                    {
                        amtToRestore = (int) (vitalToRestore.MaxValue*restoration.PercentToRestore);
                    }

                    vitalToRestore.CurrentValue += amtToRestore;
                    restoration.IntervalTimer = 0;
                }
            }
        }


        private void SpawnDoTDamageTick(DamageOverTime dot)
        {
            if (!string.IsNullOrEmpty(dot.DamageTickPrefab))
            {
                var prefab = Resources.Load(dot.DamageTickPrefab) as GameObject;
                var tickPrefab = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
                tickPrefab.transform.parent = transform;
                tickPrefab.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, DoTTickPrefabTime);
            }
        }

        private void HandleEvent(string eventId)
        {
            Debug.LogWarning("Not implemented: handling events [" + eventId + "]");
        }

        private void HandleTimers()
        {
            var updateStats = false;

            #region SkillMetaImmunity
            for (int index = 0; index < Character.SkillMetaImmunitiesID.Count; index++)
            {
                var timedEffect = Character.SkillMetaImmunitiesID[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.SkillMetaImmunitiesID.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region SkillMetaSusceptibility
            for (int index = 0; index < Character.SkillMetaSusceptibilities.Count; index++)
            {
                var timedEffect = Character.SkillMetaSusceptibilities[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.SkillMetaSusceptibilities.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region VitalRegenBonus
            for (int index = 0; index < Character.VitalRegenBonuses.Count; index++)
            {
                var timedEffect = Character.VitalRegenBonuses[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.VitalRegenBonuses.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region StatusReductions
            for (int index = 0; index < Character.StatusReductions.Count; index++)
            {
                var timedEffect = Character.StatusReductions[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.StatusReductions.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region Restorations
            for (int index = 0; index < Character.Restorations.Count; index++)
            {
                var timedEffect = Character.Restorations[index];

                if (timedEffect.RestorationType != RestorationType.Time_Based) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.Restorations.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region ProcEffects
            for (int index = 0; index < Character.ProcEffects.Count; index++)
            {
                var timedEffect = Character.ProcEffects[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.ProcEffects.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region AuraEffects
            for (int index = 0; index < Character.AuraEffects.Count; index++)
            {
                var timedEffect = Character.AuraEffects[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.PassiveEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.RemoveAuraEffect(timedEffect);
                SpawnExpiredPrefab(timedEffect.PassiveEffect.OnExpiredPrefab);
                index--;
                updateStats = true;
            }
            #endregion

            #region TimedPassives
            for (int index = 0; index < Character.TimedPassiveEffects.Count; index++)
            {
                var timedEffect = Character.TimedPassiveEffects[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                Character.RemoveTimedPassiveEffect(timedEffect);
                SpawnExpiredPrefab(timedEffect.OnExpiredPrefab);
                index--;
                updateStats = true;
            }
            #endregion

            #region StatusEffects
            for (int index = 0; index < Character.StatusEffects.Count; index++)
            {
                var timedEffect = Character.StatusEffects[index];

                if (!timedEffect.Effect.HasDuration) continue;
                timedEffect.Effect.Duration -= Time.deltaTime;

                if (!(timedEffect.Effect.Duration <= 0)) continue;
                SpawnExpiredPrefab(timedEffect.Effect.OnExpiredPrefab);
                Character.RemoveStatusEffect(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region CurrentDoTs
            for (int index = 0; index < Character.CurrentDoTs.Count; index++)
            {
                var timedEffect = Character.CurrentDoTs[index];

                if (!timedEffect.HasDuration) continue;
                timedEffect.Duration -= Time.deltaTime;

                if (!(timedEffect.Duration <= 0)) continue;
                SpawnExpiredPrefab(timedEffect.OnExpiredPrefab);
                Character.CurrentDoTs.Remove(timedEffect);
                index--;
                updateStats = true;
            }
            #endregion

            #region EnemySkillTimers
            if(Character.CharacterType != CharacterType.Player)
            {
                var cc = (CombatCharacter) Character;
                foreach(var skill in cc.EnemySkills.Where(s => s.CastType == Rm_EnemySkillCastType.EveryNthSeconds))
                {
                    if(skill.NthSecondsTimer > 0)
                        skill.NthSecondsTimer -= Time.deltaTime;
                }
            }
            #endregion

            if (updateStats)
            {
                Character.FullUpdateStats();
            }
        }

        private void SpawnExpiredPrefab(string prefabPath)
        {
            if (!string.IsNullOrEmpty(prefabPath))
            {
                var o = Character.SpawnPrefab(prefabPath, transform.position, Quaternion.identity, transform);
                o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, BaseCharacter.ImpactPrefabTime);
            }
        }

        void Start()
        {
            DoStart();

            Character.CharacterMono = this;

            Character.FullUpdateStats();

            StartCoroutine(RegenHealth());
        }

        private IEnumerator RegenHealth()
        {
            while(true)
            {
                if(Character.Alive )
                {
                        foreach (var vitalToRegen in Character.VitalRegenBonuses)
                        {
                            var vit = Character.GetVitalByID(vitalToRegen.VitalID);
                            if (!Controller.InCombat || vit.RegenWhileInCombat)
                            {
                                vit.CurrentValue += (int) (Mathf.Ceil(vit.MaxValue*vitalToRegen.RegenBonus));
                                if (vit.CurrentValue < 0)
                                {
                                    vit.CurrentValue = 0;
                                }

                                if (Character is PlayerCharacter)
                                {
                                    RPG.Events.OnUpdatedPlayerStats(new RPGEvents.UpdatedPlayerStatsArgs());
                                }
                            }
                        }
                    
                    yield return new WaitForSeconds(Rm_RPGHandler.Instance.ASVT.RegenInterval);
                }
                yield return null;
            }
        }

        protected virtual void DoUpdate()
        {
        }
        protected virtual void DoStart()
        {
        }
        protected void RefreshPrefabs()
        {
            /*
        public List<Restoration> Restorations ;
             */

            foreach(var auraEffect in Character.AuraEffects)
            {
                DestroyHelper destroyCondition;
                Character.AddEffectPrefabs(auraEffect.PassiveEffect, out destroyCondition);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.AuraEffectNotActive, Controller, auraEffect.SkillId);
                }
            }

            foreach(var friendlyAuraEffect in Character.FriendlyAuras)
            {
                DestroyHelper destroyCondition;
                Character.AddEffectPrefabs(friendlyAuraEffect.AuraEffect.PassiveEffect, out destroyCondition);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.FriendlyAuraNotAvailable, Controller, friendlyAuraEffect.AuraEffect.SkillId);
                }
            }

            foreach(var timedPassiveEffect in Character.TimedPassiveEffects)
            {
                DestroyHelper destroyCondition;
                Character.AddEffectPrefabs(timedPassiveEffect, out destroyCondition);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.TimedPassiveNotActive, Controller, timedPassiveEffect.ID);
                }
            }

            foreach(var statusEffect in Character.StatusEffects)
            {
                DestroyHelper destroyCondition;
                Character.AddEffectPrefabs(statusEffect.Effect, out destroyCondition);
                if (destroyCondition != null)
                {
                    destroyCondition.Init(DestroyCondition.StatusEffectNotActive, Controller, statusEffect.ID);
                }
            }

            foreach(var damageOverTime in Character.CurrentDoTs)
            {
                if (!string.IsNullOrEmpty(damageOverTime.ActivePrefab))
                {
                    var o = Character.SpawnPrefab(damageOverTime.ActivePrefab, transform.position, Quaternion.identity, transform);
                    o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, damageOverTime.Duration);
                    var secondCondition = o.AddComponent<DestroyHelper>();
                    secondCondition.Init(DestroyCondition.DoTNotActive, Controller, damageOverTime.InstanceID);
                }
            }
        }
    }
}