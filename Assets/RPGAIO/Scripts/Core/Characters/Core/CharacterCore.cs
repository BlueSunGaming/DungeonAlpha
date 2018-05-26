using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public static class CharacterCore
    {
        public static Dictionary<AttackStyle, float> AttackStyleRangeValues = new Dictionary<AttackStyle, float>();
        public static Dictionary<AttackStyle, float> AttackStyleCastRangeValues = new Dictionary<AttackStyle, float>();

        static CharacterCore()
        {
            AttackStyleRangeValues = new Dictionary<AttackStyle, float>
                                      {
                                        { AttackStyle.Melee , 1.20f },
                                        { AttackStyle.Ranged , 8.00f }
                                      };

            AttackStyleCastRangeValues = new Dictionary<AttackStyle, float>
                                      {
                                        { AttackStyle.Melee , 4.00f },
                                        { AttackStyle.Ranged , 6.00f }
                                      };
        }

        public static float CharacterRange(AttackStyle attackStyle )
        {
            float rangeValue;
            var foundValue = AttackStyleRangeValues.TryGetValue(attackStyle, out rangeValue);

            if (!foundValue)
                throw new KeyNotFoundException();

            return rangeValue; 
        }

        public static float CharacterCastRange(AttackStyle attackStyle )
        {
            float rangeValue;
            var foundValue = AttackStyleRangeValues.TryGetValue(attackStyle, out rangeValue);

            if (!foundValue)
                throw new KeyNotFoundException();

            return rangeValue; 
        }
    }
}
