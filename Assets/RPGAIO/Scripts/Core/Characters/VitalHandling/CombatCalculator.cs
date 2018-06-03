namespace LogicSpawn.RPGMaker.Core
{
    public class CombatCalculator
    {
        private static readonly CombatCalculator MyInstance = new CombatCalculator();
        
        private const int MaxEvadeRange = 1000;
        private const float MaxEvadeBonus = 0.25f;
        private const float BaseAccuracy = 0.60f;

        public static CombatCalculator Instance
        {
            get { return MyInstance; }
        }

        public AccuracyCheck AccuracyCheck(float accuracy, float evasion)
        {
            var accuracyCheck = new AccuracyCheck {AccuracyIsHigher = accuracy > evasion};

            var range = accuracyCheck.AccuracyIsHigher ? accuracy - evasion : evasion - accuracy;
            var evasionAccuracyBonus = range / MaxEvadeRange * MaxEvadeBonus;

            accuracyCheck.PercentageChance = BaseAccuracy + evasionAccuracyBonus;

            return accuracyCheck;
        }

        public float GetDamageReductionPercent(float characterArmor, int physicalDamage)
        {
            //1 Point of damage reduced for every 10 points of armour
            var reduction = characterArmor / (characterArmor + (10 * physicalDamage));

            return reduction;
        }
    }
}