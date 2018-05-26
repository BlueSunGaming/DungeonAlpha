using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class AddTraitExpNode : SimpleNode
    {
        public override string Name
        {
            get { return "Add Trait Exp"; }
        }

        public override string Description
        {
            get { return "Gives the player trait experience."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Amount", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
            Add("Trait", PropertyType.Trait, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var traitId = (string)ValueOf("Trait");
            var amount = Convert.ToInt32(ValueOf("Amount"));
            GetObject.PlayerCharacter.AddTraitExp(traitId, amount);
        }
    }
}