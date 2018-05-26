using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class CustomExtensions
    {
        public static bool ContainsAnyOf(this List<string> listOfString, List<string> containsAnyOf )
        {
            for (int i = 0; i < listOfString.Count; i++)
            {
                if(containsAnyOf.Any(c => c.Equals(listOfString[i])))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsAnyOf(this List<string> listOfString, List<StringField> containsAnyOf )
        {
            for (int i = 0; i < listOfString.Count; i++)
            {
                if(containsAnyOf.Any(c => c.ID.Equals(listOfString[i])))
                {
                    return true;
                }
            }

            return false;
        }
    }
}