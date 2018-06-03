using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsUsingMainWeaponType : BooleanNode
    {
        public override string Name
        {
            get { return "Main Weapon Type Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the player is using the specified weapon type."; }
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
            var weaponId = (string) ValueOf("Weapon Type");
            var weapon = combatant.Equipment.EquippedWeapon != null ? combatant.Equipment.EquippedWeapon as Weapon : null;
            var offhand = combatant.Equipment.EquippedOffHand != null ? combatant.Equipment.EquippedOffHand as Weapon : null;

            return weapon != null && weapon.WeaponTypeID == weaponId;
        }
    }
}