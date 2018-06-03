using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonRPG.Character;

namespace DungeonRPG.Character
{
    // TODO: Give this a better name
    public class PlayerAttributes : MonoBehaviour
    {
        const int MAX_ATTRIBUTE_VALUE = 25;
        const int MIN_ATTRIBUTE_VALUE = 3;

        // Range accuracy, spot traps, find hidden items, detect stealthed enemies
        [SerializeField]
        protected int Perception = 1;

        // Defense, health, natural immunities to poison/disease 
        [SerializeField]
        protected int Vitality = 1;

        // Attack Speed modifier, dodge chance, ranged damage/accurate modifier, lockpick skill, disarming trap skill
        [SerializeField]
        protected int Agility = 1;

        // Melee attack damage and carry weight modifier
        [SerializeField]
        protected int Strength = 1;

        // Speaking skill to influence likeability, gambling skill, initial NPC responses
        [SerializeField]
        protected int Charisma = 1;

        // Magic damage, experience modifier, and skill point modifier
        [SerializeField]
        protected int Intelligence = 1;

        // Intangible ability to affect everything positively or negativity
        [SerializeField]
        protected int Luck = 1;

        protected virtual float GetDamageModifier(DungeonRPG.Weapons.Weapon wpn)
        {
            float baseDamage =  wpn.GetBaseDamage();
            float dam = baseDamage + (baseDamage * (0.03f*Strength));
            return dam;
        }

        protected virtual float GetMaxCarryWeight()
        {
            return Strength * Vitality * 10.0f;
        }

        protected virtual float GetChanceToHit()
        {
            return GetLuckModifierRandomAwfulness() * Agility * 1.0f;
        }

        protected virtual float GetDodgeChance()
        {
            return Strength * Agility * 10.0f;
        }

        protected virtual float GetDefenseRating(List<DungeonRPG.Armor.Armor> armorPiecesList)
        {
            float retVal = 0.0f;
            if (armorPiecesList.Count > 0)
            {
                float totalBaseArmor = 0.0f;
                foreach (var armorPiece in armorPiecesList)
                {
                    totalBaseArmor += armorPiece.GetBaseProtection();
                }

                retVal = totalBaseArmor + (totalBaseArmor * (.03f * Vitality));
            }

            return retVal;
        }

        protected virtual float GetLuckModifierRandomAwfulness()
        {
            UnityEngine.Random rnd = new UnityEngine.Random();
            float randVal = UnityEngine.Random.Range(1, 100);
            float retVal = Luck* randVal;
            //Debug.Log("GetLuckModifierRandomAwfulness is " + retVal);
            return retVal;
        }

        private int CheckMinMaxAttributeVal(int attributeValueToCheck )
        {
            int retVal = 3;
            if (attributeValueToCheck > MAX_ATTRIBUTE_VALUE)
            {
                retVal = MAX_ATTRIBUTE_VALUE;
            }
            else if (attributeValueToCheck < MIN_ATTRIBUTE_VALUE)
            {
                retVal = MIN_ATTRIBUTE_VALUE;
            }

            return retVal;
        }
    }

}