using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class UI
        {
            public static string FormatString(Rm_UnityColors color, string text)
            {
                var formattedText = string.Format("<color={0}>{1}</color>", color.ToString(), text);
                return formattedText;
            }
            public static string FormatLine(Rm_UnityColors color, string text)
            {
                var formattedText = string.Format("<color={0}>{1}</color>\n", color.ToString(), text);
                return formattedText;
            }

        }
    }
}