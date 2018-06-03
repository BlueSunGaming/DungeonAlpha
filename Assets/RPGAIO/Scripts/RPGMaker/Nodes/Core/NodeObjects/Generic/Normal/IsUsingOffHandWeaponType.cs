using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsUsingOffHandWeaponType : BooleanNode
    {
        public override string Name
        {
            get { return "OffHand Weapon Type Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the player is using the specified weapon type in their offhand."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Weapon Type", PropertyType.WeaponTypeDefinition, null, "", PropertySource.InputOnly, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = GetObject.PlayerCharacter;
            var weaponId = (string)ValueOf("Weapon Type");
            var offhand = combatant.Equipment.EquippedOffHand != null ? combatant.Equipment.EquippedOffHand as Weapon : null;

            return offhand != null && offhand.WeaponTypeID == weaponId;
        }
    }
}