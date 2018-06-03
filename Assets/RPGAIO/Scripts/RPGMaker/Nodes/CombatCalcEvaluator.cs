using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LogicSpawn.RPGMaker
{
    public class CombatCalcEvaluator
    {
        private static CombatCalcEvaluator MyInstance = new CombatCalcEvaluator();

        public static CombatCalcEvaluator Instance
        {
            get { return MyInstance; }
        }

        public DamageOutcome Evaluate(BaseCharacter attacker, BaseCharacter defender, Damage attackerDamage)
        {
            var damageDealt = attackerDamage.DamageDealt;
            var doNotEval = false;
            var evalWithDummy = false;

            if(attacker == null)
            {
                if(Rm_RPGHandler.Instance.Combat.SkipEvaluatingDmgWithNullAttack)
                {
                    doNotEval = true;
                }
                else
                {
                    attacker = new BaseCharacter();
                    attacker.Init();
                    evalWithDummy = true;
                }
            }

            if (!doNotEval && !evalWithDummy && (attacker.CharacterType == CharacterType.Player || defender.CharacterType == CharacterType.Player))
            {
                var participants = new[] {attacker, defender};
                if (participants.All(p => p != null) && participants.Any(p => p.CharacterType != CharacterType.Player))
                {
                    var player = participants.First(p => p.CharacterType == CharacterType.Player);
                    var other = participants.First(p => p.CharacterType != CharacterType.Player);

                    var cc = other as CombatCharacter;
                    if (!cc.AttackedOnceByPlayer)
                    {
                        var repDef = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.FirstOrDefault(r => r.ID == cc.ReputationId);
                        if (repDef != null)
                        {
                            if (repDef.ValueLossForNPCAttack > 0)
                            {
                                var playerRep = GetObject.PlayerSave.QuestLog.AllReputations.FirstOrDefault(r => r.ReputationID == cc.ReputationId);
                                if (playerRep != null)
                                {
                                    playerRep.Value -= repDef.ValueLossForNPCAttack;
                                }
                            }
                        }
                        cc.AttackedOnceByPlayer = true;
                    }
                }
            }

            var combatCalcTree = Rm_RPGHandler.Instance.Nodes.DamageTakenTree;

            if(defender.CharacterType == CharacterType.Player)
            {
                var playerDifficulty = GetObject.PlayerSave.Difficulty;
                var difficultyDef = Rm_RPGHandler.Instance.Player.Difficulties.FirstOrDefault(d => d.ID == playerDifficulty);
                if(difficultyDef != null)
                {
                    attackerDamage.ApplyMultiplier(difficultyDef.DamageMultiplier);
                }
            }

            if(!doNotEval)
            {
                var nodeChain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "DamageTaken")
                {
                    Attacker = attacker,
                    Defender = defender,
                    Damage = attackerDamage,
                    DamageDealt = damageDealt
                };

                AttackOutcome? outcome = null;
                while (!nodeChain.Done)
                {
                    nodeChain.Evaluate();

                    if (nodeChain.CurrentNode is MissedHitNode)
                    {
                        outcome = AttackOutcome.Missed;
                    }
                    else if (nodeChain.CurrentNode is EvadedHitNode)
                    {
                        outcome = AttackOutcome.Evaded;
                    }
                }

                if (outcome != null && (outcome != AttackOutcome.Success || outcome != AttackOutcome.Critical))
                {
                    return new DamageOutcome(null, outcome.Value);
                }
                else
                {
                    return new DamageOutcome(attackerDamage, damageDealt, nodeChain.Damage.IsCritical ? AttackOutcome.Critical : AttackOutcome.Success);
                }
            }
            else
            {
                return new DamageOutcome(attackerDamage,attackerDamage.DamageDealt,AttackOutcome.Success);
            }
            
        }

        public static Damage EvaluateDamageDealt(BaseCharacter character)
        {
            var damage = character.Damage;
            var dmgHasVariance = Rm_RPGHandler.Instance.Items.DamageHasVariance;
            var combatant = character;
            var combatCalcTree = Rm_RPGHandler.Instance.Nodes.DamageDealtTree;

            NodeChain nodeChain = null;
            if (dmgHasVariance)
            {
                nodeChain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MIN_Physical")
                                {
                                    CombatantA = combatant,
                                    IntValue = damage.MinDamage
                                };
                while (!nodeChain.Done)
                {
                    nodeChain.Evaluate();
                } 
            }
            

            var nodeChainMax = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MAX_Physical") {Combatant = combatant, IntValue = damage.MaxDamage};
            while (!nodeChainMax.Done)
            {
                nodeChainMax.Evaluate();
            }

            damage.MinDamage = dmgHasVariance ? nodeChain.IntValue : nodeChainMax.IntValue;
            damage.MaxDamage = nodeChainMax.IntValue;

            foreach(var element in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
            {
                var eleDamage = damage.ElementalDamages.FirstOrDefault(e => e.ElementID == element.ID);
                NodeChain chain = null;
                if(dmgHasVariance)
                {
                    chain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MIN_" + element.ID) { Combatant = combatant, IntValue = eleDamage.MinDamage };
                    while (!chain.Done)
                    {
                        chain.Evaluate();
                    }
                }
                

                var chainMax = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MAX_" + element.ID) { Combatant = combatant, IntValue = eleDamage.MaxDamage };
                while (!chainMax.Done)
                {
                    chainMax.Evaluate();
                }

                eleDamage.MinDamage = dmgHasVariance ? chain.IntValue : chainMax.IntValue;
                eleDamage.MaxDamage = chainMax.IntValue;
            }

            return damage;
        }

        public static Damage EvaluateSkillDamage(string damageScalingTreeID, Damage damage, BaseCharacter character)
        {
            var dmgHasVariance = Rm_RPGHandler.Instance.Items.DamageHasVariance;
            var combatant = character;
            var combatCalcTree = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.First(n => n.ID == damageScalingTreeID);

            NodeChain nodeChain = null;
            if (dmgHasVariance)
            {
                nodeChain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MIN_Physical")
                {
                    CombatantA = combatant,
                    IntValue = damage.MinDamage
                };
                while (!nodeChain.Done)
                {
                    nodeChain.Evaluate();
                }
            }


            var nodeChainMax = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MAX_Physical") { Combatant = combatant, IntValue = damage.MaxDamage };
            while (!nodeChainMax.Done)
            {
                nodeChainMax.Evaluate();
            }

            damage.MinDamage = dmgHasVariance ? nodeChain.IntValue : nodeChainMax.IntValue;
            damage.MaxDamage = nodeChainMax.IntValue;

            foreach (var element in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
            {
                var eleDamage = damage.ElementalDamages.FirstOrDefault(e => e.ElementID == element.ID);
                NodeChain chain = null;
                if (dmgHasVariance)
                {
                    chain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MIN_" + element.ID) { Combatant = combatant, IntValue = eleDamage.MinDamage };
                    while (!chain.Done)
                    {
                        chain.Evaluate();
                    }
                }


                var chainMax = new NodeChain(combatCalcTree, typeof(CombatStartNode), "MAX_" + element.ID) { Combatant = combatant, IntValue = eleDamage.MaxDamage };
                while (!chainMax.Done)
                {
                    chainMax.Evaluate();
                }

                eleDamage.MinDamage = dmgHasVariance ? chain.IntValue : chainMax.IntValue;
                eleDamage.MaxDamage = chainMax.IntValue;
            }

            return damage;
        }

//        public static Damage EvaluateSkillDamage(string damageScalingTreeID, Damage damage, BaseCharacter character)
//        {
//
//            var physDmg = Random.Range(damage.MinDamage, damage.MaxDamage + 1);
//            var combatant = character;
//            var combatCalcTree = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.First(n => n.ID == damageScalingTreeID);
//            var nodeChain = new NodeChain(combatCalcTree, typeof(CombatStartNode), "Physical") { Combatant = combatant, IntValue = physDmg };
//
//            while (!nodeChain.Done)
//            {
//                nodeChain.Evaluate();
//            }
//
//            damage.MinDamage = nodeChain.IntValue;
//            damage.MaxDamage = nodeChain.IntValue;
//
//            foreach (var element in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
//            {
//                var eleDamage = damage.ElementalDamages.FirstOrDefault(e => e.ElementID == element.ID);
//                if (eleDamage != null)
//                {
//                    var eleDmg = Random.Range(eleDamage.MinDamage, eleDamage.MaxDamage + 1);
//                    var chain = new NodeChain(combatCalcTree, typeof(CombatStartNode), element.ID) { Combatant = combatant, IntValue = eleDmg };
//                    while (!chain.Done)
//                    {
//                        chain.Evaluate();
//                    }
//
//                    eleDamage.MinDamage = chain.IntValue;
//                    eleDamage.MaxDamage = chain.IntValue;
//                }
//            }
//
//            return damage;
//        }

//        public Rm_Node[] GetResultNodes(bool booleanNodes)
//        {
//            if (booleanNodes)
//            {
//                return Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees
//                    .SelectMany(n => n.Nodes)
//                    .Where(n => n.NodeType == Rm_NodeType.Result_Node && n.IsBooleanResultNode).ToArray();
//            }
//
//            return Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees
//                    .SelectMany(n => n.Nodes)
//                    .Where(n => n.NodeType == Rm_NodeType.Result_Node).ToArray();
//        }
        
    }
}