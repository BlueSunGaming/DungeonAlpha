using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class InteractableRepository
    {
        public List<NonPlayerCharacter> AllNpcs;
        public List<Interactable> AllInteractables;

        public InteractableRepository()
        {
            AllNpcs = new List<NonPlayerCharacter>();
            AllInteractables = new List<Interactable>();
        }

        public NonPlayerCharacter GetNPC(string npcID)
        {
            return AllNpcs.First(n => n.ID == npcID);
        }

        public Interactable GetInteractable(string objectID)
        {
            return AllInteractables.First(n => n.ID == objectID);
        }
    }
}