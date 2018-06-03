using System;

namespace LogicSpawn.RPGMaker
{
    public class Rm_PetDefinition
    {
        public string ID;
        public string Name;
        public string CharacterID;
        public bool IsNpc;
        public PetBehaviour DefaultBehaviour;

        public Rm_PetDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "Unnamed Pet";
            CharacterID = "";
            IsNpc = false;
            DefaultBehaviour = PetBehaviour.PetOnly;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum PetBehaviour
    {
        Aggresive,
        Assist,
        PetOnly
    }
}