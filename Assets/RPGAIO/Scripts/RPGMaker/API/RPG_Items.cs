using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Items
        {
            /// <summary>
            /// Get's a rarity name by it's id.
            /// </summary>
            /// <param name="id">The ID of the rarity definition</param>
            /// <returns>Name of rarity</returns>
            public static string GetRarityName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.Items.ItemRarities.FirstOrDefault(s => s.ID == id);
                if (rmStatisticDefintion != null)
                    return rmStatisticDefintion.Name;

                return null;
            }

            /// <summary>
            /// Get's a rarity ID by it's name.
            /// </summary>
            /// <param name="name">The name of the rarity definition</param>
            /// <returns>ID of rarity</returns>
            public static string GetRarityID(string name)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.Items.ItemRarities.FirstOrDefault(s => s.Name == name);
                if (rmStatisticDefintion != null)
                    return rmStatisticDefintion.ID;

                return null;
            }

            /// <summary>
            /// Get's a rarity color by it's ID.
            /// </summary>
            /// <param name="id">The id of the rarity definition</param>
            /// <returns>Color of rarity</returns>
            public static Rm_UnityColors GetRarityColorById(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.Items.ItemRarities.FirstOrDefault(s => s.ID == id);
                if (rmStatisticDefintion != null)
                    return rmStatisticDefintion.Color;

                return Rm_UnityColors.None;
            }

            /// <summary>
            /// Get's a weapon type name by it's id.
            /// </summary>
            /// <param name="id">The ID of the weapon type</param>
            /// <returns>Name of weapon type</returns>
            public static string GetWeaponTypeName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.Items.WeaponTypes.FirstOrDefault(s => s.ID == id);
                if (rmStatisticDefintion != null)
                    return rmStatisticDefintion.Name;

                return null;
            }

            /// <summary>
            /// Get's a weapon type ID by it's name.
            /// </summary>
            /// <param name="name">The name of the weapon type</param>
            /// <returns>ID of weapon type</returns>
            public static string GetWeaponTypeID(string name)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.Items.WeaponTypes.FirstOrDefault(s => s.Name == name);
                if (rmStatisticDefintion != null)
                    return rmStatisticDefintion.ID;

                return null;
            }

            /// <summary>
            /// Get's an item from the item repository by ID.
            /// </summary>
            /// <param name="itemId">The id of the item</param>
            /// <returns>A copy of the Item with id</returns>
            public static Item GetItem(string itemId)
            {
                return GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.Items.Get(itemId));
            }

            /// <summary>
            /// Get's an item from the craftable item repository by ID.
            /// </summary>
            /// <param name="itemId">The id of the item</param>
            /// <returns>A copy of the Item with id</returns>
            public static Item GetCraftableItem(string itemId)
            {
                return GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemId));
            }

            /// <summary>
            /// Get's an item from the quest item repository by ID.
            /// </summary>
            /// <param name="itemId">The id of the item</param>
            /// <returns>A copy of the Item with id</returns>
            public static Item GetQuestItem(string itemId)
            {
                return GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemId));
            }

            /// <summary>
            /// Get's an item's name from the item repository by ID.
            /// </summary>
            /// <param name="itemId">The id of the item</param>
            /// <returns>The item's name</returns>
            public static string GetItemName(string itemId)
            {
                var item = (GetItem(itemId) ?? GetQuestItem(itemId)) ?? GetCraftableItem(itemId);

                return item != null ? item.Name : "";
            }
        }
    }
}