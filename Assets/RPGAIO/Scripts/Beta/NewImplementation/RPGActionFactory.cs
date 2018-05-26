using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace Assets.Scripts.Beta.NewImplementation
{
    public static class RPGActionFactory
    {
        public static RPGAction WaitForSeconds(float seconds)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Time" , seconds}                         
            };
            return new RPGAction(RPGActionType.WaitForSeconds, parameters);
        }

        public static RPGAction StartMecanimCasting()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.StartMecanimCasting, parameters);
        }
        public static RPGAction EndMecanimCasting()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.EndMecanimCasting, parameters);
        }

        public static RPGAction MoveToPosition(Vector3 position, float stopRange = -1, float speed = -1f)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Position" , position},
                {"StopRange", stopRange}         
            };

            if (speed != -1f)
            {
                parameters.Add("MoveSpeed", speed);
            }
            return new RPGAction(RPGActionType.MoveToPosition, parameters);
        }

        public static RPGAction MoveToPosition(BaseCharacterMono combatant, float stopRange = -1, float speed = -1f)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Combatant" , combatant},
                {"StopRange", stopRange}                   
            };

            if(speed != -1f)
            {
                parameters.Add("MoveSpeed", speed);
            }

            return new RPGAction(RPGActionType.MoveToPosition, parameters);
        }

        public static RPGAction BasicJump(float height, float completion = 1.00f)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Height" , height},
                {"Completion", completion}         
            };
            return new RPGAction(RPGActionType.BasicJump, parameters);
        }
        public static RPGAction WaitToLand()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.WaitToLand, parameters);
        }

        public static RPGAction JumpToPosition(Vector3 position, float peakHeight, float speed = -1f)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Position" , position},
                {"Height" , peakHeight}                       
            };

            if (speed != -1f)
            {
                parameters.Add("MoveSpeed", speed);
            }
            return new RPGAction(RPGActionType.JumpToPosition, parameters);
        }

        public static RPGAction JumpToPosition(BaseCharacterMono combatantObject, float peakHeight, float speed = -1f)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Combatant" , combatantObject},
                {"Height" , peakHeight}                       
            };

            if (speed != -1f)
            {
                parameters.Add("MoveSpeed", speed);
            }
            return new RPGAction(RPGActionType.JumpToPosition, parameters);
        }

        public static RPGAction WarpToPosition(Vector3 position)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Position" , position}                      
            };
            return new RPGAction(RPGActionType.WarpToPosition, parameters);
        }

        public static RPGAction WarpToPosition(BaseCharacterMono combatantObject)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Combatant" , combatantObject}                    
            };
            return new RPGAction(RPGActionType.WarpToPosition, parameters);
        }

        public static RPGAction PlayAnimation(AnimationDefinition definition)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"AnimDef" , definition}                    
            };
            return new RPGAction(RPGActionType.PlayAnimation, parameters);
        }

        public static RPGAction PlayAnimation(string animationname, float speed = 1.0f, WrapMode wrapMode = WrapMode.Loop, bool backwards = false)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Animation" , animationname},                   
                {"Speed" , speed},                   
                {"WrapMode" , wrapMode},                   
                {"Backwards" , backwards}                   
            };
            return new RPGAction(RPGActionType.PlayAnimation, parameters);
        }
        public static RPGAction PlaySound(AudioContainer audioContainer, AudioType audioType)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"AudioContainer" , audioContainer},                
                {"AudioType" , audioType}            
            };

            return new RPGAction(RPGActionType.PlaySound, parameters);
        }
        public static RPGAction PlaySound(AudioContainer audioContainer, AudioType audioType, float duration)
        {            
            var parameters = new Dictionary<string, object>()
            {
                {"AudioContainer" , audioContainer},                
                {"AudioType" , audioType} ,               
                {"Duration" , duration}                
            };

            return new RPGAction(RPGActionType.PlaySound, parameters);
        }

        public static RPGAction DamageTarget(BaseCharacterMono target, Damage damageToDeal,
            List<RPGAction> onHitActions = null, List<RPGAction> onCritHitActions = null, Skill skillRef = null)
        {
            if(onHitActions == null)
            {
                onHitActions = new List<RPGAction>();    
            }
            if (onCritHitActions == null)
            {
                onCritHitActions = new List<RPGAction>();    
            }

            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target},                
                {"DamageToDeal" , damageToDeal},
                {"HitActions" , onHitActions},
                {"CritHitActions" , onCritHitActions},
                {"SkillRef" , skillRef}
            };

            return new RPGAction(RPGActionType.DamageTarget, parameters);
        }

        public static RPGAction AutoAttack(BaseCharacterMono target, Damage damageToDeal, Vector3 targetPos)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target},                
                {"TargetPos" , targetPos},                
                {"DamageToDeal" , damageToDeal}
            };

            return new RPGAction(RPGActionType.AutoAttack, parameters);
        }

        public static RPGAction DamageTargetMelee(BaseCharacterMono target, Damage damageToDeal, Vector3 targetPos)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target},                
                {"TargetPos" , targetPos},                
                {"DamageToDeal" , damageToDeal}
            };

            return new RPGAction(RPGActionType.DamageTargetMelee, parameters);
        }

        public static RPGAction RemoveCombatant(Transform transform)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"CombatantTransform" , transform}
            };

            return new RPGAction(RPGActionType.RemoveCombatant, parameters);
        }

        public static RPGAction RepeatQueue(RPGActionQueue queue)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Queue" , queue}
            };

            return new RPGAction(RPGActionType.RepeatQueue, parameters);
        }

        public static RPGAction WaitForNextAttack()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.WaitForNextAttack, parameters);
        }

        public static RPGAction RespawnPlayer()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.RespawnPlayer, parameters);
        }

        public static RPGAction SpawnPrefab(string path, Vector3 position, Quaternion rotation, Action<GameObject> doAction, Transform parent = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"PrefabPath", path},
                {"Position", position},
                {"Rotation", rotation},
                {"ParentTransform", parent},
                {"DoAction", doAction},
                {"WithParent", parent != null}
            };
            return new RPGAction(RPGActionType.SpawnPrefab, parameters);
        }

        public static RPGAction AddDoT(BaseCharacterMono target, DamageOverTime damageOverTime)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},   
                {"DamageOverTime", damageOverTime}
            };
            return new RPGAction(RPGActionType.AddDamageOverTime, parameters);
        }

        public static RPGAction AddAuraEffect(BaseCharacterMono target, AuraSkill auraSkill)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"AuraSkill" , auraSkill}
            };

            return new RPGAction(RPGActionType.AddEffect, parameters);
        }
        public static RPGAction AddTimedPassiveEffect(BaseCharacterMono target, TimedPassiveEffect timedPassiveEffect)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"TimedPassiveEffect" , timedPassiveEffect}
            };

            return new RPGAction(RPGActionType.AddEffect, parameters);
        }
        public static RPGAction AddStatusEffect(BaseCharacterMono target, StatusEffect statusEffect, bool? withDuration = null, float duration = 0)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"StatusEffect" , statusEffect},
                {"Duration" , duration}
            };

            if (withDuration != null)
            {
                parameters.Add("WithDuration", withDuration.Value);
            }

            return new RPGAction(RPGActionType.AddEffect, parameters);
        }
        public static RPGAction AddStatusEffect(BaseCharacterMono target, string statusEffectID, bool? withDuration = null, float duration = 0)
        {
            var effect = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == statusEffectID));
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"StatusEffect" , effect },
                {"Duration" , duration}
            };

            if(withDuration != null)
            {
                parameters.Add("WithDuration", withDuration.Value);
            }

            if(parameters["StatusEffect"] == null)
            {
                Debug.Log("Trying to add status effect which is null inside action.");
            }
            return new RPGAction(RPGActionType.AddEffect, parameters);
        }
        public static RPGAction AddRestorationEffect(BaseCharacterMono target, Restoration restoration)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"Restoration" , restoration}
            };

            return new RPGAction(RPGActionType.AddEffect, parameters);
        }

        public static RPGAction RemoveStatusEffect(BaseCharacterMono target, string removeStatusEffectID)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"StatusEffectToRemove" , removeStatusEffectID}
            };

            return new RPGAction(RPGActionType.RemoveStatusEffect, parameters);
        }

        public static RPGAction RunEvent(string eventId, bool asynchronously = false)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"EventID" , eventId},
                {"Async", asynchronously}
            };

            return new RPGAction(RPGActionType.RunEvent, parameters);
        }

        public static RPGAction KnockBack(BaseCharacterMono target, Direction dir, float distance)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Target" , target},
                {"Direction" , dir},
                {"Distance", distance}
            };

            return new RPGAction(RPGActionType.KnockBack, parameters);
        }

        public static RPGAction GivePlayerItem(ItemGroup itemGroup, string itemId, int quantity, bool forceAdd = false)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"ItemGroup" , itemGroup},
                {"ItemId" , itemId},
                {"Quantity" , quantity},
                {"ForceAdd", forceAdd}
            };

            return new RPGAction(RPGActionType.GivePlayerItem, parameters);
        }

        public static RPGAction DoAction(IRPGController controller, Action<IRPGController> doAction)
        {
            var parameters = new Dictionary<string, object>()
            {                
                {"Controller" , controller},
                {"Action" , doAction}
            };

            return new RPGAction(RPGActionType.DoAction, parameters);
        }

        public static RPGAction FaceTarget(Vector3 target)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target}                     
            };
            return new RPGAction(RPGActionType.FaceTarget, parameters);
        }

        public static RPGAction PullTowards(BaseCharacterMono target, BaseCharacterMono characterMono, float distance)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target},
                {"Combatant" , characterMono},                     
                {"Distance" , distance}                     
            };
            return new RPGAction(RPGActionType.PullTowards, parameters);
        }

        public static RPGAction PullTowards(BaseCharacterMono target, Vector3 targetPosition, float distance)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target" , target},
                {"TargetPosition" , targetPosition},                     
                {"Distance" , distance}                     
            };
            return new RPGAction(RPGActionType.PullTowards, parameters);
        }

        public static RPGAction SetLastAttackTime()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.SetLastAttackTime, parameters);
        }

        public static RPGAction DealBonusTaunt(BaseCharacterMono target, int bonusTaunt)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"Target",target},
                {"BonusTaunt", bonusTaunt}
            };
            return new RPGAction(RPGActionType.DealBonusTaunt, parameters);
        }

        public static RPGAction RespawnNpc()
        {
            var parameters = new Dictionary<string, object>();
            return new RPGAction(RPGActionType.RespawnNPC, parameters);
        }
    }
}