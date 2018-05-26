using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class UpdatePlayerStatsNode : SimpleNode
    {
        public override string Name
        {
            get { return "Update Player Stats"; }
        }

        public override string Description
        {
            get { return "Used at the end of modifying player stats. (Not for built-in combat/stat scaling)"; }
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

        }

        protected override void Eval(NodeChain nodeChain)
        {
            GetObject.PlayerCharacter.FullUpdateStats();
        }
    }
}