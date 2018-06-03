using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Game
        {
            /// <summary>
            /// Get's a list of all the defined credits for the game.
            /// </summary>
            public static List<Rm_CreditsInfo> Credits
            {
                get { return Rm_RPGHandler.Instance.GameInfo.CreditsInfo; }
            }
        }
    }
}