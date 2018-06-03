using System;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class MathExtensions
    {
        public static int RoundToNearest100(this int i, int nearest)
        {
            return (int) RoundToNearest(i, nearest);
        }

        public static float RoundToNearest(this float passednumber, float roundto)
        {
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return (float)Math.Ceiling(passednumber / roundto) * roundto;
            }
        }
        public static int RoundToNearest(this int passednumber, int roundto)
        {
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return (int)Math.Ceiling((double)passednumber / roundto) * roundto;
            }
        }

        /// <summary>
        /// Converts a given DateTime into a Unix timestamp
        /// </summary>
        /// <param name="value">Any DateTime</param>
        /// <returns>The given DateTime in Unix timestamp format</returns>
        public static int ToUnixTimestamp(this DateTime value)
        {
            return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}