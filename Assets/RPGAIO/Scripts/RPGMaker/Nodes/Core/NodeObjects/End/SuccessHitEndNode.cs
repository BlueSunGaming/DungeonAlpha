using System.Collections.Generic;
using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SuccessHitEndNode : BooleanNode
    {

        [JsonIgnore]
        public override string Name
        {
            get { return "Success"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "End point for a successful attack that can then be a normal hit or critical hit."; }
        }

        [JsonIgnore]
        public override string SubText
        {
            get { return "End point for successful attack"; }
        }

        public override bool ShowInSearch
        {
            get { return false; }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 2;
            }
        }

        public override bool ShowTarget
        {
            get { return false; }
        }

        public override bool CanBeDeleted
        {
            get { return false; }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return index == 0 ? "On Hit" : "On Critical Hit";
        }


        public override bool IsStartNode
        {
            get { return false; }
        }

        public SuccessHitEndNode()
        {
            
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" }, new StringField { ID = "" } };
        }

        protected override void SetupParameters()
        {

        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var attacker = nodeChain.Attacker;
            var attackerCrit = attacker.GetStatByID("Critical Chance").TotalValue;

            var critRng = Random.Range(0.0f, 1.0f);
            var isCrit = critRng <= attackerCrit;


            nodeChain.Damage.IsCritical = isCrit;   
            return !isCrit;
        }
    }
}