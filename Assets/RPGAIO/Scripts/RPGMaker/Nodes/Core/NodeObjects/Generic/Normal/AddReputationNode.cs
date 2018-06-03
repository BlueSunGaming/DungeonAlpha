using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Reputation", "")]
    public class AddReputationNode : SimpleNode
    {
        public override string Name
        {
            get { return "Add Reputation"; }
        }

        public override string Description
        {
            get { return "Adds value to the player's reputation."; }
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
            Add("Reputation", PropertyType.ReputationDefinition, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Amount", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var player = GetObject.PlayerCharacter;
            var reputation = (string)ValueOf("Reputation");
            var amount = Convert.ToInt32(ValueOf("Amount"));

            player.AddReputation(reputation, amount);
        }
    }
}