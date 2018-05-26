using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Pet", "")]
    public class RemovePetNode : SimpleNode
    {
        public override string Name
        {
            get { return "Remove Pet"; }
        }

        public override string Description
        {
            get { return "Removes the player's pet."; }
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
            var player = GetObject.PlayerCharacter;
            if(player.CurrentPet != null)
            {
                player.CurrentPet.Remove();
            }
        }
    }
}