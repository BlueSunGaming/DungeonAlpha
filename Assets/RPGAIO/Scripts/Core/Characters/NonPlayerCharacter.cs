using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class NonPlayerCharacter : CombatCharacter
    {


        public bool CanBeKilled ;
        public bool CanFight ;
        public Interactable Interaction ;

        public NonPlayerCharacter()
        {
            Name = "New NPC";
            Interaction = new Interactable {ImagePath = ""};
            CharacterType = CharacterType.NPC;
            ReputationId = "Core_NPCReputation";
            CanFight = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}