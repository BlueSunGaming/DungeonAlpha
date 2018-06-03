using System;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker
{
    public class PetData
    {
        public string ID;
        public string Name;
        public string CharacterID;
        public bool IsNpc;
        public PetBehaviour CurrentBehaviour;

        public PetData()
        {
            ID = Guid.NewGuid().ToString();
            CurrentBehaviour = PetBehaviour.PetOnly;
        }
        public PetData(Rm_PetDefinition definition)
        {
            ID = Guid.NewGuid().ToString();
            Name = definition.Name;
            CurrentBehaviour = definition.DefaultBehaviour;
            CharacterID = definition.CharacterID;
            IsNpc = definition.IsNpc;
        }
    }
}