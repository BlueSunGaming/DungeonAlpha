using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Npc
        {
            /// <summary>
            /// Get the name of an NonPlayerCharacter by ID.
            /// </summary>
            public static string GetNpcName(string id)
            {
                var npc = Rm_RPGHandler.Instance.Repositories.Interactable.GetNPC(id);
                return npc != null ? npc.Name : "";
            }

            /// <summary>
            /// Get the name of an EnemyCharacter by ID.
            /// </summary>
            public static string GetEnemyName(string id)
            {
                var enemy = Rm_RPGHandler.Instance.Repositories.Enemies.Get(id);
                return enemy != null ? enemy.Name : "";
            }

            /// <summary>
            /// Get the name of an Interactable by ID.
            /// </summary>
            public static string GetInteractableName(string id)
            {
                var interactable = Rm_RPGHandler.Instance.Repositories.Interactable.GetInteractable(id);
                return interactable != null ? interactable.Name : "";
            }
        }
    }
}