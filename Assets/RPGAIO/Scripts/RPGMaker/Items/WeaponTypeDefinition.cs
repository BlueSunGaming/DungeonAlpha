using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class WeaponTypeDefinition
    {

        public string ID;
        public string Name;
        public string PrefabPath;
        public bool IsTwoHanded;
        public bool AllowDualWield;

        public AttackStyle AttackStyle;


        public float ProjectileSpeed;
        public string AutoAttackPrefabPath;
        public string AutoAttackImpactPrefabPath;
        public AudioContainer ProjectileTravelSound;
        public AudioContainer AutoAttackImpactSound;


        public float AttackRange;
        public float AttackSpeed;

        public WeaponTypeDefinition()
        {
            ID = Guid.NewGuid().ToString();
            AttackStyle = AttackStyle.Melee;
            AttackRange = 2.0f;
            AttackSpeed = 1.2f;


            ProjectileTravelSound = new AudioContainer();
            AutoAttackImpactSound = new AudioContainer();
            ProjectileSpeed = 10f;
        }
    }
}