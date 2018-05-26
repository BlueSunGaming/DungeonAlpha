using System.Linq;
using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Weapon : BuffItem //TODO: elemental damages in a list, so user can name them himself, linked to armor defenses
    {
        public string WeaponTypeID ;
        public bool OverrideAttackSpeed;
        public float NewAttackSpeed;

        public bool OverrideAttackRange ;
        public float NewAttackRange;

        public bool OverrideProjectileSpeed;
        public float NewProjectileSpeed;

        public string OverrideProjectilePrefabPath;

        public bool AllowDualWield ;
        public bool HasProcEffect ;
        public Rm_ProcEffect ProcEffect ;
        public Damage Damage;

        [JsonIgnore]
        public bool TwoHanded
        {

            get
            {
                return Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).IsTwoHanded;
            }
        }
        [JsonIgnore]
        public AttackStyle AttackStyle
        {

            get
            {
                return Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).AttackStyle;
            }
        }
        [JsonIgnore]
        public float AttackRange
        {

            get
            {
                return OverrideAttackRange ? NewAttackRange : Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).AttackRange;
            }
        }
        [JsonIgnore]
        public float AttackSpeed
        {

            get
            {
                return OverrideAttackSpeed ? NewAttackSpeed : Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).AttackSpeed;
            }
        }
        [JsonIgnore]
        public float ProjectileSpeed
        {

            get
            {
                return OverrideProjectileSpeed ? NewProjectileSpeed : Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).ProjectileSpeed;
            }
        }
        [JsonIgnore]
        public string ProjectilePrefab
        {

            get
            {
                return !string.IsNullOrEmpty(OverrideProjectilePrefabPath) ? OverrideProjectilePrefabPath : Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == WeaponTypeID).AutoAttackPrefabPath;
            }
        }

        public Weapon()
        {
            Damage = new Damage(){MinDamage = 1, MaxDamage = 1};
            ItemType = ItemType.Weapon;
            OverrideAttackSpeed = false;
            OverrideAttackRange = false;
            OverrideProjectileSpeed = false;
            NewAttackRange = 2.0F;
            NewAttackSpeed = 1.0f;
            NewProjectileSpeed = 25.0f;
            AllowDualWield = true;  
            OverrideProjectilePrefabPath = "";
            WeaponTypeID = "Default_WepType";
            ProcEffect = new Rm_ProcEffect();
        }

        public bool AddProc(Rm_ProcEffect procToAdd)
        {
            if(ProcEffect == null)
            {
                ProcEffect = procToAdd;
                return true;
            }
            return false;
        }

        public override string GetTooltipDescription()
        {
            var tooltip = "";

            if(Damage.SumOfElementalDamage + Damage.SumOfElementalMinDamage > 0)
            {
                if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                {
                    tooltip += Damage.MinDamage + "-" + Damage.MaxDamage + " Damage" + "\n";
                }
                else
                {
                    tooltip += Damage.MaxDamage + " Damage" + "\n";
                }
            }
            

            foreach (var elementalDmg in Damage.ElementalDamages.Where(e => e.MinDamage + e.MaxDamage > 0))
            {
                var color = RPG.Combat.GetElementalColorById(elementalDmg.ElementID);
                var name = RPG.Combat.GetElementalNameById(elementalDmg.ElementID);

                if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                {
                    tooltip += RPG.UI.FormatLine(color, string.Format("+{0}-{1} {2} Damage", elementalDmg.MinDamage, elementalDmg.MaxDamage, name));
                }
                else
                {
                    tooltip += RPG.UI.FormatLine(color, string.Format("+{0} {1} Damage", elementalDmg.MaxDamage, name));
                }
            }

            tooltip += "\n";

            tooltip += "Attack Speed: " + AttackSpeed + " , " + "Range: " + AttackRange +"\n\n";

            

            var baseDescription = base.GetTooltipDescription();

            //todo: Procs

            return tooltip + baseDescription;
        }
    }
}