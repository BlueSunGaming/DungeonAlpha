using System;
using UnityEngine;
using RPG.Character;

namespace RPG.Character
{

    public class PlayerAttributes : ICharacterAttributes
    {
        [SerializeField]
        private int Perception;

        [SerializeField]
        private int Vitality;

        [SerializeField]
        private int Agility;

        [SerializeField]
        private int Strength;

        [SerializeField]
        private int Charisma;

        [SerializeField]
        private int Intelligence;

        [SerializeField]
        private int Luck;

        int ICharacterAttributes.Strength { get { return Strength; } set { Strength = value; } }

        int ICharacterAttributes.Perception { get { return Perception; } set { Perception = value; } }

        int ICharacterAttributes.Vitality { get { return Vitality; } set { Vitality = value; } }

        int ICharacterAttributes.Agility { get { return Agility; } set { Agility = value; } }

        int ICharacterAttributes.Charisma { get { return Charisma; } set { Charisma = value; } }

        int ICharacterAttributes.Intelligence { get { return Intelligence; } set { Intelligence = value; } }

        int ICharacterAttributes.Luck { get { return Luck; } set { Luck = value; } }

        float GetDamageModifier()
        {
            throw new NotImplementedException();
        }

        float GetMaxCarryWeight()
        {
            throw new NotImplementedException();
        }

        float GetChangeToHit()
        {
            throw new NotImplementedException();
        }

        float GetDodgeChance()
        {
            throw new NotImplementedException();
        }
    }

}