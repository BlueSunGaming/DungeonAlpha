using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsUsingOffHand : BooleanNode
    {
        public override string Name
        {
            get { return "Is Using OffHand"; }
        }

        public override string Description
        {
            get { return "Returns true if the player has an offhand equipped."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = GetObject.PlayerCharacter;
            return combatant.Equipment.EquippedOffHand != null;
        }
    }
}